using System.Collections;
using System.Collections.Generic;
using Cameras;
using Kuchinashi;
using QFramework;
using UnityEngine;

namespace StartScene
{
    public class StartSceneController : MonoSingleton<StartSceneController>
    {
        private CanvasGroup mSplashCanvasGroup;

        private void Awake()
        {
            mSplashCanvasGroup = transform.Find("Splash").GetComponent<CanvasGroup>();
            mSplashCanvasGroup.alpha = 0;

            FirstTimeEnterGame();
        }

        private void FirstTimeEnterGame()
        {
            ActionKit.Sequence()
                .Delay(1f)
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mSplashCanvasGroup, 1f)))
                .Delay(2f)
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mSplashCanvasGroup, 0f)))
                .Delay(1f)
                .Callback(() => TranslatorCanvasManager.StartMainMenu())
                .Start(this);
        }
    }
}
