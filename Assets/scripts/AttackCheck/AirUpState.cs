using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirUpState : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        attackIndex = _comboCharacter._attackNumber;
        duration = 0.5f;
        animator.SetTrigger("Attack" + attackIndex);
        _comboCharacter._attackNumber = 1;
        //Debug.Log("Player Attack " + attackIndex + " Fired!");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
                stateMachine.SetNextStateToMain();
        }
    }
}
