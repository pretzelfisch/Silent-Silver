using System;

namespace SilverPublish
{
    internal static class JsonHelper
    {
        public static Newtonsoft.Json.JsonSerializer Serializer()
        {
            return new Newtonsoft.Json.JsonSerializer();
        }
        public static T DeserializeFromString<T>(String inputString) where T : new()
        {
            T data;
            if (string.IsNullOrWhiteSpace(inputString))
                return new T();
            using (var stream = new System.IO.MemoryStream(System.Text.Encoding.Default.GetBytes(inputString)))
            {
                data = DeserializeFromStream<T>(stream);
            }
            return data;
        }
        public static T DeserializeFromStream<T>(System.IO.Stream inputStream)
        {
            var serializer = Serializer();
            T data;
            using (var streamReader = new System.IO.StreamReader(inputStream))
            {
                data = (T)serializer.Deserialize(streamReader, typeof(T));
            }
            return data;
        }
        public static String SerializeToString<T>(T data)
        {
            var serializer = Serializer();
            using (var sw = new System.IO.StringWriter())
            using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                serializer.Serialize(writer, data);
                return sw.ToString();
            }
        }

    }


}
