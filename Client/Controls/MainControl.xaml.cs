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
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : BaseControl
    {
        public MainControl()
        {
            InitializeComponent();
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            return;
        }

        private void btMainNewGame_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.GameLobby);
        }

        private void btMainRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void btMainLeaderboard_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.Leaderboard);
        }

        private void btMainGameHistory_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.MatchHistory);
        }

        private void grid_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void cbMainShowFullGames_Checked(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void cbMainShowRunningGames_Checked(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            mainGameList.Children.Clear();

            Response response = Exchange(new Document(), "RoomModule", "GetRooms");

            if (response.Header.Code != ResponseCode.Ok)
            {
                lbMainError.Content = response.Header.Message;
                Abort();
            }

            foreach (RoomDocument roomdocument in ((RoomsDocument)response.Body).Rooms)
            {
                if (!(bool)cbMainShowFullGames.IsChecked && roomdocument.RoomStatus == RoomStatus.Full) continue;
                if (!(bool)cbMainShowRunningGames.IsChecked && roomdocument.RoomStatus == RoomStatus.Ongoing) continue;

                mainGameList.Children.Add(new JoinGameControl(roomdocument));
            }
        }
    }
}
