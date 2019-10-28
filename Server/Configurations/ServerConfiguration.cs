using System.IO;
using System.Text.RegularExpressions;
using Server.General;

namespace Server.Configurations
{
    public class ServerConfiguration : Configuration
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }

        public override bool IsAvailable() => File.Exists(Pathmanager.ServerConfigurationPath);

        public override void Load()
        {
            ServerConfiguration serverConfig = LoadConfiguration<ServerConfiguration>(Pathmanager.ServerConfigurationPath);

            Ip = serverConfig.Ip;
            Port = serverConfig.Port;
            Ssl = serverConfig.Ssl;
        }

        public override void Setup()
        {
            Ip = UserInput.InputIp();
            Port = UserInput.InputInteger("Port", 31415, true, false);
            Ssl = UserInput.InputBoolean("Use SSL", false);

            SaveConfiguration(Pathmanager.ServerConfigurationPath);
        }
    }
}
