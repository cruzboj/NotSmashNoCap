using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNormalA : PlayerBaseState
{
    public PlayerNormalA(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { IsRootState = true; }

    public override void EnterState()
    {
        // Start the attack animation
        Ctx.Animator.SetBool(Ctx.IsAttackNHash, true);
    }

    public override void UpdateState()
    {
        HandleAttackN();
        CheckSwitchStates();
    }

    public override void ExitState(){}

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
        if (Ctx.IsAttackNPressed)
        {
            Debug.Log("Delay");
            Delay();
        }

    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("End Delay");
        Ctx.IsAttackNPressed = false;
        Ctx.Animator.SetBool(Ctx.IsAttackNHash, false);
    }

}
