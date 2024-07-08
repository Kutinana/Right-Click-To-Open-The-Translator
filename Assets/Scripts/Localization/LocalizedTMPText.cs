using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using QFramework;
using TMPro;
using UnityEngine;

namespace Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTMPText : MonoBehaviour
    {
        public SerializableDictionary<Language, string> LocalizedText;

        private void Awake()
        {
            TypeEventSystem.Global.Register<OnLanguageChangedEvent>(e => {
                SetLanguageTextName();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        protected void OnEnable()
        {
            SetLanguageTextName();
        }

        internal void SetLanguageTextName()
        {
            string value = LocalizationHelper.Get(LocalizedText).Replace("\\n", "\n");

            if (string.IsNullOrEmpty(value) != true)
            {
                gameObject.GetComponent<TMP_Text>().text = value;
            }
        }
    }
}
