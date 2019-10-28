using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ResponseHeader: Header
    {
        public List<User> Targets { get; set; }
        public ResposneCode Code { get; set; }
        public string Message { get; set; }
    }
}
