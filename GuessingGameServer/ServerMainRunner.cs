using GuessingGameServer.GameLogic;
using GuessingGameServer.TCP_Connection.ServerListener;
using GuessingGameServer.TCP_Connection.ServerSender;

namespace GuessingGameServer
{
    internal class ServerMainRunner
    {
        private static readonly List<GameStateInfo> gameStatesInfo = new List<GameStateInfo>();
        public void mainRunner() 
        {
            ConnectionHandler connectionHandler = new ConnectionHandler();
            Task clientHandlerTask = Task.Run(async () => await connectionHandler.MainServerListener(gameStatesInfo));

            ServerClientSender sender = new ServerClientSender();
            //Task clientHandlerTask = Task.Run(async () => await connectionHandler.MainServerListener(gameStatesInfo));

            //starts listener and sender
        }
    }
}
