using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace ClientUI
{
    class Protocol
    {
        private int LoginPing;
        private int startTheGame;
        private int guessTheWord;
        private int playAgain;
        private int quitTheGame;
       
        
        private Guid protocolID { get; set; }
        private string? UserName { get; set; }
        private string? Password { get; set; }
        
        private Int32 checkIP;
        private Int32 checkPort;
        
        private Guid sessionID { get; set; }
        
        private string? checkMessage { get; set; }
        private string? serverState { get; set; }
        private string? seeSentence { get; set; }
        private string? totalWords { get; set; }
        private int timerInSeconds { get; set; }
        private string findWords { get; set; }
        private string guessTheWords { get; set; }
        private int guess {  get; set; }
        private string checkResult { get; set; }
        private string endTheGame { get; set; }


        internal Protocol()
        {
            protocolID = Guid.NewGuid();
            UserName = string.Empty;
            Password = string.Empty;
            sessionID = Guid.NewGuid();
            checkMessage = string.Empty;
            serverState = string.Empty;
            seeSentence = string.Empty;
            totalWords = string.Empty;
            timerInSeconds = 0;
            findWords = string.Empty;
            guessTheWords = string.Empty;
            guess = 0;
            checkResult = string.Empty;
            endTheGame = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkDictionary"></param>
        /// <returns></returns>
       public static string BuildMessage(Dictionary<string, string> checkDictionary)
       {
            StringBuilder sb = new StringBuilder();
            
            foreach (KeyValuePair<string, string> checkKey in checkDictionary)
            {
                sb.AppendLine(checkKey + ":"+ checkKey.Value);

            }
            sb.Append("END");

            return sb.ToString();

       }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawText"></param>
        /// <returns></returns>
        private static Dictionary<string, string> RawMessage(string rawText)
        {
            Dictionary<string, string> checkFields = new Dictionary<string, string>();

            string[] lookLines = rawText.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (string lookLine in lookLines)
            {
                if (lookLine.Trim() == "END")
                {
                    break;
                }



                string[] checkParts = lookLine.Split(':', 2);

                if (checkParts.Length == 2)
                {
                    string lookKey = checkParts[0].Trim();
                    string lookValue = checkParts[1].Trim();

                    checkFields[checkParts[0].Trim()] = checkParts[1].Trim();
                }

            }
            return checkFields;

        }
       
    }

}
