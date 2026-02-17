/*
* FILE : ConnectionProtocol.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* handles each request made by the client to the server and the respective action
*/


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
