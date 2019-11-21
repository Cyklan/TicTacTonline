using Client.Communication;
using Models;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for ConnectionControl.xaml
    /// </summary>
    public partial class ConnectionControl : BaseControl
    {

        public ConnectionControl()
        {
            InitializeComponent();
            lbConnectionError.Foreground = new SolidColorBrush(Colors.Red);
            lbConnectionError.Content = "";
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            return;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            btConnectionConnect_Click(this, null);
        }

        private void btConnectionConnect_Click(object sender, RoutedEventArgs e)
        {
            DateTime startWaitTime = DateTime.Now;

            btConnectionConnect.IsEnabled = false;
            btConnectionConnect.Content = "Connecting";
            lbConnectionError.Content = "";
            DoEvents();

            try
            {
                GetMain.Connect();
            }
            catch (Exception ex)
            {
                btConnectionConnect.IsEnabled = true;
                btConnectionConnect.Content = "Connect";
                lbConnectionError.Content = ex.Message;
                return;
            }

            while (!Client.IsConnected)
            {
                System.Threading.Thread.Sleep(250);
                DoEvents();

                if (DateTime.Now > startWaitTime.AddSeconds(15))
                {
                    btConnectionConnect.IsEnabled = true;
                    btConnectionConnect.Content = "Connect";
                    lbConnectionError.Content = "Connection timed out";
                    Client.Close();
                    return;
                }
            }

            GetMain.ChangeControl(MainWindow.Controls.Login);
        }

        private void btConnectionOptions_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.Options);
        }
    }
}
