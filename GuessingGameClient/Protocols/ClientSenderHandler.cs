using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClientUI.DAL.ClientManager;

namespace GuessingGameClient.Protocols
{
    internal class clientSenderHandler
    {
        public async Task testFunction(string UserName, Guid clientGuid)
        {
            string checkResponse = await ClientWorker.Run(200, UserName, clientGuid);

            char delimiter = '|';
            string[] parsedResponse = checkResponse.Split(delimiter);
            string tester = "";
            foreach (string line in parsedResponse)
            {
                tester += line;
            }
            MessageBox.Show(tester);
        }
    }
}
