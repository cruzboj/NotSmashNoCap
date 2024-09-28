using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    private float currentYRotation = 0f; // Track the current rotation
    //intis
    private Rigidbody _rb;
    private PlayerInput _playerInput;
    private Animator _animator;

    //gravity
    //new - castRay
    [SerializeField] public LayerMask layerMask; //set to Ground
    RaycastHit hit;
    private float rayBeam = 0.1f;
    public Vector3 boxSize = new Vector3(1f, 0.3f, 1f); //1 ,0.3 ,1

    public bool _grounded = true;
    private float _downForce = 9.8f;
    //public float _groundCheckDistance = 0.1f;

    //movement
    public bool _isMovementPressed;
    public bool IsMovementPressed { get { return _isMovementPressed; } set { _isMovementPressed = value; } }
    Vector2 _currentMovementInput;
    Vector2 _currentMovement;
    public float _walkSpeed = 3f;
    public float _runSpeed = 6f;
    private float _currentSpeed;
    private float _threshold = 0.9f;
    private float _rotationFactorPerFrame = 25.0f;

    //jump
    public bool _isJumpPressed = false; // JumpPressed
    public int _jumpCount = 0; // jump counter
    private int _maxJumpCount = 1; // max Jumps allowed
    private float _jumpForce = 12f;

    //Attack Normal
    public bool _isAttackNPressed = false;
    public bool _upPressed = false;
    public bool IsAttackNPressed { get { return _isAttackNPressed; } set { _isAttackNPressed = value; } }

    public int _attackCycle;
    private int _numberOfAttacks = 3; // Total number of attacks in the cycle
    public int NumberOfAttacks { get { return _numberOfAttacks; } }
    public int AttackCycle { get => _attackCycle; set { _attackCycle = value >= _numberOfAttacks ? 0 : value; } }

    //new Animator States
    private string _animationState;
    //const string _PLAYER_IDLE = "Player_idle";
    //const string _PLAYER_WALK = "Player_walk";
    //const string _PLAYER_RUN = "Player_run";
    //const string _PLAYER_JUMP = "Player_jump";
    //const string _PLAYER_JUMP2 = "Player_jump2";
    //const string _PLAYER_ATTACK = "Player_attack";
    public string AnimationState {  get { return _animationState; } set { _animationState = value; } }

    // Animator Hashes
    private int _isWalkingHash;
    private int _isRunningHash;
    private int _isJumpingHash;
    private int _isAttackNHash;

    //State variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    //getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Rigidbody Rb { get { return _rb; } }
    public PlayerInput PlayerInput { get { return _playerInput; } }
    public Animator Animator { get { return _animator; } }
    public bool Grounded {get { return _grounded; } set { _grounded = value; } }
    public float DownForce { get { return _downForce; } }
    //public bool IsMovementPressed { get { return _isMovementPressed; } }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
    public Vector2 CurrentMovement { get { return _currentMovement; } set { _currentMovement = value; } }
    public float WalkSpeed { get { return _walkSpeed; } }
    public float RunSpeed { get { return _runSpeed; } }
    public float Threshold { get { return _threshold; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public int JumpCount { get { return _jumpCount; } set { _jumpCount = value; }}
    public int MaxJumpCount { get { return _maxJumpCount; } }
    public float JumpForce { get { return _jumpForce; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } set { _isJumpPressed = value; }}
    public int IsWalkingHash { get { return _isWalkingHash; } }
    public int IsRunningHash { get { return _isRunningHash; } }
    public int IsAttackNHash { get { return _isAttackNHash; } }

    //new - CastRay
    private void OnDrawGizmos()
    {
        //hitbox for grounded.
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position - transform.up * rayBeam, boxSize);
    }
    private void Awake()
    {
        _playerInput = new PlayerInput();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        // Animator parameter hashes
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _isAttackNHash = Animator.StringToHash("isAttackN");

        //set the player input callbacks
        _playerInput.CharacterControls.Move.started += onMovementInput;
        _playerInput.CharacterControls.Move.performed += onMovementInput;
        _playerInput.CharacterControls.Move.canceled += onMovementInput;
        _playerInput.CharacterControls.Jump.started += onJump;
        _playerInput.CharacterControls.AttackN.started += onAttackN;
        _playerInput.CharacterControls.AttackN.canceled += onAttackN;
        _playerInput.CharacterControls.Up.started += onUpAir;
        _playerInput.CharacterControls.Up.canceled += onUpAir;
    }
    void Update()
    {
        handleRotation();
        GroundCheck();
        _currentState.UpdateStates();
        //Debug.Log("Current State: " + _currentState.GetType().Name);
    }
    void FixedUpdate()
    {
        // Apply movement based on current speed
        if (_isMovementPressed)
        {
            Vector3 movement = new Vector3(_currentMovementInput.x * _currentSpeed, _rb.velocity.y, 0);
            _rb.velocity = movement;
            //Debug.Log($"Movement applied: {_currentMovementInput.x} with speed: {_currentSpeed}");
        }

        // Apply additional down force when not grounded
        if (!_grounded)
        {
            _rb.AddForce(Vector3.down * _downForce * 2, ForceMode.Acceleration);
        }
        else
        {
            _rb.AddForce(Vector3.down * _downForce, ForceMode.Acceleration);
        }

    }
    

    void onMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.magnitude > 0;

        // Check input intensity for walk/run
        float inputMagnitude = Mathf.Abs(_currentMovementInput.x);
        _currentSpeed = inputMagnitude >= _threshold ? _runSpeed : _walkSpeed;
        //Debug.Log($"Movement input: {_currentMovementInput.x}, Speed set to: {_currentSpeed}");
    }

    void onJump(InputAction.CallbackContext context)
    {
        if (context.started) //if jump is pressed
        {
            _isJumpPressed = true;
        }
    }
    void onAttackN(InputAction.CallbackContext context)
    {
        if (context.started) // If Attack Normal is pressed
        {
            _isAttackNPressed = true;
        }
        else if (context.canceled) // If Attack Normal is released
        {
            _isAttackNPressed = false;
        }
    }
    void onUpAir(InputAction.CallbackContext context)
    {
        if (context.started) // If Attack Normal is pressed
        {
            _upPressed = true;
        }
        else if (context.canceled) // If Attack Normal is released
        {
            _upPressed = false;
        }
    }
    void handleRotation()
    {
        // Check if there is movement input to determine rotation direction
        if (_currentMovementInput.x > 0) // Moving right
        {
            currentYRotation = 90f; // Snap to 90 degrees
        }
        else if (_currentMovementInput.x < 0) // Moving left
        {
            currentYRotation = -90f; // Snap to -90 degrees
        }

        // Create a target rotation based on the current Y angle
        Quaternion targetRotation = Quaternion.Euler(0f, currentYRotation, 0f);

        // Smoothly interpolate to the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationFactorPerFrame);
    }

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
        // connected device
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
        // disconnected device
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Disconnected:
                Debug.Log("Device Disconnected::" + device.name);
                break;
            case InputDeviceChange.Reconnected:
                Debug.Log("Device Reconnected::" + device.name);
                break;
        }
    }
    //void changeAnimeationState(string newState)
    //{
    //    //stop the same animation from interruptting itself
    //    if (_animationState == newState) return;

    //    //play the animation
    //    _animator.Play(newState);

    //    //reassign the current state
    //    _animationState = newState;
    //}
    public void StartStateCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
    private bool GroundCheck()
    {
        // Check if the box collides with the ground and set the grounded status
        if (Physics.CheckBox(transform.position, boxSize, transform.rotation, layerMask))
        {
            //Debug.Log("Grounded");
            _jumpCount = 0;
            _grounded = true;
            return true;
        }
        else
        {
            //Debug.Log("NOT grounded");
            _grounded = false;
            return false;
        }
    }
}
