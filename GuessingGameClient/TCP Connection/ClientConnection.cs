/*
* FILE : ClientConnection.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* handles each request made by the client 
*
*/
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ClientUI.Protocols;

namespace ClientUI.TCP_Connection
{
    class ClientConnection
    {
        private TcpClient tcpClient;
        private StreamReader reader;
        private StreamWriter writer;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task ConnectTheClient(string needIP, string needPort)
        {
            tcpClient = new TcpClient();

            IPAddress address = IPAddress.Parse(needIP);

            if(!int.TryParse(needPort, out int port))
            {
                throw new Exception("Invalid port number");
            }

            await tcpClient.ConnectAsync(address, port);

     
            NetworkStream stream = tcpClient.GetStream();
            
            reader = new StreamReader(stream);
            
            writer = new StreamWriter(stream);
            
            writer.AutoFlush = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookMessage"></param>
        /// <returns></returns>
        private async Task SendMessage(string lookMessage)
        {
            string[] lookLines = lookMessage.Split(Environment.NewLine);
       
            foreach(string checkLine in lookLines)
            {
                await writer.WriteLineAsync(checkLine);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<string> ReadMessageAtEnd()
        {
            StringBuilder sb = new StringBuilder();
            string checkLine;

            while (true)
            {
                checkLine = await reader.ReadLineAsync();

                if (checkLine == null)
                {
                    throw new Exception("Connection closed");
                }

                if (checkLine.Contains("|END|"))
                {
                    sb.AppendLine(checkLine);
                    break;
                }

                sb.AppendLine(checkLine);
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="fieldsDictionary"></param>
        /// <returns></returns>
        public async Task <string> CheckRequestResponse(string ip, string port, Dictionary<string, string> fieldsDictionary)
        {
            await ConnectTheClient(ip, port);

            string checkMsg = ClientProtocol.BuildMessage(fieldsDictionary);

            await SendMessage(checkMsg);

            string checkResponse = await ReadMessageAtEnd();

            tcpClient.Close();

            return checkResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseClient()
        {
            reader?.Dispose();
            writer?.Dispose();
            tcpClient.Close();
        }
    }
}
