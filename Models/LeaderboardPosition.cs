using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class LeaderboardPosition
    {
        public int Position { get; set; }
        public string UserName { get; set; }
        public int Elo { get; set; }
    }
}
