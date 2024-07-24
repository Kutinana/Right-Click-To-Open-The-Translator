using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Newtonsoft.Json;

namespace DataSystem
{
    public partial class GameProgressData : ReadableAndWriteableData, ISingleton
    {
        [JsonIgnore] public override string Path { get => System.IO.Path.Combine(Application.persistentDataPath, "save"); }
        public static GameProgressData Instance
        {
            get => _instance ??= new GameProgressData().DeSerialization<GameProgressData>();
            private set => _instance = value;
        }
        private static GameProgressData _instance;
        public void OnSingletonInit() {}

        [JsonProperty] internal SaveData Save = new();
        [JsonProperty] public Dictionary<string, CharacterProgressData> Dictionary = new();
    }
}