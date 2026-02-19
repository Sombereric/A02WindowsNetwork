/*
* FILE : ConnectionProtocol.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* handles each request made by the client to the server and the respective action
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClientUI.Protocols;

namespace ClientUI.DAL.ClientManager
{
    public class ClientWorker
    {
        private static Guid clientGuid = Guid.NewGuid();

        public static async Task<string> Run(int protocolId, string userName)
        {
            string serverIp = ConfigurationManager.AppSettings["ServerIP"];
            string portText = ConfigurationManager.AppSettings["ServerPort"];

            bool checkOk = true;

            if (Int32.TryParse(portText, out Int32 result))
            {
            }

            int serverPort = result;

            string commandAction = string.Empty; // this will command the action for login
            string actionData = string.Empty; // this will send the data action
            string checkRequst = string.Empty; // final request messagw

            TcpClient checkClient = null;
            NetworkStream networkStream = null;
            StreamReader reader = null;
            StreamWriter writer = null;

            StringBuilder checkResponse = new StringBuilder();

            if (!int.TryParse(portText, out serverPort))
            {
                checkOk = false;
            }

            if (checkOk)
            {
                switch (protocolId)
                {
                    case 200:
                        commandAction = "Login";
                        actionData = userName;
                        break;

                    case 201:
                        commandAction = "Guess";
                        actionData = userName;
                        break;
                    case 202:
                        commandAction = "NewGame";
                        actionData = userName + ':' + serverIp + ':' + portText;
                        break;

                    case 203:
                        commandAction = "-";
                        actionData = "-";
                        break;

                    case 204:
                        commandAction = "QuitGames";
                        actionData = "-";
                        break;

                    default:
                        checkOk = false;
                        break;
                }

                // this will build the protcol message directly 
                if (checkOk == true)
                {
                    string timeSent = DateTime.UtcNow.ToString("o");

                    checkRequst = protocolId.ToString() + "|" + clientGuid.ToString() + "|" + timeSent + "|" + commandAction + "|" + actionData + "|END|";
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