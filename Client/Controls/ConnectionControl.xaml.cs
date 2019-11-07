using Client.Communication;
using Models;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for ConnectionControl.xaml
    /// </summary>
    public partial class ConnectionControl : BaseControl
    {

        public ConnectionControl(User user, WebsocketClient client) : base(user, client)
        {
            InitializeComponent();
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            return;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            while (!client.IsConnected) { System.Threading.Thread.Sleep(250); DoEvents(); }
            GetMain.ChangeControl(MainWindow.Controls.Login);
        }

        /// <summary>
        /// Application.DoEvents gibts nicht in WPF, daher so -> Fenster kann noch bewegt, geschlossen, ... werden, während auf Verbindung gewartet wird.
        /// </summary>
        private void DoEvents()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch
            {
                // Wenn die Form geschlossen wird, wird manchmal eine NullReference Exception geworfen
            }
         
        }
    }
}
