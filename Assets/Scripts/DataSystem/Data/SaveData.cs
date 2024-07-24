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

    public enum MissionProgress
    {
        Locked,
        Progressing,
        Completed
    }

    public class SaveData
    {
        public string LastScene = "";
        public Dictionary<string, PuzzleProgress> PuzzleProgress = new Dictionary<string, PuzzleProgress>();
        public Dictionary<string, MissionProgress> MissionProgress = new Dictionary<string, MissionProgress>();
        public List<string> ReadNarrations = new List<string>();
        public Dictionary<string, int> Inventory = new Dictionary<string, int>();
    }
}