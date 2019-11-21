using System;
using System.IO;
using Server.General;

namespace Server.Configurations
{
    /// <summary>
    /// Konfigurations-Basisklasse
    /// Stellt den erbenden Konfigurationsdateien allgemeine Methoden zur Verfügung
    /// </summary>
    public abstract class Configuration
    {
        protected Converter converter { get; set; }
        protected Pathmanager pathmanager { get; set; }

        public Configuration()
        {
            converter = new Converter();
            pathmanager = new Pathmanager();
        }

        /// <summary>
        /// Läd Konfigurationen aus einer Konfigurationsdatei
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pathToFile"></param>
        /// <returns></returns>
        protected T LoadConfiguration<T>(string pathToFile)
        {
            return converter.ConvertJsonToObject<T>(File.ReadAllText(pathToFile));
        }

        /// <summary>
        /// Speichert Konfigurationen in einer Konfigurationsdatei
        /// </summary>
        /// <param name="pathToFile"></param>
        protected void SaveConfiguration(string pathToFile)
        {
            File.WriteAllText(pathToFile, converter.ConvertObjectToJson(this));
        }

        /// <summary>
        /// Testet, ob die Konfigurationsdatei existiert
        /// </summary>
        /// <returns></returns>
        public abstract bool IsAvailable();

        /// <summary>
        /// Erstellt die Standard Konfigurationsdatei
        /// </summary>
        public abstract void Setup();

        /// <summary>
        /// Läd die Konfiguration aus der Konfigurationsdatei
        /// </summary>
        public abstract void Load();
    }
}
