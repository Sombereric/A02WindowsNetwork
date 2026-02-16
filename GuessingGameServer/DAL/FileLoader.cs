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
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessingGameServer.DAL
{
    internal class fileLoader
    {

        private static readonly string filePath = ConfigurationManager.AppSettings["FilePath"];


        /// <summary>
        /// 
        /// </summary>
        public static GameFileData LoadRandomGame()
        {
            string[] gameFiles = { "game1.txt", "game2.txt", "game3.txt", "game4.txt"};

            Random random = new Random();

            int randomIndex = random.Next(gameFiles.Length);

            string selectedFile = Path.Combine(filePath, gameFiles[randomIndex]); // combine file

            return LoadGameFile(selectedFile);


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="Exception"></exception>
        public static GameFileData? LoadGameFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            string[] checkLine = File.ReadAllLines(filePath);

            int underLength = 3; // made sure the minium is 3 for the lines

            if(checkLine.Length < underLength)
            {
                return null;
            }

            string checkSentence = checkLine[0].Trim();

            int totalWords = int.Parse(checkLine[1]);

            List<string> wordsList = new List<string>();

            int wordStart = 2; // this will start the index number made sure i started at 2
            
            for (int checkCount = wordStart; checkCount < checkLine.Length; checkCount++)
            {
                string checkWord = checkLine[checkCount].Trim();

                if (checkWord.Length > 0)
                {
                    wordsList.Add(checkWord);
                }
            }

            return new GameFileData(checkSentence, totalWords, wordsList);
        }
    }
}
