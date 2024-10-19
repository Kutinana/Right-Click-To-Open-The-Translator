using System.Collections;
using System.Collections.Generic;
using DataSystem;
using Kuchinashi;
using QFramework;
using TMPro;
using Translator;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public struct OnCharacterRecordedEvent
    {
        public string id;
        public OnCharacterRecordedEvent(string _id) { id = _id; }
    }

    public struct OnCharacterRefreshEvent { }

    public class CharacterRecordPanelManager : MonoSingleton<CharacterRecordPanelManager>
    {
        private CanvasGroup canvasGroup;
        private Button button;

        private CharacterData character;
        private Image _image;
        private TMP_InputField _inputField;
        private Button _confirmButton;

        public bool IsActive = false;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            button = GetComponent<Button>();
            button.onClick.AddListener(() => {
                if (IsActive) StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(canvasGroup, 0f, 0.2f));
            });

            _image = transform.Find("Character").GetComponent<Image>();
            _inputField = transform.Find("InputField").GetComponent<TMP_InputField>();
            _confirmButton = transform.Find("Confirm").GetComponent<Button>();

            _inputField.text = "";
            _confirmButton.onClick.AddListener(() =>
            {
                AudioKit.PlaySound("InteractClick", volume: AudioMng.Instance.effectVolume * 0.8f);
                UserDictionary.WriteInAndSave(character.Id, _inputField.text);
                StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(canvasGroup, 0f, 0.2f));

                TypeEventSystem.Global.Send(new OnCharacterRecordedEvent(character.Id));
            });
        }

        private void Update()
        {
            if (IsActive)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(canvasGroup, 0f, 0.2f));
                }
                if (Input.GetKeyDown(KeyCode.Return) && _inputField.IsActive())
                {
                    _confirmButton.onClick.Invoke();
                }
            }
        }

        public CharacterRecordPanelManager Init(CharacterData c)
        {
            character = c;
            _image.sprite = c.Sprite;
            _inputField.SetTextWithoutNotify(UserDictionary.Read(c.Id));
            AudioKit.PlaySound("InteractClick", volume: AudioMng.Instance.effectVolume * 0.8f);
            
            IsActive = true;
            StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(canvasGroup, 1f, 0.2f));

            return this;
        }

        public CharacterRecordPanelManager Init(Character c)
        {
            character = c.data;
            _image.sprite = c.data.Sprite;
            _inputField.SetTextWithoutNotify(UserDictionary.Read(c.data.Id));
            AudioKit.PlaySound("InteractClick", volume: AudioMng.Instance.effectVolume * 0.8f);
            
            IsActive = true;
            StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(canvasGroup, 1f, 0.2f));

            return this;
        }

        public static void ActivateInputField()
        {
            Instance._inputField.ActivateInputField();
            return;
        }
    }
}