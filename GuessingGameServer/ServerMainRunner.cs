using GuessingGameServer.GameLogic;
using GuessingGameServer.TCP_Connection.ServerListener;
using GuessingGameServer.TCP_Connection.ServerSender;

namespace GuessingGameServer
{
    internal class ServerMainRunner
    {
        private static readonly List<GameStateInfo> gameStatesInfo = new List<GameStateInfo>();
        public readonly object GameStateLocker = new object();
        public void mainRunner() 
        {
            Console.WriteLine("I cam");
            ConnectionHandler connectionHandler = new ConnectionHandler();
            Task clientHandlerTask = Task.Run(async () => await connectionHandler.MainServerListener(gameStatesInfo, GameStateLocker));


            Console.WriteLine("Started listener task. Press Enter to quit.");
            Console.ReadLine();

            //ServerClientSender sender = new ServerClientSender();
            //Task clientHandlerTask = Task.Run(async () => await connectionHandler.MainServerListener(gameStatesInfo));

            //starts listener and sender
        }
    }
}
