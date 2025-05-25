using UnityEngine;
using EditorAttributes;

public class PlayerVisual : MonoBehaviour
{
    private enum AnimState
    {
        Idle,
        Walk,
        Jump
    }
    
    [SerializeField, Required] private Player _player;
    [SerializeField, Required] private Animator _animator;
    [SerializeField, Required] private SpriteRenderer _sprite;

    [Header("SFXs")]
    [SerializeField] private AudioSource _sfxJump;
    [SerializeField] private AudioSource _sfxLand;

    private AnimState _state;

    private void Start()
    {
        SetAnimationState(AnimState.Idle);
    }

    private void OnEnable()
    {
        _player.Jumped += OnJumped;
        _player.Landed += OnLanded;
    }

    private void OnDisable()
    {
        _player.Jumped -= OnJumped;
        _player.Landed -= OnLanded;
    }

    private void Update()
    {
        HandleAnimation();
        HandleSpriteFlipping();
    }

    private void HandleAnimation()
    {
        if (!_player.IsGrounded)
        {
            SetAnimationState(AnimState.Jump);
        }
        else
        {
            if (Mathf.Abs(_player.Rigidbody.linearVelocityX) < 0.5f) {
                SetAnimationState(AnimState.Idle);
            } else {
                SetAnimationState(AnimState.Walk);
            }
        }
    }

    private void HandleSpriteFlipping()
    {
        var moveInput = _player.MoveInput;
        if (moveInput.x < 0)
            _sprite.flipX = true;
        else if (moveInput.x > 0)
            _sprite.flipX = false;
    }

    private void SetAnimationState(AnimState state)
    {
        if (state == _state)
            return;
        
        switch (state)
        {
            case AnimState.Idle:
                _animator.Play("PlayerIdle");
                break;
            case AnimState.Walk:
                _animator.Play("PlayerWalk");
                break;
            case AnimState.Jump:
                _animator.Play("PlayerJump");
                break;
        }

        _state = state;
    }

    private void OnJumped()
    {
        _sfxJump.Play();
    }

    private void OnLanded()
    {
        _sfxLand.Play();
    }
}
