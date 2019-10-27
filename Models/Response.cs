using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public enum ResposneCode
    {
        Ok,
        UnplannedError,
        PlannedError
    }

    public class Response
    {
        public ResponseHeader Header { get; set; }
        public Document Body { get; set; }
    }
}
