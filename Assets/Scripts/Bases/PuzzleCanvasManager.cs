using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Kuchinashi;
using UnityEngine.Video;
using Translator;
using Puzzle;
using Hint;

namespace Cameras
{
    public class PuzzleCanvasManager : MonoSingleton<PuzzleCanvasManager>
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }

}