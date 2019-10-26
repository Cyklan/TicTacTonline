using System;
using System.Collections.Generic;
using Server.Communication;
using Models;

namespace Server.Modules
{
    public abstract class Module
    {
        public string Name;

        public Module(string name)
        {
            Name = name;
        }

        public abstract CommunicationWrapper ProcessRequest(CommunicationWrapper request);

    }
}
