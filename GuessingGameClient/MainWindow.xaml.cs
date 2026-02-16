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


namespace GuessingGameClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string checkResponse = await ClientWorker.Run(200, UserNameTB.Text);
                UIGamePage uIGamePage = new UIGamePage();

                this.Close();

                uIGamePage.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
        }

    }
}
