using Models;
using Server.Communication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Modules
{
    /// <summary>
    /// Kümmert sich um alles, was das Leaderboard können muss
    /// </summary>
    class LeaderboardModule: Module
    {

        public LeaderboardModule() : base("MatchHistoryModule") { }

        /// <summary>
        /// Gibt die Rangliste aus
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("Get")]
        public Response GetLeaderboard(Request request)
        {
            throw new Exception();
        }

    }

}
