namespace Server.Configurations
{
    public class ServerConfiguration : Configuration
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }

        public override void Load()
        {
            ServerConfiguration serverConfig = LoadConfiguration<ServerConfiguration>(General.Pathmanager.ServerConfigurationPath);

            Ip = serverConfig.Ip;
            Port = serverConfig.Port;
            Ssl = serverConfig.Ssl;
        }

    }
}
