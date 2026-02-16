using System.Threading.Tasks;
using GuessingGameServer.UserInterface;

namespace GuessingGameServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ServerMainRunner serverMainRunner = new ServerMainRunner();
            serverMainRunner.mainRunner();

            await serverMainRunner.mainRunner();
        }
    }
}
