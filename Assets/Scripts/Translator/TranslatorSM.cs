using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Translator
{
    public enum States
    {
        Off,
        Translation,
        Dictionary,
        PuzzleList,
        Puzzle
    }

    public class TranslatorSM : MonoBehaviour , ISingleton
    {
        public FSM<States> stateMachine = new FSM<States>();

        public void OnSingletonInit() {}

        [HideInInspector] public CanvasGroup canvasGroup;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            stateMachine.AddState(States.Off, new OffState(stateMachine, this));
            stateMachine.AddState(States.Translation, new TranslationState(stateMachine, this));

            stateMachine.StartState(States.Off);
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(1))
            {
                ToggleTranslator();
            }
        }

        private void ToggleTranslator()
        {
            stateMachine.ChangeState(stateMachine.CurrentStateId == States.Off ? States.Translation : States.Off);
        }
    }

    public struct OnTranslatorEnabledEvent {}
    public struct OnTranslatorDisabledEvent {}

    public class OffState : AbstractState<States, TranslatorSM>
    {
        public OffState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) {}
        protected override bool OnCondition() =>  mFSM.CurrentStateId != States.Off;

        protected override void OnEnter()
        {
            mTarget.StopAllCoroutines();
            mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 0f, 0.1f));

            TypeEventSystem.Global.Send<OnTranslatorDisabledEvent>();
        }

        protected override void OnExit()
        {
        }
    }

    public class TranslationState : AbstractState<States, TranslatorSM>
    {
        public TranslationState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) {}
        protected override bool OnCondition() =>  mFSM.CurrentStateId != States.Translation;

        protected override void OnEnter()
        {
            mTarget.StopAllCoroutines();
            mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 1f, 0.1f));

            TypeEventSystem.Global.Send<OnTranslatorEnabledEvent>();
        }

        protected override void OnExit()
        {
        }
    }
}