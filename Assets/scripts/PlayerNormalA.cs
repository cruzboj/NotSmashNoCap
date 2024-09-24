using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNormalA : PlayerBaseState
{
    // animations
    const string _PLAYER_ATTACK = "Player_attack";
    const string _PLAYER_ATTACK2 = "Player_attack2";
    const string _PLAYER_ATTACK3 = "Player_attack3";
    const string _PLAYER_DASHATTACK = "Player_DashAttack";

    private bool _isAttacking = false;

    public PlayerNormalA(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        // Start the attack animation sequence
        HandleAttackN();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        GroundCheck();
        HandleMovement();
    }

    public override void ExitState()
    {
        // Increment the attack cycle when exiting the state
        Ctx.AttackCycle++;
    }

    public override void InitalizeSubState() { }

    public override void CheckSwitchStates()
    {
        if (Ctx.Grounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    void HandleAttackN()
    {
        if (Ctx.Grounded && Ctx.CurrentMovementInput.magnitude < Ctx.Threshold)
        {
            // Use the context to start the coroutine
            Ctx.StartStateCoroutine(ExecuteAttackCycle());
        }
        else if(Ctx.Grounded && Ctx.CurrentMovementInput.magnitude >= Ctx.Threshold)
        {
            changeAnimationState(_PLAYER_DASHATTACK);
        }
    }

    IEnumerator ExecuteAttackCycle()
    {
        _isAttacking = true;


        // Execute attack animations based on the current cycle
        switch (Ctx.AttackCycle)
        {
            case 0:
                changeAnimationState(_PLAYER_ATTACK);
                break;
            case 1:
                changeAnimationState(_PLAYER_ATTACK2);
                break;
            case 2:
                changeAnimationState(_PLAYER_ATTACK3);
                break;
        }

        // Wait for a short duration before allowing the next attack
        yield return new WaitForSeconds(0.8f);
        
        _isAttacking = false;

        // Exit the state to trigger the next attack in the cycle
        Ctx.AttackCycle += 1;
        if (Ctx.AttackCycle >=  Ctx.NumberOfAttacks) Ctx.AttackCycle = 0;
    }

    void changeAnimationState(string newState)
    {
        // Prevent the same animation from interrupting itself
        if (Ctx.AnimationState == newState) return;

        // Play the new animation
        Ctx.Animator.Play(newState);

        // Set the current animation state
        Ctx.AnimationState = newState;
    }
    private bool GroundCheck()
    {
        // Check if the box collides with the ground and set the grounded status
        if (Physics.CheckBox(Ctx.transform.position, Ctx.boxSize, Ctx.transform.rotation, Ctx.layerMask))
        {
            Debug.Log("Grounded");
            Ctx.JumpCount = 0;
            Ctx.Grounded = true;
            return true;
        }
        else
        {
            Debug.Log("NOT grounded");
            Ctx.Grounded = false;
            return false;
        }
    }
    public void HandleMovement()
    {
        if (_isAttacking)
        {
            Ctx.CurrentMovement = Vector2.zero; 
            return;
        }
    }
}
