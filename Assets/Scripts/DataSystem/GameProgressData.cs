using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.IO;
using Newtonsoft.Json;
using Puzzle;
using Hint;

namespace DataSystem
{
    public partial class GameProgressData : ReadableAndWriteableData, ISingleton
    {
        [JsonIgnore] public override string Path { get => System.IO.Path.Combine(Application.persistentDataPath, "save.json"); }
        public static GameProgressData Instance
        {
            get => _instance ??= new GameProgressData().DeSerialization<GameProgressData>();
            private set => _instance = value;
        }
        private static GameProgressData _instance;
        public void OnSingletonInit() {}

        [JsonProperty] internal SaveData Save = new();

        public static string GetLastScene()
        {
            return Instance.Save.LastScene ??= "Zero";
        }

        public static void SaveLastScene(string name)
        {
            Instance.Save.LastScene = name;
            Instance.Serialization();
        }

        public static PuzzleProgress GetPuzzleProgress(string _id)
        {
            return Instance.Save.PuzzleProgress.ContainsKey(_id) ? Instance.Save.PuzzleProgress[_id] : PuzzleProgress.UnSolved;
        }

        public static void Unlock(PuzzleBase puzzle)
        {
            if (Instance.Save.PuzzleProgress.ContainsKey(puzzle.Id)) return;

            Instance.Save.PuzzleProgress.Add(puzzle.Id, PuzzleProgress.UnSolved);
            Instance.Serialization();
        }

        public static void Unlock(HintBase hint)
        {
            if (Instance.Save.PuzzleProgress.ContainsKey(hint.Id)) return;

            Instance.Save.PuzzleProgress.Add(hint.Id, PuzzleProgress.Solved);
            Instance.Serialization();
        }

        public static void Solve(PuzzleBase puzzle)
        {
            if (Instance.Save.PuzzleProgress.ContainsKey(puzzle.Id))
            {
                Instance.Save.PuzzleProgress[puzzle.Id] = PuzzleProgress.Solved;
            }
            else
            {
                Instance.Save.PuzzleProgress.Add(puzzle.Id, PuzzleProgress.Solved);
            }

            Instance.Serialization();
        }

        public static void IncreaseInventory(string _id, int _delta)
        {
            var inventory = Instance.Save.Inventory;
            if (inventory.ContainsKey(_id))
            {
                inventory[_id] += _delta;
            }
            else inventory.Add(_id, _delta);

            Instance.Serialization();
            TypeEventSystem.Global.Send(new OnInventoryIncreasedEvent(new Dictionary<string, int>() {{_id, _delta}}));
        }

        public static void IncreaseInventory(Dictionary<string, int> _items)
        {
            var inventory = Instance.Save.Inventory;
            foreach (var item in _items)
            {
                if (inventory.ContainsKey(item.Key)) inventory[item.Key] += item.Value;
                else inventory.Add(item.Key, item.Value);
            }

            Instance.Serialization();
            TypeEventSystem.Global.Send(new OnInventoryIncreasedEvent(_items));
        }

        public static bool TryDecreaseInventory(string _id, int _delta)
        {
            var inventory = Instance.Save.Inventory;
            if (inventory.TryGetValue(_id, out var value))
            {
                if (value >= _delta)
                {
                    inventory[_id] -= _delta;

                    Instance.Serialization();
                    return true;
                }
            }
            return false;
        }

        public static bool TryDecreaseInventory(Dictionary<string, int> _items)
        {
            var inventory = Instance.Save.Inventory;

            foreach (var item in _items)
            {
                if (!inventory.TryGetValue(item.Key, out var value) || value < item.Value)
                {
                    return false;
                }
            }
            
            foreach (var item in _items)
            {
                inventory[item.Key] -= item.Value;
            }
            Instance.Serialization();
            return true;
        }

        public static void Clean()
        {
            if (File.Exists(Instance.Path))
            {
                File.Delete(Instance.Path);
            }
            _instance = new();
        }

        public static void TryInherit(GameProgressData _data)
        {
            try
            {
                Instance.Save.PuzzleProgress = _data.Save.PuzzleProgress ?? new();

                Instance.Serialization();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}