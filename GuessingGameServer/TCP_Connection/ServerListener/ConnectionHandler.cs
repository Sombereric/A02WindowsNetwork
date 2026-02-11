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
        public async Task MainServerListener(List<GameStateInfo> gameStateInfos)
        {
            //gets the server port and IP from the config file
            string ServerIP = ConfigurationManager.AppSettings["ServerIP"];
            string ServerPort = ConfigurationManager.AppSettings["ServerPort"];

            TcpListener server = null;

            try
            {
                if (!Int32.TryParse(ServerPort, out Int32 port))
                {
                    ui.WriteToConsole("Failure to parse server port");
                }
                if (!IPAddress.TryParse(ServerIP, out IPAddress serverIPParsed))
                {
                    ui.WriteToConsole("Failure to parse server IP");
                }

                //creates the listener
                server = new TcpListener(serverIPParsed, port);

                //start listening for clients requests
                server.Start();

                ui.WriteToConsole("Server Listening on " + serverIPParsed.ToString() + ":" + port);

                TaskRunner(server);

                ui.WriteToConsole("To stop the server Press Enter...");
                ui.ReadFromConsole();

                cts.Cancel();

                StopTheClient();

                server.Stop();

                ui.WriteToConsole("Server Closed");
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
        /// where the tasks are handed 
        /// </summary>
        /// <param name="server">the server clients connect to</param>
        private void TaskRunner(TcpListener server)
        {
            //accept loop on a task so console can stop server
            Task.Run(() =>
            {
                try
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        ui.WriteToConsole("Server connection begun");
                        serverClientWorker(server);
                    }
                }
                catch (Exception ex)
                {
                    ui.WriteToConsole("Unexpected TCP Server failure: " + ex.Message);
                }
            });
        }
        /// <summary>
        /// where the work is actually done on the server side
        /// </summary>
        /// <param name="server">the name of the server listening</param>
        private void serverClientWorker(TcpListener server)
        {
            TcpClient client = server.AcceptTcpClient();

            ui.WriteToConsole("Client Connected");

            lock (locker)
            {
                clients.Add(client);
            }

            try
            {
                //where the tasks are created to handle
                Task clientHandlerTask = Task.Run(async () => await ConnectionClientHandler(client));
            }
            catch (Exception ex)
            {
                ui.WriteToConsole("Failure during client connection: " + ex.Message);
            }
            return;
        }
        private async Task ConnectionClientHandler(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            string checkMessage = "";

            bool finishRead = false;
            int bufferSizeDefault = 1024;

            string checkBuffer = ConfigurationManager.AppSettings["BufferSizeBytes"];

            //attempts the parse the buffer size in the config file
            if (!int.TryParse(checkBuffer, out int bufferSize))
            {
                ui.WriteToConsole("Failed to parse buffer in app.config using default of 1024.");
                bufferSize = bufferSizeDefault;
            }

            byte[] buffer = new byte[bufferSize];

            try
            {
                while (!finishRead && !cts.IsCancellationRequested)
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

                if (protocolMessage.Length != 6)
                {
                    connectionProtocol.ServerProtocolManager(protocolMessage);
                }

                //server response

            }
            catch (Exception ex)
            {
                ui.WriteToConsole("Client handler error: " + ex.Message);
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