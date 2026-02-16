using System;
using System.Threading.Tasks;
using System.Windows;
using ClientUI.DAL.ClientManager;

namespace GuessingGameClient
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class UIGamePage : Window
    {
        public UIGamePage()
        {
            InitializeComponent();
        }

        private async void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // tell server we are quitting (protocol 203)
                string checkResponse = await ClientWorker.Run(203, "-");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return;
        }

        private async void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                string checkGuessText = GuessBox.Text;

                if (checkGuessText == null)
                {
                    return;
                }

                
                checkGuessText = checkGuessText.Trim();

                // validate guess is not empty
                if (checkGuessText.Length == 0)
                {
                    return;
                }

                string checkResponse = await ClientWorker.Run(201, checkGuessText);

                GuessBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return;
        }
    }
}
