using DataSystem;
using UnityEngine;
namespace Puzzle.InEnergy.Cable
{
    public class SubPuzzleYellow : PuzzleBase
    {
        public static SubPuzzleYellow Instance;

        public void SubPuzzleSolved()
        {
            GameProgressData.Solve(this);
            Debug.Log("Yellow");
        }
    }
}