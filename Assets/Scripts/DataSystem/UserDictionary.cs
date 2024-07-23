using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.IO;
using Newtonsoft.Json;
using Puzzle;

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

        [JsonProperty] internal Dictionary<string, CharacterProgressData> Dictionary { get; set; } = new();

        public static Dictionary<string, CharacterProgressData> GetDictionary()
        {
            return Instance.Dictionary;
        }

        public static bool IsEmpty()
        {
            return Instance.Dictionary.Count == 0;
        }

        public static CharacterProgressData Read(string id)
        {
            return Instance.Dictionary.TryGetValue(id, out var value) ? value : null;
        }

        public static void Unlock(string id)
        {
            if (Instance.Dictionary.ContainsKey(id)) return;

            Instance.Dictionary.Add(id, new CharacterProgressData(id));
            Instance.Serialization();
        }

        public static void Unlock(List<string> ids)
        {
            foreach (var id in ids)
            {
                if (Instance.Dictionary.ContainsKey(id)) continue;
                Instance.Dictionary.Add(id, new CharacterProgressData(id));
            }
            Instance.Serialization();
        }

        public static void AddRelatedPuzzleAndSave(string id, string puzzleId)
        {
            if (!Instance.Dictionary.TryGetValue(id, out var value))
            {
                value = new CharacterProgressData(id);
                Instance.Dictionary.Add(id, value);
            }
            value.RelatedPuzzles.Add(puzzleId);

            Instance.Serialization();
        }

        public static void AddRelatedPuzzleAndSave(List<string> ids, string puzzleId)
        {
            foreach (var id in ids)
            {
                if (!Instance.Dictionary.TryGetValue(id, out var value))
                {
                    value = new CharacterProgressData(id);
                    Instance.Dictionary.Add(id, value);
                }
                value.RelatedPuzzles.Add(puzzleId);
            }
            Instance.Serialization();
        }

        public static void WriteInAndSave(string id, string meaning)
        {
            if (!Instance.Dictionary.TryGetValue(id, out var value))
            {
                value = new CharacterProgressData(id);
                Instance.Dictionary.Add(id, value);
            }
            value.Meaning = meaning;

            Instance.Serialization();
        }

        public static void Clean()
        {
            _instance = new();
            Instance.Serialization();
        }
    }
}