using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Kuchinashi;
using UnityEngine.Video;
using Translator;
using Puzzle;
using Hint;
using UI;
using DataSystem;
using Localization;

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

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Y))
            {
                if (GameProgressData.GetInventory().TryGetValue("bomb", out var value) && value >= 1)
                {
                    var bomb = GameDesignData.GetObtainableObjectDataById("bomb");
                    DialogBoxController.CallUp(LocalizationHelper.Get("Str_UseItemConfirm", LocalizationHelper.Get(bomb.Name)),
                        confirmCallback: () => { Debug.Log("Confirm");}, cancelCallback: () => { Debug.Log("Cancel");});
                }
                else
                {
                    ShortMessageController.CallUp(LocalizationHelper.Get("Str_RoadIsBlocked"));
                }
            }
        }

        public static void StartMainMenu()
        {
            Instance.StartCoroutine(Instance.StartMainMenuCoroutine());
        }

        private IEnumerator StartMainMenuCoroutine()
        {
            AudioMng.Instance.PlayAmbient("AmbientResearcher");

            TranslatorSM.StateMachine.ChangeState(States.Off);
            PuzzleManager.Exit();
            HintManager.Exit();

            yield return CanvasGroupHelper.FadeCanvasGroup(Instance.mMainMenu, 1f, 0.02f);
            yield return new WaitForSeconds(0.5f);

            videoPlayer.Play();
        }
    }

}