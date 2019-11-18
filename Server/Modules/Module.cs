﻿using System;
using System.Reflection;
using Models;
using Server.General;
using Server.Communication;

namespace Server.Modules
{
    public abstract class Module
    {
        public string Name { get; set; }
        private readonly bool authenticate;
        protected Log Log { get; set; }

        public Module(string name, bool authenticate = true)
        {
            Name = name;
            this.authenticate = authenticate;
            Log = new Log();
        }

        public Response ProcessRequest(Request request)
        {
            if (authenticate) Authenticate(request.Header.User);

            foreach (MethodInfo m in GetType().GetRuntimeMethods())
            {
                object[] attributes = m.GetCustomAttributes(typeof(FunctionAttribute), true);
                if (attributes.Length <= 0) continue;

                if (((FunctionAttribute)attributes[0]).Name.ToLower() == request.Header.Identifier.Function.ToLower())
                {
                    return (Response)m.Invoke(this, new object[] { request });
                }
            }

            throw new NotImplementedException($"Function {request.Header.Identifier.Function} is not part of module {request.Header.Identifier.Module}");
        }

        protected void Authenticate(User user)
        {
            using Database.DatabaseQueries db = new Database.DatabaseQueries(user);
            if (!db.IsUserLoggedIn(user))
            {
                throw new Exception($"User {user} is not logged in.");
            }
        }

    }
}
