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
using Client.Communication;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : BaseControl
    {

        public LoginControl()
        {
            InitializeComponent();
            lbLoginError.Foreground = new SolidColorBrush(Colors.Red);
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            return;
        }

        private void btLoginLogin_Click(object sender, RoutedEventArgs e)
        {
            lbLoginError.Content = "";

            User = new User(tbLoginUsername.Text, "0:0", tbLoginPassword.Password);

            Response response = Exchange(new Document(), "LoginModule", "Login");

            User = response.Header.Targets.First();

            switch (response.Header.Code)
            {
                case ResponseCode.PlannedError:
                    lbLoginError.Content = response.Header.Message;
                    User = null;
                    break;

                case ResponseCode.Ok:

                    GetMain.ChangeControl(MainWindow.Controls.Main);
                    break;

                default:
                    throw new Exception("Invalid response code.");
            }

        }

        private void btLoginRegister_Click(object sender, RoutedEventArgs e)
        {
          ChangeControl(MainWindow.Controls.Register);
        }

        private void btLoginOptions_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.Options);
        }

        private void tbLoginPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                btLoginLogin_Click(this, null);
            }
        }
    }
}
