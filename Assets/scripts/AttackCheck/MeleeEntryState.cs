using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEntryState : State
{
    private PlayerStateMachine _playerStateMachine;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        _playerStateMachine = _stateMachine.GetComponentInParent<PlayerStateMachine>();

        if (_playerStateMachine._grounded) 
        { 
            State nextState = (State)new GroundEntryState();
            stateMachine.SetNextState(nextState);
        }
        else
        {
            State nextState = (State)new AirState();
            stateMachine.SetNextState(nextState);
        }
    }
}
