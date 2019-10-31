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

        private Pathmanager pathmanager = new Pathmanager();
        private Converter converter = new Converter();

        public bool IsAvailable() => File.Exists(pathmanager.WebsocketConfigurationPath);

        public void Load()
        {
            WebsocketConfiguration ws = converter.ConvertJsonToObject<WebsocketConfiguration>(File.ReadAllText(pathmanager.WebsocketConfigurationPath));
            Ip = ws.Ip;
            Port = ws.Port;
            Ssl = ws.Ssl;
        }

        public void Setup()
        {
            Ip = "127.0.0.1";
            Port = 31415;
            Ssl = false;

            File.WriteAllText(pathmanager.WebsocketConfigurationPath, converter.ConvertObjectToJson(this));
        }

        public Uri GetWebsocketUri()
        {
            if (Ssl)
            {
                return new Uri(@$"wss://{Ip}:{Port}/");
            }
            else
            {
                return new Uri(@$"ws://{Ip}:{Port}/");
            }
        }

    }
}
