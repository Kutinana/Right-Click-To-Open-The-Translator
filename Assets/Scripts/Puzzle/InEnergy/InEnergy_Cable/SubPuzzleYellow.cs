using DataSystem;
using UnityEngine;
namespace Puzzle.InEnergy.Cable
{
    public class SubPuzzleYellow : PuzzleBase
    {
        public static SubPuzzleYellow Instance;

        public void SubPuzzleSolved()
        {
            GameProgressData.Instance.Save.PuzzleProgress.Add("SubPuzzleYellow", PuzzleProgress.Solved);
            Debug.Log("Yellow");
        }
    }
}