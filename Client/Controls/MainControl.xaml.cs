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
            return;
        }

        private void btMainNewGame_Click(object sender, RoutedEventArgs e)
        {
            ChangeControl(MainWindow.Controls.GameLobby);
        }

        private void btMainRefresh_Click(object sender, RoutedEventArgs e)
        {
            mainGameList.Children.Clear();

            Response response = Exchange(new Request()
            {
                Header = new RequestHeader()
                {
                    Identifier = new Identifier()
                    {
                        Module = "RoomModule",
                        Function = "GetRooms"
                    },
                    User = User
                },
                Body = new Document()
            });

            if (response.Header.Code != ResponseCode.Ok)
            {
                lbMainError.Content = response.Header.Message;
                Abort();
            }

            foreach (RoomDocument roomdocument in ((RoomsDocument)response.Body).Rooms)
            {
                mainGameList.Children.Add(new JoinGameControl(roomdocument));
            }
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

        }

        private void grid_Loaded(object sender, RoutedEventArgs e)
        {
            btMainRefresh_Click(this, null);
        }
    }
}
