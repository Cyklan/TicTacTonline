using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Communication
{
    public class CommunicationWrapper
    {
        public List<User> Targets = new List<User>();
        public Response Response;
        public Request Request;
        public byte[] ResponseData;
        public byte[] RequestData;
    }
}
