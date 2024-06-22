using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    public enum PuzzleProgress
    {
        NotFound,
        UnSolved,
        Solved
    }

    public class SaveData
    {
        public string LastScene;
        public Dictionary<string, PuzzleProgress> PuzzleProgress = new Dictionary<string, PuzzleProgress>();
        public Dictionary<string, int> Inventory = new Dictionary<string, int>();
    }
}