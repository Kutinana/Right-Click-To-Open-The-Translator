using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Puzzle
{
    public class PuzzleManager : MonoBehaviour , ISingleton
    {
        public static PuzzleManager Instance => SingletonProperty<PuzzleManager>.Instance;
        public void OnSingletonInit() {}

        
    }
}