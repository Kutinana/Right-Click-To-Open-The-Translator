using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Localization;
using QFramework;
using TMPro;
using UnityEngine;

namespace UI
{
    public struct OnInitialNarrationStartEvent {}
    public class NarrationManager : MonoSingleton<NarrationManager>
    {
        private CanvasGroup mCanvasGroup;
        private TMP_Text mText;

        private static Coroutine CurrentCoroutine;

        private void Awake()
        {
            mCanvasGroup = GetComponent<CanvasGroup>();
            mText = transform.Find("Text").GetComponent<TMP_Text>();

            TypeEventSystem.Global.Register<OnInitialNarrationStartEvent>(e => {
                if (PlayerPrefs.HasKey("FirstTime") && PlayerPrefs.GetInt("FirstTime") == 1)
                {
                    ShowInitialNarration();
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public static void ShowNarration(string text)
        {
            Instance.mText.SetText(text);

            if (CurrentCoroutine != null) Instance.StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = Instance.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Instance.mCanvasGroup, 1f));
        }

        public static void HideNarration()
        {
            if (CurrentCoroutine != null) Instance.StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = Instance.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Instance.mCanvasGroup, 0f));
        }

        public static void ShowInitialNarration()
        {
            Instance.StartCoroutine(Instance.InitialNarrationCoroutine());
        }

        private IEnumerator InitialNarrationCoroutine()
        {
            yield return new WaitForSeconds(2f);
            
            foreach (var text in LocalizationManager.GetPlot().InitialNarration)
            {
                ShowNarration(text);
                yield return new WaitForSeconds(2f);
                HideNarration();
            }
        }
    }

}