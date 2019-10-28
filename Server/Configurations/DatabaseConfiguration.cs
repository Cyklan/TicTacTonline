using System.IO;
using Server.General;
using System.Text.RegularExpressions;

namespace Server.Configurations
{
    public class DatabaseConfiguration : Configuration
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public override bool IsAvailable() => File.Exists(Pathmanager.DatabaseConfigurationPath);

        public override void Load()
        {
            DatabaseConfiguration databaseConfig = LoadConfiguration<DatabaseConfiguration>(Pathmanager.DatabaseConfigurationPath);

            Ip = databaseConfig.Ip;
            Port = databaseConfig.Port;
            Database = databaseConfig.Database;
            User = databaseConfig.User;
            Password = databaseConfig.Password;
        }

        public override void Setup()
        {
            Ip = UserInput.InputIp();
            Port = UserInput.InputInteger("Port", 3306, true, false);
            Database = UserInput.InputString("Database schema", "tictactonline");
            User = UserInput.InputString("Database user", "TTO");
            Password = UserInput.InputString("User password", "tictactonline");

            SaveConfiguration(Pathmanager.DatabaseConfigurationPath);
        }
    }
}
