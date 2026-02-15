


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessingGameServer.DAL
{
    internal class GameFileData
    {
        public string CheckSentence {  get; set; }
        public int TotalWords { get; set; }

        public List <string> CheckWords { get; set; }

        // constructor
        public GameFileData(string checkSentence, int totalWords, List<string> checkWords)
        {
            CheckSentence = checkSentence;
            TotalWords = totalWords;
            CheckWords = checkWords;

        }
    }
}
