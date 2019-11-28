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
    /// Interaction logic for MatchHistoryPosition.xaml
    /// </summary>
    public partial class MatchHistoryPositionControl : BaseControl
    {
        public MatchHistoryPositionControl(MatchHistoryPosition position, User user)
        {
            InitializeComponent();

            lbUser1.Content = user.Name;
            lbUser2.Content = position.EnemyUserName;

            if (position.IsWin is null) { lbStatus.Content = "Tie"; return; }
            if (position.IsWin is true) { lbStatus.Content = "Win"; return; }
            if (position.IsWin is false) { lbStatus.Content = "Lose"; return; }
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            return;
        }

    }
}
