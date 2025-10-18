using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Kuchinashi.Utils.Progressable;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class QuitPanelController : MonoSingleton<QuitPanelController>
    {
        [SerializeField] private ProgressableGroup progressable;
        [SerializeField] private Progressable canvasProgressable;


        [Header("Settings")]
        public float SmoothTime = 1f;

        private void Awake()
        {
            progressable ??= GetComponent<ProgressableGroup>();
            progressable.Progress = 0f;
            
            canvasProgressable ??= GetComponent<CanvasGroupAlphaProgressable>();
            canvasProgressable.Progress = 0f;
        }

        public static void StartQuitting()
        {
            Instance.StartCoroutine(Instance.QuitCoroutine());
        }

        private IEnumerator QuitCoroutine()
        {
            canvasProgressable.SmoothDamp(0.1f);
            progressable.SmoothDamp(SmoothTime);

            yield return new WaitUntil(() => progressable.Progress >= 1f || Input.GetMouseButtonDown(0));

            Debug.Log("Quit Application.");
            Application.Quit();
        }
    }

}