/*
* FILE : ServerClientSender.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* handles the response of the client sender
*/

using System.Net.Sockets;
using System.Text;

namespace GuessingGameServer.TCP_Connection.ServerSender
{
    internal class ServerClientSender
    {

        /// <summary>
        ///  this will just the response from the client to the server
        /// </summary>
        /// <param name="respondID"></param>
        /// <param name="serverState"></param>
        /// <param name="gameData"></param>
        /// <returns></returns>
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
        /// <returns></returns>
        public static async Task SendResponse(NetworkStream stream, string respondMessage)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(respondMessage + "\n");

            await stream.WriteAsync(bytes, 0, bytes.Length);

            await stream.FlushAsync();
        }
    }
}