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
using GuessingGameServer.UserInterface;

namespace GuessingGameServer
{
    internal class ServerMainRunner
    {
        private readonly List<GameStateInfo> gameStatesInfo = new List<GameStateInfo>();
        private readonly object GameStateLocker = new object();

        /// <summary>
        /// runs the server method
        /// </summary>
        /// <returns>Life time of the server</returns>
        public async Task MainRunner() 
        {
            ConnectionHandler connectionHandler = new ConnectionHandler();
            await connectionHandler.MainServerListener(gameStatesInfo, GameStateLocker);
            return;
        }
    }
}