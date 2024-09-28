using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerRunState : PlayerBaseState
{
    const string _PLAYER_RUN = "Player_run";
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        changeAnimeationState(_PLAYER_RUN);
        //Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        //Ctx.Animator.SetBool(Ctx.IsRunningHash, true);
        //Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState(){
        //Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
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
    }
    void changeAnimeationState(string newState)
    {
        //stop the same animation from interruptting itself
        if (Ctx.AnimationState == newState) return;

        //play the animation
        Ctx.Animator.Play(newState);

        //reassign the current state
        Ctx.AnimationState = newState;
    }
}
