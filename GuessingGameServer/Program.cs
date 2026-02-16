using GuessingGameServer.UserInterface;

namespace GuessingGameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServerMainRunner serverMainRunner = new ServerMainRunner();
            serverMainRunner.mainRunner();

            UI uI = new UI();
            Console.WriteLine(uI.ToString());

        }
    }
}
