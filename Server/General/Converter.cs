﻿using System.Text;
using Newtonsoft.Json;

namespace Server.General
{
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
            return JsonConvert.SerializeObject(obj);
        }

        public byte[] ConvertStringToBytes(string data)
        {
            return new ASCIIEncoding().GetBytes(data);
        }

    }
}
