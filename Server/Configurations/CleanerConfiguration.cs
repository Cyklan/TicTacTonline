using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Configurations
{
    /// <summary>
    /// Kümmert sich um die Konfiguration der Cleaner-Klasse,
    /// da diese aus einer Datei ausgelesen wird
    /// </summary>
    public class CleanerConfiguration : Configuration
    {
        public int IntervalInMinutes { get; set; }

        public override bool IsAvailable() => File.Exists(pathmanager.CleanerConfigurationPath);


        public override void Load()
        {
            CleanerConfiguration databaseConfig = LoadConfiguration<CleanerConfiguration>(pathmanager.CleanerConfigurationPath);

            IntervalInMinutes = databaseConfig.IntervalInMinutes;
        }


        public override void Setup()
        {
            IntervalInMinutes = 1;

            SaveConfiguration(pathmanager.CleanerConfigurationPath);
        }
    }
}
