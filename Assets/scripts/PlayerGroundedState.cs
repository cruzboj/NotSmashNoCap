using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitalizeSubState();
    }

    public override void EnterState(){}

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void InitalizeSubState()
    {
        if (!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.CurrentMovementInput.magnitude >= Ctx.Threshold)
        {
            SetSubState(Factory.Run());
        }
    }
    public override void CheckSwitchStates()
    {
        if (Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Jump());
        }
        else if (Ctx.IsAttackNPressed)
        {
            SwitchState(Factory.AttackN());
        }
    }
}
