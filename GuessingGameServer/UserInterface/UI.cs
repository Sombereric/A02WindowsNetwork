/*
* FILE : UI.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-10
* DESCRIPTION :
* all ui writes and reads
*/

namespace GuessingGameServer.UserInterface
{
    internal class UI
    {
        /// <summary>
        /// writes things to the console on the server
        /// </summary>
        /// <param name="message">the message to write</param>
        public void WriteToConsole(string message)
        {
            Console.WriteLine(message);
            return;
        }
        /// <summary>
        /// reads input from console
        /// </summary>
        public void ReadFromConsole()
        {
            Console.ReadLine();
            return;
        }
    }
}
