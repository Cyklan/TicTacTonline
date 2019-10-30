using System;
using System.IO;
using Server.General;

namespace Server.Configurations
{
    public abstract class Configuration
    {
        protected Converter converter { get; set; }
        protected Pathmanager pathmanager { get; set; }

        public Configuration()
        {
            converter = new Converter();
            pathmanager = new Pathmanager();
        }


        protected T LoadConfiguration<T>(string pathToFile)
        {
            return converter.ConvertJsonToObject<T>(File.ReadAllText(pathToFile));
        }

        protected void SaveConfiguration(string pathToFile)
        {
            File.WriteAllText(pathToFile, converter.ConvertObjectToJson(this));
        }

        public abstract bool IsAvailable();

        public abstract void Setup();

        public abstract void Load();
    }
}
