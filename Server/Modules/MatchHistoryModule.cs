using Models;
using Server.Communication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Modules
{
    public class MatchHistoryModule : Module
    {
        public MatchHistoryModule() : base("MatchHistoryModule") { }

        [Function("Get")]
        public Response GetMatchHistory(Request request)
        {
            throw new NotImplementedException();
        }

    }
}
