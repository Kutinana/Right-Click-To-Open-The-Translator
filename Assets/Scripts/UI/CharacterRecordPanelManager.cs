using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CharacterRecordPanelManager : MonoBehaviour
    {
        public static CharacterRecordPanelManager Instance;

        private Character character;

        private CanvasGroup _canvasGroup;
        private TMP_InputField _inputField;
        private Button _confirmButton;

        private void Awake()
        {
            Instance = this;

            _canvasGroup = GetComponent<CanvasGroup>();
            _inputField = transform.Find("InputField").GetComponent<TMP_InputField>();
            _confirmButton = transform.Find("Confirm").GetComponent<Button>();

            _canvasGroup.alpha = 0;
            _confirmButton.onClick.AddListener(() => {
                UserDictionary.WriteInAndSave(character.data.Id, _inputField.text);
                StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(_canvasGroup, 0f, 0.1f));

                character.Refresh();
            });
        }

        public CharacterRecordPanelManager Init(Character c)
        {
            character = c;
            StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(_canvasGroup, 1f, 0.1f));

            return this;
        }
    }
}