using System.Collections.Generic;
using UnityEngine;

public class BreakableGlass : MonoBehaviour
{
    public static readonly List<BreakableGlass> Instances = new();

    [SerializeField] private AudioSource _sfxBreak;
    [SerializeField] private ParticleSystem _fxBreak;

    private Player _plr;
    private BoxCollider2D _collider;
    private bool _isTriggerMode;

    private void Awake()
    {
        _plr = Player.Instance;
        _collider = GetComponent<BoxCollider2D>();
        
        if (_sfxBreak == null){
            _sfxBreak = gameObject.AddComponent<AudioSource>();
            _sfxBreak.clip = Resources.Load<AudioClip>("Audio/JumpPad");
            _sfxBreak.playOnAwake = false;
        }
    }

    private void OnEnable()
    {
        Instances.Add(this);
        _plr.Pounded += OnPounded;
        _plr.PoundReset += OnPoundReset;
    }

    private void OnDisable()
    {
        Instances.Remove(this);
        _plr.Pounded -= OnPounded;
        _plr.PoundReset -= OnPoundReset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTriggerMode &&
            other.CompareTag("Player") &&
            other.TryGetComponent(out Player player) &&
            player.IsPounding)
        {
            // SFX
            if (_sfxBreak && _sfxBreak.clip)
            {
                _sfxBreak.transform.SetParent(null);
                _sfxBreak.Play();
                Destroy(_sfxBreak.gameObject, _sfxBreak.clip.length);
            }

            // FX
            _fxBreak.transform.SetParent(null);
            _fxBreak.Play();

            // Cam shake
            Camera2D.Current.AddShake(.1f, 1f, .4f);

            Destroy(gameObject);
        }
    }

    private void SetTriggerMode(bool active)
    {
        _isTriggerMode = active;
        _collider.isTrigger = active;
    }

    private void OnPounded()
    {
        SetTriggerMode(true);
    }

    private void OnPoundReset()
    {
        SetTriggerMode(false);
    }
}