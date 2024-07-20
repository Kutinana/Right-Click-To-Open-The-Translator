using DataSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace Puzzle.InEnergy.Cable
{
    public class SubPuzzleRed: PuzzleBase
    {
        public static SubPuzzleRed Instance;

        public void SubPuzzleSolved()
        {
            GameProgressData.Solve(this);
            Debug.Log("Red");
        }
    }
}