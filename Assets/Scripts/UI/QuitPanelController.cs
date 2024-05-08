using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class QuitPanelController : MonoSingleton<QuitPanelController>
    {
        private CanvasGroup canvasGroup;

        private Image mCatImage;
        private Image mPersonImage;

        public Vector2 targetPosition;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            mCatImage = transform.Find("Cat").GetComponent<Image>();
            mPersonImage = transform.Find("Person").GetComponent<Image>();
        }

        public static void StartQuitting()
        {
            Instance.StartCoroutine(Instance.QuitCoroutine());
        }

        private IEnumerator QuitCoroutine()
        {
            yield return CanvasGroupHelper.FadeCanvasGroup(canvasGroup, 1f);

            while (Mathf.Abs(mPersonImage.transform.localPosition.y - targetPosition.y) > 0.3f)
            {
                mPersonImage.transform.localPosition = Vector3.Lerp(mPersonImage.transform.localPosition, targetPosition, 0.03f);
                mCatImage.transform.localScale = Vector3.Lerp(mCatImage.transform.localScale, new Vector3(1.1f, 1.1f, 1), 0.03f);
                yield return new WaitForFixedUpdate();

                if (Input.GetMouseButtonDown(0))
                    break;
            }
            mPersonImage.transform.localPosition = targetPosition;
            mCatImage.transform.localScale = new Vector3(1.1f, 1.1f, 1);

            Application.Quit();
        }
    }

}