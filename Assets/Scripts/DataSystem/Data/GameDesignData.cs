using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    public class GameDesignData
    {
        public static Dictionary<string, CharacterData> CharacterDataDic
        {
            get => _characterDataDic ??= GenerateCharacterDataDictionary();
            set => _characterDataDic = value;
        }
        private static Dictionary<string, CharacterData> _characterDataDic;
        public static CharacterData GetCharacterDataById(string id) => CharacterDataDic[id];

        private static Dictionary<string, CharacterData> GenerateCharacterDataDictionary()
        {
            var characters = Resources.LoadAll<CharacterData>("ScriptableObjects/CharacterData");
            var dic = new Dictionary<string, CharacterData>();

            foreach (var character in characters)
            {
                dic[character.Id] = character;
            }
            return dic;
        }

        public static PuzzleData GetPuzzleDataById(string id)
        {
            return Resources.Load<PuzzleData>("ScriptableObjects/PuzzleData/" + id);
        }

        public static HintData GetHintDataById(string id)
        {
            return Resources.Load<HintData>("ScriptableObjects/HintData/" + id);
        }
    }

}