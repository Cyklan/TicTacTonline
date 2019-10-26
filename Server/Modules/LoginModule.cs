using System;
using System.Collections.Generic;
using Server.Communication;
using Models;

namespace Server.Modules
{
    public class LoginModule : Module
    {
        public LoginModule() : base("LoginController") { }

        public override CommunicationWrapper ProcessRequest(CommunicationWrapper request)
        {
            switch (request.Request.Header.Identifier.Function.ToLower())
            {
                case "login":
                    Login(request);
                    break;
                case "register":
                    Register(request);
                    break;
                default:
                    throw new NotImplementedException($"Function {request.Request.Header.Identifier.Function} is not part of module {request.Request.Header.Identifier.Module}");
            }

            return request;
        }

        private void Login(CommunicationWrapper request)
        {

        }

        private void Register(CommunicationWrapper request)
        {

        }

    }
}
