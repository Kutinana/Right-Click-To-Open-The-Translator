using System.Collections;
using System.Collections.Generic;
using DataSystem;
using Kuchinashi;
using Kuchinashi.Utils.Progressable;
using Localization;
using QFramework;
using TMPro;
using Translator;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Narration
{
    public struct OnNarrationStartEvent
    {
        public OnNarrationStartEvent(string id) : this()
        {
            Id = id;
        }

        public string Id { get; }
    }
    public struct OnNarrationEndEvent
    {
        public OnNarrationEndEvent(string id) : this()
        {
            Id = id;
        }

        public string Id { get; }

    }
    public struct OnInitialNarrationStartEvent { }

    public enum NarrationType
    {
        None,
        Left,
        LeftNeko,
        Right,
        RightNeko,
        FullScreen,
        FullScreenNeko,
        Option,
        OptionNeko
    }

    public partial class NarrationManager : MonoSingleton<NarrationManager>
    {
        public SerializableDictionary<string, Sprite> Tachies;

        private CanvasGroup mCanvasGroup;
        private string narrationPlaying = null;

        private Coroutine CurrentNarrationCoroutine;
        private static Coroutine CurrentTypeTextCoroutine;

        public FSM<NarrationType> StateMachine = new();

        private CanvasGroupAlphaProgressable leftCanvasGroup;
        private Image leftImage;
        private TMP_Text leftText;

        private CanvasGroupAlphaProgressable leftNekoCanvasGroup;
        private Image leftNekoImage;
        private Transform leftNekoText;

        private CanvasGroupAlphaProgressable rightCanvasGroup;
        private Image rightImage;
        private TMP_Text rightText;

        private CanvasGroupAlphaProgressable rightNekoCanvasGroup;
        private Image rightNekoImage;
        private Transform rightNekoText;

        private CanvasGroupAlphaProgressable fullScreenCanvasGroup;
        private Image fullScreenImage;
        private TMP_Text fullScreenText;

        public GameObject NekoCharacterPrefab;

        public string ToShowNarrationId { get; set; }
        public static bool IsNarrating => Instance.CurrentNarrationCoroutine != null;

        private void Awake()
        {
            mCanvasGroup = GetComponent<CanvasGroup>();

            leftCanvasGroup = transform.Find("Left").GetComponent<CanvasGroupAlphaProgressable>();
            leftImage = transform.Find("Left/Tachie").GetComponent<Image>();
            leftText = transform.Find("Left/Text").GetComponent<TMP_Text>();

            leftNekoCanvasGroup = transform.Find("LeftNeko").GetComponent<CanvasGroupAlphaProgressable>();
            leftNekoImage = transform.Find("LeftNeko/Tachie").GetComponent<Image>();
            leftNekoText = transform.Find("LeftNeko/NekoText");

            rightCanvasGroup = transform.Find("Right").GetComponent<CanvasGroupAlphaProgressable>();
            rightImage = transform.Find("Right/Tachie").GetComponent<Image>();
            rightText = transform.Find("Right/Text").GetComponent<TMP_Text>();

            rightNekoCanvasGroup = transform.Find("RightNeko").GetComponent<CanvasGroupAlphaProgressable>();
            rightNekoImage = transform.Find("RightNeko/Tachie").GetComponent<Image>();
            rightNekoText = transform.Find("RightNeko/NekoText");

            fullScreenCanvasGroup = transform.Find("FullScreen").GetComponent<CanvasGroupAlphaProgressable>();
            fullScreenImage = transform.Find("FullScreen/Tachie").GetComponent<Image>();
            fullScreenText = transform.Find("FullScreen/Text").GetComponent<TMP_Text>();

            EventRegister();

            StateMachine.AddState(NarrationType.None, new NoneState(StateMachine, this));
            StateMachine.AddState(NarrationType.Left, new LeftState(StateMachine, this));
            StateMachine.AddState(NarrationType.LeftNeko, new LeftNekoState(StateMachine, this));
            StateMachine.AddState(NarrationType.Right, new RightState(StateMachine, this));
            StateMachine.AddState(NarrationType.RightNeko, new RightNekoState(StateMachine, this));
            StateMachine.AddState(NarrationType.FullScreen, new FullScreenState(StateMachine, this));
            StateMachine.AddState(NarrationType.FullScreenNeko, new FullScreenNekoState(StateMachine, this));

            StateMachine.StartState(NarrationType.None);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (IsNarrating)
                {
                    GameProgressData.CompleteNarration(narrationPlaying);
                    StopNarration();
                }
            }
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return))
            {
                if (CurrentTypeTextCoroutine != null) StopCoroutine(CurrentTypeTextCoroutine);
                CurrentTypeTextCoroutine = null;
            }
        }

        public static void StartNarration(string id, float delay = 0f)
        {
            if (string.IsNullOrEmpty(id)) return;

            var content = LocalizationManager.GetNarration(id);

            if (Instance.CurrentNarrationCoroutine != null) Instance.StopCoroutine(Instance.CurrentNarrationCoroutine);
            Instance.CurrentNarrationCoroutine = Instance.StartCoroutine(Instance.NarrationCoroutine(content, delay));
            Instance.narrationPlaying = id;
            TypeEventSystem.Global.Send(new OnNarrationStartEvent(id));
        }

        public static void StopNarration()
        {
            Instance.StateMachine.ChangeState(NarrationType.None);
            Instance.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Instance.mCanvasGroup, 0f));

            if (Instance.CurrentNarrationCoroutine != null) Instance.StopCoroutine(Instance.CurrentNarrationCoroutine);
            Instance.CurrentNarrationCoroutine = null;
            
            TypeEventSystem.Global.Send(new OnNarrationEndEvent(Instance.narrationPlaying));
            Instance.narrationPlaying = "";
        }

        private IEnumerator NarrationCoroutine(List<NarrationSentence> content, float delay = 0f)
        {
            yield return new WaitForSeconds(delay);

            Instance.StateMachine.ChangeState(NarrationType.None);
            yield return CanvasGroupHelper.FadeCanvasGroup(Instance.mCanvasGroup, 1f);

            foreach (var sentence in content)
            {
                if (sentence.type is NarrationType.Left)
                {
                    leftText.SetText("");
                    if (!string.IsNullOrEmpty(sentence.narrator) && Tachies.TryGetValue(sentence.narrator, out var value) && value != null)
                    {
                        leftImage.sprite = value;
                        leftImage.color = Color.white;
                    }
                    else leftImage.color = new Color(1f, 1f, 1f, 0f);

                    StateMachine.ChangeState(NarrationType.Left);

                    CurrentTypeTextCoroutine = StartCoroutine(TypeTextCoroutine(leftText, sentence.content));
                    yield return new WaitUntil(() => CurrentTypeTextCoroutine == null);
                }
                else if (sentence.type is NarrationType.LeftNeko)
                {
                    foreach (Transform child in leftNekoText) Destroy(child.gameObject);
                    if (!string.IsNullOrEmpty(sentence.narrator) && Tachies.TryGetValue(sentence.narrator, out var value) && value != null)
                    {
                        leftNekoImage.sprite = value;
                        leftNekoImage.color = Color.white;
                    }
                    else leftNekoImage.color = new Color(1f, 1f, 1f, 0f);

                    StateMachine.ChangeState(NarrationType.LeftNeko);
                    
                    CurrentTypeTextCoroutine = StartCoroutine(TypeNekoTextCoroutine(leftNekoText, sentence.content));
                    yield return new WaitUntil(() => CurrentTypeTextCoroutine == null);
                }
                else if (sentence.type is NarrationType.Right)
                {
                    rightText.SetText("");
                    if (!string.IsNullOrEmpty(sentence.narrator) && Tachies.TryGetValue(sentence.narrator, out var value))
                    {
                        rightImage.sprite = value;
                        rightImage.color = Color.white;
                    }
                    else rightImage.color = new Color(1f, 1f, 1f, 0f);

                    StateMachine.ChangeState(NarrationType.Right);
                    
                    CurrentTypeTextCoroutine = StartCoroutine(TypeTextCoroutine(rightText, sentence.content));
                    yield return new WaitUntil(() => CurrentTypeTextCoroutine == null);
                }
                else if (sentence.type is NarrationType.RightNeko)
                {
                    foreach (Transform child in rightNekoText) Destroy(child.gameObject);
                    if (!string.IsNullOrEmpty(sentence.narrator) && Tachies.TryGetValue(sentence.narrator, out var value))
                    {
                        rightNekoImage.sprite = value;
                        rightNekoImage.color = Color.white;
                    }
                    else rightNekoImage.color = new Color(1f, 1f, 1f, 0f);

                    StateMachine.ChangeState(NarrationType.RightNeko);

                    CurrentTypeTextCoroutine = StartCoroutine(TypeNekoTextCoroutine(rightNekoText, sentence.content));
                    yield return new WaitUntil(() => CurrentTypeTextCoroutine == null);
                }
                else if (sentence.type is NarrationType.FullScreen)
                {
                    fullScreenText.SetText("");
                    fullScreenImage.color = new Color(1f, 1f, 1f, 0f);

                    Instance.StateMachine.ChangeState(NarrationType.FullScreen);
                    
                    CurrentTypeTextCoroutine = StartCoroutine(TypeTextCoroutine(fullScreenText, sentence.content));
                    yield return new WaitUntil(() => CurrentTypeTextCoroutine == null);
                    fullScreenText.SetText(sentence.content);
                }

                yield return new WaitForSeconds(0.2f);
                yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return));
            }

            yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return));

            GameProgressData.CompleteNarration(narrationPlaying);
            StopNarration();
        }

        private IEnumerator TypeTextCoroutine(TMP_Text textfield, string text)
        {
            var len = text.Length;
            var speed = 1 / 24f;

            for (var i = 0; i < len; i++)
            {
                textfield.text += text[i];
                if (text[i] is not ' ' or '\r' or '\n') AudioKit.PlaySound("InteractClick");
                yield return new WaitForSeconds(speed);
            }
            textfield.SetText(text);

            CurrentTypeTextCoroutine = null;
        }

        private IEnumerator TypeNekoTextCoroutine(Transform textfield, string text)
        {
            var speed = 1 / 24f;
            foreach (var character in NekoLanguageRenderer.RenderToCharacterData(text))
            {
                var obj = Instantiate(Instance.NekoCharacterPrefab, leftNekoText);
                obj.GetComponent<Character>().Initialize(character);

                yield return new WaitForSeconds(speed);
            }

            CurrentTypeTextCoroutine = null;
        }

        private void EventRegister()
        {
            TypeEventSystem.Global.Register<OnInitialNarrationStartEvent>(e =>
            {
                if (!GameProgressData.HaveReadNarration("InitialNarration"))
                {
                    StartNarration("InitialNarration", 2f);
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnItemUseEvent>(e =>
            {
                switch (e.Data.Id)
                {
                    case "disk1":
                        StartNarration("Disk1");
                        break;
                    case "disk2":
                        StartNarration("Disk2");
                        break;
                    case "disk3":
                        StartNarration("Disk3");
                        break;
                    case "disk4":
                        StartNarration("Disk4");
                        break;
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
    }

    public partial class NarrationManager
    {
        public class NoneState : AbstractState<NarrationType, NarrationManager>
        {
            public NoneState(FSM<NarrationType> fsm, NarrationManager target) : base(fsm, target) { }
            protected override bool OnCondition() => mFSM.CurrentStateId is not NarrationType.None;
        }

        public class LeftState : AbstractState<NarrationType, NarrationManager>
        {
            public LeftState(FSM<NarrationType> fsm, NarrationManager target) : base(fsm, target) { }
            protected override bool OnCondition() => mFSM.CurrentStateId is not NarrationType.Left;

            protected override void OnEnter()
            {
                mTarget.leftCanvasGroup.LinearTransition(0.2f);
            }

            protected override void OnExit()
            {
                mTarget.leftCanvasGroup.InverseLinearTransition(0.2f);
            }
        }

        public class LeftNekoState : AbstractState<NarrationType, NarrationManager>
        {
            public LeftNekoState(FSM<NarrationType> fsm, NarrationManager target) : base(fsm, target) { }
            protected override bool OnCondition() => mFSM.CurrentStateId is not NarrationType.LeftNeko;

            protected override void OnEnter()
            {
                mTarget.leftNekoCanvasGroup.LinearTransition(0.2f);
            }

            protected override void OnExit()
            {
                mTarget.leftNekoCanvasGroup.InverseLinearTransition(0.2f);
            }
        }

        public class RightState : AbstractState<NarrationType, NarrationManager>
        {
            public RightState(FSM<NarrationType> fsm, NarrationManager target) : base(fsm, target) { }
            protected override bool OnCondition() => mFSM.CurrentStateId is not NarrationType.Right;

            protected override void OnEnter()
            {
                mTarget.rightCanvasGroup.LinearTransition(0.2f);
            }

            protected override void OnExit()
            {
                mTarget.rightCanvasGroup.InverseLinearTransition(0.2f);
            }
        }

        public class RightNekoState : AbstractState<NarrationType, NarrationManager>
        {
            public RightNekoState(FSM<NarrationType> fsm, NarrationManager target) : base(fsm, target) { }
            protected override bool OnCondition() => mFSM.CurrentStateId is not NarrationType.RightNeko;

            protected override void OnEnter()
            {
                mTarget.rightNekoCanvasGroup.LinearTransition(0.2f);
            }

            protected override void OnExit()
            {
                mTarget.rightNekoCanvasGroup.InverseLinearTransition(0.2f);
            }
        }

        public class FullScreenState : AbstractState<NarrationType, NarrationManager>
        {
            public FullScreenState(FSM<NarrationType> fsm, NarrationManager target) : base(fsm, target) { }
            protected override bool OnCondition() => mFSM.CurrentStateId is not NarrationType.FullScreen;

            protected override void OnEnter()
            {
                mTarget.fullScreenCanvasGroup.LinearTransition(0.2f);
            }

            protected override void OnExit()
            {
                mTarget.fullScreenCanvasGroup.InverseLinearTransition(0.2f);
            }
        }

        public class FullScreenNekoState : AbstractState<NarrationType, NarrationManager>
        {
            public FullScreenNekoState(FSM<NarrationType> fsm, NarrationManager target) : base(fsm, target) { }
            protected override bool OnCondition() => mFSM.CurrentStateId is not NarrationType.FullScreenNeko;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(NarrationManager))]
    [CanEditMultipleObjects]
    public class NarrationManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            NarrationManager manager = (NarrationManager)target;

            EditorGUILayout.Separator();

            manager.ToShowNarrationId = EditorGUILayout.TextField(manager.ToShowNarrationId);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Start"))
            {
                NarrationManager.StartNarration(manager.ToShowNarrationId);
            }
            if (GUILayout.Button("Stop"))
            {
                NarrationManager.StopNarration();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

#endif
}