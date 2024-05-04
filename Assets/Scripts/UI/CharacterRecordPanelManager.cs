using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using TMPro;
using Translator;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public struct OnCharacterRecordedEvent {
        public string id;
        public OnCharacterRecordedEvent(string _id) { id = _id; }
    }

    public struct OnCharacterRefreshEvent {}

    public class CharacterRecordPanelManager : MonoBehaviour
    {
        public static CharacterRecordPanelManager Instance;

        private Character character;
        private Image _image;
        private TMP_InputField _inputField;
        private Button _confirmButton;

        private void Awake()
        {
            Instance = this;

            _image = transform.Find("Character").GetComponent<Image>();
            _inputField = transform.Find("InputField").GetComponent<TMP_InputField>();
            _confirmButton = transform.Find("Confirm").GetComponent<Button>();

            _inputField.text = "";
            _confirmButton.onClick.AddListener(() => {
                UserDictionary.WriteInAndSave(character.data.Id, _inputField.text);
                TranslatorSM.ReturnToPreviousState();

                TypeEventSystem.Global.Send(new OnCharacterRecordedEvent(character.data.Id));
            });
        }

        private void Update()
        {
            if (TranslatorSM.StateMachine.CurrentStateId == States.Recorder)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    TranslatorSM.ReturnToPreviousState();
                }
                if (Input.GetKeyDown(KeyCode.Return) && _inputField.IsActive())
                {
                    _confirmButton.onClick.Invoke();
                }
            }
        }

        public CharacterRecordPanelManager Init(Character c)
        {
            character = c;
            _image.sprite = c.data.Sprite;
            _inputField.SetTextWithoutNotify(UserDictionary.Read(c.data.Id));
            
            TranslatorSM.StateMachine.ChangeState(States.Recorder);

            return this;
        }

        public static void ActivateInputField()
        {
            Instance._inputField.ActivateInputField();
            return;
        }
    }
}