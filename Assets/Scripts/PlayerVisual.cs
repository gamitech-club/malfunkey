using UnityEngine;
using EditorAttributes;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField, Required] private Player _player;

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

    private void OnJumped()
    {
        _sfxJump.Play();
    }
}
