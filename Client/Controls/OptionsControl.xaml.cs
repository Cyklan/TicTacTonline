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
using Client.Communication;
using Models;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for OptionsControl.xaml
    /// </summary>
    public partial class OptionsControl : BaseControl
    {
        private WebsocketConfiguration config = new Communication.WebsocketConfiguration();

        public OptionsControl()
        {
            InitializeComponent();
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            return;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            config.Load();
            tbOptionsIp.Text = config.Ip;
            tbOptionsPort.Text = config.Port.ToString();
            cbOptionsSsl.IsChecked = config.Ssl;
            cbOptionsSsl.Content = (bool)cbOptionsSsl.IsChecked ? "Enabled" : "Disabled";
        }

        private void btOptionsCancel_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.Login);
        }

        private void btOptionsOk_Click(object sender, RoutedEventArgs e)
        {
            config.Ip = tbOptionsIp.Text;
            int.TryParse(tbOptionsPort.Text, out int port);
            config.Port = port;
            config.Ssl = (bool)cbOptionsSsl.IsChecked;

            config.Save();
            
            ChangeControl(MainWindow.Controls.Connection);
        }

        private void cbOptionsSsl_Checked(object sender, RoutedEventArgs e)
        {
            cbOptionsSsl.Content = "Enabled";
        }

        private void cbOptionsSsl_Unchecked(object sender, RoutedEventArgs e)
        {
            cbOptionsSsl.Content = "Disabled";
        }
    }
}
