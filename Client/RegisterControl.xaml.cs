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
    /// Interaction logic for RegisterControl.xaml
    /// </summary>
    public partial class RegisterControl : UserControl, IControl
    {
        private User user;
        private WebsocketClient client;
        public event EventHandler ChangeToLogin;

        public RegisterControl(User user, WebsocketClient client)
        {
            InitializeComponent();
            this.user = user;
            this.client = client;
        }

        public void HandleSpontaneousResponse(Response response)
        {
            return;
        }

        private void btRegisterRegister_Click(object sender, RoutedEventArgs e)
        {
            //user = new User(tb_register_username.Text, "0:0", tb_register_password.Password);
            //RequestHeader header = new RequestHeader() { User = user };
            //header.Identifier = new Identifier() { Function = "login", Module = "loginModule" };

            //Response response = client.Exchange(new Request() { Header = header, Body = new Document() });

            //if (response.Header.Code != ResponseCode.Ok)
            //{
            //    MessageBox.Show(response.Header.Message);
            //}

            //user = response.Header.Targets.First();
            
            if (false)
            {
                this.ChangeToLogin(this, e);
            }
            else
            {
                lbRegisterError.Content = "User already exists!";
                lbRegisterError.Foreground = new SolidColorBrush(Colors.Red);
            }

            //TODO nächstes Control
        }

        private void btRegisterLogin_Click(object sender, RoutedEventArgs e)
        {
            this.ChangeToLogin(this, e);
        }
    }
}
