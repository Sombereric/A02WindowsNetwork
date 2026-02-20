/*
* FILE : ServerClientSender.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* handles the response of the client sender
*/

using System.Net;
using System.Net.Sockets;
using System.Text;
using GuessingGameServer.GameLogic;
using GuessingGameServer.UserInterface;

namespace GuessingGameServer.TCP_Connection.ServerSender
{
    internal class ServerClientSender
    {
        private UI ui = new UI();
        /// <summary>
        /// connects to client 
        /// </summary>
        /// <param name="gameStateInfos">carries all the ip and ports</param>
        /// <param name="GameStateLocker">makes sure only one task can work at once</param>
        /// <returns>returns task handler</returns>
        public async Task connectToClient(List<GameStateInfo> gameStateInfos, object GameStateLocker)
        {
            List<IPAddress> ipList = new List<IPAddress>();
            List<int> portList = new List<int>();

            lock (GameStateLocker)
            {
                //finds the client ip and port and adds it to a list
                foreach (GameStateInfo info in gameStateInfos)
                {
                    if (info != null && info.ClientIp != null && info.Port > 0)
                    {
                        ipList.Add(info.ClientIp);
                        portList.Add(info.Port);
                    }
                }
            }

            // Send shutdown message to each client listener
            for (int i = 0; i < ipList.Count; i++)
            {
                try
                {
                    TcpClient checkClient = new TcpClient();
                    await checkClient.ConnectAsync(ipList[i], portList[i]);

                    NetworkStream networkStream = checkClient.GetStream();

                    string message = BuildResponse(500, "Shutting Down","Returning To Main Menu");

                    await SendResponse(networkStream, message);
                }
                catch (Exception ex)
                {
                    ui.WriteToConsole("Shutdown notify failed for ip:port = " + ex.Message);
                }
            }
            return;
        }

        /// <summary>
        ///  this will just the response from the client to the server
        /// </summary>
        /// <param name="respondID">the protocol id</param>
        /// <param name="serverState">what state the server is in</param>
        /// <param name="gameData">the game related data</param>
        /// <returns>the response string for the clients</returns>
        public static string BuildResponse(int respondID, string serverState, string gameData)
        {
            if (serverState == null)
            {
                serverState = string.Empty;
            }
            if (gameData == null)
            {
                gameData = string.Empty;
            }

            string readResponse = respondID.ToString() + "|" + serverState + "|" + gameData + "|END|";

            return readResponse;
        }

        /// <summary>
        /// this method will check the send response
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="respondMessage"></param>
        /// <returns>returns task handler</returns>
        public static async Task SendResponse(NetworkStream stream, string respondMessage)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(respondMessage);

            await stream.WriteAsync(bytes, 0, bytes.Length);

            await stream.FlushAsync();
        }
    }
}