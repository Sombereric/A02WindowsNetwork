/*
* FILE : ConnectionHandler.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* where the connection from the client to the server is handled on the server end
*/

using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GuessingGameServer.GameLogic;
using GuessingGameServer.UserInterface;

namespace GuessingGameServer.TCP_Connection.ServerListener
{
    internal class ConnectionHandler
    {
        private static readonly object locker = new object();

        private UI ui = new UI();
        private static CancellationTokenSource cts = new CancellationTokenSource();
        private static readonly List<TcpClient> clients = new List<TcpClient>();

        ConnectionProtocol connectionProtocol = new ConnectionProtocol();

        /// <summary>
        /// where the server listens for connecting clients
        /// </summary>
        public async Task MainServerListener(List<GameStateInfo> gameStateInfos, object GameStateLocker)
        {
            //used to stop the server should any connection setup fail
            bool serverSetupSuccess = true;
            //gets the server port and IP from the config file
            string ServerPort = ConfigurationManager.AppSettings["ServerPort"];

            TcpListener server = null;
            try
            {
                //attempts to parse the port of the server from the app config
                if (!Int32.TryParse(ServerPort, out Int32 port))
                {
                    //logs the error and stops the server settingup
                    ui.WriteToConsole("Failure to parse server port");
                    serverSetupSuccess = false;
                }
                if (serverSetupSuccess)
                {
                    //creates the listener
                    server = new TcpListener(IPAddress.Any, port);

                    //start listening for clients requests
                    server.Start();
                    Task runnerTask = TaskRunner(server, gameStateInfos, GameStateLocker);

                    //lets admins know server is running
                    ui.WriteToConsole("server running");

                    ui.WriteToConsole("To stop the server Press Enter...");
                    ui.ReadFromConsole();

                    cts.Cancel();

                    server.Stop();

                    StopTheClient();

                    await runnerTask;

                    ui.WriteToConsole("Server Closed");
                }
            }
            catch (Exception ex)
            {
                ui.WriteToConsole("Unexpected server failure " + ex.Message);
            }
            return;
        }
        /// <summary>
        /// stops the sub server clients
        /// </summary>
        private void StopTheClient()
        {
            List<TcpClient> clientsToStop = null;

            lock (locker)
            {
                clientsToStop = new List<TcpClient>(clients);
                clients.Clear();
            }

            //loops over all clients
            for (int checkCount = 0; checkCount < clientsToStop.Count; checkCount++)
            {
                TcpClient client = clientsToStop[checkCount];

                try
                {
                    NetworkStream stream = client.GetStream();

                    byte[] stopMessage = Encoding.UTF8.GetBytes("STOP");

                    stream.Write(stopMessage, 0, stopMessage.Length);
                    stream.Flush();
                    stream.Close();

                    ui.WriteToConsole("Stop sent to client");
                }
                catch (Exception ex)
                {
                    ui.WriteToConsole("Unexpected server failure " + ex.Message);
                }

                try
                {
                    if (client != null)
                    {
                        client.Close();
                    }
                }
                catch (Exception ex)
                {
                    ui.WriteToConsole("Unexpected server failure " + ex.Message);
                }
            }
        }
        /// <summary>
        /// this is where the tasks and connections are handled and created
        /// </summary>
        /// <param name="server">the server name</param>
        /// <param name="gameStateInfos">the list of game states currently connected</param>
        /// <param name="GameStateLocker">protects the game state list from multiple changes</param>
        /// <returns>returns the task handler</returns>
        private async Task TaskRunner(TcpListener server, List<GameStateInfo> gameStateInfos, object GameStateLocker)
        {
            List<Task> tasks = new List<Task>();
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    ui.WriteToConsole("Server connection begun");
                    TcpClient client = await server.AcceptTcpClientAsync();
                    Task handlerTask = ConnectionClientHandler(client, gameStateInfos, GameStateLocker);
                    tasks.Add(handlerTask);
                    tasks.RemoveAll(task => task.IsCompleted);
                }
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
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    ui.WriteToConsole(ex.Message);
                }
                
            }
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="gameStateInfos"></param>
        /// <param name="GameStateLocker"></param>
        /// <returns></returns>
        private async Task ConnectionClientHandler(TcpClient client, List<GameStateInfo> gameStateInfos, object GameStateLocker)
        {
            NetworkStream stream = client.GetStream();
            string checkMessage = "";

            bool finishRead = false;
            int bufferSizeDefault = 1024;

            string checkBuffer = ConfigurationManager.AppSettings["BufferSize"];

            //attempts the parse the buffer size in the config file
            if (!int.TryParse(checkBuffer, out int bufferSize))
            {
                ui.WriteToConsole("Failed to parse buffer in app.config using default of 1024.");
                bufferSize = bufferSizeDefault;
            }

            byte[] buffer = new byte[bufferSize];

            try
            {
                while (!finishRead && !cts.Token.IsCancellationRequested)
                {
                    int bytesRead = 0;
                    bytesRead = await stream.ReadAsync(buffer, 0, bufferSize, cts.Token);

                    if (bytesRead == 0)
                    {
                        finishRead = true;
                    }

                    //IMPORTANT: decode only what was read
                    checkMessage += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (checkMessage.Contains("|END|"))
                    {
                        finishRead = true;
                    }
                }
                char delimiter = '|';
                string[] protocolMessage = checkMessage.Split(delimiter);

                string checkResponse = "";

                if (protocolMessage.Length == 7)
                {
                    checkResponse = connectionProtocol.ServerProtocolManager(protocolMessage, gameStateInfos, GameStateLocker);
                }
                else
                {
                    checkResponse = "400|Invalid Request|-|END|";
                }

                // send it to client
                byte[] responseBytes = Encoding.UTF8.GetBytes(checkResponse);
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
                stream.Close();
                client.Close();

                lock (locker)
                {
                    clients.Remove(client);
                }
            }
            return;
        }
    }
}