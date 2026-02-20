/*
* FILE : GameFileData.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* the data formated into a class
*/

namespace GuessingGameServer.DAL
{
    internal class GameFileData
    {
        public string CheckSentence {  get; set; }
        public int TotalWords { get; set; }
        public List <string> CheckWords { get; set; }

        /// <summary>
        /// constructor for the loaded game file
        /// </summary>
        /// <param name="checkSentence">the sentence to guess from</param>
        /// <param name="totalWords">the number of hidden words</param>
        /// <param name="checkWords">the list of words to find</param>
        public GameFileData(string checkSentence, int totalWords, List<string> checkWords)
        {
            CheckSentence = checkSentence;
            TotalWords = totalWords;
            CheckWords = checkWords;
        }
    }
}