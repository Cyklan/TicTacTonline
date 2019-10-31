using Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User user;
        private WebsocketClient client;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            user = new User();
            client = new WebsocketClient();
            client.OnConnect += clientConnected;
            client.OnDisconnect += clientDisconnected;

            client.Initialize();
        }

        private void bt_test_Click(object sender, RoutedEventArgs e)
        {
            grid_main.Children.Add(new LoginControl(user, client));
        }

        private void clientConnected(object sender, EventArgs e)
        {
            MessageBox.Show("verbunden");
        }

        private void clientDisconnected(object sender, EventArgs e)
        {
            MessageBox.Show("Verbindung verloren");
        }

      
    }
}
