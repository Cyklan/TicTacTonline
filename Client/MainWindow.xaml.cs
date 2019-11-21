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
            Main,
            GameLobby,
            Game,
            Options
        }

        public User User { get; set; }
        public WebsocketClient Client { get; set; }
        public RoomDocument CurrentGame { get; set; }

        private BaseControl currentControl;
        private Dictionary<Controls, Type> controls;

        public MainWindow()
        {
            bool shutdown = false;

            InitializeComponent();

            try
            {
                RegisterControls();
                ChangeControl(Controls.Connection);
            }
            catch (Exception ex)
            {
                shutdown = true;
                throw ex;
            }
            finally
            {
                // Programm beenden falls ein Fehler auftrifft und das Fenster noch nicht angezeigt wird
                if (shutdown) Application.Current.Shutdown(-1);
            }

        }

        private void RegisterControls()
        {
            controls = new Dictionary<Controls, Type>
            {
                { Controls.Login , typeof(LoginControl) },
                { Controls.Register , typeof(RegisterControl) },
                { Controls.Connection, typeof(ConnectionControl) },
                { Controls.Leaderboard, typeof(LeaderboardControl) },
                { Controls.Main, typeof(MainControl) },
                { Controls.GameLobby, typeof(GameLobbyControl) },
                { Controls.Game, typeof(GameControl) },
                { Controls.Options, typeof(OptionsControl) }
            };
        }

        #region "Public Stuff"

        public void ChangeControl(Controls control)
        {
            // Neue Instanz des Controls erzeugen
            currentControl = (BaseControl)Activator.CreateInstance(controls[control]);
            // Control anzeigen
            grid_main.Children.Clear();
            grid_main.Children.Add(currentControl);
        }

        public void Connect()
        {
            User = new User();
            Client = new WebsocketClient();

            Client.OnSpontaneousReceive += ClientReceive;

            if (!Client.Initialize()) throw new Exception("Server unreachable");
        }

        #endregion

        #region "Events"

        private void ClientReceive(object sender, EventArgs e)
        {
            if (currentControl is null) { return; }
            currentControl.HandleSpontaneousResponse((Response)sender);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(User is null) { return; }

            // abmelden, wenn die Form geschlossen wird, antwort ist dabei egal, weil die Form dann zu ist.
            RequestHeader header = new RequestHeader() { User = User };
            header.Identifier = new Identifier() { Function = "logout", Module = "loginModule" };

            try
            {
                Client.Exchange(new Request() { Header = header, Body = new Document() });
            }
            catch { /* https://i.imgur.com/c4jt321.png */}

        }

        #endregion

    }
}
