using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl , IControl
    {
        private User user;
        private WebsocketClient client;

        public LoginControl(User user, WebsocketClient client)
        {
            InitializeComponent();
            this.user = user;
            this.client = client;
        }

        public void HandleSpontaneousResponse(Response response)
        {
            return;
        }

        private void bt_login_login_Click(object sender, RoutedEventArgs e)
        {
            user = new User(tb_login_username.Text, "0:0", tb_login_password.Text);
            RequestHeader header = new RequestHeader() { User = user };
            header.Identifier = new Identifier() { Function = "login", Module = "loginModule" };

            Response response = client.Exchange(new Request() { Header = header, Body = new Document() });

            if(response.Header.Code != ResponseCode.Ok)
            {
                MessageBox.Show(response.Header.Message);
            }

            user = response.Header.Targets.First();

            //TODO nächstes Control
        }

        private void bt_login_register_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
