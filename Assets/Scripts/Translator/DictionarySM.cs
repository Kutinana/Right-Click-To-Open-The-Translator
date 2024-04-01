using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

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

        public Coroutine CurrentCoroutine = null;

        public void OnSingletonInit() {}

        private void Awake()
        {
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
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnExit()
        {
        }
    }

    public class PuzzleListState : AbstractState<States, DictionarySM>
    {
        public PuzzleListState(FSM<States> fsm, DictionarySM target) : base(fsm, target) {}
        protected override bool OnCondition() => mTarget.CurrentCoroutine == null && mFSM.CurrentStateId != States.PuzzleList;

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