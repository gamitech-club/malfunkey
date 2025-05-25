using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using EditorAttributes;

public class Player : MonoBehaviour
{
    [SerializeField, Required] private Rigidbody2D _rb;

    [Header("Ground check")]
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private Bounds _groundCheckBounds;

    [Header("Player")]
    [SerializeField] private float _maxSpeed = 6f;
    [SerializeField] private float _acceleration = 35f;
    [SerializeField] private float _deceleration = 60f;
    [SerializeField] private float _airAccelerationMult = 1f;
    [SerializeField] private float _airDecelerationMult = 1f;
    [SerializeField] private float _velPower = 1.5f;
    [SerializeField] private float _jumpPower = 6f;
    [SerializeField] private float _maxFallSpeed = 15f;

    [Header("Limiters")]
    [HelpBox("Set to -1 for no limit", drawAbove: true)]
    [SerializeField] private int _leftKeyLimit = -1;
    [SerializeField] private int _rightKeyLimit = -1;
    [SerializeField] private int _jumpLimit = -1;
    [SerializeField] private int _poundLimit = -1;

    public event Action Jumped;
    public event Action Landed;

    public Rigidbody2D Rigidbody => _rb;
    public Vector2 MoveInput => _moveInput;
    public bool IsGrounded => _isGrounded;
    public bool IsPounding => _isPounding;
    public int LeftKeyAvailable => _leftKeyAvailable;
    public int RightKeyAvailable => _rightKeyAvailable;
    public int JumpAvailable => _jumpAvailable;
    public int PoundAvailable => _poundAvailable;
    public bool IsJumpKeyPressed => _jumpAction.IsPressed();
    public bool IsPoundKeyPressed => _poundAction.IsPressed();

    // Input
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _poundAction;
    private Vector2 _moveInput;

    private bool _isGrounded;
    private bool _isInCharging;
    private bool _isPounding;
    private int _leftKeyAvailable;
    private int _rightKeyAvailable;
    private int _jumpAvailable;
    private int _poundAvailable;

    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _poundAction = InputSystem.actions.FindAction("Pound");

        _leftKeyAvailable = _leftKeyLimit;
        _rightKeyAvailable = _rightKeyLimit;
        _jumpAvailable = _jumpLimit;
        _poundAvailable = _poundLimit;

        Assert.IsNotNull(_rb, $"[{name}] Rigidbody not assigned");
        Assert.IsNotNull(_moveAction, $"[{name}] 'Move' action not found");
        Assert.IsNotNull(_jumpAction, $"[{name}] 'Jump' action not found");
    }

    private void Update()
    {
        HandleInput();
        HandleGroundCheck();
        HandleMaxFallSpeed();

        #if UNITY_EDITOR
        if (Keyboard.current.backslashKey.wasPressedThisFrame)
        {
            var cursorPos = Camera2D.Current.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _rb.linearVelocity = Vector2.zero;
            transform.position = cursorPos;
        }
        #endif
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw ground check
        Gizmos.DrawWireCube(transform.position + _groundCheckBounds.center, _groundCheckBounds.size);
    }

    private void OnGUI()
    {
        #if UNITY_EDITOR
        GUI.contentColor = Color.white;
        GUILayout.Label($"Vel: {_rb.linearVelocity} | Speed: {_rb.linearVelocity.magnitude}");
        GUILayout.Label($"Move Input: {_moveInput.x} | {Mathf.Abs(_rb.linearVelocityX) > _maxSpeed}");
        GUILayout.Label($"Left: {_leftKeyAvailable} | Right: {_rightKeyAvailable}");
        GUILayout.Label($"Jump: {_jumpAvailable} | Pound: {_poundAvailable}");
        #endif
    }

    private void HandleInput()
    {
        var newMoveInput = _moveAction.ReadValue<Vector2>();
        var prevMoveInput = _moveInput;

        if (_isPounding || PauseMenu.Instance.IsPaused)
        {
            newMoveInput = Vector2.zero;
        }

        // Checking if left & right key just released
        if (newMoveInput.x == 0 && !_isInCharging)
        {
            if (prevMoveInput.x < 0 && _leftKeyAvailable > 0)
                _leftKeyAvailable--;
            if (prevMoveInput.x > 0 && _rightKeyAvailable > 0)
                _rightKeyAvailable--;
        }

        // Limiting movement
        if (newMoveInput.x < 0 && _leftKeyAvailable == 0)
            newMoveInput.x = 0;
        if (newMoveInput.x > 0 && _rightKeyAvailable == 0)
            newMoveInput.x = 0;

        // Horizontal movement
        _moveInput = newMoveInput;

        // Jumping
        if (_jumpAction.WasPerformedThisFrame() && _isGrounded)
        {
            if (_jumpAvailable == -1 || _jumpAvailable > 0)
                Jump();
        }

        // Pounding
        if (_poundAction.WasPerformedThisFrame() && !_isGrounded)
        {
            if (_poundAvailable == -1 || _poundAvailable > 0)
                Pound();
        }
    }

    private void HandleGroundCheck()
    {
        bool willLand = Physics2D.OverlapBox(transform.position + _groundCheckBounds.center, _groundCheckBounds.size, 0f, _groundLayers);
        if (willLand && !_isGrounded)
        {
            if (_isPounding)
            {
                Invoke(nameof(ResetPound), .15f);
                Camera2D.Current.AddShake(.1f, 1f, .4f);
            }

            Landed?.Invoke();
        }

        _isGrounded = willLand;
    }

    private void HandleMovement()
    {
        float targetSpeed = _moveInput.x * _maxSpeed;
        float accel = _isGrounded ? _acceleration : _acceleration * _airAccelerationMult;
        float decel = _isGrounded ? _deceleration : _deceleration * _airDecelerationMult;
        float accelDecel = Mathf.Abs(targetSpeed) > .01f ? accel : decel;

        float speedDiff = targetSpeed - _rb.linearVelocityX;

        // Applies acceleration to speed difference.
        // The raises to set power so that the acceleration increases with higher speeds (more responsive).
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelDecel, _velPower) * Mathf.Sign(speedDiff);

        // Applies force to rigidbody
        _rb.AddForceX(movement);
    }

    private void HandleMaxFallSpeed()
    {
        if (_rb.linearVelocityY < -_maxFallSpeed)
            _rb.linearVelocityY = -_maxFallSpeed;
    }

    private void Jump()
    {
        _rb.linearVelocityY = _jumpPower;
        if (_jumpAvailable > 0 && !_isInCharging)
            _jumpAvailable--;
        
        Jumped?.Invoke();
    }

    private void Pound()
    {
        _isPounding = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.linearVelocityY = -_maxFallSpeed;

        if (_poundAvailable > 0 && !_isInCharging)
            _poundAvailable--;
    }

    private void ResetPound()
    {
        _isPounding = false;
    }

    public void OnEnterChargingStation()
    {
        _isInCharging = true;
        _leftKeyAvailable = _leftKeyLimit;
        _rightKeyAvailable = _rightKeyLimit;
        _jumpAvailable = _jumpLimit;
        _poundAvailable = _poundLimit;
    }

    public void OnExitChargingStation()
    {
        _isInCharging = false;
    }
}
