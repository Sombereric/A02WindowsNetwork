/*
 * FILE : ConnectionHandler.cs
 * PROJECT : PROG2126 - Assignment #2
 * PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
 * FIRST VERSION : 2026-2-9
 * DESCRIPTION :
 * Handles incoming TCP connections from clients.
 * Responsible for:
 *   - Starting/stopping the server
 *   - Accepting clients
 *   - Spawning tasks for each client
 *   - Reading/parsing client messages
 *   - Sending responses back
 */

using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GuessingGameServer.GameLogic;
using GuessingGameServer.TCP_Connection.ServerSender;
using GuessingGameServer.UserInterface;

namespace GuessingGameServer.TCP_Connection.ServerListener
{
    internal class ConnectionHandler
    {
        // UI used for logging server-side messages
        private UI ui = new UI();

        // Used to gracefully shut down all async tasks
        private static CancellationTokenSource cts = new CancellationTokenSource();

        // (Currently unused) list of connected clients
        private static readonly List<TcpClient> clients = new List<TcpClient>();

        // Handles interpreting protocol messages
        ConnectionProtocol connectionProtocol = new ConnectionProtocol();

        // Handles sending messages to connected clients
        ServerClientSender sender = new ServerClientSender();

        /// <summary>
        /// the main listener of the server
        /// </summary>
        /// <param name="gameStateInfos">the game state list</param>
        /// <param name="GameStateLocker">the locker the game state</param>
        /// <returns>returns the task handler</returns>
        public async Task MainServerListener(List<GameStateInfo> gameStateInfos, object GameStateLocker)
        {
            bool serverSetupSuccess = true;

            // Retrieve port number from app.config
            string ServerPort = ConfigurationManager.AppSettings["ServerPort"];

            TcpListener server = null;

            try
            {
                // Validate that the port number is valid
                if (!Int32.TryParse(ServerPort, out Int32 port))
                {
                    ui.WriteToConsole("Failure to parse server port");
                    serverSetupSuccess = false;
                }

                if (serverSetupSuccess)
                {
                    // Create TCP listener on all network interfaces
                    server = new TcpListener(IPAddress.Any, port);

                    // Begin listening for incoming connections
                    server.Start();

                    // Start background task that accepts clients
                    Task runnerTask = TaskRunner(server, gameStateInfos, GameStateLocker);

                    ui.WriteToConsole("server running");
                    ui.WriteToConsole("To stop the server Press Enter...");

                    // Block until admin presses Enter
                    ui.ReadFromConsole();

                    // Signal cancellation to all running tasks
                    cts.Cancel();

                    // Stop accepting new clients
                    server.Stop();

                    // Wait for all client handler tasks to finish
                    await runnerTask;

                    // Final cleanup / final send (if required by assignment logic)
                    await sender.connectToClient(gameStateInfos, GameStateLocker);

                    ui.WriteToConsole("Server Closed");
                }
            }
            catch (Exception ex)
            {
                ui.WriteToConsole("Unexpected server failure " + ex.Message);
            }
        }

        /// <summary>
        /// used to handle all requests asychronously 
        /// </summary>
        /// <param name="server">the server stream</param>
        /// <param name="gameStateInfos">the list of game states</param>
        /// <param name="GameStateLocker">the game state locker for the list</param>
        /// <returns>the handler</returns>
        private async Task TaskRunner(TcpListener server, List<GameStateInfo> gameStateInfos, object GameStateLocker)
        {
            List<Task> tasks = new List<Task>();

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    ui.WriteToConsole("Server connection begun");

                    // Wait for a client to connect
                    TcpClient client = await server.AcceptTcpClientAsync();

                    // Start handling the client in a separate async task
                    Task handlerTask = ConnectionClientHandler(client, gameStateInfos, GameStateLocker);

                    tasks.Add(handlerTask);

                    // Clean up completed tasks to avoid memory buildup
                    tasks.RemoveAll(task => task.IsCompleted);
                }
            }
            catch (Exception ex)
            {
                // Ignore exceptions caused by cancellation
                if (!cts.Token.IsCancellationRequested)
                {
                    ui.WriteToConsole(ex.Message);
                }
            }
            finally
            {
                // Wait for all active client handlers to finish
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    ui.WriteToConsole(ex.Message);
                }
            }
        }

        /// <summary>
        /// handles the connection logic
        /// </summary>
        /// <param name="client">the client to connect</param>
        /// <param name="gameStateInfos">the game state list</param>
        /// <param name="GameStateLocker">the locker for the server list</param>
        /// <returns>task handler</returns>
        private async Task ConnectionClientHandler(TcpClient client, List<GameStateInfo> gameStateInfos, object GameStateLocker)
        {
            bool badRequest = false;

            // Get stream for reading/writing to this client
            NetworkStream stream = client.GetStream();

            string checkMessage = "";
            string checkResponse = "";

            bool finishRead = false;

            int bufferSizeDefault = 1024;

            // Read buffer size from config
            string checkBuffer = ConfigurationManager.AppSettings["BufferSize"];

            // Validate buffer size
            if (!int.TryParse(checkBuffer, out int bufferSize))
            {
                ui.WriteToConsole("Failed to parse buffer in app.config using default of 1024.");
                bufferSize = bufferSizeDefault;
            }

            // Allocate byte buffer
            byte[] buffer = new byte[bufferSize];

            try
            {
                // Keep reading until:
                // - "|END|" marker is found
                // - client disconnects
                // - cancellation requested
                while (!finishRead && !cts.Token.IsCancellationRequested)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, bufferSize, cts.Token);

                    // If 0 bytes read → client disconnected
                    if (bytesRead == 0)
                    {
                        finishRead = true;
                    }

                    // Decode only the actual bytes read
                    checkMessage += Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Stop reading once protocol end marker detected
                    if (checkMessage.Contains("|END|"))
                    {
                        finishRead = true;
                    }
                }

                // Locate protocol terminator
                int endIndex = checkMessage.IndexOf("|END|");

                if (endIndex == -1)
                {
                    // If no end marker found → invalid request
                    checkResponse = "400|Invalid Request|failure to parse data|END|";
                    badRequest = true;
                }

                if (!badRequest)
                {
                    int endMarkerLength = "|END|".Length;

                    // Extract only the complete protocol message
                    string fullMessage = checkMessage.Substring(0, endIndex + endMarkerLength);

                    // Split message using '|' delimiter
                    char delimiter = '|';
                    string[] protocolMessage = fullMessage.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    // Expecting exactly 6 protocol fields
                    if (protocolMessage.Length == 6)
                    {
                        // Delegate protocol logic to protocol manager
                        checkResponse = connectionProtocol.ServerProtocolManager(
                            protocolMessage,
                            gameStateInfos,
                            GameStateLocker
                        );
                    }
                    else
                    {
                        checkResponse = "400|Invalid Request|failure to parse data|END|";
                    }
                }

                // Convert response string to bytes
                byte[] responseBytes = Encoding.UTF8.GetBytes(checkResponse);

                // Send response back to client
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                await stream.FlushAsync();
            }
            catch (Exception ex)
            {
                if (!cts.Token.IsCancellationRequested)
                {
                    ui.WriteToConsole(ex.Message);
                }
            }
            finally
            {
                // Always close connection after handling request
                stream.Close();
                client.Close();
            }
        }
    }
}