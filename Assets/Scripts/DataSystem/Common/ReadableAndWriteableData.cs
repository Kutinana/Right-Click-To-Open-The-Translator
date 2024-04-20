using System;
using System.IO;
using Newtonsoft.Json;

namespace DataSystem
{
    public class ReadableAndWriteableData : IReadableData , IWriteableData
    {
        public static void Serialization(string _path, object _object)
        {
            if (File.Exists(_path)) File.Delete(_path);
            File.Create(_path).Dispose();

            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
            File.WriteAllText(_path, JsonConvert.SerializeObject(_object, Formatting.Indented, settings));
        }

        public static ReadableData DeSerialization(string _path)
        {
            if (string.IsNullOrEmpty(_path) || !File.Exists(_path)) return null;

            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.DeserializeObject<ReadableData>(File.ReadAllText(_path), settings);
        }

        public static T DeSerialization<T>(string _path) where T : new()
        {
            if (string.IsNullOrEmpty(_path) || !File.Exists(_path)) return new T();

            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(_path), settings);
        }
    }
}