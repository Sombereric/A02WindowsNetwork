using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI
{
    class ClientSessionGame
    {   
        public Int32 sessionID;
        public string sentenceString;
        public string totalWordsExpected;
        public string findTheWords {  get; set; }
        public string guessWords { get; set; }

        public int timeRemaining = 0;

        public Boolean checkGameOver = false;

        private CancellationTokenSource timercts;

        public static void ResetFromTheServer()
        {
        }

        public static void StartTimerTask() 
        {

        }

    }

    
}
