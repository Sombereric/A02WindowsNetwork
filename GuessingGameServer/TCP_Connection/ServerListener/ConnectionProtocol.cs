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
        /// <summary>
        /// the protocol manager
        /// </summary>
        /// <param name="protocolMessage">the split sent data</param>
        /// <param name="gameStateInfos">list of game states</param>
        /// <param name="GameStateLocker">the locker to protect the gameStateInfos</param>
        /// <returns>the formated return string</returns>
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
            //returns the build response data string
            return serverResponseData;
        }
        /// <summary>
        /// a new user logins 
        /// </summary>
        /// <param name="userNamePasswordIpPort">the user data used to login</param>
        /// <param name="guidText">the unique id</param>
        /// <param name="gameStateInfos">the gamestate list</param>
        /// <param name="GameStateLocker">the locker of the game state list</param>
        /// <returns>the response string for the client</returns>
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
            //attempts to parse the guid
            Guid parsedGuid;
            if (!Guid.TryParse(guidText, out parsedGuid))
            {
                ResponseID = "400";
                ServerState = "Unable to Parse Guid";
                requestFailure = true;
            }

            // split the login data
            string[] checkParts = userNamePasswordIpPort.Split(':');

            //invalid data sent
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
                //attempts to parse user ip and port
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


                fileLoader fileLoader = new fileLoader();
                GameFileData gameFileData = fileLoader.LoadRandomGame();

                gameRelatedData = gameFileData.CheckSentence + ':' + gameFileData.TotalWords;
                GameStateInfo gameStateInfo = new GameStateInfo();

                lock (GameStateLocker)
                {
                    // create new game state object
                    gameStateInfo.ClientGuid = parsedGuid;
                    gameStateInfo.Port = checkPort;
                    gameStateInfo.ClientIp = checkIP;
                    gameStateInfo.TotalWordsToFind = gameFileData.CheckWords;
                    gameStateInfo.NumberOfWordsLeft = gameFileData.TotalWords;
                    gameStateInfos.Add(gameStateInfo);
                }

                // add new state to list
                lock (GameStateLocker)
                {
                    gameStateInfos.Add(gameStateInfo);
                }
                //if the responseid length is zero that means it succeeded
                if (ResponseID.Length == 0)
                {
                    ResponseID = "200";
                    ServerState = "Successful Login";
                }
            }
            //where the response string 
            return ResponseID + '|' + ServerState + '|' + gameRelatedData + "|END|";
        }
        /// <summary>
        /// this method will made the guess
        /// </summary>
        /// <param name="gameStateInfos">the list of current users</param>
        /// <param name="guess">the guess the user made</param>
        /// <param name="clientGuid">the unique id of the user</param>
        /// <returns>returns the response of the search</returns>
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

                //attempts to parse the guid
                Guid parsedGuid;
                if (!Guid.TryParse(clientGuid, out parsedGuid))
                {
                    ResponseID = "400";
                    ServerState = "Unable to Parse Guid";
                    requestFailure = true;
                }

                GameStateInfo stateInfo = null;
                //checks for the current user loggined 
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

                //if the gamestate was failed
                if (stateInfo == null)
                {
                    ResponseID = "400";
                    ServerState = "No state info found";
                    requestFailure = true;
                }
                string cleanTheGuess = "";

                //cleans the white space from the word
                if (guess != null)
                {
                    cleanTheGuess = guess.Trim();
                }

                //checks to see if the word was found or not
                if (stateInfo != null)
                {
                    lock (stateInfo.GameStateLocker)
                    {
                        gameRelatedData = foundWordEditor(stateInfo, cleanTheGuess);
                    }
                }
                //if the responseid is zero that means the game creation has succeeded
                if (ResponseID.Length == 0)
                {
                    ResponseID = "200";
                    ServerState = "Successful Guess";
                }
            }
            //what is sent to the clients as a response from the server
            return ResponseID + '|' + ServerState + '|' + gameRelatedData + "|END|";
        }
        /// <summary>
        /// handles the checking of the currently found words
        /// </summary>
        /// <param name="stateInfo">the current client to check</param>
        /// <param name="cleanTheGuess">the cleaned guess</param>
        /// <returns>returns the response of the search</returns>
        private string foundWordEditor(GameStateInfo stateInfo, string cleanTheGuess)
        {
            string gameRelatedData = "";
            int lookResult = stateInfo.WordChecker(cleanTheGuess); // check the guess result

            //determines if the word was found or not within the lists
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
        /// handles new games for new players or play again presses
        /// </summary>
        /// <param name="gameStateInfos">the list of current users</param>
        /// <param name="protocolMessage">the list of data sent from the client</param>
        /// <param name="GameStateLocker">a locker to protect the list of game states</param>
        /// <returns>returns a formated response to the client</returns>
        private string NewGame(List<GameStateInfo> gameStateInfos, string[] protocolMessage, object GameStateLocker)
        {
            bool requestFailure = false;

            string ResponseID = "";
            string ServerState = "";
            string gameRelatedData = "";

            while (!requestFailure)
            {
                //attempts to parse guid and exits loop if fails
                Guid parsedGuid;
                if (!Guid.TryParse(protocolMessage[1], out parsedGuid))
                {
                    ResponseID = "400";
                    ServerState = "Unable to parse Guid";
                    requestFailure = true;
                }

                //protects the game state data
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

                //attempts to parse data and exits loop if fails
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

                //if player pressed play again
                // lock state during reset
                lock (stateInfo.GameStateLocker)
                {
                    //adds all the game state items
                    stateInfo.NumberOfWordsLeft = 0;
                
                    stateInfo.TotalWordsFound.Clear();
                    stateInfo.TotalWordsToFind.Clear();
                    stateInfo.GameStopwatch.Reset();
                    stateInfo.GameStopwatch.Start();
                
                    stateInfo.TotalWordsToFind = gameFileData.CheckWords;
                    stateInfo.NumberOfWordsLeft = gameFileData.TotalWords;
                }
                //if the responseid is 0 that means it did not fail and needs to be filled out
                if (ResponseID.Length == 0)
                {
                    ResponseID = "200";
                    ServerState = "Successful Game State Update";
                }
                //breaks out of the one through loop
                break;
            }
            //what is sent to the clients as a response from the server
            return ResponseID + '|' + ServerState + '|' + gameRelatedData + "|END|";
        }
        /// <summary>
        /// when a client quits the game it removes them from the server list 
        /// </summary>
        /// <param name="gameStateInfos">the list of current users</param>
        /// <param name="guidText">the guid of the client who wishes to quit</param>
        /// <param name="GameStateLocker">a locker to protect the list of game states</param>
        /// <returns>returns a formated response to the client</returns>
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
                //searches the list for the matching GUID
                lock (GameStateLocker)
                {
                    for (int checkCount = 0; checkCount < gameStateInfos.Count; checkCount++)
                    {
                        //if the GUID is found delete the user and exit the list
                        if (gameStateInfos[checkCount] != null && gameStateInfos[checkCount].ClientGuid == parsedGuid)
                        {
                            gameStateInfos.RemoveAt(checkCount);
                            ResponseID = "200";
                            ServerState = "Game client Removed";
                            deleteSuccess = true;
                            //breaks out the for loop
                            break;
                        }
                    }
                }
                //if the delete from the list was a failure
                if (!deleteSuccess)
                {
                    ResponseID = "400";
                    ServerState = "Failure To Delete Client Info";
                }
            }
            //what is sent to the clients as a response from the server
            return ResponseID + '|' + ServerState + '|' + gameRelatedData + "|END|";
        }
    }
}