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
        public GameControl()
        {
            InitializeComponent();
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            throw new NotImplementedException();
        }

        private void tbGameChatMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tbGameChat.AppendText("User: " + tbGameChatMessage.Text + "\n");
                tbGameChat.ScrollToEnd();
                tbGameChatMessage.Text = "";
            }
        }
        private bool _autoScroll = true;
        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0)
            {
                _autoScroll = ScrollViewer.VerticalOffset == ScrollViewer.ScrollableHeight;
            }

            if (_autoScroll && e.ExtentHeightChange != 0)
            {
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.ExtentHeight);
            }

            
        }

        private void btGame_1_1_Click(object sender, RoutedEventArgs e)
        {
            var uriSource = new Uri("../Resources/icon_cross_dark.png", UriKind.Relative);
            imgBt_1_1.Source = new BitmapImage(uriSource);
        }

        private void btGame_1_1_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void btGame_1_1_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void imgBt_MouseEnter(object sender, MouseEventArgs e)
        {
            Image i;
            Button b = getButton(getName(((Button)sender).Name));
            if (!checkButtonState(b))
            {
                return;
            }
            else if (true /*Room.Game.CurrentPlayer.Name.ToLower() == User.Name.ToLower()*/)
            {
                if ( true /*Room.Game.Player1 == Room.Game.CurrentPlayer*/)
                {
                    i = getButtonImage(getName(((Button)sender).Name));
                    Uri u = new Uri("../Resources/icon_cross_bright.png", UriKind.Relative);
                    i.Source = new BitmapImage(u);
                }
                else
                {
                    i = getButtonImage(getName(((Button)sender).Name));
                    Uri u = new Uri("../Resources/icon_circle_bright.png", UriKind.Relative);
                    i.Source = new BitmapImage(u);
                }
            }



        }

        private void imgBt_MouseLeave(object sender, MouseEventArgs e)
        {
            Image i;
            Button b = getButton(getName(((Button)sender).Name));
            if (!checkButtonState(b))
            {
                return;
            }
            i = getButtonImage(getName(((Button)sender).Name));
            Uri u = new Uri("../Resources/icon_empty.png", UriKind.Relative);
            i.Source = new BitmapImage(u);
        }

        private bool checkButtonState(Button b)
        {
            if(b.IsEnabled == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string getName(string name)
        {
            string[] tmp = name.Split('_');

            return $"{tmp[1]}_{tmp[2]}"; 
        }

        private Button getButton(string name)
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

        private Image getButtonImage(string name)
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
            Button b = getButton(getName(((Button)sender).Name));
            if (checkButtonState(b) && true /*Room.Game.CurrentPlayer.Name.ToLower() == User.Name.ToLower()*/)
            {
                if (true /*Room.Game.Player1 == Room.Game.CurrentPlayer*/)
                {
                    i = getButtonImage(getName(((Button)sender).Name));
                    Uri u = new Uri("../Resources/icon_cross_dark.png", UriKind.Relative);
                    i.Source = new BitmapImage(u);
                }
                else
                {
                    i = getButtonImage(getName(((Button)sender).Name));
                    Uri u = new Uri("../Resources/icon_circle_dark.png", UriKind.Relative);
                    i.Source = new BitmapImage(u);
                }
                b.IsEnabled = false;
            }
            else
            {
                return;
            }

        }
    }
}
