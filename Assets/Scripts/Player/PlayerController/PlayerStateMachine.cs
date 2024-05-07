using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine: StateMachine
{
    private Animator animator;
    private PlayerInput playerInput;
    private PlayerController playerController;
    private PlayerState lastState = null;
    
    public PlayerState[] states;
    public PlayerState InitialState;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();

        statetable = new Dictionary<System.Type, IState>(states.Length);

        Init();
    }
    private void Init()
    {
        foreach(PlayerState state in states)
        {
            state.Init(this, animator, playerInput, playerController);
            statetable.Add(state.GetType(), state);
        }
    }
    private void Start()
    {
        SwitchOn(InitialState);
    }
    public PlayerState GetLastState()
    {
        return lastState;
    }
    public void SetLastState(PlayerState playerState)
    {
        this.lastState = playerState;
    }
}