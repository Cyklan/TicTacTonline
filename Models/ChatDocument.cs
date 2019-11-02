using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ChatDocument : Document
    {
        public User Target { get; set; }
        public string Message { get; set; }
        public int RoomId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
