using System;
using System.Threading.Tasks;
using System.Windows;
using ClientUI.DAL.ClientManager;

namespace GuessingGameClient
{
    public partial class UIGamePage : Window
    {
        public UIGamePage()
        {
            InitializeComponent();
        }

        private bool isGameOver = false;
        private int secondsLeft = 100;

        /// <summary>
        /// this will run the countdown timer
        /// </summary>
        /// <returns></returns>
        private async Task RunTimerAsync()
        {
            while (secondsLeft > 0 && isGameOver == false)
            {
                RemainingTimeLbl.Content = "Remaining Time:";
                TimerBox.Text = secondsLeft.ToString();

                await Task.Delay(1000); // wait 1 secound withot freezing
                secondsLeft--;
            }

            if (isGameOver == false)
            {
                isGameOver = true;
                SubmitBtn.IsEnabled = false;
                MessageBox.Show("Time is up! Game over.");
                this.Close();
            }

            return;
        }

        /// <summary>
        /// Updates the game UI using a response string from the server
        /// </summary>
        /// <param name="serverResponse"></param>
        private void UpdateUI(string serverResponse)
        {
            if (serverResponse == null)
            {
                return;
            }

            serverResponse = serverResponse.Replace(Environment.NewLine, "");

            string[] parts = serverResponse.Split('|');

            if (parts.Length < 3)
            {
                return;
            }

            if (parts[0] == "400")
            {
                return;
            }

            StringDisplayTB.Text = parts[1];
            WordCountBox.Text = parts[2];

            return;
        }

        /// <summary>
        /// tarts the timer and performs initial server request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WindowLoading(object sender, RoutedEventArgs e)
        {
            Task timerTask = RunTimerAsync(); // start the timer

            try
            {
                string checkUser = "Player"; // use a temporary username
                 
                await ClientWorker.Run(200, checkUser);

                string gameResponse = await ClientWorker.Run(202, "-");
                
                UpdateUI(gameResponse); // based on the response it will update
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return;
        }


        /// <summary>
        /// Ends the game, notifies the server,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isGameOver = true;

                ClientWorker.Run(203, "-");

                this.Close();
            }
            catch (Exception ex)
            {
                this.Close();
            }

            return;
        }

        /// <summary>
        /// Validates the guess input, sends it to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string checkGuessText = GuessBox.Text;

                if (checkGuessText == null)
                {
                    return;
                }

                checkGuessText = checkGuessText.Trim(); // remove spaces

                if (checkGuessText.Length == 0)
                {
                    return;
                }

                string checkResponse = await ClientWorker.Run(201, checkGuessText);
                MessageBox.Show(checkResponse);

                UpdateUI(checkResponse);

                GuessBox.Text = string.Empty; // clears the input
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return;
        }
    }
}
