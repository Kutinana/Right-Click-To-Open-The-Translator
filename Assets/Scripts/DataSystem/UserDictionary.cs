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
        public static Dictionary<string, string> userDictionary = new Dictionary<string, string>();

        public static UserDictionary Instance
        {
            get { return SingletonProperty<UserDictionary>.Instance; }
        }

        public void Dispose()
        {
            SingletonProperty<UserDictionary>.Dispose();
        }

        public void OnSingletonInit()
        {
            userDictionary = new Dictionary<string, string>();
            DeSerialization();
        }

        public static string Read(string id)
        {
            return userDictionary.ContainsKey(id) ? userDictionary[id] : "";
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

        private static void DeSerialization()
        {
            if (File.Exists(userDictionaryPath))
            {
                string json = File.ReadAllText(userDictionaryPath);
                userDictionary = JsonMapper.ToObject<Dictionary<string, string>>(json);
            }
        }
    }
}