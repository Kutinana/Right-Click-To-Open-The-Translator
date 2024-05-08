using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
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

        private static List<string> InitialNarration = new() {
            "这到底…是怎么回事……",
            "突然出现的发光的涂鸦，刚刚好像在加载的什么程序，屏幕上的信息，奇怪的符号…",
            "面前看起来一模一样的墙和完全不一样的地板…",
            "还有身上这一身…不会是变成章鱼了吧",
            "……",
            "先四处看看吧…"
        };

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
            
            foreach (var text in InitialNarration)
            {
                ShowNarration(text);
                yield return new WaitForSeconds(2f);
                HideNarration();
            }
        }
    }

}