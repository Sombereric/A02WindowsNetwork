using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientUI.DAL.ClientManager;
using GuessingGameClient.Protocols;


namespace GuessingGameClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Guid clientGuid = Guid.NewGuid();
        clientSenderHandler clientSenderHandler = new clientSenderHandler();
        public MainWindow()
        {
            InitializeComponent();
            StartBtn.IsEnabled = false;
        }
        /// <summary>
        /// this will start the game
        /// </summary>
        private async void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await clientSenderHandler.testFunction(UserNameTB.Text, clientGuid);

                UIGamePage uIGamePage = new UIGamePage(clientGuid);
                uIGamePage.Show();

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// this will quit the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param
        private async void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
             Close();
        }
        /// <summary>
        /// disables the start button when the text box is empty
        /// </summary>
        private void UserNameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UserNameTB.Text.Length == 0)
            {
                StartBtn.IsEnabled = false;
            }
            else
            {
                StartBtn.IsEnabled = true;
            }
        }
    }
}
