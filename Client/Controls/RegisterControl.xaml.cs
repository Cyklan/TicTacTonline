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

        public RegisterControl()
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


            if (!new System.Text.RegularExpressions.Regex(@"^.*(?=.{8,})((?=.*[!@#$%^&*()\-_=+{};:,<.>]){1})(?=.*\d)((?=.*[a-z]){1})((?=.*[A-Z]){1}).*$").IsMatch(tbRegisterPassword.Password))
            {
                lbRegisterError.Content = "Insecure Password";
                return;
            }

            User = new User(tbRegisterUsername.Text, "0:0", tbRegisterPassword.Password);

            RequestHeader header = new RequestHeader() { User = User };
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

        private void tbRegisterPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btRegisterRegister_Click(this, null);
            }
        }
    }
}
