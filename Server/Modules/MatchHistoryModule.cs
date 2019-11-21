using Models;
using Server.Communication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Modules
{
    /// <summary>
    /// Kümmert sich um alle bereits gespielten Spiele
    /// </summary>
    public class MatchHistoryModule : Module
    {
        public MatchHistoryModule() : base("MatchHistoryModule") { }

        /// <summary>
        /// Gibt eine Liste mit allen bereits gespielten Spielen aus
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Function("Get")]
        public Response GetMatchHistory(Request request)
        {
            throw new NotImplementedException();
        }

    }
}
