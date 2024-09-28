using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    const string _PLAYER_IDLE = "Player_idle";
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitalizeSubState();
    }

    public override void EnterState()
    { 
        //changeAnimeationState(_PLAYER_IDLE); 
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        GroundCheck();
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

        if (Ctx.IsAttackNPressed)
        {
            SwitchState(Factory.Attack());
        }
    }
    private bool GroundCheck()
    {
        // Check if the box collides with the ground and set the grounded status
        if (Physics.CheckBox(Ctx.transform.position, Ctx.boxSize, Ctx.transform.rotation, Ctx.layerMask))
        {
            //Debug.Log("Grounded");
            Ctx.JumpCount = 0;
            Ctx.Grounded = true;
            return true;
        }
        else
        {
            //Debug.Log("NOT grounded");
            Ctx.Grounded = false;
            return false;
        }
    }
    //void changeAnimeationState(string newState)
    //{
    //    //stop the same animation from interruptting itself
    //    if (Ctx.AnimationState == newState) return;

    //    //play the animation
    //    Ctx.Animator.Play(newState);

    //    //reassign the current state
    //    Ctx.AnimationState = newState;
    //}
}
