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
    /// Interaction logic for LeaderboardControl.xaml
    /// </summary>
    public partial class LeaderboardControl : BaseControl
    {
        public LeaderboardControl()
        {
            InitializeComponent();
        }

        private void btLeaderboardBack_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.Main);
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            throw new NotImplementedException();
        }
    }
}
