using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using Translator;
using UnityEngine;
using UnityEngine.UI;

namespace Hint
{
    public struct OnHintInitializedEvent {}
    public struct OnHintExitEvent {}
    public partial class HintManager : MonoBehaviour , ISingleton
    {
        public enum States
        {
            None,
            InActive,
            Active
        }

        public static HintManager Instance => SingletonProperty<HintManager>.Instance;
        public void OnSingletonInit() {}
        public static FSM<States> StateMachine => Instance.stateMachine;
        public FSM<States> stateMachine = new FSM<States>();

        private CanvasGroup canvasGroup;

        public static HintBase CurrentHint = null;
        public Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            TypeEventSystem.Global.Register<OnHintExitEvent>(e => {
                if (CurrentHint != null) CurrentHint.OnExit();

                StateMachine.ChangeState(States.None);
            });

            TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e => {
                if (CurrentHint != null) StateMachine.ChangeState(States.InActive);
            });
            TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e => {
                if (CurrentHint != null) StateMachine.ChangeState(States.Active);
            });

            stateMachine.AddState(States.None, new NoneState(stateMachine, this));
            stateMachine.AddState(States.Active, new ActiveState(stateMachine, this));
            stateMachine.AddState(States.InActive, new InActiveState(stateMachine, this));

            stateMachine.StartState(States.None);
        }

        public static void Initialize(string _id)
        {
            if (CurrentHint != null) return;

            var data = GameDesignData.GetHintDataById(_id);
            CurrentHint = Instantiate(data.HintPrefab, Instance.transform).GetComponent<HintBase>();
            CurrentHint.Id = _id;

            CurrentHint.OnEnter();
            StateMachine.ChangeState(States.Active);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.I) && CurrentHint == null) Initialize("hint5");
            if (CurrentHint != null)
            {
                CurrentHint.OnUpdate();
            }
        }
    }

    public partial class HintManager
    {
        public class NoneState : AbstractState<States, HintManager>
        {
            public NoneState(FSM<States> fsm, HintManager target) : base(fsm, target) {}
            protected override bool OnCondition() =>  mFSM.CurrentStateId != States.None;

            protected override void OnEnter()
            {
                mTarget.StartCoroutine(OnEnterCoroutine());
            }

            IEnumerator OnEnterCoroutine()
            {
                yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 0f, 0.1f));
                if (CurrentHint != null) Destroy(CurrentHint.gameObject);
                
                CurrentHint = null;
                mTarget.CurrentCoroutine = null;
            }
        }

        public class ActiveState : AbstractState<States, HintManager>
        {
            public ActiveState(FSM<States> fsm, HintManager target) : base(fsm, target) {}
            protected override bool OnCondition() =>  mFSM.CurrentStateId != States.Active;

            protected override void OnEnter()
            {
                mTarget.StartCoroutine(OnEnterCoroutine());

                mTarget.canvasGroup.interactable = true;
                foreach (var col in mTarget.GetComponentsInChildren<Collider2D>())
                {
                    col.enabled = true;
                }
            }

            IEnumerator OnEnterCoroutine()
            {
                yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 1f, 0.1f));

                mTarget.CurrentCoroutine = null;
            }
        }

        public class InActiveState : AbstractState<States, HintManager>
        {
            public InActiveState(FSM<States> fsm, HintManager target) : base(fsm, target) {}
            protected override bool OnCondition() =>  mFSM.CurrentStateId != States.InActive;

            protected override void OnEnter()
            {
                mTarget.canvasGroup.interactable = false;
                foreach (var col in mTarget.GetComponentsInChildren<Collider2D>())
                {
                    col.enabled = false;
                }
            }
        }
    }
}