using System.Collections.Generic;
using UnityEngine;

public class BreakableGlass : MonoBehaviour
{
    public static readonly List<BreakableGlass> Instances = new();

    [SerializeField] private AudioSource _sfxJumpPad;
    [SerializeField] private ParticleSystem _breakParticles;
    private BoxCollider2D _collider;
    private bool _isTriggerMode;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        Instances.Add(this);
    }

    private void OnDisable()
    {
        Instances.Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTriggerMode &&
            other.CompareTag("Player") &&
            other.TryGetComponent(out Player player) &&
            player.IsPounding){

            _sfxJumpPad?.Play();

            var particles = Instantiate(_breakParticles, transform.position, Quaternion.identity);
            particles.Play();
            Destroy(particles.gameObject, particles.main.duration + particles.main.startLifetime.constantMax);
            
            Destroy(gameObject);
        }
    }
    public void SetTriggerMode(bool active)
    {
        _isTriggerMode = active;
        _collider.isTrigger = active;
    }
}