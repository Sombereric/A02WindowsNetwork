using GuessingGameServer.GameLogic;
using GuessingGameServer.TCP_Connection.ServerListener;
using GuessingGameServer.TCP_Connection.ServerSender;

namespace GuessingGameServer
{
    internal class ServerMainRunner
    {
        private static readonly List<GameStateInfo> gameStatesInfo = new List<GameStateInfo>();
        public readonly object GameStateLocker = new object();
        public async Task mainRunner() 
        {
            ConnectionHandler connectionHandler = new ConnectionHandler();

            await connectionHandler.MainServerListener(gameStatesInfo, GameStateLocker);

        }
    }
}
