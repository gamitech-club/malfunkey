using UnityEngine;
using EditorAttributes;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField, Required] private Player _player;
    [SerializeField, Required] private Animator _animator;
    [SerializeField, Required] private SpriteRenderer _sprite;

    [Header("SFXs")]
    [SerializeField] private AudioSource _sfxJump;

    private void OnEnable()
    {
        _player.Jumped += OnJumped;
    }

    private void OnDisable()
    {
        _player.Jumped -= OnJumped;
    }

    private void Update()
    {
        HandleAnimation();
        HandleSpriteFlipping();
    }

    private void HandleAnimation()
    {
        _animator.Play("PlayerIdle");
    }

    private void HandleSpriteFlipping()
    {
        var moveInput = _player.MoveInput;
        if (moveInput.x < 0)
            _sprite.flipX = true;
        else if (moveInput.x > 0)
            _sprite.flipX = false;
    }

    private void OnJumped()
    {
        _sfxJump.Play();
    }
}
