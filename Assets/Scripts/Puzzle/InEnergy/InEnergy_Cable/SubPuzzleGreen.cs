using DataSystem;
using UnityEngine;

namespace Puzzle.InEnergy.Cable
{
    public class SubPuzzleGreen : PuzzleBase
    {
        public static SubPuzzleGreen Instance;

        public void SubPuzzleSolved()
        {
            GameProgressData.Solve(this);
            Debug.Log("Green");
        }
    }
}