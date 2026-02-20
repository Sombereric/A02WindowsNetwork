/*
* FILE : ConnectionProtocol.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* Responsible for loading game data from text files.
*/

using GuessingGameServer.UserInterface;

namespace GuessingGameServer.DAL
{
    internal class fileLoader
    {
        // UI object (currently unused, but could be used for logging)
        private UI ui = new UI();

        /// <summary>
        /// picks a random file to load
        /// </summary>
        /// <returns>the formated loaded data</returns>
        public static GameFileData LoadRandomGame()
        {
            // List of possible game files stored in the project directory
            string[] gameFiles = { "game1.txt", "game2.txt", "game3.txt", "game4.txt" };

            // Random generator used to select a file
            Random random = new Random();

            // Choose a random index from the file list
            int randomIndex = random.Next(gameFiles.Length);

            // Retrieve the selected file name
            string selectedFile = Path.Combine(gameFiles[randomIndex]);

            // Load the contents of the chosen file
            return LoadGameFile(selectedFile);
        }

       /// <summary>
       /// lodas the data from the file
       /// </summary>
       /// <param name="filePath">the file to laod from</param>
       /// <returns>the class of loaded data</returns>
        public static GameFileData? LoadGameFile(string filePath)
        {
            GameFileData gameFileData = null;

            // Used to detect if loading fails
            bool loaderFailure = false;

            try
            {
                // Loop runs until file is successfully loaded or failure occurs
                while (!loaderFailure)
                {
                    // Check if the file exists
                    if (!File.Exists(filePath))
                    {
                        loaderFailure = true;
                    }

                    // Read all lines from the file
                    string[] checkLine = File.ReadAllLines(filePath);

                    // Minimum valid file length is 3 lines:
                    // Sentence + separator + at least one word
                    int underLength = 3;

                    if (checkLine.Length < underLength)
                    {
                        loaderFailure = true;
                    }

                    // First line contains the main sentence for the game
                    string checkSentence = checkLine[0].Trim();

                    // List to store all valid guess words
                    List<string> wordsList = new List<string>();

                    // Words start at line index 2 (3rd line)
                    // Index 1 is skipped because it may be blank or unused
                    int wordStart = 2;

                    // Extract each word from the file and add to list
                    for (int checkCount = wordStart; checkCount < checkLine.Length; checkCount++)
                    {
                        string checkWord = checkLine[checkCount].Trim();

                        // Only add non-empty words
                        if (checkWord.Length > 0)
                        {
                            wordsList.Add(checkWord.ToLower());
                        }
                    }

                    // Total number of valid words loaded
                    int totalWords = wordsList.Count();

                    // Package everything into a GameFileData object
                    gameFileData = new GameFileData(checkSentence, totalWords, wordsList);

                    // Exit loop after successful load
                    break;
                }
            }
            catch (Exception ex)
            {
                // Log any unexpected file loading errors
                Console.WriteLine("Failure to load files " + ex.Message);
            }

            // Return loaded game data (or null if failed)
            return gameFileData;
        }
    }
}