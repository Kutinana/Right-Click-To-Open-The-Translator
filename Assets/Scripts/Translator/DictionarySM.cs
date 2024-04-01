using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UnityEngine;
using UI;
using Translator;
using UnityEngine.UI;

namespace Dictionary
{
    public enum States
    {
        Home,
        PuzzleList,
        Puzzle
    }

    public class DictionarySM : MonoBehaviour , ISingleton
    {
        public static DictionarySM Instance => SingletonProperty<DictionarySM>.Instance;
        public static FSM<States> StateMachine => Instance.stateMachine;

        private FSM<States> stateMachine = new FSM<States>();

        public GameObject CharacterPrefab;
        public Coroutine CurrentCoroutine = null;

        internal CanvasGroup characterListCanvasGroup;
        internal CanvasGroup puzzleListCanvasGroup;
        internal CanvasGroup puzzleCanvasGroup;

        private Button puzzleListBackButton;

        public void OnSingletonInit() {}

        private void Awake()
        {
            characterListCanvasGroup = transform.Find("CharacterList").GetComponent<CanvasGroup>();
            puzzleListCanvasGroup = transform.Find("PuzzleList").GetComponent<CanvasGroup>();
            puzzleCanvasGroup = transform.Find("Puzzle").GetComponent<CanvasGroup>();

            puzzleListBackButton = transform.Find("PuzzleList/Back").GetComponent<Button>();
            puzzleListBackButton.onClick.AddListener(() => stateMachine.ChangeState(States.Home));

            TypeEventSystem.Global.Register<CallForPuzzleListEvent>(e => {
                stateMachine.ChangeState(States.PuzzleList);
            });

            stateMachine.AddState(States.Home, new HomeState(stateMachine, this));
            stateMachine.AddState(States.PuzzleList, new PuzzleListState(stateMachine, this));
            stateMachine.AddState(States.Puzzle, new PuzzleState(stateMachine, this));

            stateMachine.StartState(States.Home);
        }
    }
    
    public class HomeState : AbstractState<States, DictionarySM>
    {
        public HomeState(FSM<States> fsm, DictionarySM target) : base(fsm, target) {}
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null && mFSM.CurrentStateId != States.Home;

        protected override void OnEnter()
        {
            GenerateCharacterList();
            mTarget.StartCoroutine(OnEnterCoroutine());
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnExit()
        {
            mTarget.StartCoroutine(OnExitCoroutine());
        }

        IEnumerator OnEnterCoroutine()
        {
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.characterListCanvasGroup, 1f, 0.1f));

            mTarget.CurrentCoroutine = null;
        }

        IEnumerator OnExitCoroutine()
        {
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.characterListCanvasGroup, 0f, 0.1f));

            mTarget.CurrentCoroutine = null;
        }

        private void GenerateCharacterList()
        {
            var parent = mTarget.transform.Find("CharacterList/Scroll View/Viewport/Content");
            for (var i = 0; i < parent.childCount; i++)
            {
                GameObject.Destroy(parent.GetChild(i).gameObject);
            }

            foreach (var c in UserDictionary.userDictionary)
            {
                var go = GameObject.Instantiate(mTarget.CharacterPrefab, parent);
                go.GetComponent<Character>().Initialize(GameDesignData.GetCharacterDataById(c.Key), isInteractable: TranslatorSM.StateMachine.CurrentStateId != Translator.States.Off, isBlack: true);
                go.transform.localScale = new Vector2(1.5f, 1.5f);
            }
        }
    }

    public struct CallForPuzzleListEvent {
        public string id;
        public CallForPuzzleListEvent(string _id) { id = _id; }
    }

    public class PuzzleListState : AbstractState<States, DictionarySM>
    {
        public PuzzleListState(FSM<States> fsm, DictionarySM target) : base(fsm, target) {}
        protected override bool OnCondition() =>
            mTarget.CurrentCoroutine == null
            && mFSM.CurrentStateId != States.PuzzleList
            && TranslatorSM.StateMachine.CurrentStateId == Translator.States.Dictionary;

        protected override void OnEnter()
        {
            mTarget.StartCoroutine(OnEnterCoroutine());
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnExit()
        {
            mTarget.StartCoroutine(OnExitCoroutine());
        }

        IEnumerator OnEnterCoroutine()
        {
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.puzzleListCanvasGroup, 1f, 0.1f));

            mTarget.CurrentCoroutine = null;
        }

        IEnumerator OnExitCoroutine()
        {
            yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.puzzleListCanvasGroup, 0f, 0.1f));

            mTarget.CurrentCoroutine = null;
        }
    }

    public class PuzzleState : AbstractState<States, DictionarySM>
    {
        public PuzzleState(FSM<States> fsm, DictionarySM target) : base(fsm, target) {}
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null && mFSM.CurrentStateId != States.Puzzle;

        protected override void OnEnter()
        {
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnExit()
        {
        }
    }
}