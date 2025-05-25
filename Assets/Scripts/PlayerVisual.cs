using UnityEngine;
using EditorAttributes;

public class PlayerVisual : MonoBehaviour
{
    private enum AnimState
    {
        Idle,
        Walk,
        Jump,
        PoundFall,
        PoundStandup
    }
    
    [SerializeField, Required] private Player _player;
    [SerializeField, Required] private Animator _animator;
    [SerializeField, Required] private SpriteRenderer _sprite;

    [Header("SFXs")]
    [SerializeField] private AudioSource _sfxJump;
    [SerializeField] private AudioSource _sfxLand;
    [SerializeField] private AudioSource _sfxPound;
    [SerializeField] private AudioSource _sfxPoundLand;

    private AnimState _state;

    private void Start()
    {
        SetAnimationState(AnimState.Idle);
    }

    private void OnEnable()
    {
        _player.Jumped += OnJumped;
        _player.Landed += OnLanded;
        _player.Pounded += OnPounded;
    }

    private void OnDisable()
    {
        _player.Jumped -= OnJumped;
        _player.Landed -= OnLanded;
        _player.Pounded -= OnPounded;
    }

    private void Update()
    {
        HandleAnimation();
        HandleSpriteFlipping();
    }

    private void HandleAnimation()
    {
        if (_player.IsPounding)
        {
            if (!_player.IsGrounded)
                SetAnimationState(AnimState.PoundFall);
            else
                SetAnimationState(AnimState.PoundStandup);

            return;
        }

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
            case AnimState.PoundFall:
                _animator.Play("PlayerPoundFall");
                break;
            case AnimState.PoundStandup:
                _animator.Play("PlayerPoundStandup");
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
        if (_player.IsPounding)
            _sfxPoundLand.Play();
        else
            _sfxLand.Play();
    }

    private void OnPounded()
    {
        _sfxPound.Play();
    }
}
