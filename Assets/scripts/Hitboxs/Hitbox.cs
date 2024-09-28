using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private PlayerStateMachine _playerStateMachine;
    private ComboCharacter _comboCharacter;
    private Collider _capsuleCollider; // Reference to the capsule collider

    private void Awake()
    {
        _playerStateMachine = GetComponentInParent<PlayerStateMachine>();
        _comboCharacter = GetComponentInParent<ComboCharacter>();
        _capsuleCollider = GetComponent<Collider>();
        _capsuleCollider.enabled = false;
        //if (_comboCharacter == null)
        //{
        //    Debug.LogError("ComboCharacter component not found!");
        //}
    }
    private void Update()
    {
        // If attack is pressed, enable the collider, otherwise disable it
            if (_comboCharacter._isAttackNPressed)
            {
                _capsuleCollider.enabled = true; // Enable collider when attacking
            }
            else
            {
                _capsuleCollider.enabled = false; // Disable collider when not attacking
            }
    }

    void OnTriggerEnter(Collider other)
    {
        //normal left and right
        if (_comboCharacter != null && _comboCharacter._isAttackNPressed && _playerStateMachine._grounded) 
        {
            if (other.gameObject.tag == "AttackCol")
            {
                print("Enter - Attack hit");
                _comboCharacter._attackNumber++;
                if(_comboCharacter._attackNumber == 4)
                {
                    _comboCharacter._attackNumber = 1; 
                }
                OnTriggerExit(other);
            }
            else
            {
                _comboCharacter._attackNumber = 1;
            }
        }
    }

    //void OnTriggerStay(Collider other)
    //{

    //    if (_comboCharacter != null && _comboCharacter._isAttackNPressed)
    //    {
    //        if (other.gameObject.tag == "AttackCol")
    //        {
    //            print("Enter - Attack hit");
    //        }
    //    }
    //}

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "AttackCol")
        {
            print("Exit");
        }
    }
}