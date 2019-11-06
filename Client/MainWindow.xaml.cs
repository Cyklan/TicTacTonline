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
        private IControl currentControl;



        public MainWindow()
        {
            InitializeComponent();
            LoginControl loginControl = new LoginControl(user, client);
            loginControl.LoginSuccess += new EventHandler(Main_LoginSuccess);
            loginControl.ChangeToRegister += new EventHandler(Main_ChangeToRegister);
            currentControl = loginControl;
            grid_main.Children.Add(loginControl);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            user = new User();
            client = new WebsocketClient();
            client.OnConnect += clientConnected;
            client.OnDisconnect += clientDisconnected;
            client.OnSpontaneousReceive += clientReceive;

            if (!client.Initialize()) MessageBox.Show("Server unreachable");

        }

        private void clientConnected(object sender, EventArgs e)
        {
            MessageBox.Show("verbunden");
        }

        private void clientDisconnected(object sender, EventArgs e)
        {
            MessageBox.Show("Verbindung verloren");
        }

        private void clientReceive(object sender, EventArgs e)
        {
            if (currentControl is null) { return; }
            currentControl.HandleSpontaneousResponse((Response)sender);
        }

        void Main_ChangeToRegister(object sender, EventArgs e)
        {
            grid_main.Children.Clear();
            RegisterControl registerControl = new RegisterControl(user, client);
            registerControl.ChangeToLogin += new EventHandler(Main_ChangeToLogin);
            grid_main.Children.Add(registerControl);
        }

        void Main_LoginSuccess(object sender, EventArgs e)
        {
            grid_main.Children.Clear();
        }

        void Main_ChangeToLogin(object sender, EventArgs e)
        {
            grid_main.Children.Clear();
            LoginControl loginControl = new LoginControl(user, client);
            loginControl.LoginSuccess += new EventHandler(Main_LoginSuccess);
            loginControl.ChangeToRegister += new EventHandler(Main_ChangeToRegister);
            grid_main.Children.Add(loginControl);
        }

        public static void changeControl(UserControl u)
        {
            
        }
    }
}
