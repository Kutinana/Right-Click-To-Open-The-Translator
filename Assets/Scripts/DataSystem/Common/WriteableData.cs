using System;
using System.IO;
using Newtonsoft.Json;

namespace DataSystem
{
    public class WriteableData : IWriteableData
    {
        public static void Serialization(string _path, object _object)
        {
            if (File.Exists(_path)) File.Delete(_path);
            File.Create(_path).Dispose();

            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
            File.WriteAllText(_path, JsonConvert.SerializeObject(_object, Formatting.Indented, settings));
        }
    }

    public interface IWriteableData
    {
        public virtual void Serialization(string _path)
        {
            if (File.Exists(_path)) File.Delete(_path);
            File.Create(_path).Dispose();

            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
            File.WriteAllText(_path, JsonConvert.SerializeObject(this, Formatting.Indented, settings));
        }
    }
}