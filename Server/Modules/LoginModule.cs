using System.Collections.Generic;
using System.Linq;
using Models;
using Server.Database;

namespace Server.Modules
{
    public class LoginModule : Module
    {
        public LoginModule() : base("LoginModule", false) { }

        private Response Login(Request request)
        {
            ResponseHeader header = new ResponseHeader { Targets = new List<User> { request.Header.User } };

            using (DatabaseQueries db = new DatabaseQueries(request.Header.User))
            {
                if (db.LoginUser(request.Header.User))
                {
                    header.Code = ResponseCode.Ok;
                    header.Message = "Logged in successfully";
                }
                else
                {
                    header.Code = ResponseCode.PlannedError;
                    header.Message = "Incorrect username or password";
                }
            }

            return new Response { Header = header, Body = new Document() };
        }

        private Response Register(Request request)
        {
            ResponseHeader header = new ResponseHeader { Targets = new List<User> { request.Header.User } };

            using (DatabaseQueries db = new DatabaseQueries(request.Header.User))
            {
                if (db.GetUsers().Any(x => x.Name.ToLower() == request.Header.User.Name.ToLower()))
                {
                    header.Code = ResponseCode.PlannedError;
                    header.Message = "User name is already taken";
                }
                else
                {
                    db.RegisterUser(request.Header.User);
                    header.Code = ResponseCode.Ok;
                    header.Message = "Registered successfully";
                }
            }

            return new Response { Header = header, Body = new Document() };
        }

    }
}
