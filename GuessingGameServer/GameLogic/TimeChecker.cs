/*
* FILE : TimeChecker.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
handles the time being made
 */

using System.Diagnostics;

namespace GuessingGameServer.GameLogic
{
    internal class TimeChecker
    {
        //where the time started on the state bag is checked to see if the user ran out of time

        /// <summary>
        /// this call the time and stop if
        /// </summary>
        /// <param name="gameState"></param>
        /// <returns></returns>
        public static bool TimeIsUp(GameStateInfo gameState)
        {
            if (gameState == null)
            {
                return true;
            }

            if(gameState.GameStopwatch == null)
            {
                return true;
            }

            double elapsedSeconds = gameState.GameStopwatch.Elapsed.TotalSeconds;

            if(elapsedSeconds >= gameState.TimeLimitSeconds)
            {
                return true;
            }

            return false;
        }

        
        private int checkTime;
        private string stopTime;

        /// <summary>
        /// this will check the time
        /// </summary>
        private static void checkTimer()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

            TimeSpan timeSpan = stopwatch.Elapsed;

            if (timeSpan.TotalSeconds < 1000)
            {
                stopwatch.Stop();
            }

            else if (timeSpan.TotalSeconds > 1000)
            {
                stopwatch.Restart();
            }
        }



    }


}