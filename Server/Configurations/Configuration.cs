using System;
using System.IO;
using Server.General;

namespace Server.Configurations
{
    public abstract class Configuration
    {
        protected T LoadConfiguration<T>(string pathToFile)
        {
            return Converter.ConvertJsonToObject<T>(File.ReadAllText(pathToFile));
        }

        protected void SaveConfiguration(string pathToFile)
        {
            string json = Converter.ConvertObjectToJson(this);
            try
            {
                File.WriteAllText(pathToFile, json);
            } catch (Exception e) 
            {
                Log.Add(e.ToString(), MessageType.Error);
            }
        }

        public abstract bool IsAvailable();

        public abstract void Setup();

        public abstract void Load();
    }
}
