using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Kuchinashi;

namespace Cameras
{
    public class TranslatorCanvasManager : MonoSingleton<TranslatorCanvasManager>
    {
        private CanvasGroup mMainMenu;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            mMainMenu = transform.Find("MainMenu").GetComponent<CanvasGroup>();
        }

        public static void StartMainMenu()
        {
            Instance.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Instance.mMainMenu, 1f));
        }
    }

}