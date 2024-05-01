using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace Cameras
{
    public class TranslatorCanvasManager : MonoSingleton<TranslatorCanvasManager>
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }

}