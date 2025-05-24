using UnityEngine;

public class ChargingStation : MonoBehaviour
{
    [SerializeField]
    private AudioSource _sfxCharge;
    private Player _player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !collision.TryGetComponent(out Player player))
            return;
        
        _player = player;
        _player.OnEnterChargingStation();
        _sfxCharge.Play();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!_player)
            return;
        
        _player.OnExitChargingStation();
    }
}
