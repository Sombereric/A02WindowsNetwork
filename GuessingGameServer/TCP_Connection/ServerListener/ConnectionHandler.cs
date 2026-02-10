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
            }
            catch (Exception ex) 
            {
                ui.WriteToConsole("");
            }
            return;
        }
    }
}
