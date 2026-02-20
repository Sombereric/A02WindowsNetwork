/*
* FILE : ConnectionProtocol.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* handles each request made by the client to the server and the respective action
*/

using GuessingGameServer.UserInterface;

namespace GuessingGameServer.DAL
{
    internal class fileLoader
    {
        private UI ui = new UI();
        /// <summary>
        /// loads a random game file
        /// </summary>
        /// <returns>returns the data from said loaded file</returns>
        public static GameFileData LoadRandomGame()
        {
            string[] gameFiles = { "game1.txt", "game2.txt", "game3.txt", "game4.txt"};

            Random random = new Random();

            int randomIndex = random.Next(gameFiles.Length);

            string selectedFile = Path.Combine(gameFiles[randomIndex]); // combine file

            return LoadGameFile(selectedFile);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="Exception"></exception>
        public static GameFileData? LoadGameFile(string filePath)
        {
            GameFileData gameFileData = null;
            bool loaderFailure = false;
            try
            {
                while (!loaderFailure)
                {
                    if (!File.Exists(filePath))
                    {
                        loaderFailure = true;
                    }

                    string[] checkLine = File.ReadAllLines(filePath);

                    int underLength = 3; // made sure the minium is 3 for the lines

                    if (checkLine.Length < underLength)
                    {
                        loaderFailure = true;
                    }

                    string checkSentence = checkLine[0].Trim();

                    List<string> wordsList = new List<string>();

                    int wordStart = 2; // this will start the index number made sure i started at 2

                    for (int checkCount = wordStart; checkCount < checkLine.Length; checkCount++)
                    {
                        string checkWord = checkLine[checkCount].Trim();

                        if (checkWord.Length > 0)
                        {
                            wordsList.Add(checkWord.ToLower());
                        }
                    }

                    int totalWords = wordsList.Count();

                    gameFileData = new GameFileData(checkSentence, totalWords, wordsList);
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failure to load files " + ex.Message);
            }
            return gameFileData;
        }
    }
}