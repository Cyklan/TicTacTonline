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
    /// Interaction logic for JoinGameControl.xaml
    /// </summary>
    public partial class JoinGameControl : BaseControl
    {
        RoomDocument ControlRoom { get; set; }

        public JoinGameControl(RoomDocument room)
        {
            InitializeComponent();

            ControlRoom = room;

            lbRoomName.Content = room.Name;

            lbUser1.Content = room.Game.Player1 != null ? room.Game.Player1.Name : "";
            lbUser2.Content = room.Game.Player2 != null ? room.Game.Player2.Name : "";
            lbStatus.Content = room.RoomStatus;
        }

        private void btJoinGame_Click(object sender, RoutedEventArgs e)
        {
            if(ControlRoom.RoomStatus != RoomStatus.Open) { return; }

            Response response = Exchange(new Request()
            {
                Header = new RequestHeader()
                {
                    Identifier = new Identifier()
                    {
                        Module = "RoomModule",
                        Function = "JoinRoom"
                    },
                    User = User
                },
                Body = ControlRoom
            });

            if (response.Header.Code != ResponseCode.JoinedRoom)
            {
                throw new Exception(response.Header.Message);
            }

            base.Room = (RoomDocument)response.Body;
            ChangeControl(MainWindow.Controls.GameLobby);
        }

        public override void HandleSpontaneousResponse(Response response)
        {
            return;
        }
    }
}
