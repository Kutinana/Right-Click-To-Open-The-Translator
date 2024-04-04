using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.IO;
using LitJson;

namespace DataSystem
{
    public class UserDictionary : ISingleton, IUtility
    {
        public static readonly string userDictionaryPath = Application.persistentDataPath + "/UserDictionary.json";
        public static Dictionary<string, string> userDictionary
        {
            get
            {
                if (_userDictionary == null) return DeSerialization();
                return _userDictionary;
            }
            set
            {
                _userDictionary = value;
            }
        }
        private static Dictionary<string, string> _userDictionary;

        public static UserDictionary Instance => SingletonProperty<UserDictionary>.Instance;

        public void Dispose()
        {
            SingletonProperty<UserDictionary>.Dispose();
        }

        public void OnSingletonInit() {}

        public static string Read(string id)
        {
            return userDictionary.ContainsKey(id) ? userDictionary[id] : "";
        }

        public static void Unlock(string id)
        {
            if (userDictionary.ContainsKey(id)) return;

            userDictionary.Add(id, "");
            Serialization();
        }

        public static void Unlock(List<string> ids)
        {
            foreach (var id in ids)
            {
                if (userDictionary.ContainsKey(id)) continue;
                userDictionary.Add(id, "");
            }
            Serialization();
        }

        public static void WriteInAndSave(string id, string content)
        {
            if (userDictionary.ContainsKey(id))
            {
                userDictionary.Remove(id);
            }
            userDictionary.Add(id, content);

            Serialization();
        }

        private static void Serialization()
        {
            if (File.Exists(userDictionaryPath))
            {
                File.Delete(userDictionaryPath);
                File.Create(userDictionaryPath).Dispose();
            }

            string json = Kuchinashi.JsonHelper.JsonFormatter(JsonMapper.ToJson(userDictionary));
            File.WriteAllText(userDictionaryPath, json);
        }

        private static Dictionary<string, string> DeSerialization()
        {
            if (File.Exists(userDictionaryPath))
            {
                string json = File.ReadAllText(userDictionaryPath);
                _userDictionary = JsonMapper.ToObject<Dictionary<string, string>>(json);
            }
            _userDictionary ??= new Dictionary<string, string>();
            
            return _userDictionary;
        }
    }
}