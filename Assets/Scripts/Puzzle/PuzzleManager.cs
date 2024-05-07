using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using Translator;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle
{
    public partial class PuzzleManager : MonoBehaviour , ISingleton
    {
        public enum States
        {
            None,
            InActive,
            Active
        }

        public static PuzzleManager Instance => SingletonProperty<PuzzleManager>.Instance;
        public void OnSingletonInit() {}
        public static FSM<States> StateMachine => Instance.stateMachine;
        public FSM<States> stateMachine = new FSM<States>();

        private CanvasGroup canvasGroup;

        public string ToInstantiatePuzzleId { get; set; }
        public static PuzzleBase CurrentPuzzle = null;
        // public static List<PuzzleBase> LoadedPuzzles = new List<PuzzleBase>();
        public Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            TypeEventSystem.Global.Register<OnTranslatorEnabledEvent>(e => {
                if (CurrentPuzzle != null) StateMachine.ChangeState(States.InActive);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            TypeEventSystem.Global.Register<OnTranslatorDisabledEvent>(e => {
                if (CurrentPuzzle != null) StateMachine.ChangeState(States.Active);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            stateMachine.AddState(States.None, new NoneState(stateMachine, this));
            stateMachine.AddState(States.Active, new ActiveState(stateMachine, this));
            stateMachine.AddState(States.InActive, new InActiveState(stateMachine, this));

            stateMachine.StartState(States.None);
        }

        public static void Initialize(string _id)
        {
            if (CurrentPuzzle != null) return;

            TypeEventSystem.Global.Send<OnPuzzleInitializedEvent>();

            var data = GameDesignData.GetPuzzleDataById(_id);
            CurrentPuzzle = Instantiate(data.Prefab, Instance.transform).GetComponent<PuzzleBase>();
            CurrentPuzzle.Id = _id;

            // LoadedPuzzles.Add(CurrentPuzzle);

            GameProgressData.Unlock(CurrentPuzzle);

            CurrentPuzzle.OnEnter();
            StateMachine.ChangeState(States.Active);
        }

        private void Update()
        {
            if (CurrentPuzzle != null)
            {
                CurrentPuzzle.OnUpdate();
            }
            stateMachine.CurrentState.Update();
        }

        public static void Exit()
        {
            if (CurrentPuzzle != null)
            {
                CurrentPuzzle.OnExit();

                StateMachine.ChangeState(States.None);
            }
        }

        public static void Solved(float _delay = 1f)
        {
            if (CurrentPuzzle != null)
            {
                Instance.StartCoroutine(Instance.SolvedCoroutine(_delay));
            }
        }

        private IEnumerator SolvedCoroutine(float _delay)
        {
            yield return new WaitForSeconds(_delay);

            AudioKit.PlaySound("Accomplish",volumeScale: AudioMng.Instance.effectVolume);

            CurrentPuzzle.OnSolved();
            TypeEventSystem.Global.Send(new OnPuzzleSolvedEvent(CurrentPuzzle));
        
            StateMachine.ChangeState(States.None);
            GameProgressData.Solve(CurrentPuzzle);
        }
    }

    public partial class PuzzleManager
    {
        public class NoneState : AbstractState<States, PuzzleManager>
        {
            public NoneState(FSM<States> fsm, PuzzleManager target) : base(fsm, target) {}
            protected override bool OnCondition() =>  mFSM.CurrentStateId != States.None;

            protected override void OnEnter()
            {
                mTarget.StartCoroutine(OnEnterCoroutine());
            }

            IEnumerator OnEnterCoroutine()
            {
                yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 0f, 0.1f));
                if (CurrentPuzzle != null)
                {
                    Destroy(CurrentPuzzle.gameObject);

                    TypeEventSystem.Global.Send(new OnPuzzleExitEvent(CurrentPuzzle));
                }
                
                CurrentPuzzle = null;
                mTarget.CurrentCoroutine = null;
            }
        }

        public class ActiveState : AbstractState<States, PuzzleManager>
        {
            public ActiveState(FSM<States> fsm, PuzzleManager target) : base(fsm, target) {}
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

            protected override void OnUpdate()
            {
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    PuzzleManager.Exit();
                }
            }

            IEnumerator OnEnterCoroutine()
            {
                yield return mTarget.CurrentCoroutine = mTarget.StartCoroutine(Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(mTarget.canvasGroup, 1f, 0.1f));

                mTarget.CurrentCoroutine = null;
            }
        }

        public class InActiveState : AbstractState<States, PuzzleManager>
        {
            public InActiveState(FSM<States> fsm, PuzzleManager target) : base(fsm, target) {}
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
    
    #if UNITY_EDITOR

    [CustomEditor(typeof(PuzzleManager))]
    [CanEditMultipleObjects]
    public class PuzzleManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            PuzzleManager manager = (PuzzleManager)target;

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            manager.ToInstantiatePuzzleId = EditorGUILayout.TextField(manager.ToInstantiatePuzzleId);
            if (GUILayout.Button("Initialize"))
            {
                PuzzleManager.Initialize(manager.ToInstantiatePuzzleId);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    #endif
}