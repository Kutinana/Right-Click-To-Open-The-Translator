using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UnityEngine;
using UI;
using Translator;
using UnityEngine.UI;
using Kuchinashi;
using TMPro;

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
        public GameObject PuzzlePrefab;
        public Coroutine CurrentCoroutine = null;

        internal CanvasGroup characterListCanvasGroup;
        internal CanvasGroup detailCanvasGroup;
        internal CanvasGroup puzzleCanvasGroup;

        private Image currentCharacterImage;
        private TMP_Text currentCharacterMeaning;

        private Button backToCharacterListBtn;

        public CharacterData CurrentCharacterData;

        public void OnSingletonInit() {}

        private void Awake()
        {
            characterListCanvasGroup = transform.Find("CharacterList").GetComponent<CanvasGroup>();
            detailCanvasGroup = transform.Find("CharacterList/Detail").GetComponent<CanvasGroup>();
            puzzleCanvasGroup = transform.Find("Puzzle").GetComponent<CanvasGroup>();

            currentCharacterImage = detailCanvasGroup.transform.Find("Character").GetComponent<Image>();
            currentCharacterMeaning = detailCanvasGroup.transform.Find("Meaning").GetComponent<TMP_Text>();

            backToCharacterListBtn = puzzleCanvasGroup.transform.Find("Back").GetComponent<Button>();
            backToCharacterListBtn.onClick.AddListener(() =>
            {
                stateMachine.ChangeState(States.Home);
            });

            TypeEventSystem.Global.Register<OnCharacterDataUpdateEvent>(e => {
                CurrentCharacterData = e.Data;
                StartCoroutine(ShowCharacterDetailCoroutine());
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<CallForPuzzleEvent>(e => {
                puzzleCanvasGroup.transform.Find("Image").GetComponent<Image>().sprite = e.Data.Thumbnail;
                stateMachine.ChangeState(States.Puzzle);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            stateMachine.AddState(States.Home, new HomeState(stateMachine, this));
            stateMachine.AddState(States.Puzzle, new PuzzleState(stateMachine, this));

            stateMachine.StartState(States.Home);
        }

        public void GenerateCharacterList()
        {
            var parent = transform.Find("CharacterList/Scroll View/Viewport/Content");
            for (var i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }

            if (UserDictionary.IsEmpty()) return;
            foreach (var c in UserDictionary.GetDictionary())
            {
                if (string.IsNullOrEmpty(c.Key)) continue;

                var go = Instantiate(CharacterPrefab, parent);
                go.GetComponent<DictionaryCharacterController>().Initialize(GameDesignData.GetCharacterDataById(c.Key), isInteractable: TranslatorSM.StateMachine.CurrentStateId != Translator.States.Off, isBlack: true);
            }
        }

        public IEnumerator ShowCharacterDetailCoroutine()
        {
            yield return CanvasGroupHelper.FadeCanvasGroup(detailCanvasGroup, 0f, 0.2f);

            currentCharacterImage.sprite = CurrentCharacterData.Sprite;
            currentCharacterMeaning.SetText(UserDictionary.Read(CurrentCharacterData.Id));

            var parent = detailCanvasGroup.transform.Find("PuzzleList/Scroll View/Viewport/Content");
            for (var i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }

            foreach (var p in CurrentCharacterData.RelatedPuzzles)
            {
                if (GameProgressData.GetPuzzleProgress(p.Id) == PuzzleProgress.NotFound) continue;

                var go = Instantiate(PuzzlePrefab, parent);
                go.GetComponent<PuzzleThumbnailController>().Initialize(p);
            }

            yield return CanvasGroupHelper.FadeCanvasGroup(detailCanvasGroup, 1f, 0.2f);
        }
    }
    
    public class HomeState : AbstractState<States, DictionarySM>
    {
        public HomeState(FSM<States> fsm, DictionarySM target) : base(fsm, target) {}
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null && mFSM.CurrentStateId != States.Home;

        protected override void OnEnter()
        {
            mTarget.CurrentCoroutine = mTarget.StartCoroutine(OnEnterCoroutine());
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnExit()
        {
            mTarget.CurrentCoroutine = mTarget.StartCoroutine(OnExitCoroutine());
        }

        IEnumerator OnEnterCoroutine()
        {
            yield return new WaitUntil(() => mTarget.CurrentCoroutine == null);
            yield return mTarget.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mTarget.characterListCanvasGroup, 1f, 0.1f));

            mTarget.CurrentCoroutine = null;
        }

        IEnumerator OnExitCoroutine()
        {
            yield return new WaitUntil(() => mTarget.CurrentCoroutine == null);
            yield return mTarget.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mTarget.characterListCanvasGroup, 0f, 0.1f));

            mTarget.CurrentCoroutine = null;
        }
    }

    public struct OnCharacterDataUpdateEvent {
        public CharacterData Data;
        public OnCharacterDataUpdateEvent(CharacterData _data) { Data = _data; }
    }

    public struct CallForPuzzleEvent {
        public PuzzleDataBase Data;
        public CallForPuzzleEvent(PuzzleDataBase _data) { Data = _data; }
    }

    public class PuzzleState : AbstractState<States, DictionarySM>
    {
        public PuzzleState(FSM<States> fsm, DictionarySM target) : base(fsm, target) {}
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null && mFSM.CurrentStateId != States.Puzzle;

        protected override void OnEnter()
        {
            mTarget.CurrentCoroutine = mTarget.StartCoroutine(OnEnterCoroutine());
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnExit()
        {
            mTarget.CurrentCoroutine = mTarget.StartCoroutine(OnExitCoroutine());
        }

        IEnumerator OnEnterCoroutine()
        {
            yield return new WaitUntil(() => mTarget.CurrentCoroutine == null);
            yield return mTarget.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mTarget.puzzleCanvasGroup, 1f, 0.1f));

            mTarget.CurrentCoroutine = null;
        }

        IEnumerator OnExitCoroutine()
        {
            yield return new WaitUntil(() => mTarget.CurrentCoroutine == null);
            yield return mTarget.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(mTarget.puzzleCanvasGroup, 0f, 0.1f));

            mTarget.CurrentCoroutine = null;
        }
    }
}