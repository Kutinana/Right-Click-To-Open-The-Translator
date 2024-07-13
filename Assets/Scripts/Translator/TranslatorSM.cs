using System.Collections;
using System.Collections.Generic;
using Dictionary;
using Kuchinashi;
using Localization;
using QFramework;
using TMPro;
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
        Settings,
        Memo
    }

    public class TranslatorSM : MonoBehaviour, ISingleton
    {
        public static TranslatorSM Instance => SingletonProperty<TranslatorSM>.Instance;
        public static FSM<States> StateMachine => Instance.stateMachine;
        private FSM<States> stateMachine = new FSM<States>();

        public static bool CanActivate = true;
        public Coroutine CurrentCoroutine = null;

        public void OnSingletonInit() { }

        [HideInInspector] public CanvasGroup canvasGroup;
        [HideInInspector] public CanvasGroup recorderCanvasGroup;
        [HideInInspector] public CanvasGroup dictionaryCanvasGroup;
        [HideInInspector] public CanvasGroup settingsCanvasGroup;
        [HideInInspector] public CanvasGroup memoCanvasGroup;

        [HideInInspector] public Toggle translatorToggle;
        [HideInInspector] public Toggle dictionaryToggle;
        [HideInInspector] public Toggle settingsToggle;
        [HideInInspector] public Toggle memoToggle;

        private CanvasGroup tutorialCanvasGroup;

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
            memoCanvasGroup = transform.Find("Memo").GetComponent<CanvasGroup>();
            memoCanvasGroup.alpha = 0;

            translatorToggle = transform.Find("Menu/Image/Translator").GetComponent<Toggle>();
            translatorToggle.onValueChanged.AddListener(value =>
            {
                AudioMng.PlayBtnPressed(0);
                if (value) stateMachine.ChangeState(States.Translation);
            });
            translatorToggle.SetIsOnWithoutNotify(true);
            dictionaryToggle = transform.Find("Menu/Image/Dictionary").GetComponent<Toggle>();
            dictionaryToggle.onValueChanged.AddListener(value =>
            {
                AudioMng.PlayBtnPressed(0);
                if (value) stateMachine.ChangeState(States.Dictionary);
            });
            settingsToggle = transform.Find("Menu/Image/Settings").GetComponent<Toggle>();
            settingsToggle.onValueChanged.AddListener(value =>
            {
                AudioMng.PlayBtnPressed(0);
                if (value) stateMachine.ChangeState(States.Settings);
            });
            memoToggle = transform.Find("Menu/Image/Memo").GetComponent<Toggle>();
            memoToggle.onValueChanged.AddListener(value =>
            {
                AudioMng.PlayBtnPressed(0);
                if (value) stateMachine.ChangeState(States.Memo);
            });

            tutorialCanvasGroup = transform.Find("Tutorial").GetComponent<CanvasGroup>();
            tutorialCanvasGroup.alpha = 0;
            tutorialCanvasGroup.blocksRaycasts = false;

            stateMachine.AddState(States.Off, new OffState(stateMachine, this));
            stateMachine.AddState(States.Translation, new TranslationState(stateMachine, this));
            stateMachine.AddState(States.Recorder, new RecorderState(stateMachine, this));
            stateMachine.AddState(States.Dictionary, new DictionaryState(stateMachine, this));
            stateMachine.AddState(States.Settings, new SettingsState(stateMachine, this));
            stateMachine.AddState(States.Memo, new MemoState(stateMachine, this));

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

        public void StartTutorial()
        {
            tutorialCanvasGroup.transform.Find("Hint Word").GetComponent<TMP_Text>().SetText(LocalizationManager.GetCommonString("Str_FirstTimeActivateTranslator"));
            StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(tutorialCanvasGroup, 1f));
            tutorialCanvasGroup.GetComponent<Button>().onClick.AddListener(() =>
            {
                StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(tutorialCanvasGroup, 0f));
            });
        }
    }

    public struct OnTranslatorEnabledEvent { }
    public struct OnTranslatorDisabledEvent { }
    public struct OnRecorderEnabledEvent { }
    public struct OnRecorderDisabledEvent { }

    public class OffState : AbstractState<States, TranslatorSM>
    {
        public OffState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) { }
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null && mFSM.CurrentStateId != States.Off;

        protected override void OnEnter()
        {
            AudioMng.Instance.PlayTranslatorSFX(isEnter: false);
            TranslatorFilterController.Instance.DisableFilter();
            mTarget.StartCoroutine(OnEnterCoroutine());
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonUp(1))
            {
                mFSM.ChangeState(States.Translation);
            }
        }

        protected override void OnExit()
        {
            AudioMng.Instance.PlayTranslatorSFX(isEnter: true);
            TranslatorFilterController.Instance.EnableFilter();
        }

        IEnumerator OnEnterCoroutine()
        {
            TypeEventSystem.Global.Send<OnTranslatorDisabledEvent>();
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 0f, 0.1f));

            mTarget.recorderCanvasGroup.blocksRaycasts = false;
            mTarget.dictionaryCanvasGroup.blocksRaycasts = false;
            mTarget.settingsCanvasGroup.blocksRaycasts = false;

            mTarget.CurrentCoroutine = null;
        }
    }

    public class TranslationState : AbstractState<States, TranslatorSM>
    {
        public TranslationState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) { }
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null
            && mFSM.CurrentStateId != States.Translation
            && TranslatorSM.CanActivate
            && !NarrationManager.IsNarrating;

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

            yield return mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.dictionaryCanvasGroup, 0f, 0.1f));
            yield return mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.settingsCanvasGroup, 0f, 0.1f));

            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 1f, 0.1f));

            mTarget.recorderCanvasGroup.blocksRaycasts = false;
            mTarget.translatorToggle.SetIsOnWithoutNotify(true);

            mTarget.CurrentCoroutine = null;
            mTarget.canvasGroup.interactable = true;
        }
    }

    public class RecorderState : AbstractState<States, TranslatorSM>
    {
        public RecorderState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) { }
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null
            && mFSM.CurrentStateId != States.Recorder
            && TranslatorSM.CanActivate;

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

            mTarget.recorderCanvasGroup.blocksRaycasts = true;
            CharacterRecordPanelManager.ActivateInputField();

            mTarget.CurrentCoroutine = null;
        }

        IEnumerator OnExitCoroutine()
        {
            TypeEventSystem.Global.Send<OnRecorderDisabledEvent>();
            yield return mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.recorderCanvasGroup, 0f, 0.1f));

            mTarget.recorderCanvasGroup.blocksRaycasts = false;
            mTarget.CurrentCoroutine = null;
        }
    }

    public class DictionaryState : AbstractState<States, TranslatorSM>
    {
        public DictionaryState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) { }
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null
            && mFSM.CurrentStateId != States.Dictionary
            && TranslatorSM.CanActivate;

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
            mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.dictionaryCanvasGroup, 0f, 0.1f));
        }

        IEnumerator OnEnterCoroutine()
        {
            mTarget.canvasGroup.interactable = false;
            yield return mTarget.CurrentCoroutine;

            yield return mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 1f, 0.1f));
            yield return mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.dictionaryCanvasGroup, 1f, 0.1f));

            mTarget.dictionaryCanvasGroup.blocksRaycasts = true;
            mTarget.dictionaryToggle.SetIsOnWithoutNotify(true);

            mTarget.CurrentCoroutine = null;
            mTarget.canvasGroup.interactable = true;
        }
    }

    public class SettingsState : AbstractState<States, TranslatorSM>
    {
        public SettingsState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) { }
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null
            && mFSM.CurrentStateId != States.Settings
            && TranslatorSM.CanActivate;

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
            mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.settingsCanvasGroup, 0f, 0.1f));
        }

        IEnumerator OnEnterCoroutine()
        {
            mTarget.canvasGroup.interactable = false;
            yield return mTarget.CurrentCoroutine;

            yield return mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 1f, 0.1f));
            yield return mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.settingsCanvasGroup, 1f, 0.1f));

            mTarget.settingsCanvasGroup.blocksRaycasts = true;
            mTarget.settingsToggle.SetIsOnWithoutNotify(true);

            mTarget.CurrentCoroutine = null;
            mTarget.canvasGroup.interactable = true;
        }
    }

    public class MemoState : AbstractState<States, TranslatorSM>
    {
        public MemoState(FSM<States> fsm, TranslatorSM target) : base(fsm, target) { }
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null
            && mFSM.CurrentStateId != States.Memo
            && TranslatorSM.CanActivate;

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
            mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.memoCanvasGroup, 0f, 0.1f));
        }

        IEnumerator OnEnterCoroutine()
        {
            mTarget.canvasGroup.interactable = false;
            yield return mTarget.CurrentCoroutine;

            yield return mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 1f, 0.1f));
            yield return mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.memoCanvasGroup, 1f, 0.1f));

            mTarget.memoCanvasGroup.blocksRaycasts = true;
            mTarget.memoToggle.SetIsOnWithoutNotify(true);

            mTarget.CurrentCoroutine = null;
            mTarget.canvasGroup.interactable = true;
        }
    }
}