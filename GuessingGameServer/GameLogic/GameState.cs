using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GuessingGameServer.GameLogic
{
    internal class GameStateInfo
    {
        //Game States unique id
        public Guid ClientGuid;

        //clients port and ip used to allow the server to communicate with players
        public IPAddress ClientIp;
        public Int32 Port;

        //game Stats
        public int NumberOfWordsLeft;
        public int GameFile;
        public DateTime GameStart;
        public List<string> TotalWordsToFind = new List<string>();
        public List<string> TotalWordsFound = new List<string>();

        // timer to stop the game
        public Stopwatch GameStopwatch;
        public int TimeLimitSeconds;

        //allows multiple game states to be edited at once but not multiple edits to one game state
        public readonly object GameStateLocker = new object();
        /// <summary>
        /// checks if a guessed word is already found or not
        /// </summary>
        /// <param name="GuessedWord">the word that the user guessed</param>
        /// <returns>the result of the guess. 0 = new found word, 1 = not a found word, 2 = already found</returns>
        public int WordChecker(string GuessedWord)
        {
            int guessedWordResult = 0;

            if (!CheckIfNewWord(GuessedWord))
            {
                //flags the word as not found
                guessedWordResult = 1;
            }
            else if (CheckIfWordAlreadyFound(GuessedWord))
            {
                //flags the word as already found
                guessedWordResult = 2;
            }           
            return guessedWordResult;
        }
        /// <summary>
        /// checks the guessed word against the list of available words in the current game
        /// </summary>
        /// <param name="GuessedWord">the guessed word</param>
        /// <returns>if the word is new or not</returns>
        public bool CheckIfNewWord(string GuessedWord)
        {
            bool NewWord = false;

            foreach(string word in TotalWordsToFind)
            {
                if (word == GuessedWord)
                {
                    NewWord = true;
                }
            }
            return NewWord;
        }
        /// <summary>
        /// checks to see if the user has already found the word
        /// </summary>
        /// <param name="GuessedWord">the guessed word</param>
        /// <returns>if the word is already found</returns>
        public bool CheckIfWordAlreadyFound(string GuessedWord)
        {
            bool AlreadyFound = false;

            foreach (string word in TotalWordsFound)
            {
                if (word == GuessedWord)
                {
                    AlreadyFound = true;
                }
            }
            return AlreadyFound;
        }
        /// <summary>
        /// a function to add the newly found word to the list of found words
        /// </summary>
        /// <param name="GuessedWord">the word to add</param>
        public void AddToWordsFound(string GuessedWord)
        {
            lock (GameStateLocker)
            {
                TotalWordsFound.Add(GuessedWord);
            }
            return;
        }
    }
}
