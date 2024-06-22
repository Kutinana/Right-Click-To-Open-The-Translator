using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.IO;
using Newtonsoft.Json;

namespace DataSystem
{
    public class UserDictionary : ReadableAndWriteableData , ISingleton
    {
        [JsonIgnore] public override string Path { get => System.IO.Path.Combine(Application.persistentDataPath, "UserDictionary.json"); }
        public static UserDictionary Instance
        {
            get => _instance ??= new UserDictionary().DeSerialization<UserDictionary>();
            private set => _instance = value;
        }
        private static UserDictionary _instance;
        public void OnSingletonInit() {}

        [JsonProperty] internal Dictionary<string, string> Dictionary { get; set; } = new Dictionary<string, string>();

        public static Dictionary<string, string> GetDictionary()
        {
            return Instance.Dictionary;
        }

        public static bool IsEmpty()
        {
            return Instance.Dictionary.Count == 0;
        }

        public static string Read(string id)
        {
            return Instance.Dictionary.ContainsKey(id) ? Instance.Dictionary[id] : "";
        }

        public static void Unlock(string id)
        {
            if (Instance.Dictionary.ContainsKey(id)) return;

            Instance.Dictionary.Add(id, "");
            Instance.Serialization();
        }

        public static void Unlock(List<string> ids)
        {
            foreach (var id in ids)
            {
                if (Instance.Dictionary.ContainsKey(id)) continue;
                Instance.Dictionary.Add(id, "");
            }
            Instance.Serialization();
        }

        public static void WriteInAndSave(string id, string content)
        {
            if (Instance.Dictionary.ContainsKey(id))
            {
                Instance.Dictionary.Remove(id);
            }
            Instance.Dictionary.Add(id, content);

            Instance.Serialization();
        }

        public static void Clean()
        {
            _instance = new();
            Instance.Serialization();
        }
    }
}