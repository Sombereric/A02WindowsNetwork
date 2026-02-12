using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ClientUI.GameLogic
{
    class ClientSessionGame
    {
        public Guid ClientIDGuid { get; set; }
       
        public string? checkSentence;
        public int totalWordsExpected {  get; set; }
        public int findTheWords {  get; set; }
        List<string> guessWordList;
        List<string> foundWords;

        public int timeRemaining = 0;

        public bool checkGameOver = false;

        private CancellationTokenSource? timerCts;

        private Stopwatch? gameStopwatch;

        // constructor
        public ClientSessionGame()
        {
            ClientIDGuid = Guid.NewGuid();
            checkSentence = string.Empty;
            totalWordsExpected = 0;
            findTheWords = 0;

            guessWordList = new List<string>();
            foundWords = new List<string>();

            timeRemaining = 0;
            checkGameOver = false;
            timerCts = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetFromTheServer()
        {   
            
            
            if(timerCts != null)
            {
               timerCts.Cancel();
               timerCts.Dispose();
               timerCts = null;
            }

            checkSentence = string.Empty;
            
            totalWordsExpected = 0;
            
            findTheWords = 0;

            
            guessWordList.Clear();
            
            foundWords.Clear();
            
            timeRemaining = 0;
            
            checkGameOver = true;
            return;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameData"></param>
        public void StartTheResponseGameData(Dictionary<string, string> gameData)
        {
            bool validData = true;

            checkSentence = string.Empty;

            totalWordsExpected = 0;

            timeRemaining = 0;
            
            if (gameData == null)
            {
                validData = false;
            }
            
            if (validData == true)
            {
                if (gameData.ContainsKey("SENTENCE") == true)
                {
                    checkSentence = gameData["SENTENCE"];

                }

                if (gameData.ContainsKey("TOTALWORDS") == true)
                {
                    string totalWordsText = gameData["TOTALWORDS"];
                    
                    int parsedTotalWords = 0;

                    if (int.TryParse(totalWordsText, out parsedTotalWords) == true)
                    {
                        totalWordsExpected = parsedTotalWords;
                    }
                }

            if (gameData.ContainsKey("TIME_LIMIT") == true)
            {
                string checkTime = gameData["TIME_LIMIT"];
                
                int parsedTime = 0;
                
                if (int.TryParse(checkTime, out parsedTime) == true)
                {
                    timeRemaining = parsedTime;
                }


                guessWordList.Clear();
                foundWords.Clear();

                if(checkSentence.Length == 0 || totalWordsExpected <= 0 || timeRemaining <= 0)
                {
                   checkGameOver = true;
                }

                else
                { 
                    checkGameOver = false;    
                }

              
            }

            else
            {
               checkGameOver = true;    
            }

               
        }
            return;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkWord"></param>
        public void TryAddGuess(string checkWord)
        {   
            bool pleaseExit = false;

            string normalizedTheWord = string.Empty;

            if (checkWord == null)
            {
                pleaseExit = true;
            }
            else
            {
                normalizedTheWord = checkWord.Trim().ToLower();

                if (normalizedTheWord.Length == 0)
                {
                    pleaseExit = true;
                }

                else if (guessWordList.Contains(normalizedTheWord) == true)
                {
                    pleaseExit = true;

                }
            }

            if(pleaseExit == false)
            {
                guessWordList.Add(normalizedTheWord);
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameData"></param>
        public void ApplyGuessResponseData(Dictionary<string, string> gameData)
        {
           bool isCorrect = false;
           
           bool isGameOverFromTheServer = false;
           
           string checkResult = string.Empty;

            if (gameData == null)
            {
                return;
            }

            if(gameData.ContainsKey("RESULT") == true)
            {
                checkResult = gameData["RESULT"];

                if(checkResult.ToUpper() == "CORRECT")
                {
                    isCorrect = true;
                }
            }

            if(gameData.ContainsKey("GAME_OVER") == true)
            {
                if (gameData["GAME_OVER"].ToUpper() == "TRUE")
                {
                    isGameOverFromTheServer = true;
                }
            }

            if (isCorrect == true)
            {
                if (guessWordList.Count > 0)
                {
                    string lastGuess = guessWordList[guessWordList.Count - 1];

                    if (foundWords.Contains(lastGuess) == false)
                    { 
                        foundWords.Add(lastGuess);
                    }
                }
            }

            if (isGameOverFromTheServer == true)
            { 
                checkGameOver = true;
            }

            return;
        }


        public void StartTimerTask()
        {
            bool canStartTime = true;

            if (timerCts != null)
            {
                canStartTime = false;
            }

            if (timeRemaining <= 0)
            {
                canStartTime = false;
            }

            if (checkGameOver == true)
            {
                canStartTime = false;
            }

            if(canStartTime == true)
            {
                timerCts = new CancellationTokenSource();
                
                CancellationToken token = timerCts.Token;

                gameStopwatch = new Stopwatch();
                gameStopwatch.Start();

                Task timerTask = Task.Run(async () =>
                {
                    while (token.IsCancellationRequested == false)
                    {
                        int elapsedSeconds = (int)gameStopwatch.Elapsed.TotalSeconds;

                        int checkRemaing = timeRemaining - elapsedSeconds;

                        if (checkRemaing <= 0)
                        {
                            timeRemaining = 0;
                            checkGameOver = true;
                            break;
                        }

                        await Task.Delay(200, token);
                    }
                }, token);
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopTimer()
        {
            if (timerCts != null)
            {
                timerCts.Cancel();
                timerCts.Dispose();
                timerCts = null;
            }

            if (gameStopwatch != null)
            {
                gameStopwatch.Stop();
                gameStopwatch = null;
            }

            return;
        }
    }


    
}
