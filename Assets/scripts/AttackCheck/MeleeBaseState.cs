using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBaseState : State
{
    // How long this state should be active for before moving on
    public float duration;
    // Cached animator component
    protected Animator animator;
    // bool to check whether or not the next attack in the sequence should be played or not
    public bool shouldCombo;
    // The attack index in the sequence of attacks
    protected int attackIndex;

    //(new) input
    protected ComboCharacter _comboCharacter;
    protected Transform characterTransform; 
    

    // The cached hit collider component of this attack
    protected Collider hitCollider;
    // Cached already struck objects of said attack to avoid overlapping attacks on same target
    private List<Collider> collidersDamaged;
    // The Hit Effect to Spawn on the afflicted Enemy
    private GameObject HitEffectPrefab;

    // Input buffer Timer
    private float AttackPressedTimer = 0;

    public override void OnEnter(StateMachine _stateMachine)
    {
        _comboCharacter = _stateMachine.GetComponent<ComboCharacter>();
        base.OnEnter(_stateMachine);
        animator = GetComponent<Animator>();
        collidersDamaged = new List<Collider>();
        HitEffectPrefab = GetComponent<ComboCharacter>().Hiteffect;
        _comboCharacter = GetComponent<ComboCharacter>(); //ComboCharacter for new input
        characterTransform = _stateMachine.GetComponent<Transform>(); // Store the transform reference (control rotation in attacks)
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        AttackPressedTimer -= Time.deltaTime;

        //if (animator.GetFloat("Weapon.Active") > 0f)
        //{
        //    //Attack();
        //}


        //if (_comboCharacter._isAttackNPressed)
        //{
        //    AttackPressedTimer = 2;
        //}

        //if (animator.GetFloat("AttackWindow.Open") > 0f && AttackPressedTimer > 0)
        //{
        //    shouldCombo = true;
        //}
    }

    public override void OnExit()
    {
        base.OnExit();
    }

}
