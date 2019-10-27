using Newtonsoft.Json;

namespace Server.Configurations
{
    public abstract class Configuration
    {
        protected T LoadConfiguration<T>(string pathToFile)
        {
            return General.Converter.ConvertJsonToObject<T>(System.IO.File.ReadAllText(pathToFile));
        }

        public abstract void Load();
    }
}
