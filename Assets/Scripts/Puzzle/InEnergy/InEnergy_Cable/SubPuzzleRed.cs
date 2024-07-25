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
            GameProgressData.Instance.Save.PuzzleProgress.Add("SubPuzzleRed", PuzzleProgress.Solved);
            Debug.Log("Red");
        }
    }
}