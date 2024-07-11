using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    public class GameDesignData
    {
        internal static Dictionary<string, CharacterData> CharacterDataDic
        {
            get => _characterDataDic ??= GenerateDataDictionary<CharacterData>("ScriptableObjects/CharacterData");
            set => _characterDataDic = value;
        }
        private static Dictionary<string, CharacterData> _characterDataDic;
        public static CharacterData GetCharacterDataById(string id) => CharacterDataDic.TryGetValue(id, out var value) ? value : null;

        internal static Dictionary<string, PuzzleData> PuzzleDataDic
        {
            get => _puzzleDataDic ??= GenerateDataDictionary<PuzzleData>("ScriptableObjects/PuzzleData");
            set => _puzzleDataDic = value;
        }
        private static Dictionary<string, PuzzleData> _puzzleDataDic;
        public static PuzzleData GetPuzzleDataById(string id) => PuzzleDataDic.TryGetValue(id, out var value) ? value : null;

        internal static Dictionary<string, HintData> HintDataDic
        {
            get => _hintDataDic ??= GenerateDataDictionary<HintData>("ScriptableObjects/HintData");
            set => _hintDataDic = value;
        }
        private static Dictionary<string, HintData> _hintDataDic;
        public static HintData GetHintDataById(string id) => HintDataDic.TryGetValue(id, out var value) ? value : null;

        private static Dictionary<string, T> GenerateDataDictionary<T>(string _path) where T : ScriptableObject, IHaveId
        {
            var data = Resources.LoadAll<T>(_path);
            var dic = new Dictionary<string, T>();

            foreach (var item in data)
            {
                dic[item.Id] = item;
            }
            return dic;
        }
    }

}