using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    const string _PLAYER_IDLE = "Player_idle";
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    { }
    public override void EnterState()
    {
        changeAnimeationState(_PLAYER_IDLE);
        //Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        //Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
        //Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState() { }
    public override void InitalizeSubState() {
    
    }
    public override void CheckSwitchStates()
    {
        if (Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.CurrentMovementInput.magnitude >= Ctx.Threshold)
        {
            SwitchState(Factory.Run());
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
