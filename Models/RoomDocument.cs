using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public enum RoomStatus
    {
        Open,
        Full,
        Ongoing,
        Closed
    }

    public class RoomDocument : Document
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Password { get; set; }
        public RoomStatus RoomStatus { get; set; }
        public Game Game { get; set; }
    }
}
