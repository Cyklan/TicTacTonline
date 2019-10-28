using System;
using System.IO;
using System.Reflection;

namespace Server.General
{
    public static class Pathmanager
    {
        public static string StartupDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        public static string LogDirectory
        {
            get
            {
                string logPath = Path.Combine(StartupDirectory, "Log");
                if (!File.Exists(logPath)) { Directory.CreateDirectory(logPath); }
                return logPath;
            }
        }

        public static string ConfigurationDirectory
        {
            get
            {
                string configPath = Path.Combine(StartupDirectory, "Configuration");
                if (!File.Exists(configPath)) { Directory.CreateDirectory(configPath); }
                return configPath;
            }
        }

        public static string ServerConfigurationPath
        {
            get
            {
                return Path.Combine(ConfigurationDirectory, "ServerConfiguration.json");
            }
        }

        public static string DatabaseConfigurationPath
        {
            get
            {
                return Path.Combine(ConfigurationDirectory, "DatabaseConfiguration.json");
            }
        }

        public static string LogFilePath
        {
            get
            {
                return Path.Combine(LogDirectory, $"{DateTime.Now.ToString("dd.MM.yyyy")} - Log.txt");
            }
        }
    }
}
