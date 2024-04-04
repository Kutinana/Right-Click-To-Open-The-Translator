using System.Collections;
using System.Collections.Generic;
using Dictionary;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Translator
{
    public enum States
    {
        Off,
        Translation,
        Recorder,
        Dictionary,
        Settings
    }

    public class TranslatorSM : MonoBehaviour , ISingleton
    {
        public static TranslatorSM Instance => SingletonProperty<TranslatorSM>.Instance;
        public static FSM<States> StateMachine => Instance.stateMachine;
        private FSM<States> stateMachine = new FSM<States>();

        public Coroutine CurrentCoroutine = null;

        public void OnSingletonInit() {}

        [HideInInspector] public CanvasGroup canvasGroup;
        [HideInInspector] public CanvasGroup recorderCanvasGroup;
        [HideInInspector] public CanvasGroup dictionaryCanvasGroup;
        [HideInInspector] public CanvasGroup settingsCanvasGroup;

        private Toggle translatorToggle;
        private Toggle dictionaryToggle;
        private Toggle settingsToggle;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            recorderCanvasGroup = transform.Find("Recorder").GetComponent<CanvasGroup>();
            recorderCanvasGroup.alpha = 0;
            dictionaryCanvasGroup = transform.Find("Dictionary").GetComponent<CanvasGroup>();
            dictionaryCanvasGroup.alpha = 0;
            settingsCanvasGroup = transform.Find("Settings").GetComponent<CanvasGroup>();
            settingsCanvasGroup.alpha = 0;

            translatorToggle = transform.Find("Menu/Translator").GetComponent<Toggle>();
            translatorToggle.onValueChanged.AddListener(value => {
                if (value) stateMachine.ChangeState(States.Translation);
            });
            translatorToggle.SetIsOnWithoutNotify(true);
            dictionaryToggle = transform.Find("Menu/Dictionary").GetComponent<Toggle>();
            dictionaryToggle.onValueChanged.AddListener(value => {
                if (value) stateMachine.ChangeState(States.Dictionary);
            });
            settingsToggle = transform.Find("Menu/Settings").GetComponent<Toggle>();
            settingsToggle.onValueChanged.AddListener(value => {
                if (value) stateMachine.ChangeState(States.Settings);
            });

            stateMachine.AddState(States.Off, new OffState(stateMachine, this));
            stateMachine.AddState(States.Translation, new TranslationState(stateMachine, this));
            stateMachine.AddState(States.Recorder, new RecorderState(stateMachine, this));
            stateMachine.AddState(States.Dictionary, new DictionaryState(stateMachine, this));
            stateMachine.AddState(States.Settings, new SettingsState(stateMachine, this));

            stateMachine.StartState(States.Off);
        }

        private void Update()
        {
            stateMachine.Update();
        }

        public static void ReturnToPreviousState()
        {
            StateMachine.ChangeState(StateMachine.PreviousStateId);
        }
    }

    public struct OnTranslatorEnabledEvent {}
    public struct OnTranslatorDisabledEvent {}
    public struct OnRecorderEnabledEvent {}
    public struct OnRecorderDisabledEvent {}

    public class OffState : AbstractState<States, TranslatorSM>
    {
        public OffState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) {}
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null && mFSM.CurrentStateId != States.Off;

        protected override void OnEnter()
        {
            mTarget.StartCoroutine(OnEnterCoroutine());
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonUp(1))
            {
                mFSM.ChangeState(mFSM.PreviousStateId == States.Off ? States.Translation : mFSM.PreviousStateId);
            }
        }

        protected override void OnExit()
        {
        }

        IEnumerator OnEnterCoroutine()
        {
            TypeEventSystem.Global.Send<OnTranslatorDisabledEvent>();
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 0f, 0.1f));

            mTarget.CurrentCoroutine = null;
        }
    }

    public class TranslationState : AbstractState<States, TranslatorSM>
    {
        public TranslationState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) {}
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null && mFSM.CurrentStateId != States.Translation;

        protected override void OnEnter()
        {
            mTarget.StartCoroutine(OnEnterCoroutine());
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonUp(1))
            {
                mFSM.ChangeState(States.Off);
            }
        }

        protected override void OnExit()
        {
        }

        IEnumerator OnEnterCoroutine()
        {
            TypeEventSystem.Global.Send<OnTranslatorEnabledEvent>();
            mTarget.canvasGroup.interactable = false;

            switch (mFSM.PreviousStateId)
            {
                case States.Dictionary:
                    yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.dictionaryCanvasGroup, 0f, 0.1f));
                    break;
                case States.Settings:
                    yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.settingsCanvasGroup, 0f, 0.1f));
                    break;
                default:
                    break;
            }
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 1f, 0.1f));

            mTarget.CurrentCoroutine = null;
            mTarget.canvasGroup.interactable = true;
        }
    }

    public class RecorderState : AbstractState<States, TranslatorSM>
    {
        public RecorderState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) {}
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null && mFSM.CurrentStateId != States.Recorder;

        protected override void OnEnter()
        {
            mTarget.StartCoroutine(OnEnterCoroutine());
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonUp(1))
            {
                mFSM.ChangeState(mFSM.PreviousStateId);
            }
        }

        protected override void OnExit()
        {
            mTarget.StartCoroutine(OnExitCoroutine());
        }

        IEnumerator OnEnterCoroutine()
        {
            TypeEventSystem.Global.Send<OnRecorderEnabledEvent>();
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.recorderCanvasGroup, 1f, 0.1f));
            
            CharacterRecordPanelManager.ActivateInputField();

            mTarget.CurrentCoroutine = null;
        }

        IEnumerator OnExitCoroutine()
        {
            TypeEventSystem.Global.Send<OnRecorderDisabledEvent>();
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.recorderCanvasGroup, 0f, 0.1f));

            mTarget.CurrentCoroutine = null;
        }
    }

    public class DictionaryState : AbstractState<States, TranslatorSM>
    {
        public DictionaryState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) {}
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null && mFSM.CurrentStateId != States.Dictionary;

        protected override void OnEnter()
        {
            DictionarySM.Instance.GenerateCharacterList();
            mTarget.StartCoroutine(OnEnterCoroutine());
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonUp(1))
            {
                mFSM.ChangeState(States.Off);
            }
        }

        protected override void OnExit()
        {
        }

        IEnumerator OnEnterCoroutine()
        {
            mTarget.canvasGroup.interactable = false;
            switch (mFSM.PreviousStateId)
            {
                case States.Settings:
                    yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.settingsCanvasGroup, 0f, 0.1f));
                    break;
                default:
                    break;
            }
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 1f, 0.1f));
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.dictionaryCanvasGroup, 1f, 0.1f));

            mTarget.CurrentCoroutine = null;
            mTarget.canvasGroup.interactable = true;
        }
    }

    public class SettingsState : AbstractState<States, TranslatorSM>
    {
        public SettingsState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) {}
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null && mFSM.CurrentStateId != States.Settings;

        protected override void OnEnter()
        {
            mTarget.StartCoroutine(OnEnterCoroutine());
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonUp(1))
            {
                mFSM.ChangeState(States.Off);
            }
        }

        protected override void OnExit()
        {
        }

        IEnumerator OnEnterCoroutine()
        {
            mTarget.canvasGroup.interactable = false;
            switch (mFSM.PreviousStateId)
            {
                case States.Dictionary:
                    yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.dictionaryCanvasGroup, 0f, 0.1f));
                    break;
                default:
                    break;
            }
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 1f, 0.1f));
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.settingsCanvasGroup, 1f, 0.1f));

            mTarget.CurrentCoroutine = null;
            mTarget.canvasGroup.interactable = true;
        }
    }
}