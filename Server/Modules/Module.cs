﻿using System;
using System.Collections.Generic;
using Server.Communication;
using Models;
using System.Reflection;

namespace Server.Modules
{
    public abstract class Module
    {
        public string Name;

        public Module(string name)
        {
            Name = name;
        }

        public Response ProcessRequest(Request request)
        {
            foreach (MethodInfo m in GetType().GetMethods())
            {
                if (m.Name.ToLower() == request.Header.Identifier.Function.ToLower())
                {
                    return (Response)m.Invoke(this, new object[] { request });
                }
            }

            throw new NotImplementedException($"Function {request.Header.Identifier.Function} is not part of module {request.Header.Identifier.Module}");
        }

    }
}
