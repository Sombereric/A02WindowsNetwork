using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        
    }
}
