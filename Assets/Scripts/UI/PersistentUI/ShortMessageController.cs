using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using Localization;
using QFramework;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ShortMessageController : MonoSingleton<ShortMessageController>
    {
        private TMP_Text text;

        private Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            text = GetComponentInChildren<TMP_Text>();

            TypeEventSystem.Global.Register<OnInventoryIncreasedEvent>(e =>
            {
                if (e.items.Count == 1)
                {
                    var name = LocalizationHelper.Get(GameDesignData.GetObtainableObjectDataById(e.items.First().Key).Name);
                    CallUp(LocalizationHelper.Get("Str_ObtainedSingleObject", name));
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public static void CallUp(string _text)
        {
            Instance.Reset();

            Instance.text.SetText(_text);
            Instance.CurrentCoroutine = Instance.StartCoroutine(Instance.ShowCoroutine());
        }

        private void Reset()
        {
            if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = null;

            GetComponent<RectTransform>().anchoredPosition = new Vector2(400, -100);
        }

        private IEnumerator ShowCoroutine()
        {
            var rect = GetComponent<RectTransform>();

            var target = new Vector2(0, -100);
            while (!Mathf.Approximately(rect.anchoredPosition.x, 0f))
            {
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, target, 0.15f);
                yield return new WaitForFixedUpdate();
            }
            
            yield return new WaitForSeconds(1f);

            target = new Vector2(400, -100);
            while (!Mathf.Approximately(rect.anchoredPosition.x, 400f))
            {
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, target, 0.15f);
                yield return new WaitForFixedUpdate();
            }

            CurrentCoroutine = null;
        }
    }
}
