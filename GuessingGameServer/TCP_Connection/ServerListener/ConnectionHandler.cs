using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GuessingGameServer.UserInterface;

namespace GuessingGameServer.TCP_Connection.ServerListener
{
    internal class ConnectionHandler
    {
        private UI ui = new UI();
        private static CancellationTokenSource cts = new CancellationTokenSource();
        public void MainServerListener()
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

                //where the task 

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
                        ui.WriteToConsole("Unexpected TCP Server failure: " + ex);
                    }
                });

                ui.WriteToConsole("To stop the server Press Enter...");
                ui.ReadFromConsole();

                cts.Cancel();

                StopTheClient();

                server.Stop();

                ui.WriteToConsole("Server Closed");
            }
            catch (Exception ex) 
            {
                ui.WriteToConsole("Unexpected server failure " + ex);
            }
            return;
        }
        /// <summary>
        /// where the work is actually done on the server side
        /// </summary>
        /// <param name="server">the name of the server listening</param>
        private void serverClientWorker(TcpListener server)
        {

        }
        /// <summary>
        /// stops the sub server clients
        /// </summary>
        private void StopTheClient()
        {

        }
    }
}