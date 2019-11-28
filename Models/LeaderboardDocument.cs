using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class LeaderboardDocument : Document
    {
        public List<LeaderboardPosition> LeaderboardPositions { get; set; }
    }
}
