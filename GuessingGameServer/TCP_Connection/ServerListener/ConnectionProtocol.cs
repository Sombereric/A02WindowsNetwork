/*
* FILE : ConnectionProtocol.cs
* PROJECT : PROG2126 - Assignment #2
* PROGRAMMER : Eric Moutoux, Will Jessel, Zemmatt Hagos
* FIRST VERSION : 2026-2-9
* DESCRIPTION :
* handles each request made by the client to the server and the respective action
*/

using System.Net;
using GuessingGameServer.GameLogic;

namespace GuessingGameServer.TCP_Connection.ServerListener
{
    internal class ConnectionProtocol
    {
        //code from protocol will be something from the client like 200, 300, something like that
        //this is what will be turned into a task to allow for multiple connections at once 
        public void ServerProtocolManager(string[] protocolMessage, List<GameStateInfo> gameStateInfos, object GameStateLocker)
        {
            //protocol Message format  Protocol ID|Client GUID|Time Sent|Action|Action Data|END|
            switch (protocolMessage[0])
            { 
                case "200": //client login
                    bool SuccessState = false;
                    string[] parsedActionData;
                    //creates state bag and picks a random file
                    //each one of these will have the proper function call for the data that they should get
                    //for example this will handle the guid and other such items
                    //check the read.me for the details of each protocol

                    GameStateInfo gameStateInfo = new GameStateInfo();

                    //reverse if statements as one can fail but still pass
                    //add locker to each thing as it should only be allowed to do one at a time
                    if (Guid.TryParse(protocolMessage[1], out Guid clientGuid))
                    {
                        gameStateInfo.ClientGuid = clientGuid;
                        SuccessState = true;
                    }
                    parsedActionData = ParseActionData(protocolMessage[4]);

                    if (IPAddress.TryParse(parsedActionData[1], out IPAddress clientIP))
                    {
                        gameStateInfo.ClientIp = clientIP;
                        SuccessState = true;
                    }

                    if (Int32.TryParse(parsedActionData[2], out Int32 clientPort))
                    {
                        gameStateInfo.Port = clientPort;
                        SuccessState = true;
                    }

                    lock (GameStateLocker)
                    {
                        gameStateInfos.Add(gameStateInfo);
                    }
                    break;
                case "201": //new game
                    //help
                    break;
                case "202": //Guess
                    //could be guess made and calls the correct files
                    guessMade(protocolMessage[3]);
                    break;
                case "203": //Play Game
                    
                    break;
                case "204": //Quit Game
                    
                    break;
                default: 
                    break;
            }
            return;
        }
        // a example method that can handle 201 protocol code. this can change but gives the idea
        private void guessMade(string guess)
        {
            //this would be where the guess is handled against the state bag. 
            //this is why state bags needs to be a class as to allow for us to create a list
        }
        private string[] ParseActionData(string actionData)
        {
            string[] parsedActionData = actionData.Split(':');


            return parsedActionData;
        }
    }
}
