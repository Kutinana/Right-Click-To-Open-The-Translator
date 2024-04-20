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
        [JsonIgnore] public static string Path { get => System.IO.Path.Combine(Application.persistentDataPath, "GameProgressData.json"); }
        public static SaveData Instance
        {
            get => _instance ??= DeSerialization<SaveData>(Path);
            private set => _instance = value;
        }
        private static SaveData _instance;
        public void OnSingletonInit() {}

        public static PuzzleProgress GetPuzzleProgress(string _id)
        {
            return Instance.PuzzleProgress.ContainsKey(_id) ? Instance.PuzzleProgress[_id] : PuzzleProgress.UnSolved;
        }

        public static HintProgress GetHintProgress(string _id)
        {
            return Instance.HintProgress.ContainsKey(_id) ? Instance.HintProgress[_id] : HintProgress.NotFound;
        }

        public static void Unlock(PuzzleBase puzzle)
        {
            if (Instance.PuzzleProgress.ContainsKey(puzzle.Id)) return;

            Instance.PuzzleProgress.Add(puzzle.Id, PuzzleProgress.UnSolved);
            Serialization(Path, Instance);
        }

        public static void Unlock(HintBase hint)
        {
            if (Instance.HintProgress.ContainsKey(hint.Id)) return;

            Instance.HintProgress.Add(hint.Id, HintProgress.Found);
            Serialization(Path, Instance);
        }

        public static void Solve(PuzzleBase puzzle)
        {
            if (Instance.PuzzleProgress.ContainsKey(puzzle.Id))
            {
                Instance.PuzzleProgress[puzzle.Id] = PuzzleProgress.Solved;
            }
            else
            {
                Instance.PuzzleProgress.Add(puzzle.Id, PuzzleProgress.Solved);
            }

            Serialization(Path, Instance);
        }

        public static void IncreaseInventory(string _id, int _delta)
        {
            var inventory = Instance.Inventory;
            if (inventory.ContainsKey(_id))
            {
                inventory[_id] += _delta;
            }
            else inventory.Add(_id, _delta);

            Serialization(Path, Instance);
            TypeEventSystem.Global.Send(new OnInventoryIncreasedEvent(new Dictionary<string, int>() {{_id, _delta}}));
        }

        public static void IncreaseInventory(Dictionary<string, int> _items)
        {
            var inventory = Instance.Inventory;
            foreach (var item in _items)
            {
                if (inventory.ContainsKey(item.Key)) inventory[item.Key] += item.Value;
                else inventory.Add(item.Key, item.Value);
            }

            Serialization(Path, Instance);
            TypeEventSystem.Global.Send(new OnInventoryIncreasedEvent(_items));
        }

        public static bool TryDecreaseInventory(string _id, int _delta)
        {
            var inventory = Instance.Inventory;
            if (inventory.TryGetValue(_id, out var value))
            {
                if (value >= _delta)
                {
                    inventory[_id] -= _delta;

                    Serialization(Path, Instance);
                    return true;
                }
            }
            return false;
        }

        public static bool TryDecreaseInventory(Dictionary<string, int> _items)
        {
            var inventory = Instance.Inventory;

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
            Serialization(Path, Instance);
            return true;
        }

        public static void Clean()
        {
            if (Directory.Exists(Application.persistentDataPath))
            {
                var list = Directory.GetFiles(Application.persistentDataPath);
                foreach (var file in list)
                {
                    File.Delete(file);
                }
            }
        }
    }
}