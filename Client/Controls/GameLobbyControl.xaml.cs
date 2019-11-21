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
    /// Interaction logic for GameLobbyControl.xaml
    /// </summary>
    public partial class GameLobbyControl : BaseControl
    {
        private bool autoScroll = true;

        public GameLobbyControl()
        {
            InitializeComponent();
            lbGameLobbyError.Foreground = new SolidColorBrush(Colors.Red);
            lbGameLobbyError.Content = "";
        }

        private User GetOpponent()
        {
            return Room.Game.Player1.Name.ToLower() == User.Name.ToLower() ? Room.Game.Player2 : Room.Game.Player1;
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            Dispatcher.Invoke(() =>
             {

                 if (response.Header.Code == ResponseCode.Message)
                 {
                     tbGameLobbyChat.AppendText($"{GetOpponent().Name}: { ((ChatDocument)response.Body).Message }\n");
                     return;
                 }

                 if (response.Header.Code == ResponseCode.JoinedRoom)
                 {
                     Room = (RoomDocument)response.Body;
                     HandlePlayerJoined(GetOpponent());
                     return;
                 }

                 if (response.Header.Code == ResponseCode.LeftRoom)
                 {
                     Room = ((RemovePlayerFromRoomDocument)response.Body).Room;
                     HandlePlayerLeft();
                     return;
                 }

                 if (response.Header.Code == ResponseCode.GameStart)
                 {
                     //TODO
                     return;
                 }
             });

        }

        private void tbGameLobbyChatMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (Room.Game.Player1 is null || Room.Game.Player2 is null) { return; }

            if (e.Key == Key.Enter)
            {
                tbGameLobbyChat.AppendText($"{User.Name}: " + tbGameLobbyChatMessage.Text + "\n");
                tbGameLobbyChat.ScrollToEnd();

                Send(new Request()
                {
                    Header = new RequestHeader()
                    {
                        Identifier = new Identifier()
                        {
                            Function = "SendMessage",
                            Module = "GameModule"
                        },
                        User = User
                    },
                    Body = new ChatDocument()
                    {
                        Message = tbGameLobbyChatMessage.Text,
                        RoomId = Room.Id,
                        Target = GetOpponent(),
                        Timestamp = DateTime.Now
                    }
                });

                tbGameLobbyChatMessage.Text = "";

            }
        }

        private void btGameLobbyOpen_Click(object sender, RoutedEventArgs e)
        {
            lbGameLobbyError.Content = "";

            Response response = Exchange(new Request()
            {
                Header = new RequestHeader()
                {
                    Identifier = new Identifier()
                    {
                        Module = "RoomModule",
                        Function = "CreateRoom"
                    },
                    User = User
                },
                Body = new RoomDocument()
                {
                    Name = tbgameLobbyName.Text,
                    Password = ""
                }
            });

            if (response.Header.Code != ResponseCode.Ok)
            {
                lbGameLobbyError.Content = response.Header.Message;
                Abort();
            }

            Room = (RoomDocument)response.Body;

            tbgameLobbyName.IsReadOnly = true;
            btGameLobbyOpen.Visibility = Visibility.Hidden;
            lbGameLobbyPlayerHost.Content = User.Name;

            UpdateButtons();
        }

        private void btGameLobbyLeave_Click(object sender, RoutedEventArgs e)
        {
            RemovePlayer(User);
            Room = null;
            ChangeControl(MainWindow.Controls.Main);
        }

        private void btGameLobbyKick_Click(object sender, RoutedEventArgs e)
        {
            RemovePlayer(GetOpponent());
            HandlePlayerLeft();
        }

        private void btGameLobbyStart_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.Game);
        }

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0)
            {
                autoScroll = ScrollViewer.VerticalOffset == ScrollViewer.ScrollableHeight;
            }

            if (autoScroll && e.ExtentHeightChange != 0)
            {
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.ExtentHeight);
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            lbGameLobbyPlayerHost.Content = "";
            lbgameLobbyPlayer2.Content = "";

            if (Room != null)
            {
                lbGameLobbyPlayerHost.Content = Room.Game.Player1 != null ? Room.Game.Player1.Name : "";
                lbgameLobbyPlayer2.Content = Room.Game.Player2 != null ? Room.Game.Player2.Name : "";
                tbgameLobbyName.Text = Room.Name;
                tbgameLobbyName.IsReadOnly = true;
                btGameLobbyOpen.Visibility = Visibility.Hidden;
            }

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            if (lbGameLobbyPlayerHost.Content.ToString() == User.Name.ToString())
            {
                btGameLobbyKick.IsEnabled = false;
                btGameLobbyStart.IsEnabled = true;
                btGameLobbyKick2.IsEnabled = true;
                btGameLobbyStart.IsEnabled = true;
            }
            else
            {
                btGameLobbyKick.IsEnabled = false;
                btGameLobbyStart.IsEnabled = false;
                btGameLobbyKick2.IsEnabled = false;
                btGameLobbyStart.IsEnabled = false;
            }

            if (Room != null && (Room.Game.Player1 is null || Room.Game.Player2 is null))
            {
                btGameLobbyStart.IsEnabled = false;
            }

        }

        private void RemovePlayer(User user)
        {
            lbGameLobbyError.Content = "";

            Response response = Exchange(new Request()
            {
                Header = new RequestHeader()
                {
                    Identifier = new Identifier
                    {
                        Function = "LeaveRoom",
                        Module = "RoomModule"
                    },
                    User = User
                },
                Body = new RemovePlayerFromRoomDocument()
                {
                    PlayerToRemove = user,
                    Room = Room
                }
            });

            if (response.Header.Code != ResponseCode.LeftRoom)
            {
                lbGameLobbyError.Content = response.Header.Message;
                Abort();
            }

            lbgameLobbyPlayer2.Content = "";
        }

        private void HandlePlayerJoined(User user)
        {
            lbgameLobbyPlayer2.Content = user.Name;

            UpdateButtons();
        }

        private void HandlePlayerLeft()
        {
            // Wenn ein Spieler rausgeworfen wird
            bool leave = true;
            if (Room.Game.Player1 != null)
            {
                if (Room.Game.Player1.Name.ToLower() == User.Name.ToLower()) leave = false;
            }

            if (Room.Game.Player2 != null)
            {
                if (Room.Game.Player2.Name.ToLower() == User.Name.ToLower()) leave = false;
            }

            if (leave)
            {
                Room = null;
                ChangeControl(MainWindow.Controls.Main);
                return;
            }

            // Wenn ein Spieler raum verlässt
            if (lbGameLobbyPlayerHost.Content.ToString() != User.Name.ToString())
            {
                lbGameLobbyPlayerHost.Content = lbgameLobbyPlayer2.Content;
                lbgameLobbyPlayer2.Content = "";
            }
            else
            {
                lbgameLobbyPlayer2.Content = "";
            }

            UpdateButtons();
        }

    }
}
