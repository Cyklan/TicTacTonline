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
            throw new NotImplementedException();
        }

        private void btMainNewGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btMainRefresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btMainLeaderboard_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.Leaderboard);
        }

        private void btMainGameHistory_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btMainOptions_Click(object sender, RoutedEventArgs e)
        {
            mainGameList.Children.Add(new JoinGameControl());
        }
    }
}
