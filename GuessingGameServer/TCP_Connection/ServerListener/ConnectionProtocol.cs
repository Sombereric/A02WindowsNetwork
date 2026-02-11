using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessingGameServer.TCP_Connection.ServerListener
{
    internal class ConnectionProtocol
    {
        //code from protocol will be something from the client like 200, 300, something like that
        //this is what will be turned into a task to allow for multiple connections at once 
        public void ServerProtocolManager(string[] protocolMessage)
        {
            //protocol Message format  Protocol ID|Client GUID|Time Sent|Action|Action Data|END|
            switch (protocolMessage[0]) 
            {
                
                case "200": //client login
                    //creates state bag and picks a random file
                    //each one of these will have the proper function call for the data that they should get
                    //for example this will handle the guid and other such items
                    //check the read.me for the details of each protocol
                    break;
                case "201": //new game
                    
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
    }
}
