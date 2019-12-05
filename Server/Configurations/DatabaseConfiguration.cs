using System.IO;
using Server.General;

namespace Server.Configurations
{
    /// <summary>
    /// Kümmert sich um die Konfiguration der Datenbank,
    /// da diese aus einer Datei ausgelesen wird
    /// </summary>
    public class DatabaseConfiguration : Configuration
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public override bool IsAvailable() => File.Exists(pathmanager.DatabaseConfigurationPath);

        public override void Load()
        {
            DatabaseConfiguration databaseConfig = LoadConfiguration<DatabaseConfiguration>(pathmanager.DatabaseConfigurationPath);

            Ip = databaseConfig.Ip;
            Port = databaseConfig.Port;
            Database = databaseConfig.Database;
            User = databaseConfig.User;
            Password = databaseConfig.Password;
        }

        public override void Setup()
        {
            Ip = "127.0.0.1";
            Port = 3306;
            Database = "";
            User = "";
            Password = "";

            SaveConfiguration(pathmanager.DatabaseConfigurationPath);
        }
    }
}
