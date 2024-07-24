using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.IO;
using Newtonsoft.Json;
using Puzzle;

namespace DataSystem
{
    public class UserDictionary
    {
        public static Dictionary<string, CharacterProgressData> Dictionary => GameProgressData.Instance.Dictionary;

        public static Dictionary<string, CharacterProgressData> GetDictionary() => Dictionary;
        public static bool IsEmpty() => Dictionary.Count == 0;
        public static string Read(string id) => Dictionary.TryGetValue(id, out var value) ? value.Meaning : "";
        public static CharacterProgressData GetCharacterProgressData(string id) => Dictionary.TryGetValue(id, out var value) ? value : null;
        public static bool TryGetCharacterProgressData(string id, out CharacterProgressData value) => Dictionary.TryGetValue(id, out value);

        public static void Unlock(string id)
        {
            if (Dictionary.ContainsKey(id)) return;

            Dictionary.Add(id, new CharacterProgressData(id));
            GameProgressData.Instance.Serialization();
        }

        public static void Unlock(List<string> ids)
        {
            foreach (var id in ids)
            {
                if (Dictionary.ContainsKey(id)) continue;
                Dictionary.Add(id, new CharacterProgressData(id));
            }
            GameProgressData.Instance.Serialization();
        }

        public static void AddRelatedPuzzleAndSave(string id, string puzzleId)
        {
            if (!Dictionary.TryGetValue(id, out var value))
            {
                value = new CharacterProgressData(id);
                Dictionary.Add(id, value);
            }
            if (!value.RelatedPuzzles.Contains(puzzleId)) value.RelatedPuzzles.Add(puzzleId);

            GameProgressData.Instance.Serialization();
        }

        public static void AddRelatedPuzzleAndSave(List<string> ids, string puzzleId)
        {
            foreach (var id in ids)
            {
                if (!Dictionary.TryGetValue(id, out var value))
                {
                    value = new CharacterProgressData(id);
                    Dictionary.Add(id, value);
                }
                if (!value.RelatedPuzzles.Contains(puzzleId)) value.RelatedPuzzles.Add(puzzleId);
            }
            GameProgressData.Instance.Serialization();
        }

        public static void WriteInAndSave(string id, string meaning)
        {
            if (!Dictionary.TryGetValue(id, out var value))
            {
                value = new CharacterProgressData(id);
                Dictionary.Add(id, value);
            }
            value.Meaning = meaning;

            GameProgressData.Instance.Serialization();
        }

        public static void WriteInAndSave(Dictionary<string, string> pairs)
        {
            foreach (var pair in pairs)
            {
                if (!Dictionary.TryGetValue(pair.Key, out var value))
                {
                    value = new CharacterProgressData(pair.Key);
                    Dictionary.Add(pair.Key, value);
                }
                value.Meaning = pair.Value;
            }
            GameProgressData.Instance.Serialization();
        }
    }
}