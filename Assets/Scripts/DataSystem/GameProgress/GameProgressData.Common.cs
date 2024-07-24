using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace DataSystem
{
    public partial class GameProgressData
    {
        public static string GetLastScene()
        {
            return Instance.Save.LastScene ?? "Zero";
        }

        public static void SaveLastScene(string name)
        {
            Instance.Save.LastScene = name;
            Instance.Serialization();
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