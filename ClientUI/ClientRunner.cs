using System.Configuration;
using ClientUI.DAL;
using ClientUI.GameLogic;
using ClientUI.Protocols;
using ClientUI.TCP_Connection;

class CLientRunner
    {
    
        static void Main(string[] args)
        {
            //gets the server port and IP from the config file
            string ServerIP = ConfigurationManager.AppSettings["ServerIP"];
            string ServerPort = ConfigurationManager.AppSettings["ServerPort"];

           ClientConnection  networkClient = new ClientConnection();
           ClientSessionGame session = new ClientSessionGame();

        Console.WriteLine("what nigga");

            bool isRunning = true;

            //if(isRunning )
            //{
               

            //}
        }
    }

