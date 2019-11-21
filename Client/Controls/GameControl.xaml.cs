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

        public override void HandleSpontaneousResponse(Response response)
        {
            Dispatcher.Invoke(() =>
            {

                if (response.Header.Code == ResponseCode.Message)
                {
                    tbGameChat.AppendText($"{GetOpponent().Name}: { ((ChatDocument)response.Body).Message }\n");
                    return;
                }

                Room = (RoomDocument)response.Body;
                UpdateButtons();

                if (response.Header.Code == ResponseCode.GameTurnProcessed)
                {
                    return;
                }

                if (response.Header.Code == ResponseCode.GameOver)
                {
                    MessageBox.Show(response.Header.Message);
                    DisableAllButtons();
                    return;
                }

                if (response.Header.Code == ResponseCode.GameTie)
                {
                    MessageBox.Show(response.Header.Message);
                    return;
                }

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

                Send(Room, "GameModule", "HandleTurn");
            }

        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateButtons()
        {
            int x = Room.Game.Turns.Last().X + 1;
            int y = Room.Game.Turns.Last().Y + 1;
            Button button = GetButton($"{x}_{y}");
            
            switch(Room.Game.Fields[x-1, y-1])
            {
                case FieldStatus.Player1:
                    GetButtonImage($"{x}_{y}").Source = new BitmapImage(new Uri("../Resources/icon_cross_dark.png", UriKind.Relative));
                    break;
                case FieldStatus.Player2:
                    GetButtonImage($"{x}_{y}").Source = new BitmapImage(new Uri("../Resources/icon_circle_dark.png", UriKind.Relative));
                    break;
            }
            
            button.IsEnabled = false;
        }

        private void DisableAllButtons()
        {
            for (int x = 1; x <= 3; x++)
                for (int y = 1; y <= 3; y++)
                    GetButton($"{x}_{y}").IsEnabled = false;
        }
    }
}
