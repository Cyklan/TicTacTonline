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

        private void tbGameLobbyChat_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void btGameLobbyOpen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btGameLobbyLeave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btGameLobbyKick_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
