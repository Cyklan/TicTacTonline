using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Models;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for MatchHistoryControl.xaml
    /// </summary>
    public partial class MatchHistoryControl : BaseControl
    {
        public MatchHistoryControl()
        {
            InitializeComponent();
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            return;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
           ((MatchHistoryDocument)Exchange(new Document(), "MatchHistoryModule", "Get").Body).Matches.ForEach(x =>
           {
               spMatchHistory.Children.Add(new MatchHistoryPositionControl(x, User));
           });
        }

        private void btLeaderboardBack_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.Main);
        }
    }
}
