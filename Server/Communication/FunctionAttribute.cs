using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Communication
{
    public class FunctionAttribute : Attribute
    {
        public string Name { get; private set; }
        public FunctionAttribute(string name)
        {
            Name = name;
        }
    }
}
