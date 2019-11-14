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
    }
}
