using System.Collections.Generic;

namespace DataSystem
{
    public enum PuzzleProgress
    {
        NotFound,
        UnSolved,
        Solved
    }

    public enum HintProgress
    {
        NotFound,
        Found
    }

    public class SaveData
    {
        public Dictionary<string, PuzzleProgress> PuzzleProgress = new Dictionary<string, PuzzleProgress>();
        public Dictionary<string, HintProgress> HintProgress = new Dictionary<string, HintProgress>();
        public Dictionary<string, int> Inventory = new Dictionary<string, int>();
    }
}