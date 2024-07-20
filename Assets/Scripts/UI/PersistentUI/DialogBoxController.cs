using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using Kuchinashi;
using Localization;
using QFramework;
using TMPro;
using Translator;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DialogBoxController : MonoSingleton<DialogBoxController>
    {
        private CanvasGroup canvasGroup;
        private TMP_Text text;

        private Button confirmBtn;
        private Button cancelBtn;
        private ButtonExtension confirmBtnExtension;
        private ButtonExtension cancelBtnExtension;

        private Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;

            text = transform.Find("Content").GetComponent<TMP_Text>();
            
            confirmBtn = transform.Find("Confirm").GetComponent<Button>();
            confirmBtnExtension = confirmBtn.GetComponentInChildren<ButtonExtension>();
            cancelBtn = transform.Find("Cancel").GetComponent<Button>();
            cancelBtnExtension = cancelBtn.GetComponentInChildren<ButtonExtension>();

            TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e => {
                confirmBtnExtension.enabled = false;
                cancelBtnExtension.enabled = false;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e => {
                confirmBtnExtension.enabled = true;
                cancelBtnExtension.enabled = true;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public static void CallUp(string _text, Action confirmCallback = null, Action cancelCallback = null)
        {
            if (Instance.CurrentCoroutine != null) return;

            Instance.text.SetText(_text);
            Instance.confirmBtn.onClick.AddListener(() =>
            {
                confirmCallback?.Invoke();
                Instance.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Instance.canvasGroup, 0f));
            });
            Instance.cancelBtn.onClick.AddListener(() =>
            {
                cancelCallback?.Invoke();
                Instance.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Instance.canvasGroup, 0f));
            });

            Instance.confirmBtnExtension.enabled = TranslatorSM.StateMachine.CurrentStateId == States.Translation;
            Instance.cancelBtnExtension.enabled = TranslatorSM.StateMachine.CurrentStateId == States.Translation;

            Instance.CurrentCoroutine = Instance.StartCoroutine(Instance.ShowCoroutine());
        }

        private IEnumerator ShowCoroutine()
        {
            yield return CanvasGroupHelper.FadeCanvasGroup(canvasGroup, 1f);
            
            yield return new WaitUntil(() => canvasGroup.alpha == 0);

            CurrentCoroutine = null;
        }
    }
}
