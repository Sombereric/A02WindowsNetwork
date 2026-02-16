/*
* FILE : ServerMainRunner.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
  this will run the server
 */

using GuessingGameServer.GameLogic;
using GuessingGameServer.TCP_Connection.ServerListener;
using GuessingGameServer.TCP_Connection.ServerSender;

namespace GuessingGameServer
{
    internal class ServerMainRunner
    {
        private static readonly List<GameStateInfo> gameStatesInfo = new List<GameStateInfo>();
        public readonly object GameStateLocker = new object();
        
        /// <summary>
        /// runs the server method
        /// </summary>
        /// <returns></returns>
        public async Task mainRunner() 
        {
            Console.WriteLine("server running");
            ConnectionHandler connectionHandler = new ConnectionHandler();

            await connectionHandler.MainServerListener(gameStatesInfo, GameStateLocker);

        }
    }
}
