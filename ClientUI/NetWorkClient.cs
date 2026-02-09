using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace ClientUI
{
    class NetWorkClient
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
        private async Task <string> ReadMessageAtEnd()
        {
            StringBuilder sb = new StringBuilder();
            string checkLine;

            while (true)
            {
                checkLine = await reader.ReadLineAsync();
                
                if(checkLine == null)
                {
                    throw new Exception("Connection closed");
        

                }
                else if(checkLine == "END")
                {
                    break;
                }

                sb.AppendLine(checkLine);
            }

            return sb.ToString();
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

            string checkMsg = Protocol.BuildMessage(fieldsDictionary);

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
