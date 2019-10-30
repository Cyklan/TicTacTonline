using System.IO;
using Server.General;

namespace Server.Configurations
{
    public class ServerConfiguration : Configuration
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }

        public override bool IsAvailable() => File.Exists(pathmanager.ServerConfigurationPath);

        public override void Load()
        {
            ServerConfiguration serverConfig = LoadConfiguration<ServerConfiguration>(pathmanager.ServerConfigurationPath);

            Ip = serverConfig.Ip;
            Port = serverConfig.Port;
            Ssl = serverConfig.Ssl;
        }

        public override void Setup()
        {
            Ip = "127.0.0.1";
            Port = 31415;
            Ssl = false;

            SaveConfiguration(pathmanager.ServerConfigurationPath);
        }
    }
}
