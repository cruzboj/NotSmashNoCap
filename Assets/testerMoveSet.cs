using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class testerMoveSet : MonoBehaviour
{
    //intis
    private Rigidbody _rb;
    private PlayerInput _playerInput;
    private Animator _animator;

    //gravity
    public bool _grounded;
    public float _downForce = 9.8f;
    public float _groundCheckDistance = 0.5f;

    //movement
    public bool _isMovementPressed;
    Vector2 _currentMovementInput;
    Vector2 _currentMovement;
    public float _moveSpeed = 5f;
    public float _rotationFactorPerFrame = 25.0f;
    //new movement
    public float _walkSpeed = 3f;
    public float _runSpeed = 6f;
    private float _currentSpeed;
    public float _threshold = 0.8f;


    //jump
    private bool _isJumpPressed = false; // JumpPressed
    private int _jumpCount = 0; // jump counter
    private int _maxJumpCount = 2; // max Jumps allowed
    public float _jumpForce = 12f;

    // Animator Hashes
    private int _isWalkingHash;
    private int _isJumpingHash;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        // Animator parameter hashes
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isJumpingHash = Animator.StringToHash("isJumping");

        //set the player input callbacks
        _playerInput.CharacterControls.Move.started += onMovementInput;
        _playerInput.CharacterControls.Move.performed += onMovementInput;
        _playerInput.CharacterControls.Move.canceled += onMovementInput;
        _playerInput.CharacterControls.Jump.started += onJump;

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
        handleRotation();
        checkGroundedStatus();
        handleJump();
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

    void handleJump()
    {
        if (_isJumpPressed && _jumpCount < _maxJumpCount)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpForce); // jump force
            _jumpCount++;
            _isJumpPressed = false; //allow 1 jump at a time 
        }
    }

    void checkGroundedStatus()
    {
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position + Vector3.down * 0.5f; // קרוב יותר לקרקע

        Debug.DrawRay(raycastOrigin, Vector3.down * _groundCheckDistance, Color.red); // צייר את ה-Raycast

        _grounded = Physics.Raycast(raycastOrigin, Vector3.down, out hit, _groundCheckDistance);

        if (_grounded)
        {
            Debug.Log("Grounded: Raycast hit " + hit.collider.name);
            _jumpCount = 0;
        }
        else
        {
            Debug.Log("Not grounded");
        }
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        //change the position the character should point to
        positionToLookAt.x = _currentMovement.x * -1;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = 0;

        //the current rotation of our character
        Quaternion currentRotation = transform.rotation;

        if (_isMovementPressed)
        {
            //create a new rotation based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);

            //rotate the character to face the positionToLookAt
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * _rotationFactorPerFrame); //will take more frames to complite the rotation

            //transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame); //old
        }
    }

    void handleAnimation()
    {
        bool isWalking = _animator.GetBool(_isWalkingHash);
        bool isRunning = _animator.GetBool(_isJumpingHash);
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
