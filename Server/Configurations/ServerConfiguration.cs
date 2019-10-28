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
            string configDirectory = Pathmanager.ConfigurationDirectory;
            string serverConfigPath = Pathmanager.ServerConfigurationPath;


            bool invalidIp = true;
            do
            {
                Ip = UserInput.InputString("Ip Address", "127.0.0.1");
                Regex regex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
                Match ipMatch = regex.Match(Ip);

                if (ipMatch.Success) invalidIp = false;
                else Log.Add("Please enter a valid IP address", MessageType.Error);
            } while (invalidIp);

            Port = UserInput.InputInteger("Port", 31415, true, false);
            Ssl = UserInput.InputBoolean("Use SSL", false);

            Directory.CreateDirectory(configDirectory);
            SaveConfiguration<ServerConfiguration>(serverConfigPath);
        }
    }
}
