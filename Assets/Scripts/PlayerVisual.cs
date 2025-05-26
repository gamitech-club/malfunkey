using UnityEngine;
using EditorAttributes;
using DG.Tweening;

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

    [Header("FXs")]
    [SerializeField] private ParticleSystem _fxWalk;
    [SerializeField] private ParticleSystem _fxJump;
    [SerializeField] private ParticleSystem _fxLand;

    [Header("SFXs")]
    [SerializeField] private AudioSource _sfxWalk;
    [SerializeField] private AudioSource _sfxJump;
    [SerializeField] private AudioSource _sfxLand;
    [SerializeField] private AudioSource _sfxFall;
    [SerializeField] private AudioSource _sfxPound;
    [SerializeField] private AudioSource _sfxPoundLand;

    private Rigidbody2D _rb;
    private AnimState _state;
    private Tween _spriteBounceTween;
    private bool _isFalling;

    private void Awake()
    {
        _rb = _player.Rigidbody;
        _sfxWalk.Play();
    }

    private void Start()
    {
        SetAnimationState(AnimState.Idle);
    }

    private void OnEnable()
    {
        _player.Jumped += OnJumped;
        _player.Landed += OnLanded;
        _player.Pounded += OnPounded;

        if (PauseMenu.Instance)
            PauseMenu.Instance.PauseStateChanged += OnPauseStateChanged;
    }

    private void OnDisable()
    {
        _player.Jumped -= OnJumped;
        _player.Landed -= OnLanded;
        _player.Pounded -= OnPounded;

        if (PauseMenu.Instance)
            PauseMenu.Instance.PauseStateChanged -= OnPauseStateChanged;
    }

    private void Update()
    {
        HandleAnimation();
        HandleSpriteFlipping();
        HandleWalkSFX();
        HandleFallSFX();
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
            bool isIdle = _player.IsOnConveyor
                ? Mathf.Abs(_player.MoveInput.x) < 0.1f
                : Mathf.Abs(_rb.linearVelocityX) < 0.5f;

            if (isIdle) {
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

    private void HandleWalkSFX()
    {
        float volume = Mathf.InverseLerp(0, _player.MaxSpeed, Mathf.Abs(_rb.linearVelocityX));
        _sfxWalk.volume = volume;
    }

    private void HandleFallSFX()
    {
        var falling = _rb.linearVelocityY < -0.1f;
        if (falling && !_isFalling)
        {
            _sfxFall.Play();
        }

        _isFalling = falling;
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

        if (state == AnimState.Walk)
        {
            _sfxWalk.Play();
            _fxWalk.Play();
        }
        else
        {
            _sfxWalk.Stop();
            _fxWalk.Stop();
        }

        _state = state;
    }

    private void OnJumped()
    {
        _sfxJump.Play();
        _fxJump.Play();
        _spriteBounceTween?.Complete(true);
        _spriteBounceTween = _sprite.transform.DOPunchScale(Vector3.one * .25f, 0.2f, 7);
    }

    private void OnLanded()
    {
        if (_player.IsPounding)
        {
            _sfxPoundLand.Play();
        }
        else
        {
            _sfxLand.Play();
        }

        _fxLand.Play();
        _spriteBounceTween?.Complete(true);
        _spriteBounceTween = _sprite.transform.DOPunchScale(-Vector3.one * .25f, 0.2f, 7);
    }

    private void OnPounded()
    {
        _sfxPound.Play();
    }

    private void OnPauseStateChanged()
    {
        if (PauseMenu.Instance.IsPaused)
        {
            _sfxWalk.Pause();
        }
        else
        {
            _sfxWalk.UnPause();
        }
    }
}
