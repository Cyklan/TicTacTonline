using System.Text;
using Newtonsoft.Json;

namespace Server.General
{
    /// <summary>
    /// Serialisiert und deserialisiert alle Daten für die Netzwerkübertragung 
    /// sowie das Abspeichern von Konfigurationsdateien
    /// </summary>
    public class Converter
    {
        public string ConvertBytesToString(byte[] bytes)
        {
            return new ASCIIEncoding().GetString(bytes);
        }

        public T ConvertJsonToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string ConvertObjectToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj,Formatting.Indented);
        }

        public byte[] ConvertStringToBytes(string data)
        {
            return new ASCIIEncoding().GetBytes(data);
        }

    }
}
