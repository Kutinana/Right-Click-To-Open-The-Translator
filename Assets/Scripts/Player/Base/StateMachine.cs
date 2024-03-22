using System.Collections.Generic;
using UnityEngine;

public class StateMachine: MonoBehaviour
{
    IState currentState;

    protected Dictionary<System.Type, IState> statetable;

    private void Awake()
    {
        
    }
    private void Update()
    {
        currentState?.LogicUpdate();
        currentState?.PhysicsUpdate();
    }
    private void FixedUpdate()
    {
        
    }
    public void SwitchOn(IState state)
    {
        state.Enter();
        currentState = state;
    }
    public void SwitchState(IState state)
    {
        currentState?.Exit();
        SwitchOn(state);
    }
    public void SwitchState(System.Type type)
    {
        SwitchState(statetable[type]);
    }
}