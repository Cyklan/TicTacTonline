using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Client
{
    public class WebsocketConfiguration
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }

        public bool IsAvailable()
        {
            return true;
        }

        public void Load()
        {

        }

        public void Setup()
        {

        }

        public Uri GetWebsocketUri()
        {
            return new Uri("http://");
        }

    }
}
