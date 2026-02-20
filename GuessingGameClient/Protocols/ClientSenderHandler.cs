using System.Windows;
using ClientUI.DAL.ClientManager;
using GuessingGameClient.GameLogic;

namespace GuessingGameClient.Protocols
{
    internal class clientSenderHandler
    {
        public async Task testFunction(string UserName, Guid clientGuid, gameState gameState)
        {
            string checkResponse = await ClientWorker.Run(200, UserName, clientGuid);

            char delimiter = '|';
            char delimiterData = ':';
            string[] parsedResponse = checkResponse.Split(delimiter);
            string[] parsedActionData = parsedResponse[2].Split(delimiterData);
            gameState.wordToGuessFrom = parsedActionData[0];
            if (!int.TryParse(parsedActionData[1], out int wordsToGuess))
            {
                MessageBox.Show("hope this doesnt show up");
            }
            gameState.numberToGuess = wordsToGuess;
            string tester = "";

            foreach (string line in parsedResponse)
            {
                tester += line;
            }
        }
    }
}
