using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        }

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
         
        }
    }
    }
}
