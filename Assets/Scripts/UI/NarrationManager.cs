using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using QFramework;
using TMPro;
using UnityEngine;

namespace UI
{
    public class NarrationManager : MonoSingleton<NarrationManager>
    {
        private CanvasGroup mCanvasGroup;
        private TMP_Text mText;

        private static Coroutine CurrentCoroutine;

        private void Awake()
        {
            mCanvasGroup = GetComponent<CanvasGroup>();
            mText = transform.Find("Text").GetComponent<TMP_Text>();
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
    }

}