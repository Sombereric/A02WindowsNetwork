/*
* FILE : FileLoader.cs
* PROJECT : PROG2126 - Assignment #1
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-1-25
* DESCRIPTION :
* Where the game file is loaded for the game
*/

using System.Configuration;
using GuessingGameServer.UserInterface;

namespace GuessingGameServer.DAL
{
    internal class fileLoader
    {
        UI uI = new UI();

        private string filePath;
        private static readonly object locker = new object();
        
        public string[] fileReader(int fileToLoad)
        {
            string[] gameData;
            string gameFileData = "";

            switch (fileToLoad)
            {
                case 0:
                    filePath = ConfigurationManager.AppSettings["GameFile1"];
                    break;
                case 1:
                    filePath = ConfigurationManager.AppSettings["GameFile2"];
                    break;
                case 2:
                    filePath = ConfigurationManager.AppSettings["GameFile3"];
                    break;
                case 3:
                    filePath = ConfigurationManager.AppSettings["GameFile4"];
                    break;
                default:
                    break;
            }

           StreamReader reader = new StreamReader(filePath);
        
            lock (locker)
            {
                try
                {
                    gameFileData = reader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    uI.WriteToConsole(ex.Message);
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    } 
                }
            }

            gameData = gameFileData.Split('|');

            return gameData;
        }
    }
}
