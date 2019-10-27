using System;
using System.Collections.Generic;
using Server.Communication;
using Models;
using System.Reflection;
using Server.Database;
using System.Linq;

namespace Server.Modules
{
    public class LoginModule : Module
    {
        public LoginModule() : base("LoginModule") { }

        private Response Login(Request request)
        {
            ResponseHeader header = new ResponseHeader() { Targets = new List<User> { request.Header.User } };

            using (DatabaseQueries db = new DatabaseQueries())
            {
                if (db.LoginUser(request.Header.User))
                {
                    header.Code = ResposneCode.Ok;
                    header.Message = "Logged in successfully";
                }
                else
                {
                    header.Code = ResposneCode.PlannedError;
                    header.Message = "Incorrect username or password";
                }
            }

            return new Response() { Header = header, Body = new Document() };
        }

        private Response Register(Request request)
        {
            ResponseHeader header = new ResponseHeader() { Targets = new List<User> { request.Header.User } };

            using (DatabaseQueries db = new DatabaseQueries())
            {
                if (db.GetUsers().Any(x => x.Name.ToLower() != request.Header.User.Name.ToLower()))
                {
                    db.RegisterUser(request.Header.User);
                    header.Code = ResposneCode.Ok;
                    header.Message = "Registered successfully";
                }
                else
                {
                    header.Code = ResposneCode.PlannedError;
                    header.Message = "User name is already taken";
                }
            }

            return new Response() { Header = header, Body = new Document() };
        }

    }
}
