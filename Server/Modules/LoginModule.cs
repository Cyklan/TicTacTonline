﻿using System.Collections.Generic;
using System.Linq;
using Models;
using Server.Database;
using Server.Communication;

namespace Server.Modules
{
    public class LoginModule : Module
    {
        public LoginModule() : base("LoginModule", false) { }

        [Function("Login")]
        private Response Login(Request request)
        {
            ResponseHeader header = new ResponseHeader { Targets = new List<User> { request.Header.User } };

            using DatabaseQueries db = new DatabaseQueries(request.Header.User);

            try
            {
                Authenticate(request.Header.User);
                header.Code = ResponseCode.PlannedError;
                header.Message = "Already logged in";
                return new Response { Header = header, Body = new Document() };
            }
            catch { }
          

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

            return new Response { Header = header, Body = new Document() };
        }

        [Function("Register")]
        private Response Register(Request request)
        {
            ResponseHeader header = new ResponseHeader { Targets = new List<User> { request.Header.User } };

            using DatabaseQueries db = new DatabaseQueries(request.Header.User);

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

            return new Response { Header = header, Body = new Document() };
        }

        [Function("Logout")]
        private Response Logout(Request request)
        {
            ResponseHeader header = new ResponseHeader { Targets = new List<User> { request.Header.User } };

            Authenticate(request.Header.User);
            using DatabaseQueries db = new DatabaseQueries(request.Header.User);

            if (db.LogoutUser(request.Header.User))
            {
                db.RemoveUserFromRoom(request.Header.User);
    
                header.Code = ResponseCode.Ok;
                header.Message = "User logged out successfully";
            }
            else
            {
                header.Code = ResponseCode.PlannedError;
                header.Message = "You need to be logged in to log out. Please log in to log out.";
            }

            return new Response { Header = header, Body = new Document() };
        }

    }
}
