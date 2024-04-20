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
        [JsonIgnore] public static string Path { get => System.IO.Path.Combine(Application.persistentDataPath, "UserDictionary.json"); }
        public static Dictionary<string, string> Instance
        {
            get
            {
                return _instance ??= DeSerialization<Dictionary<string, string>>(Path);
            }
            set
            {
                _instance = value;
            }
        }
        private static Dictionary<string, string> _instance;
        public void OnSingletonInit() {}

        public static bool IsEmpty()
        {
            return Instance.Count == 0;
        }

        public static string Read(string id)
        {
            return Instance.ContainsKey(id) ? Instance[id] : "";
        }

        public static void Unlock(string id)
        {
            if (Instance.ContainsKey(id)) return;

            Instance.Add(id, "");
            Serialization(Path, Instance);
        }

        public static void Unlock(List<string> ids)
        {
            foreach (var id in ids)
            {
                if (Instance.ContainsKey(id)) continue;
                Instance.Add(id, "");
            }
            Serialization(Path, Instance);
        }

        public static void WriteInAndSave(string id, string content)
        {
            if (Instance.ContainsKey(id))
            {
                Instance.Remove(id);
            }
            Instance.Add(id, content);

            Serialization(Path, Instance);
        }
    }
}