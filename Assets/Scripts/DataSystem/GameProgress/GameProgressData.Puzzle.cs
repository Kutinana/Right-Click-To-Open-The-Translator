using Puzzle;

namespace DataSystem
{
    public partial class GameProgressData
    {
        public static PuzzleProgress GetPuzzleProgress(string _id)
        {
            return Instance.Save.PuzzleProgress.TryGetValue(_id, out var progress) ? progress : PuzzleProgress.NotFound;
        }

        public static void Unlock(PuzzleBase puzzle)
        {
            if (Instance.Save.PuzzleProgress.ContainsKey(puzzle.Id)) return;

            Instance.Save.PuzzleProgress.Add(puzzle.Id, PuzzleProgress.UnSolved);
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

        public static void Solve(string puzzleId)
        {
            if (Instance.Save.PuzzleProgress.ContainsKey(puzzleId))
            {
                Instance.Save.PuzzleProgress[puzzleId] = PuzzleProgress.Solved;
            }
            else
            {
                Instance.Save.PuzzleProgress.Add(puzzleId, PuzzleProgress.Solved);
            }

            Instance.Serialization();
        }
    }
}