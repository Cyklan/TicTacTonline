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
using Client.General;
using Client.Communication;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for RegisterControl.xaml
    /// </summary>
    public partial class RegisterControl : BaseControl
    {

        public RegisterControl(User user, WebsocketClient client) : base(user, client)
        {
            InitializeComponent();

            lbRegisterError.Foreground = new SolidColorBrush(Colors.Red);
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            return;
        }

        private void btRegisterRegister_Click(object sender, RoutedEventArgs e)
        {
            lbRegisterError.Content = "";

            currentUser = new User(tbRegisterUsername.Text, "0:0", tbRegisterPassword.Password);

            RequestHeader header = new RequestHeader() { User = currentUser };
            header.Identifier = new Identifier() { Function = "register", Module = "loginModule" };
       
            Response response = Exchange(new Request() { Header = header, Body = new Document() });

            switch (response.Header.Code)
            {
                case ResponseCode.PlannedError:
                    lbRegisterError.Content = response.Header.Message;
                    break;

                case ResponseCode.Ok:
                    GetMain.ChangeControl(MainWindow.Controls.Login);
                    break;

                default:
                    throw new Exception("Invalid response code.");
            }
        }

        private void btRegisterLogin_Click(object sender, RoutedEventArgs e)
        {
            GetMain.ChangeControl(MainWindow.Controls.Login);
        }

    }
}
