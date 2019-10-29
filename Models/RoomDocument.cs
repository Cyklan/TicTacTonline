using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class RoomDocument : Document
    {
        public string Name { get; set; }
        public string Uuid { get; set; }
        public Game Game { get; set; }
    }
}
