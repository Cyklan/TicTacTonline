﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Client
{
    public class Pathmanager
    {
        public static string StartupDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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

        public string WebsocketConfigurationConfigurationPath
        {
            get
            {
                return Path.Combine(ConfigurationDirectory, "ServerConfiguration.json");
            }
        }

    }
}