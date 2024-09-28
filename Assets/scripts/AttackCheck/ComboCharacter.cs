using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComboCharacter : MonoBehaviour
{

    private PlayerStateMachine _playerStateMachine;

    //new inputs
    private PlayerInput _playerInput;
    public bool _isAttackNPressed;

    private StateMachine meleeStateMachine;

    //private PlayerStateMachine _playerStateMachine; //to make the attack bool wont appire twice

    [SerializeField] public Collider hitbox;
    [SerializeField] public GameObject Hiteffect;

    public int _attackNumber = 1;

    private void Awake()
    {
        _playerStateMachine = GetComponentInParent<PlayerStateMachine>();
        _playerInput = new PlayerInput();
        _playerInput.CharacterControls.AttackN.started += onAttack;
        _playerInput.CharacterControls.AttackN.canceled += onAttack;  
    }
        // Start is called before the first frame update
        void Start()
    {

        meleeStateMachine = GetComponent<StateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAttackNPressed && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            if (_playerStateMachine._grounded)
                meleeStateMachine.SetNextState(new GroundEntryState());
            else
            {
                if (_isAttackNPressed && !_playerStateMachine._grounded)
                {
                    if (_playerStateMachine._upPressed) //up air
                    {
                        _attackNumber = 4;
                        meleeStateMachine.SetNextState(new AirUpState());
                    }
                    else if (_playerStateMachine._isMovementPressed) //left and right air
                    {
                        _attackNumber = 5;
                        meleeStateMachine.SetNextState(new AirLeftRightState());
                    }
                    else //not moving air
                    {
                        _attackNumber = 6;
                        meleeStateMachine.SetNextState(new AirState());
                    }
                }
            }
        }
    }

    void onAttack(InputAction.CallbackContext context)
    {
        if (context.started) // If Attack Normal is pressed
        {
            _isAttackNPressed = true;
            //Debug.Log("Attack button pressed");
        }
        else if (context.canceled) // If Attack Normal is released
        {
            _isAttackNPressed = false;
            //Debug.Log("Attack button released");
        }
    }

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
        // connected device
        //InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
        // disconnected device
        //InputSystem.onDeviceChange -= OnDeviceChange;
    }
}
