using Models;
using Server.Communication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Modules
{
    class LeaderboardModule: Module
    {

        public LeaderboardModule() : base("MatchHistoryModule") { }

        [Function("Get")]
        private Response GetLeaderboard(Request request)
        {
            throw new Exception();
        }

    }

}
