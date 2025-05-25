using UnityEngine;
using EditorAttributes;
using DG.Tweening;

public class ChargingStation : MonoBehaviour
{
    [SerializeField, Required] private SpriteRenderer _sprite;
    [SerializeField, Required] private AudioSource _sfxCharge;

    private Player _player;
    private Tween _enterTween;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !collision.TryGetComponent(out Player player))
            return;
        
        _player = player;
        _player.OnEnterChargingStation();
        _sfxCharge.Play();

        _enterTween?.Complete(true);
        _enterTween = _sprite.transform.DOPunchScale(Vector3.one * .25f, 0.2f, 7)
            .SetLink(gameObject);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!_player)
            return;
        
        _player.OnExitChargingStation();
    }
}
