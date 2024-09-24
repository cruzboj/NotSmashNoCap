using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, true);
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState(){
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
    }
    public override void InitalizeSubState(){}
    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.CurrentMovementInput.magnitude < Ctx.Threshold)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.IsAttackNPressed)
        {
            SwitchState(Factory.AttackN());
        }
    }
}
