using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Kuchinashi;
using UnityEngine.Video;
using Translator;

namespace Cameras
{
    public class TranslatorCanvasManager : MonoSingleton<TranslatorCanvasManager>
    {
        private CanvasGroup mMainMenu;
        private VideoPlayer videoPlayer;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            mMainMenu = transform.Find("MainMenu").GetComponent<CanvasGroup>();
            videoPlayer = transform.Find("MainMenu/Background").GetComponent<VideoPlayer>();
        }

        public static void StartMainMenu()
        {
            Instance.StartCoroutine(Instance.StartMainMenuCoroutine());
        }

        private IEnumerator StartMainMenuCoroutine()
        {
            AudioMng.Instance.PlayAmbient();

            yield return CanvasGroupHelper.FadeCanvasGroup(Instance.mMainMenu, 1f, 0.02f);
            yield return new WaitForSeconds(0.5f);

            videoPlayer.Play();
            TranslatorSM.StateMachine.ChangeState(States.Translation);
        }
    }

}