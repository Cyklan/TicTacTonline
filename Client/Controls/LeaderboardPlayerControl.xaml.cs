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
    /// Interaction logic for LeaderboardPlayerControl.xaml
    /// </summary>
    public partial class LeaderboardPlayerControl : BaseControl
    {
        public LeaderboardPlayerControl(LeaderboardPosition leaderboardPosition)
        {
            InitializeComponent();

            lbUser.Content = leaderboardPosition.UserName;
            lbElo.Content = leaderboardPosition.Elo;
            lbPosition.Content = leaderboardPosition.Position;
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            return;
        }
    }
}
