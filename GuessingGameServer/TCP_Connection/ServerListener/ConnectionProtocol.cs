/*
* FILE : ConnectionProtocol.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* handles each request made by the client to the server and the respective action
*/

using System.Net;
using GuessingGameServer.DAL;
using GuessingGameServer.GameLogic;

namespace GuessingGameServer.TCP_Connection.ServerListener
{
    internal class ConnectionProtocol
    {
        //code from protocol will be something from the client like 200, 300, something like that
        //this is what will be turned into a task to allow for multiple connections at once 
        public string ServerProtocolManager(string[] protocolMessage, List<GameStateInfo> gameStateInfos, object GameStateLocker)
        {
            string serverResponseData = "";

            //protocol Message format  Protocol ID|Client GUID|Time Sent|Action|Action Data|END|
            switch (protocolMessage[0])
            {
                case "200": //client login
                    serverResponseData = Login(protocolMessage[4], protocolMessage[1], gameStateInfos, GameStateLocker);
                    break;
                case "201": //guess game
                    serverResponseData = guessMade(gameStateInfos, protocolMessage[4], protocolMessage[1], GameStateLocker);
                    break;
                case "202": //new game
                    serverResponseData = NewGame(gameStateInfos, protocolMessage, GameStateLocker);
                    break;
                case "203": //Play Game
                    serverResponseData = NewGame(gameStateInfos, protocolMessage, GameStateLocker);
                    break;
                case "204": //Quit Game
                    serverResponseData = QuitGame(gameStateInfos, protocolMessage[1], GameStateLocker);
                    break;
                default:
                    serverResponseData = "400" + '|' + "Illegal Request" + '|' + "invalid" + "|END|";
                    break;
            }
            return serverResponseData;
        }

        /// <summary>
        /// this will handle the login logic
        /// </summary>
        /// <param name="userNamePasswordIpPort"></param>
        /// <param name="guidText"></param>
        /// <param name="gameStateInfos"></param>
        /// <param name="actionData"></param>
        private string Login(string userNamePasswordIpPort, string guidText, List<GameStateInfo> gameStateInfos, object GameStateLocker)
        {
            bool requestFailure = false;

            string ResponseID = "";
            string ServerState = "";
            string gameRelatedData = "";

            if (userNamePasswordIpPort == null || userNamePasswordIpPort.Trim().Length == 0)
            {
                ResponseID = "400";
                ServerState = "No Login Data Passed";
                requestFailure = true;
            }

            Guid parsedGuid;
            if (!Guid.TryParse(guidText, out parsedGuid))
            {
                ResponseID = "400";
                ServerState = "Unable to Parse Guid";
                requestFailure = true;
            }

            // split the login data
            string[] checkParts = userNamePasswordIpPort.Split(':');

            if (checkParts.Length != 3)
            {
                ResponseID = "400";
                ServerState = "Invalid Action Data";
                requestFailure = true;
            }

            if (!requestFailure)
            {
                string ipText = checkParts[1];
                string portText = checkParts[2];

                IPAddress checkIP;
                Int32 checkPort;

                if (!IPAddress.TryParse(ipText, out checkIP))
                {
                    ResponseID = "400";
                    ServerState = "Invalid IP";
                    requestFailure = true;
                }

                if (!Int32.TryParse(portText, out checkPort))
                {
                    ResponseID = "400";
                    ServerState = "Invalid Port";
                    requestFailure = true;
                }

                // create new game state object
                GameStateInfo newState = new GameStateInfo();
                newState.ClientGuid = parsedGuid;
                newState.ClientIp = checkIP;
                newState.Port = checkPort;

                // add new state to list
                lock (GameStateLocker)
                {
                    gameStateInfos.Add(newState);
                }
                ;

                if (ResponseID.Length == 0)
                {
                    ResponseID = "200";
                    ServerState = "Successful Login";
                }
            }
            return ResponseID + '|' + ServerState + '|' + gameRelatedData + "|END|";
        }

        /// <summary>
        /// this method will made the guess
        /// </summary>
        /// <param name="gameStateInfos"></param>
        /// <param name="guess"></param>
        /// <param name="clientGuid"></param>
        private string guessMade(List<GameStateInfo> gameStateInfos, string guess, string clientGuid, object GameStateLocker)
        {
            bool requestFailure = false;

            string ResponseID = "";
            string ServerState = "";
            string gameRelatedData = "";

            if (!requestFailure)
            {
                //this would be where the guess is handled against the state bag. 
                if (guess == null || guess.Trim().Length == 0)
                {
                    ResponseID = "400";
                    gameRelatedData = "Invalid Guess";
                    requestFailure = true;
                }

                Guid parsedGuid;
                if (!Guid.TryParse(clientGuid, out parsedGuid))
                {
                    ResponseID = "400";
                    ServerState = "Unable to Parse Guid";
                    requestFailure = true;
                }
                GameStateInfo stateInfo = null;

                lock (GameStateLocker)
                {
                    for (int checkCount = 0; checkCount < gameStateInfos.Count; checkCount++)
                    {
                        if (gameStateInfos[checkCount] != null && gameStateInfos[checkCount].ClientGuid == parsedGuid)
                        {
                            stateInfo = gameStateInfos[checkCount];
                            break;
                        }
                    }
                }

                if (stateInfo == null)
                {
                    ResponseID = "400";
                    ServerState = "No state info found";
                    requestFailure = true;
                }
                string cleanTheGuess = "";

                if (guess != null)
                {
                    cleanTheGuess = guess.Trim();
                }


                if (stateInfo != null)
                {
                    lock (stateInfo.GameStateLocker)
                    {
                        gameRelatedData = foundWordEditor(stateInfo, cleanTheGuess);
                    }
                }

                if (ResponseID.Length == 0)
                {
                    ResponseID = "200";
                    ServerState = "Successful Guess";
                }
            }
            return ResponseID + '|' + ServerState + '|' + gameRelatedData + "|END|";
        }
        /// <summary>
        /// handles the checking of the currently found words
        /// </summary>
        /// <param name="stateInfo">the current client to check</param>
        /// <param name="cleanTheGuess">the cleaned guess</param>
        /// <param name="gameRelatedData">the server readable response</param>
        /// <returns>returns the response of the search</returns>
        private string foundWordEditor(GameStateInfo stateInfo, string cleanTheGuess)
        {
            string gameRelatedData = "";
            int lookResult = stateInfo.WordChecker(cleanTheGuess); // check the guess result

            if (lookResult == 0)
            {
                stateInfo.AddToWordsFound(cleanTheGuess);

                if (stateInfo.NumberOfWordsLeft > 0)
                {
                    stateInfo.NumberOfWordsLeft--;
                    gameRelatedData = "FOUND" + ':' + stateInfo.NumberOfWordsLeft;
                }

                if (stateInfo.NumberOfWordsLeft == 0)
                {
                    gameRelatedData = "WINNER";
                }

                foreach (string word in stateInfo.TotalWordsFound)
                {
                    gameRelatedData += ':' + word;
                }
            }
            else if (lookResult == 1)
            {
                gameRelatedData = "NOT FOUND";

            }
            else if (lookResult == 2)
            {
                gameRelatedData = "ALREADY FOUND";
            }
            return gameRelatedData;
        }

        /// <summary>
        /// this will handles restting a game
        /// </summary>
        /// <param name="gameStateInfos"></param>
        /// <param name="guidText"></param>
        private string NewGame(List<GameStateInfo> gameStateInfos, string[] protocolMessage, object GameStateLocker)
        {
            bool requestFailure = false;

            string ResponseID = "";
            string ServerState = "";
            string gameRelatedData = "";

            while (!requestFailure)
            {
                Guid parsedGuid;
                if (!Guid.TryParse(protocolMessage[1], out parsedGuid))
                {
                    ResponseID = "400";
                    ServerState = "Unable to parse Guid";
                    requestFailure = true;
                }

                GameStateInfo stateInfo = null;
                lock (GameStateLocker)
                {
                    // find matching state
                    for (int checkCount = 0; checkCount < gameStateInfos.Count; checkCount++)
                    {
                        if (gameStateInfos[checkCount] != null && gameStateInfos[checkCount].ClientGuid == parsedGuid)
                        {
                            stateInfo = gameStateInfos[checkCount];
                            break;
                        }
                    }
                }

                char delimiter = ':';
                string[] actionData = protocolMessage[4].Split(delimiter);

                if (actionData.Length != 3)
                {
                    ResponseID = "400";
                    ServerState = "Unable to parse action Data";
                    requestFailure = true;
                }

                if (!Int32.TryParse(actionData[2], out int port))
                {
                    ResponseID = "400";
                    ServerState = "Unable to parse port";
                    requestFailure = true;
                }
                if (!IPAddress.TryParse(actionData[1], out IPAddress ipAddress))
                {
                    ResponseID = "400";
                    ServerState = "Unable to parse ipAddress";
                    requestFailure = true;
                }

                fileLoader fileLoader = new fileLoader();
                GameFileData gameFileData = fileLoader.LoadRandomGame();

                gameRelatedData = gameFileData.CheckSentence + ':' + gameFileData.TotalWords;

                if (stateInfo == null)
                {
                    lock (GameStateLocker)
                    {
                        GameStateInfo gameStateInfo = new GameStateInfo();
                        gameStateInfo.ClientGuid = parsedGuid;
                        gameStateInfo.Port = port;
                        gameStateInfo.ClientIp = ipAddress;
                        gameStateInfo.TotalWordsToFind = gameFileData.CheckWords;
                        gameStateInfo.NumberOfWordsLeft = gameFileData.TotalWords;
                        gameStateInfos.Add(gameStateInfo);
                    }
                }
                //if new player
                else
                {
                    // lock state during reset
                    lock (stateInfo.GameStateLocker)
                    {
                        stateInfo.TotalWordsFound.Clear();
                        stateInfo.TotalWordsToFind.Clear();
                        stateInfo.NumberOfWordsLeft = 0;
                        stateInfo.TotalWordsToFind = gameFileData.CheckWords;
                        stateInfo.NumberOfWordsLeft = gameFileData.TotalWords;
                        stateInfo.GameStopwatch.Reset();
                        stateInfo.GameStopwatch.Start();
                    }
                }

                if (ResponseID.Length == 0)
                {
                    ResponseID = "200";
                    ServerState = "Successful Game State Update";
                }
                break;
            }
            return ResponseID + '|' + ServerState + '|' + gameRelatedData + "|END|";
        }

        /// <summary>
        /// this will stop the game
        /// </summary>
        /// <param name="gameStateInfos"></param>
        /// <param name="guidText"></param>
        private string QuitGame(List<GameStateInfo> gameStateInfos, string guidText, object GameStateLocker)
        {
            bool parseFailure = false;

            string ResponseID = "";
            string ServerState = "";
            string gameRelatedData = "";
            bool deleteSuccess = false;

            Guid parsedGuid;
            if (!Guid.TryParse(guidText, out parsedGuid))
            {
                ResponseID = "400";
                ServerState = "Unable to parse Guid";
                parseFailure = true;
            }
            if (!parseFailure)
            {
                lock (GameStateLocker)
                {
                    for (int checkCount = 0; checkCount < gameStateInfos.Count; checkCount++)
                    {
                        if (gameStateInfos[checkCount] != null && gameStateInfos[checkCount].ClientGuid == parsedGuid)
                        {
                            gameStateInfos.RemoveAt(checkCount);
                            ResponseID = "200";
                            ServerState = "Game client Removed";
                            deleteSuccess = true;
                            break;
                        }
                    }
                }

                if (!deleteSuccess)
                {
                    ResponseID = "400";
                    ServerState = "Failure To Delete Client Info";
                }
            }
            return ResponseID + '|' + ServerState + '|' + gameRelatedData + "|END|";
        }
    }
}
