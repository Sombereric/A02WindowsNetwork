/*
* FILE : Program.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
  the main program that calls the runner
 */

namespace GuessingGameServer
{
    internal class Program
    {
        /// <summary>
        /// Where the program starts
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <returns>A task representing the lifetime of the server.</returns>
        static async Task Main(string[] args)
        {
            ServerMainRunner serverMainRunner = new ServerMainRunner();
            await serverMainRunner.MainRunner();
            return;
        }
    }
}