using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ClientUI.Protocols;

namespace ClientUI.DAL.ClientManager
{
    public class ClientWorker
    {
        public static async Task <string> Run(int protocolId)
        {
            string serverIp = ConfigurationManager.AppSettings["ServerIP"];
            string portText = ConfigurationManager.AppSettings["ServerPort"];

            bool checkOk = true;

            int serverPort = 0;

            string commandAction = string.Empty; // this will command the action for login
            string actionData = string.Empty; // this will send the data action
            string checkRequst = string.Empty; // final request messagw

            TcpClient checkClient = null;
            NetworkStream networkStream = null;
            StreamReader reader = null;
            StreamWriter writer = null;

            StringBuilder checkResponse = new StringBuilder();

            if (checkOk)
            {
                switch (protocolId)
                {
                    case 200:
                        commandAction = "Login";
                        actionData = "username: password" + serverIp + ":" + portText;
                        break;

                    case 201:
                        commandAction = "Guess";
                        actionData = "testWord";
                        break;

                    case 202:
                        commandAction = "New Game";
                        actionData = "-";
                        break;

                    case 203:
                        commandAction = "Quit";
                        actionData = "-";
                        break;

                    case 204:
                        commandAction = "Play Again";
                        actionData = "-";
                        break;

                    default:
                        checkOk = false;
                        break;

                }

                // this will build the protcol message directly 
                if (checkOk == true)
                {
                    Guid clientGuid = Guid.NewGuid();
                    string timeSent = DateTime.UtcNow.ToString("o");

                    checkRequst = protocolId.ToString() + "|" + clientGuid.ToString() + "|" + timeSent + "|" + timeSent + "|" + commandAction + "|" + actionData + "|END|";

                }

                // this will disconnect the request and response
                if (checkOk == true)
                {
                    try
                    {
                        checkClient = new TcpClient();
                        await checkClient.ConnectAsync(serverIp, serverPort);

                        networkStream = checkClient.GetStream();
                        reader = new StreamReader(networkStream);
                        writer = new StreamWriter(networkStream);
                        writer.AutoFlush = true;

                        await writer.WriteLineAsync(checkRequst);

                        while (true)
                        {
                            string checkLines = await reader.ReadLineAsync();

                            if (checkLines == null)
                            {
                                break;
                            }

                            checkResponse.AppendLine(checkLines);

                            if (checkLines.Contains("|END|") == true)
                            {
                                break;
                            }
                        }
                    }

                    finally
                    {

                        if (networkStream != null)
                        {
                            networkStream.Close();
                        }

                        if (checkClient != null)
                        {
                            checkClient.Close();
                        }

                    }

                }

            }
            return checkResponse.ToString();

        }
    }
}

    
