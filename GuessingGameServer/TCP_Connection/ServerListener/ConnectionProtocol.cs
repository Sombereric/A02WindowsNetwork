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
        public void ServerProtocolManager(string[] protocolMessage, List<GameStateInfo> gameStateInfos)
        {
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
                        Console.WriteLine("BAD REQUEST");
                        break;
                    }

                    if (endToken != "End")
                    {
                        Console.WriteLine("BAD REQUEST");
                    }

                    Login(actionData, guidText, gameStateInfos);

                    //creates state bag and picks a random file
                    //each one of these will have the proper function call for the data that they should get
                    //for example this will handle the guid and other such items
                    //check the read.me for the details of each protocol
                    break;
                case "201": //guess game
                    guessMade(gameStateInfos, protocolMessage[4], protocolMessage[1]);

                    break;
                case "202": //new game
                    NewGame(gameStateInfos, protocolMessage[1]);
                    break;
                case "203": //Play Game
                    NewGame(gameStateInfos, protocolMessage[1]);
                    break;
                case "204": //Quit Game
                    QuitGame(gameStateInfos, protocolMessage[1]);
                    break;
                default:
                    break;
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameStateInfos"></param>
        /// <param name="guess"></param>
        /// <param name="clientGuid"></param>
        private void guessMade(List<GameStateInfo> gameStateInfos, string guess, string clientGuid)
        {
            //this would be where the guess is handled against the state bag. 
            if (guess == null || guess.Trim().Length == 0)
            {
                Console.WriteLine("Invalid Guess");
                return;
            }

            Guid parsedGuid;
            if (!Guid.TryParse(clientGuid, out parsedGuid))
            {
                Console.WriteLine("BAD REQUEST");
                return;
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
                Console.WriteLine("BAD REQUEST");
                return;
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
                        Console.WriteLine("WINNER");
                        return;
                    }

                    Console.WriteLine("FOUND");

                }
                else if (lookResult == 1)
                {
                    Console.WriteLine("NOT FOUND");

                }
                else if (lookResult == 2)
                {
                    Console.WriteLine("ALREADY FOUND");
                }
            }
        }

        //this is why state bags needs to be a class as to allow for us to create a list


        /// <summary>
        /// this will handle the login logic
        /// </summary>
        /// <param name="userNamePasswordIpPort"></param>
        /// <param name="guidText"></param>
        /// <param name="gameStateInfos"></param>
        /// <param name="actionData"></param>
        private void Login(string userNamePasswordIpPort, string guidText, List<GameStateInfo> gameStateInfos)
        {

            if (userNamePasswordIpPort == null || userNamePasswordIpPort.Trim().Length == 0)
            {
                Console.WriteLine("BAD REQUEST");
                return;
            }

            Guid parsedGuid;
            if (!Guid.TryParse(guidText, out parsedGuid))
            {
                Console.WriteLine("BAD REQUEST");
                return;
            }

            // this will check if the user already logged in
            for (int checkCount = 0; checkCount < gameStateInfos.Count; checkCount++)
            {
                if (gameStateInfos[checkCount] != null && gameStateInfos[checkCount].ClientGuid == parsedGuid)
                {
                    Console.WriteLine("OK");
                    return;
                }
            }

            // split the login data
            string[] checkParts = userNamePasswordIpPort.Split(':');

            if (checkParts.Length < 4)
            {
                Console.WriteLine("BAD REQUEST");
                return;
            }


            string ipText = checkParts[2];
            string portText = checkParts[3];

            IPAddress checkIP;
            int checkPort;

            if (!IPAddress.TryParse(ipText, out checkIP))
            {
                Console.WriteLine("BAD REQUEST");
                return;
            }

            if(!int.TryParse(portText, out checkPort))
            {
                Console.WriteLine("BAD REQUEST");
                return;
            }

            // create new game state object
            GameStateInfo newState = new GameStateInfo();
            newState.ClientGuid = parsedGuid;
            newState.ClientIp = checkIP;
            newState.Port = checkPort;

            // add new state to list
            gameStateInfos.Add(newState);

            Console.WriteLine("OK");
            return;




        }

        /// <summary>
        /// this will handles restting a game
        /// </summary>
        /// <param name="gameStateInfos"></param>
        /// <param name="guidText"></param>
        private void NewGame(List<GameStateInfo> gameStateInfos, string guidText)
        {
            Guid parsedGuid;
            if (!Guid.TryParse(guidText, out parsedGuid))
            {
                Console.WriteLine("BAD REQUEST");
                return;
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
                Console.WriteLine("BAD REQUEST");
                return;
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

            Console.WriteLine("OK");
            return;
        }

        /// <summary>
        /// this will stop the game
        /// </summary>
        /// <param name="gameStateInfos"></param>
        /// <param name="guidText"></param>
        private void QuitGame(List<GameStateInfo> gameStateInfos, string guidText)
        {
            Guid parsedGuid;
            if (!Guid.TryParse(guidText, out parsedGuid))
            {
                Console.WriteLine("BAD REQUEST");
                return;
            }

            for (int checkCount = 0; checkCount < gameStateInfos.Count; checkCount++)
            { 
                if(gameStateInfos[checkCount] != null && gameStateInfos[checkCount].ClientGuid == parsedGuid)
                {
                    gameStateInfos.RemoveAt(checkCount);
                    Console.WriteLine("OK");
                    return;
                }
            }

            Console.WriteLine("BAD REQUEST");
        }




    }
}
