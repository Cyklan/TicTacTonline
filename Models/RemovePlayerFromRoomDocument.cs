using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class RemovePlayerFromRoomDocument : Document
    {
        public User PlayerToRemove { get; set; }
        public RoomDocument Room { get; set; }
    }
}
