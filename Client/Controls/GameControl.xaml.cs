﻿using Models;
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
    /// Interaction logic for GameControl.xaml
    /// </summary>
    public partial class GameControl : BaseControl
    {
        private bool autoScroll = true;

        public GameControl()
        {
            InitializeComponent();
        }

        private User GetOpponent()
        {
            return Room.Game.Player1.Name.ToLower() == User.Name.ToLower() ? Room.Game.Player2 : Room.Game.Player1;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

            lb_Game_Player1.Content = Room.Game.Player1.Name;
            lb_Game_Player2.Content = Room.Game.Player2.Name;

            if (Room.Game.CurrentPlayer.Name.ToLower() == Room.Game.Player1.Name.ToLower()) { lb_Game_Player1.FontWeight = FontWeights.Bold; lb_Game_Player2.FontWeight = FontWeights.Normal; }
            if (Room.Game.CurrentPlayer.Name.ToLower() == Room.Game.Player2.Name.ToLower()) { lb_Game_Player2.FontWeight = FontWeights.Bold; lb_Game_Player1.FontWeight = FontWeights.Normal; }
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            Dispatcher.Invoke(() =>
            {

                if (response.Header.Code == ResponseCode.Message)
                {
                    tbGameChat.AppendText($"{GetOpponent().Name}: { ((ChatDocument)response.Body).Message }\n");
                    return;
                }

                if (response.Header.Code == ResponseCode.LeftRoom)
                {
                    User opponent = GetOpponent();
                    Room = ((RemovePlayerFromRoomDocument)response.Body).Room;
                    Room.Game.CurrentPlayer = opponent;
                    MessageBox.Show(Exchange(Room, "GameModule", "HandleTurn").Header.Message);
                    Room = null;
                    ChangeControl(MainWindow.Controls.Main);
                    return;
                }

                Room = (RoomDocument)response.Body;
                UpdateButtons();

                if (response.Header.Code == ResponseCode.GameTurnProcessed) return;

                MessageBox.Show(response.Header.Message);
                Room = null;
                ChangeControl(MainWindow.Controls.Main);

            });
        }

        private void tbGameChatMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (Room.Game.Player1 is null || Room.Game.Player2 is null) { return; }

            if (e.Key == Key.Enter)
            {
                tbGameChat.AppendText($"{User.Name}: " + tbGameChatMessage.Text + "\n");
                tbGameChat.ScrollToEnd();

                Send(new ChatDocument()
                {
                    Message = tbGameChatMessage.Text,
                    RoomId = Room.Id,
                    Target = GetOpponent(),
                    Timestamp = DateTime.Now
                }, "GameModule", "SendMessage");

                tbGameChatMessage.Text = "";

            }
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

        private void btGame_1_1_Click(object sender, RoutedEventArgs e)
        {
            var uriSource = new Uri("../Resources/icon_cross_dark.png", UriKind.Relative);
            imgBt_1_1.Source = new BitmapImage(uriSource);
        }

        private void imgBt_MouseEnter(object sender, MouseEventArgs e)
        {
            Image i;
            Button b = GetButton(GetName(((Button)sender).Name));
            if (!CheckButtonState(b)) return;

            if (Room.Game.CurrentPlayer.Name.ToLower() == User.Name.ToLower())
            {
                if (Room.Game.Player1.Name.ToLower() == Room.Game.CurrentPlayer.Name.ToLower())
                {
                    i = GetButtonImage(GetName(((Button)sender).Name));
                    Uri u = new Uri("../Resources/icon_cross_bright.png", UriKind.Relative);
                    i.Source = new BitmapImage(u);
                }
                else
                {
                    i = GetButtonImage(GetName(((Button)sender).Name));
                    Uri u = new Uri("../Resources/icon_circle_bright.png", UriKind.Relative);
                    i.Source = new BitmapImage(u);
                }
            }

        }

        private void imgBt_MouseLeave(object sender, MouseEventArgs e)
        {
            Image i;
            Button b = GetButton(GetName(((Button)sender).Name));
            if (!CheckButtonState(b))
            {
                return;
            }
            i = GetButtonImage(GetName(((Button)sender).Name));
            Uri u = new Uri("../Resources/icon_empty.png", UriKind.Relative);
            i.Source = new BitmapImage(u);
        }

        private bool CheckButtonState(Button b)
        {
            if (b.IsEnabled == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetName(string name)
        {
            string[] tmp = name.Split('_');

            return $"{tmp[1]}_{tmp[2]}";
        }

        private Button GetButton(string name)
        {
            switch (name)
            {
                case "1_1":
                    return btGame_1_1;
                case "1_2":
                    return btGame_1_2;
                case "1_3":
                    return btGame_1_3;
                case "2_1":
                    return btGame_2_1;
                case "2_2":
                    return btGame_2_2;
                case "2_3":
                    return btGame_2_3;
                case "3_1":
                    return btGame_3_1;
                case "3_2":
                    return btGame_3_2;
                case "3_3":
                    return btGame_3_3;
                default:
                    return null;
            }
        }

        private Image GetButtonImage(string name)
        {
            switch (name)
            {
                case "1_1":
                    return imgBt_1_1;
                case "1_2":
                    return imgBt_1_2;
                case "1_3":
                    return imgBt_1_3;
                case "2_1":
                    return imgBt_2_1;
                case "2_2":
                    return imgBt_2_2;
                case "2_3":
                    return imgBt_2_3;
                case "3_1":
                    return imgBt_3_1;
                case "3_2":
                    return imgBt_3_2;
                case "3_3":
                    return imgBt_3_3;
                default:
                    return null;
            }
        }

        private void btGameField_Click(object sender, RoutedEventArgs e)
        {
            Image i;
            Button b = GetButton(GetName(((Button)sender).Name));
            if (CheckButtonState(b) && Room.Game.CurrentPlayer.Name.ToLower() == User.Name.ToLower())
            {
                string[] field = b.Name.Split('_');
                int x = int.Parse(field[1]) - 1;
                int y = int.Parse(field[2]) - 1;

                if (Room.Game.Player1.Name.ToLower() == Room.Game.CurrentPlayer.Name.ToLower())
                {
                    i = GetButtonImage(GetName(((Button)sender).Name));
                    Uri u = new Uri("../Resources/icon_cross_dark.png", UriKind.Relative);
                    i.Source = new BitmapImage(u);

                    Room.Game.Fields[x, y] = FieldStatus.Player1;
                    Room.Game.Turns.Add(new Turn() { X = x, Y = y });
                }
                else
                {
                    i = GetButtonImage(GetName(((Button)sender).Name));
                    Uri u = new Uri("../Resources/icon_circle_dark.png", UriKind.Relative);
                    i.Source = new BitmapImage(u);

                    Room.Game.Fields[x, y] = FieldStatus.Player1;
                    Room.Game.Turns.Add(new Turn() { X = x, Y = y });
                }
                b.IsEnabled = false;
                Room.Game.CurrentPlayer = GetOpponent();

                if (Room.Game.CurrentPlayer.Name.ToLower() == Room.Game.Player1.Name.ToLower()) { lb_Game_Player1.FontWeight = FontWeights.Bold; lb_Game_Player2.FontWeight = FontWeights.Normal; }
                if (Room.Game.CurrentPlayer.Name.ToLower() == Room.Game.Player2.Name.ToLower()) { lb_Game_Player2.FontWeight = FontWeights.Bold; lb_Game_Player1.FontWeight = FontWeights.Normal; }

                Send(Room, "GameModule", "HandleTurn");
            }

        }

        private void UpdateButtons()
        {
            int x = Room.Game.Turns.Last().X + 1;
            int y = Room.Game.Turns.Last().Y + 1;
            Button button = GetButton($"{x}_{y}");

            switch (Room.Game.Fields[x - 1, y - 1])
            {
                case FieldStatus.Player1:
                    GetButtonImage($"{x}_{y}").Source = new BitmapImage(new Uri("../Resources/icon_cross_dark.png", UriKind.Relative));
                    break;
                case FieldStatus.Player2:
                    GetButtonImage($"{x}_{y}").Source = new BitmapImage(new Uri("../Resources/icon_circle_dark.png", UriKind.Relative));
                    break;
            }

            button.IsEnabled = false;

            if (Room.Game.CurrentPlayer.Name.ToLower() == Room.Game.Player1.Name.ToLower()) { lb_Game_Player1.FontWeight = FontWeights.Bold; lb_Game_Player2.FontWeight = FontWeights.Normal; }
            if (Room.Game.CurrentPlayer.Name.ToLower() == Room.Game.Player2.Name.ToLower()) { lb_Game_Player2.FontWeight = FontWeights.Bold; lb_Game_Player1.FontWeight = FontWeights.Normal; }
        }

        private void DisableAllButtons()
        {
            for (int x = 1; x <= 3; x++)
                for (int y = 1; y <= 3; y++)
                    GetButton($"{x}_{y}").IsEnabled = false;
        }

        private void btGameLeave_Click(object sender, RoutedEventArgs e)
        {
            Response response = Exchange(new RemovePlayerFromRoomDocument()
            {
                PlayerToRemove = User,
                Room = Room
            }, "RoomModule", "LeaveRoom");

            if (response.Header.Code != ResponseCode.LeftRoom) Abort();
            
            Room = null;
            ChangeControl(MainWindow.Controls.Main);
        }
    }
}
