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
            Options,
            MatchHistory
        }

        public User User { get; set; }
        public WebsocketClient Client { get; set; }
        public RoomDocument CurrentRoom { get; set; }

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
                { Controls.Options, typeof(OptionsControl) },
                { Controls.MatchHistory, typeof(MatchHistoryControl) }
            };
        }

        #region "Public Stuff"

        public void UpdateInfoLabel(string text = "")
        {
            lbInfo.Content = $"Version: {typeof(MainWindow).Assembly.GetName().Version} " +
                             $"| Connected: {Client.IsConnected} " +
                             $"| User: {(User is null? "" : User.Name)}" +
                             $"{(string.IsNullOrEmpty(text)? "" : " | " + text)}";
        }

        public void ChangeControl(Controls control)
        {
            currentControl = (BaseControl)Activator.CreateInstance(controls[control]);
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

        /// <summary>
        /// abmelden und Raum verlassen, wenn die Form geschlossen wird, antwort ist dabei egal, weil die Form dann zu ist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Client.IsConnected) return;

            // warten bis diese Abfrage durch ist, bevor wir ausloggen
            try
            {
                if (CurrentRoom != null) Client.Exchange(new Request()
                {
                    Header = new RequestHeader()
                    {
                        User = User,
                        Identifier = new Identifier()
                        {
                            Function = "LeaveRoom",
                            Module = "RoomModule"
                        }
                    },
                    Body = new RemovePlayerFromRoomDocument()
                    {
                        Room = CurrentRoom,
                        PlayerToRemove = User
                    }
                });
            }
            catch { }

            if (User != null) Client.Exchange(new Request()
            {
                Header = new RequestHeader()
                {
                    User = User,
                    Identifier = new Identifier()
                    {
                        Function = "Logout",
                        Module = "LoginModule"
                    }
                },
                Body = CurrentRoom
            });
        }

        #endregion

    }
}
