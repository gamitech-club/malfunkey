using UnityEngine;
using UnityEngine.InputSystem;
using EditorAttributes;
using UnityEngine.Assertions;

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
    [SerializeField] private float _velPower = 1.5f;
    [SerializeField] private float _jumpPower = 6f;
    [SerializeField] private float _maxFallSpeed = 15f;

    private bool _isGrounded;

    // Input
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private Vector2 _moveInput;

    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");

        Assert.IsNotNull(_rb, $"[{name}] Rigidbody not assigned");
        Assert.IsNotNull(_moveAction, $"[{name}] 'Move' action not found");
        Assert.IsNotNull(_jumpAction, $"[{name}] 'Jump' action not found");
    }

    private void Update()
    {
        HandleInput();
        HandleGroundCheck();
        HandleMaxFallSpeed();
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
        GUI.contentColor = Color.white;
        GUILayout.Label($"Vel: {_rb.linearVelocity} | Speed: {_rb.linearVelocity.magnitude}");
        GUILayout.Label($"Move Input: {_moveInput.x} | {Mathf.Abs(_rb.linearVelocityX) > _maxSpeed}");
    }

    private void HandleInput()
    {
        var newMoveInput = _moveAction.ReadValue<Vector2>();
        var prevMoveInput = _moveInput;

        // Jumping
        if (_isGrounded && _jumpAction.WasPerformedThisFrame())
            _rb.linearVelocityY = _jumpPower * -Mathf.Sign(Physics2D.gravity.y);

        // Checking if left & right key just released
        if (newMoveInput.x == 0)
        {
            if (prevMoveInput.x < 0)
            {
                print("Left key released");
            }
            else if (prevMoveInput.x > 0)
            {
                print("Right key released");
            }
        }

        _moveInput = newMoveInput;
    }

    private void HandleGroundCheck()
    {
        bool willLand = Physics2D.OverlapBox(transform.position + _groundCheckBounds.center, _groundCheckBounds.size, 0f, _groundLayers);
        _isGrounded = willLand;
    }

    private void HandleMovement()
    {
        float targetSpeed = _moveInput.x * _maxSpeed;
        float speedDiff = targetSpeed - _rb.linearVelocityX;
        float accelDecel = Mathf.Abs(targetSpeed) > .01f ? _acceleration : _deceleration;

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
}
