using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class MatchHistoryPosition
    {
        public string EnemyUserName { get; set; }
        public bool? IsWin { get; set; }
        public DateTime TimeStamp { get; set; }

    }
}
