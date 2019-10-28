using System.IO;
using System.Text.RegularExpressions;
using Server.General;

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
            string configDirectory = Pathmanager.ConfigurationDirectory;
            string databaseConfigPath = Pathmanager.DatabaseConfigurationPath;

            bool invalidIp = true;
            do
            {
                Ip = UserInput.InputString("Ip Address", "127.0.0.1");

                Regex regex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
                Match ipMatch = regex.Match(Ip);

                if (ipMatch.Success) invalidIp = false;
                else Log.Add("Please enter a valid IP address", MessageType.Error);
            } while (invalidIp);

            Port = UserInput.InputInteger("Port", 3306, true, false);
            Database = UserInput.InputString("Database schema", "tictactonline");
            User = UserInput.InputString("Database user", "TTO");
            Password = UserInput.InputString("User password", "tictactonline");

            Directory.CreateDirectory(configDirectory);
            SaveConfiguration<DatabaseConfiguration>(databaseConfigPath);

        }
    }
}
