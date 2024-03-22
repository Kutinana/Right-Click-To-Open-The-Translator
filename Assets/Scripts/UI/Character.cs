using System.Collections;
using System.Collections.Generic;
using TMPro;
using DataSystem;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using Translator;

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
        private Button _button;

        private void Awake()
        {
            _image = transform.Find("Image").GetComponent<Image>();
            _text = transform.Find("Text").GetComponent<TMP_Text>();
            _button = GetComponent<Button>();

            stateMachine.AddState(States.NonInteractable, new NonInteractableState(stateMachine, this));
            stateMachine.AddState(States.Interactable, new InteractableState(stateMachine, this));

            stateMachine.StartState(States.NonInteractable);

            TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e => {
                stateMachine.ChangeState(States.Interactable);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e => {
                stateMachine.ChangeState(States.NonInteractable);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            Initialize();
        }

        private void Initialize(bool isInteractable = false)
        {
            _image.sprite = data.Sprite;
            _text.SetText(UserDictionary.Read(data.Id));

            _button.onClick.AddListener(() => {
                CharacterRecordPanelManager.Instance.Init(this);
            });

            stateMachine.ChangeState(isInteractable ? States.Interactable : States.NonInteractable);
        }

        public void Refresh()
        {
            _image.sprite = data.Sprite;
            _text.SetText(UserDictionary.Read(data.Id));
        }

        public class NonInteractableState : AbstractState<States, Character>
        {
            public NonInteractableState(FSM<States> fsm, Character target) : base(fsm, target) {}
            protected override bool OnCondition() =>  mFSM.CurrentStateId == States.Interactable;

            protected override void OnEnter()
            {
                mTarget._button.interactable = false;
                mTarget._text.enabled = false;
            }
        }

        public class InteractableState : AbstractState<States, Character>
        {
            public InteractableState(FSM<States> fsm, Character target) : base(fsm, target) {}
            protected override bool OnCondition() =>  mFSM.CurrentStateId == States.NonInteractable;

            protected override void OnEnter()
            {
                mTarget._button.interactable = true;
                mTarget._text.enabled = true;
            }
        }
    }
}