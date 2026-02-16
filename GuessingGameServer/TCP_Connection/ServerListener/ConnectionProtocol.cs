/*
* FILE : ConnectionProtocol.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* handles each request made by the client to the server and the respective action
*/

using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Net;
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

                    string guidText = protocolMessage[1]; // the client guid
                    string timeSent = protocolMessage[2]; // time it was sent
                    string takeAction = protocolMessage[3]; // the action string
                    string actionData = protocolMessage[4]; // the data sent with action
                    string endToken = protocolMessage[5]; // should end

                    if (takeAction != "Login")
                    {
                        Console.WriteLine("BAD REQUEST 1");
                        break;
                    }

                    if (endToken != "END")
                    {
                        Console.WriteLine("BAD REQUEST 2");
                    }

                    Login(actionData, guidText, gameStateInfos);

                    //creates state bag and picks a random file
                    //each one of these will have the proper function call for the data that they should get
                    //for example this will handle the guid and other such items
                    //check the read.me for the details of each protocol
                    break;
                case "201": //guess game
                    serverResponseData = guessMade(gameStateInfos, protocolMessage[4], protocolMessage[1]);
                    break;
                case "202": //new game
                    serverResponseData = NewGame(gameStateInfos, protocolMessage[1]);
                    break;
                case "203": //Play Game
                    serverResponseData = NewGame(gameStateInfos, protocolMessage[1]);
                    break;
                case "204": //Quit Game
                    serverResponseData = QuitGame(gameStateInfos, protocolMessage[1]);
                    break;
                default:
                    break;
            }
            return serverResponseData; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameStateInfos"></param>
        /// <param name="guess"></param>
        /// <param name="clientGuid"></param>
        private string guessMade(List<GameStateInfo> gameStateInfos, string guess, string clientGuid)
        {
            string ResponseID = "";
            string ServerState = "";
            string gameRelatedData = "";

            //this would be where the guess is handled against the state bag. 
            if (guess == null || guess.Trim().Length == 0)
            {
                gameRelatedData = "Invalid Guess";
            }

            Guid parsedGuid;
            if (!Guid.TryParse(clientGuid, out parsedGuid))
            {
                ResponseID = "400";
                ServerState = "Unable to Parse Guid";
            }
            GameStateInfo stateInfo = null;

            for (int checkCount = 0; checkCount < gameStateInfos.Count; checkCount++)
            {
                if (gameStateInfos[checkCount] != null && gameStateInfos[checkCount].ClientGuid == parsedGuid)
                {
                    stateInfo = gameStateInfos[checkCount];
                    break;
                }

            }

            if (stateInfo == null)
            {
                ResponseID = "400";
                ServerState = "No state info found";
            }

            string cleanTheGuess = guess.Trim();

            int lookResult; // check the guess result

            lock (stateInfo.GameStateLocker)
            {
                lookResult = stateInfo.WordChecker(cleanTheGuess);

                if (lookResult == 0)
                {
                    stateInfo.AddToWordsFound(cleanTheGuess);

                    if (stateInfo.NumberOfWordsLeft > 0)
                    {
                        stateInfo.NumberOfWordsLeft--;
                    }

                    if (stateInfo.NumberOfWordsLeft == 0)
                    {
                        gameRelatedData = "WINNER";
                    }

                    gameRelatedData = "FOUND";

                }
                else if (lookResult == 1)
                {
                    gameRelatedData = "NOT FOUND";

                }
                else if (lookResult == 2)
                {
                    gameRelatedData = "ALREADY FOUND";
                }
            }

            if (ResponseID.Length == 0)
            {
                ResponseID = "200";
                ServerState = "Successful Guess";
            }
            return ResponseID + '|' + ServerState + '|' + gameRelatedData + "|END|";
        }

        /// <summary>
        /// this will handle the login logic
        /// </summary>
        /// <param name="userNamePasswordIpPort"></param>
        /// <param name="guidText"></param>
        /// <param name="gameStateInfos"></param>
        /// <param name="actionData"></param>
        private string Login(string userNamePasswordIpPort, string guidText, List<GameStateInfo> gameStateInfos)
        {
            string ResponseID = "";
            string ServerState = "";
            string gameRelatedData = "";

            if (userNamePasswordIpPort == null || userNamePasswordIpPort.Trim().Length == 0)
            {
                ResponseID = "400";
                ServerState = "No Login Data Passed";
            }

            Guid parsedGuid;
            if (!Guid.TryParse(guidText, out parsedGuid))
            {
                ResponseID = "400";
                ServerState = "Unable to Parse Guid";
            }

            // split the login data
            string[] checkParts = userNamePasswordIpPort.Split(':');

            if (checkParts.Length != 4)
            {
                ResponseID = "400";
                ServerState = "Invalid Action Data";
            }


            string ipText = checkParts[2];
            string portText = checkParts[3];

            IPAddress checkIP;
            Int32 checkPort;

            if (!IPAddress.TryParse(ipText, out checkIP))
            {
                ResponseID = "400";
                ServerState = "Invalid IP";
            }

            if(!Int32.TryParse(portText, out checkPort))
            {
                ResponseID = "400";
                ServerState = "Invalid Port";
            }

            // create new game state object
            GameStateInfo newState = new GameStateInfo();
            newState.ClientGuid = parsedGuid;
            newState.ClientIp = checkIP;
            newState.Port = checkPort;

            // add new state to list
            gameStateInfos.Add(newState);

            if (ResponseID.Length == 0)
            {
                ResponseID = "200";
                ServerState = "Successful Guess";
            }
            return ResponseID + '|' + ServerState + '|' + gameRelatedData + "|END|";
        }

        /// <summary>
        /// this will handles restting a game
        /// </summary>
        /// <param name="gameStateInfos"></param>
        /// <param name="guidText"></param>
        private string NewGame(List<GameStateInfo> gameStateInfos, string guidText)
        {
            string ResponseID = "";
            string ServerState = "";
            string gameRelatedData = "";

            Guid parsedGuid;
            if (!Guid.TryParse(guidText, out parsedGuid))
            {
                ResponseID = "400";
                ServerState = "Unable to parse Guid";
            }

            GameStateInfo stateInfo = null;

            // find matching state
            for (int checkCount = 0; checkCount < gameStateInfos.Count; checkCount++)
            {
                if (gameStateInfos[checkCount] != null &&gameStateInfos[checkCount].ClientGuid == parsedGuid)
                { 
                    stateInfo = gameStateInfos[checkCount];
                    break;
                }
            }

            if (stateInfo == null)
            {
                ResponseID = "400";
                ServerState = "No Action State Info";
            }

            // lock state during reset
            lock (stateInfo.GameStateLocker)
            {
                stateInfo.TotalWordsFound.Clear();
                stateInfo.TotalWordsToFind.Clear();
                stateInfo.NumberOfWordsLeft = 0;

                stateInfo.GameStopwatch.Reset();
                stateInfo.GameStopwatch.Start();
            }

            if (ResponseID.Length == 0)
            {
                ResponseID = "200";
                ServerState = "Successful Guess";
            }
            return ResponseID + '|' + ServerState + '|' + gameRelatedData + "|END|";
        }

        /// <summary>
        /// this will stop the game
        /// </summary>
        /// <param name="gameStateInfos"></param>
        /// <param name="guidText"></param>
        private string QuitGame(List<GameStateInfo> gameStateInfos, string guidText)
        {
            string ResponseID = "";
            string ServerState = "";
            string gameRelatedData = "";
            bool deleteSuccess = false;

            Guid parsedGuid;
            if (!Guid.TryParse(guidText, out parsedGuid))
            {
                ResponseID = "400";
                ServerState = "Unable to parse Guid";
            }

            for (int checkCount = 0; checkCount < gameStateInfos.Count; checkCount++)
            { 
                if(gameStateInfos[checkCount] != null && gameStateInfos[checkCount].ClientGuid == parsedGuid)
                {
                    gameStateInfos.RemoveAt(checkCount);
                    ResponseID = "200";
                    ServerState = "Successful Guess";
                    deleteSuccess = true;
                }
            }

            if (!deleteSuccess)
            {
                ResponseID = "400";
                ServerState = "Failure To Delete Client Info";
            }
            return ResponseID + '|' + ServerState + '|' + gameRelatedData + "|END|";
        }
    }
}
