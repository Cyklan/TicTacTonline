using Newtonsoft.Json;
using Server.General;

namespace Server.Configurations
{
    public abstract class Configuration
    {
        protected T LoadConfiguration<T>(string pathToFile)
        {
            return Converter.ConvertJsonToObject<T>(System.IO.File.ReadAllText(pathToFile));
        }

        protected void SaveConfiguration<T> (string pathToFile)
        {
            string json = Converter.ConvertObjectToJson(this);
            try
            {
                System.IO.File.WriteAllText(pathToFile, json);
            } catch (System.Exception e) 
            {
                Log.Add(e.ToString(), MessageType.Error);
            }
        }

        public abstract bool IsAvailable();

        public abstract void Setup();

        public abstract void Load();
    }
}
