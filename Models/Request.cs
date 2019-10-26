using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Request
    {
        public RequestHeader Header { get; set; }
        public Body Body { get; set; }
    }
}
