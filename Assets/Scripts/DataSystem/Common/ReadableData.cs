using System;
using System.IO;
using Newtonsoft.Json;
using QFramework;
using UnityEngine;

namespace DataSystem
{
    public class ReadableData : IReadableData
    {
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

        public static T DeSerialization<T>(string _bundle, string _id) where T : new()
        {
            try
            {
                var resLoader = ResLoader.Allocate();
                string config = resLoader.LoadSync<TextAsset>(_bundle, _id).text;

                resLoader.Dispose();

                var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
                return JsonConvert.DeserializeObject<T>(config, settings);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return new T();
            }
        }

        public static bool Validation<T>(string _bundle, string _id, out T value) where T : new()
        {
            value = new T();
            try
            {
                // Ability of reading
                value = DeSerialization<T>(_bundle, _id);
                
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }

    public interface IReadableData
    {
        public virtual ReadableData DeSerialization(string _path)
        {
            if (string.IsNullOrEmpty(_path) || !File.Exists(_path)) return null;

            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.DeserializeObject<ReadableData>(File.ReadAllText(_path), settings);
        }

        public virtual T DeSerialization<T>(string _path) where T : new()
        {
            if (string.IsNullOrEmpty(_path) || !File.Exists(_path)) return new T();

            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(_path), settings);
        }
    }
}