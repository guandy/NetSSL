using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSSL
{
    /// <summary>
    /// JSON帮助类
    /// </summary>
    public static class JsonHelper
    {
        private static readonly JsonConverter[] JavaScriptConverters = new JsonConverter[]{
          new  DateTimeConverter()
        };
        /// <summary>
        /// 对象转成json格式的字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data, JavaScriptConverters);
        }
        /// <summary>
        /// json字符串转对象
        /// </summary>
        /// <param name="json"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object Deserialize(string json, Type targetType)
        {
            return JsonConvert.DeserializeObject(json, targetType);
        }
        /// <summary>
        /// json字符串转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            return !string.IsNullOrWhiteSpace(json)
                       ? JsonConvert.DeserializeObject<T>(json)
                       : default(T);
        }
        /// <summary>
        /// 读取json文件路径转成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        public static T Map<T>(string jsonPath)
        {
            using (FileStream fileStream = File.OpenRead(jsonPath))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(fileStream, System.Text.Encoding.UTF8);
                string nextLine;
                nextLine = sr.ReadToEnd();
                return Deserialize<T>(nextLine);
            }
        }
    }

    internal class DateTimeConverter : Newtonsoft.Json.Converters.DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss"));
        }
    }
}
