using Client.Communication;
using Client.Controls;
using Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum Controls
        {
            Login,
            Register,
            Connection,
            Leaderboard,
            Main
        }

        private User user;
        private WebsocketClient client;
        private BaseControl currentControl;
        private Dictionary<Controls, Type> controls;

        public MainWindow()
        {
            InitializeComponent();

            user = new User();
            client = new WebsocketClient();

            client.OnSpontaneousReceive += clientReceive;

            if (!client.Initialize()) throw new Exception("Server unreachable");

            RegisterControls();

            ChangeControl(Controls.Connection);
        }

        private void RegisterControls()
        {
            controls = new Dictionary<Controls, Type>
            {
                { Controls.Login , typeof(LoginControl) },
                { Controls.Register , typeof(RegisterControl) },
                { Controls.Connection, typeof(ConnectionControl) },
                { Controls.Leaderboard, typeof(LeaderboardControl) },
                { Controls.Main, typeof(MainControl) }
            };
        }

        public void ChangeControl(Controls control)
        {
            // Nutzer mit neuen Datenholen, falls sich was geändert hat -> Objektreferenz über Konstruktor geht nicht
            if (!(currentControl is null)) user = currentControl.currentUser; 
            // Neue Instanz des Controls erzeugen
            currentControl = (BaseControl)Activator.CreateInstance(controls[control], user, client);
            // Control anzeigen
            grid_main.Children.Clear();
            grid_main.Children.Add(currentControl);
        }

        #region "Events"

        private void clientReceive(object sender, EventArgs e)
        {
            if (currentControl is null) { return; }
            currentControl.HandleSpontaneousResponse((Response)sender);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // abmelden, wenn die Form geschlossen wird, antowrt ist dabei egal, weil die Form dann zu ist.
            RequestHeader header = new RequestHeader() { User = user };
            header.Identifier = new Identifier() { Function = "logout", Module = "loginModule" };

            try
            {
                client.Exchange(new Request() { Header = header, Body = new Document() });
            }
            catch { /* https://i.imgur.com/c4jt321.png */}

        }

        #endregion

    }
}
