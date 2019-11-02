using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class RoomsDocument : Document
    {
        public List<RoomDocument> Rooms { get; set; }
    }
}
