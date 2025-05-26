using UnityEngine;
using EasyTransition;

public class BackgroundMusic : MonoBehaviour
{
    #region Singleton
    private static BackgroundMusic _instance;
    public static BackgroundMusic Instance {
        get {
            if (_instance == null)
                _instance = FindFirstObjectByType<BackgroundMusic>();
            return _instance;
        }
    }
    #endregion

    [SerializeField] private MusicInfo _music = MusicInfo.Default;
    [SerializeField] private float _transitionDuration = 1f;

    public MusicInfo Music => _music;

    private void Awake()
    {
        // If an instance already exists, destroy the new one
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"Multiple instances of {nameof(BackgroundMusic)} found. Destroying the new one.");
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Only change if the music is different
        var manager = MusicManager.Instance;
        if (_music.Resource != manager.CurrentMusic.Resource)
        {
            manager.BeginChangeMusic(_music, _transitionDuration);
        }
    }
}
