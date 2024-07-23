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

        internal static Dictionary<string, PuzzleDataBase> PuzzleDataDic
        {
            get => _puzzleDataDic ??= GenerateDataDictionary<PuzzleDataBase>(new List<string>() {"ScriptableObjects/PuzzleData", "ScriptableObjects/HintData"});
            set => _puzzleDataDic = value;
        }
        private static Dictionary<string, PuzzleDataBase> _puzzleDataDic;
        public static PuzzleDataBase GetPuzzleDataById(string id) => PuzzleDataDic.TryGetValue(id, out var value) ? value : null;

        // internal static Dictionary<string, HintData> HintDataDic
        // {
        //     get => _hintDataDic ??= GenerateDataDictionary<HintData>("ScriptableObjects/HintData");
        //     set => _hintDataDic = value;
        // }
        // private static Dictionary<string, HintData> _hintDataDic;
        // public static HintData GetHintDataById(string id) => HintDataDic.TryGetValue(id, out var value) ? value : null;
        public static Dictionary<string, MissionData> MissionDataDic
        {
            get => _missionDataDic ??= GenerateDataDictionary<MissionData>("ScriptableObjects/MissionData");
            set => _missionDataDic = value;
        }
        private static Dictionary<string, MissionData> _missionDataDic;

        public static MissionData GetMissionDataById(string id) => MissionDataDic.TryGetValue(id, out var value) ? value : null;
        public static Dictionary<string, ObtainableObjectData> ObtainableObjectDataDic
        {
            get => _obtainableObjectDataDic ??= GenerateDataDictionary<ObtainableObjectData>("ScriptableObjects/ObtainableObjectData");
            set => _obtainableObjectDataDic = value;
        }
        private static Dictionary<string, ObtainableObjectData> _obtainableObjectDataDic;

        public static ObtainableObjectData GetObtainableObjectDataById(string id) => ObtainableObjectDataDic.TryGetValue(id, out var value) ? value : null;

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

        private static Dictionary<string, T> GenerateDataDictionary<T>(List<string> _paths) where T : ScriptableObject, IHaveId
        {
            var data = new List<T>();
            foreach (var path in _paths)
            {
                data.AddRange(Resources.LoadAll<T>(path));
            }

            var dic = new Dictionary<string, T>();
            foreach (var item in data)
            {
                dic[item.Id] = item;
            }
            return dic;
        }
    }

}