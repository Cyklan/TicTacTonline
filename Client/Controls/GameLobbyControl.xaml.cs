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
    /// Interaction logic for GameLobbyControl.xaml
    /// </summary>
    public partial class GameLobbyControl : BaseControl
    {
        public GameLobbyControl()
        {
            InitializeComponent();
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            throw new NotImplementedException();
        }

        private void tbGameLobbyChatMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                tbGameLobbyChat.AppendText("User: " + tbGameLobbyChatMessage.Text + "\n");
                tbGameLobbyChat.ScrollToEnd();
                tbGameLobbyChatMessage.Text = "";
            }
        }

        private void btGameLobbyOpen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btGameLobbyLeave_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.Main);
        }

        private void btGameLobbyKick_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btGameLobbyStart_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.Game);
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
    }
}
