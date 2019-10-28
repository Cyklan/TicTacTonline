using System.Text;
using Newtonsoft.Json;

namespace Server.General
{
    public static class Converter
    {
        public static string ConvertBytesToString(byte[] bytes)
        {
            return new ASCIIEncoding().GetString(bytes);
        }

        public static T ConvertJsonToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ConvertObjectToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static byte[] ConvertStringToBytes(string data)
        {
            return new ASCIIEncoding().GetBytes(data);
        }

    }
}
