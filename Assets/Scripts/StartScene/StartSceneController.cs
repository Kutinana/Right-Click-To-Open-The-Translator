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
            
            InitialSettings();

            if (PlayerPrefs.HasKey("Played") && PlayerPrefs.GetInt("Played") == 1)
            {
                NormalEnterGame();
            }
            else
            {
                FirstTimeEnterGame();
            }
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

            PlayerPrefs.SetInt("Played", 1);
        }

        private void NormalEnterGame()
        {
            ActionKit.Sequence()
                .Delay(1f)
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mSplashCanvasGroup, 1f)))
                .Delay(2f)
                .Callback(() => StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mSplashCanvasGroup, 0f)))
                .Delay(1f)
                .Callback(() => TranslatorCanvasManager.StartMainMenu())
                .Delay(1f)
                .Callback(() => SceneControl.SceneControl.SwitchSceneWithoutConfirm("TestScene"))
                .Start(this);
        }

        private void InitialSettings()
        {
            Application.targetFrameRate = 240;

            AudioKit.Settings.MusicVolume.SetValueWithoutEvent(
                PlayerPrefs.HasKey("BackgroundVolume") ? PlayerPrefs.GetFloat("BackgroundVolume") : 0.8f);
            AudioKit.Settings.SoundVolume.SetValueWithoutEvent(
                PlayerPrefs.HasKey("EffectVolume") ? PlayerPrefs.GetFloat("EffectVolume") : 0.8f);
        }
    }
}
