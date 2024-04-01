using System.Collections;
using System.Collections.Generic;
using TMPro;
using DataSystem;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Translator;
using UnityEngine.EventSystems;
using Kuchinashi;

namespace UI
{
    /// <summary>
    /// Character Controller
    /// </summary>
    public class Character : MonoBehaviour
    {
        public enum States
        {
            NonInteractable,
            Interactable
        }
        public FSM<States> stateMachine = new FSM<States>();
        
        public CharacterData data;

        private Image _image;
        private TMP_Text _text;
        private ButtonExtension _buttonExtension;

        private void Awake()
        {
            _image = transform.Find("Image").GetComponent<Image>();
            _text = transform.Find("Text").GetComponent<TMP_Text>();
            _buttonExtension = GetComponent<ButtonExtension>();

            stateMachine.AddState(States.NonInteractable, new NonInteractableState(stateMachine, this));
            stateMachine.AddState(States.Interactable, new InteractableState(stateMachine, this));

            stateMachine.StartState(TranslatorSM.StateMachine.CurrentStateId != Translator.States.Off ? States.Interactable : States.NonInteractable);

            TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e => {
                stateMachine.ChangeState(States.Interactable);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e => {
                stateMachine.ChangeState(States.NonInteractable);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnCharacterRecordedEvent>(e => {
                if (e.id == data.Id) Refresh();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            Initialize();
        }

        public void Initialize(bool isInteractable = false, bool isBlack = false)
        {
            _image.sprite = data.Sprite;
            _text.SetText(UserDictionary.Read(data.Id));

            _buttonExtension.OnClick += () => {
                CharacterRecordPanelManager.Instance.Init(this);
            };
            _buttonExtension.OnPressEnd += () => {
                TypeEventSystem.Global.Send(new Dictionary.CallForPuzzleListEvent(data.Id));
            };

            stateMachine.ChangeState(isInteractable ? States.Interactable : States.NonInteractable);

            if (isBlack)
            {
                _image.color = Color.black;
                _text.color = Color.black;
            }
        }

        public void Initialize(CharacterData data, bool isInteractable = false, bool isBlack = false)
        {
            this.data = data;
            Initialize(isInteractable, isBlack);
        }

        public void Refresh()
        {
            _image.sprite = data.Sprite;
            _text.SetText(UserDictionary.Read(data.Id));

            stateMachine.ChangeState(TranslatorSM.StateMachine.CurrentStateId != Translator.States.Off ? States.Interactable : States.NonInteractable);
        }

        public class NonInteractableState : AbstractState<States, Character>
        {
            public NonInteractableState(FSM<States> fsm, Character target) : base(fsm, target) {}
            protected override bool OnCondition() =>  mFSM.CurrentStateId == States.Interactable;

            protected override void OnEnter()
            {
                mTarget._buttonExtension.interactable = false;
                mTarget._text.enabled = false;
            }
        }

        public class InteractableState : AbstractState<States, Character>
        {
            public InteractableState(FSM<States> fsm, Character target) : base(fsm, target) {}
            protected override bool OnCondition() =>  mFSM.CurrentStateId == States.NonInteractable;

            protected override void OnEnter()
            {
                mTarget._buttonExtension.interactable = true;
                mTarget._text.enabled = true;
            }
        }
    }
}