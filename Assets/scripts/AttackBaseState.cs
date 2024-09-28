using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBaseState : PlayerBaseState
{
    // How long this state should be active for before moving on
    public float duration;
    // bool to check whether or not the next attack in the sequence should be played or not
    public bool shouldCombo;
    // The attack index in the sequence of attacks
    protected int attackIndex;

    // animations
    const string _PLAYER_ATTACK = "Player_attack";
    const string _PLAYER_ATTACK2 = "Player_attack2";
    const string _PLAYER_ATTACK3 = "Player_attack3";
    const string _PLAYER_DASHATTACK = "Player_DashAttack";

    public AttackBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        //collidersDamaged = new List<Collider2D>();
        //hitCollider = GetComponent<ComboCharacter>().hitbox;
        //HitEffectPrefab = GetComponent<ComboCharacter>().Hiteffect;

    }

    public override void UpdateState()
    {
        if (Ctx.Grounded)
        {
            Ctx.IsMovementPressed = false;
        }
        //GroundCheck();
        CheckSwitchStates();
    }

    public override void ExitState()
    {

    }

    public override void InitalizeSubState() { }

    public override void CheckSwitchStates()
    {
        if (Ctx.Grounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    void changeAnimationState(string newState)
    {
        if (Ctx.AnimationState == newState) return;
        Ctx.Animator.Play(newState);
        Ctx.AnimationState = newState;
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
}
