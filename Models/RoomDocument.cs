using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class RoomDocument : Document
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Password { get; set; }
        public Game Game { get; set; }
    }
}
