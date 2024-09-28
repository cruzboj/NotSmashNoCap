using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    const string _PLAYER_JUMP = "Player_jump";
    const string _PLAYER_JUMP2 = "Player_jump2";
    const string _PLAYER_IDLE = "Player_idle";
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        if (Ctx.JumpCount == 0)
        {
            changeAnimeationState(_PLAYER_JUMP);
            //Debug.Log("jump 0");
        }
        //Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
        //Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        //Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
        HandleJump();
    }

    public override void UpdateState()
    {
        //GroundCheck();
        CheckSwitchStates();
    }

    public override void ExitState() {
        //Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
    }

    public override void InitalizeSubState()
    {
        if (!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.CurrentMovementInput.magnitude >= Ctx.Threshold)
        {
            SwitchState(Factory.Run());
        }
    }
    public override void CheckSwitchStates()
    {
        if (Ctx.Grounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    void HandleJump()
    {
        if (Ctx.Grounded && Ctx.IsJumpPressed || Ctx.IsJumpPressed && (Ctx.JumpCount < Ctx.MaxJumpCount))
        {
            Ctx.Rb.velocity = new Vector3(Ctx.Rb.velocity.x, Ctx.JumpForce); // Apply jump force
            Ctx.JumpCount++;
            //Debug.Log("Jumped! Jump Count: " + _jumpCount);
            Ctx.IsJumpPressed = false;
            //_animator.SetBool(_isJumpingHash, true);
            changeAnimeationState(_PLAYER_JUMP2);
        }
        //else if (!GroundCheck() && _jumpCount >= _maxJumpCount)
        //{
        //    //Debug.Log("Cannot jump. Already at max jumps.");
        //}
    }

    //private bool GroundCheck()
    //{
    //    // Check if the box collides with the ground and set the grounded status
    //    if (Physics.CheckBox(Ctx.transform.position, Ctx.boxSize, Ctx.transform.rotation, Ctx.layerMask))
    //    {
    //        //Debug.Log("Grounded");
    //        Ctx.JumpCount = 0;
    //        Ctx.Grounded = true;
    //        return true;
    //    }
    //    else
    //    {
    //        //Debug.Log("NOT grounded");
    //        Ctx.Grounded = false;
    //        return false;
    //    }
    //}

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
