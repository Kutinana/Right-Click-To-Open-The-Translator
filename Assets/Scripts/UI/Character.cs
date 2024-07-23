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
    /// General Character Controller
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

        protected Image _image;
        protected TMP_Text _text;
        protected ButtonExtension _buttonExtension;

        protected virtual void Awake()
        {
            _image = transform.Find("Image").GetComponent<Image>();
            _text = transform.Find("Text").GetComponent<TMP_Text>();
            _buttonExtension = GetComponent<ButtonExtension>();

            stateMachine.AddState(States.NonInteractable, new NonInteractableState(stateMachine, this));
            stateMachine.AddState(States.Interactable, new InteractableState(stateMachine, this));

            stateMachine.StartState(TranslatorSM.StateMachine.CurrentStateId != Translator.States.Off ? States.Interactable : States.NonInteractable);
        }

        public virtual void Initialize(CharacterData _data = null, bool isInteractable = false, bool isBlack = false)
        {
            if (_data != null) data = _data;

            _image.sprite = data.Sprite;
            _text.SetText(UserDictionary.Read(data.Id).Meaning);

            stateMachine.ChangeState(isInteractable ? States.Interactable : States.NonInteractable);

            if (isBlack)
            {
                _image.color = new Color(20f / 255, 24f / 255, 46f / 255);
                _text.color = new Color(20f / 255, 24f / 255, 46f / 255);
            }
        }

        public virtual void Refresh()
        {
            _image.sprite = data.Sprite;
            _text.SetText(UserDictionary.Read(data.Id).Meaning);

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