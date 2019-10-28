using System.Collections.Generic;

namespace Models
{
    public class ResponseHeader: Header
    {
        public List<User> Targets { get; set; }
        public ResponseCode Code { get; set; }
        public string Message { get; set; }
    }
}
