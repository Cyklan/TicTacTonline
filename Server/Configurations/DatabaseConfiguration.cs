namespace Server.Configurations
{
    public class DatabaseConfiguration : Configuration
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public override void Load()
        {
            DatabaseConfiguration databaseConfig = LoadConfiguration<DatabaseConfiguration>(General.Pathmanager.DatabaseConfigurationPath);

            Ip = databaseConfig.Ip;
            Port = databaseConfig.Port;
            Database = databaseConfig.Database;
            User = databaseConfig.User;
            Password = databaseConfig.Password;
        }
    }
}
