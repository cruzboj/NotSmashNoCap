using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveSet : MonoBehaviour
{
    //intis
    private Rigidbody _rb;
    private PlayerInput _playerInput;
    private Animator _animator;

    //gravity
    //new 
    [SerializeField] public LayerMask layerMask;
    RaycastHit hit;
    public float rayBeam = 0.1f;
    public Vector3 boxSize = new Vector3(1f, 0.3f, 1f);
    //
    public bool _grounded = true;
    private float _downForce = 9.8f;
    //private float _groundCheckDistance = 0.5f;

    //movement
    public bool _isMovementPressed;
    Vector2 _currentMovementInput;
    Vector2 _currentMovement;
    private float _rotationFactorPerFrame = 25.0f;
    //new movement
    public float _walkSpeed = 3f;
    public float _runSpeed = 6f;
    private float _currentSpeed;
    private float _threshold = 0.8f;

    //Attack Normal
    public bool _isAttackNPressed;

    //jump
    private bool _isJumpPressed = false; // JumpPressed
    public int _jumpCount = 0; // jump counter
    private int _maxJumpCount = 1; // max Jumps allowed
    private float _jumpForce = 12f;

    // Animator Hashes
    private int _isWalkingHash;
    private int _isRunningHash;
    private int _isJumpingHash;
    private int _isAttackNHash;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

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
        _playerInput.CharacterControls.AttackN.performed += onAttackN;
        _playerInput.CharacterControls.AttackN.canceled += onAttackN;

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

    void Update()
    {
        GroundCheck();
        //handleGravity();
        handleRotation();
        //checkGroundedStatus();
        handleJump();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position - transform.up * rayBeam, boxSize);
    }
    private bool GroundCheck()
    {
        // Check if the box collides with the ground and set the grounded status
        ///if(Physics.BoxCast(transform.position, boxSize, -transform.up, out hit, transform.rotation, rayBeam, layerMask))
        if (Physics.CheckBox(transform.position, boxSize, transform.rotation, layerMask))
        {
            Debug.Log("Grounded");
            _jumpCount = 0;
            _grounded = true;
            return true;
        }
        else
        {
            Debug.Log("NOT grounded");
            _grounded = false;
            return false;
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

        // Update animator parameters based on speed
        if (_currentSpeed != 0)
        {
            _animator.SetBool(_isWalkingHash, _currentSpeed == _walkSpeed);
            _animator.SetBool(_isRunningHash, _currentSpeed == _runSpeed);
        }
        else
        {
            _animator.SetBool(_isWalkingHash, false);
            _animator.SetBool(_isRunningHash, false);
        }
    }

    void onJump(InputAction.CallbackContext context)
    {
        if (context.started) //if jump is pressed
        {
            _isJumpPressed = true;
        }
    }
    void handleJump()
    {
        if (_grounded && _isJumpPressed || _isJumpPressed && (_jumpCount < _maxJumpCount))
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpForce); // Apply jump force
            _jumpCount++;
            //Debug.Log("Jumped! Jump Count: " + _jumpCount);
            _isJumpPressed = false;
            _animator.SetBool(_isJumpingHash, true);
        }
        else if (!GroundCheck() && _jumpCount >= _maxJumpCount)
        {
            //Debug.Log("Cannot jump. Already at max jumps.");
        }
        else
        {
            _animator.SetBool(_isJumpingHash, false);
        }
    }
    void onAttackN(InputAction.CallbackContext context)
    {
        if (context.started) // If Attack Normal is pressed
        {
            _isAttackNPressed = true;
            Debug.Log("AttackN started");
        }
        else if (context.canceled) // If Attack Normal is released
        {
            _isAttackNPressed = false;
            Debug.Log("AttackN canceled");
        }
    }

    void handleAttackN()
    {
        if (_isAttackNPressed)
        {
            _animator.SetBool(_isAttackNHash, true);
        }
        else
        {
            _animator.SetBool(_isAttackNHash, false);
        }
    }

    void handleRotation()
    {
        //Debug.Log("F(Entered) :: handle Rotation");
        // Use the current movement input to determine the direction to look at
        Vector3 positionToLookAt;
        positionToLookAt.x = _currentMovementInput.x;
        positionToLookAt.y = 0.0f; // Ensure the rotation stays horizontal
        positionToLookAt.z = 0;

        // Only rotate if the player is pressing the movement keys
        if (_isMovementPressed)
        {
            //Debug.Log("F(Entered) :: is Rotation");
            // Calculate the new rotation direction
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);

            // Apply the rotation smoothly
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationFactorPerFrame);
        }
    }

    void handleAnimation()
    {
        bool isWalking = _animator.GetBool(_isWalkingHash);

        bool isRunning = _animator.GetBool(_isRunningHash);

        bool isJumping = _animator.GetBool(_isJumpingHash);

        bool isAttackN = _animator.GetBool(_isAttackNHash);

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
}
