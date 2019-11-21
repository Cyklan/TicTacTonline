using System;
using System.IO;
using System.Reflection;

namespace Server.General
{
    /// <summary>
    /// Gibt die Pfade aller wichtigen Verzeichnisse und Dateien zurück
    /// </summary>
    public class Pathmanager
    {
        public static string StartupDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        public string LogDirectory
        {
            get
            {
                string logPath = Path.Combine(StartupDirectory, "Log");
                if (!File.Exists(logPath)) { Directory.CreateDirectory(logPath); }
                return logPath;
            }
        }

        public string ConfigurationDirectory
        {
            get
            {
                string configPath = Path.Combine(StartupDirectory, "Configuration");
                if (!File.Exists(configPath)) { Directory.CreateDirectory(configPath); }
                return configPath;
            }
        }

        public string ServerConfigurationPath
        {
            get
            {
                return Path.Combine(ConfigurationDirectory, "ServerConfiguration.json");
            }
        }

        public string DatabaseConfigurationPath
        {
            get
            {
                return Path.Combine(ConfigurationDirectory, "DatabaseConfiguration.json");
            }
        }

        public string CleanerConfigurationPath
        {
            get
            {
                return Path.Combine(ConfigurationDirectory, "CleanerConfiguration.json");
            }
        }

        public string LogFilePath
        {
            get
            {
                return Path.Combine(LogDirectory, $"{DateTime.Now.ToString("dd.MM.yyyy")} - Log.txt");
            }
        }
    }
}
