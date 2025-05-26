using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Audio;
using EditorAttributes;

[System.Serializable]
public struct MusicInfo
{
    public AudioResource Resource;
    [Range(0f, 1f)] public float Volume;

    public MusicInfo(AudioResource resource, float volume = 1f)
    {
        Resource = resource;
        Volume = volume;
    }

    public static readonly MusicInfo Default = new(null, 1f);
    public static readonly MusicInfo Empty = new(null, 0f);
}

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    #region Singleton
    private static MusicManager _instance;
    public static MusicManager Instance {
        get {
            if (_instance == null)
                _instance = FindFirstObjectByType<MusicManager>();
            return _instance;
        }
    }
    #endregion

    [SerializeField, Required] private AudioSource _source;

    public MusicInfo CurrentMusic => _currentMusic;
    public float PlaybackTime { get => _source.time; set => _source.time = value; }

    private MusicInfo _currentMusic;
    private Sequence _sequence;

    void Awake()
    {
        // If an instance already exists, destroy the new one
        if (_instance != null && _instance != this)
        {
            // Debug.LogWarning($"Multiple instances of {nameof(MusicManager)} found. Destroying the new one.");
            Destroy(gameObject);
            return;
        }


        _instance = this;
        _source = GetComponent<AudioSource>();

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void BeginChangeMusic(MusicInfo newMusic, float duration = 1f)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        if (_source.resource)
            _sequence.Append(_source.DOFade(0f, duration * .5f));

        _sequence.AppendCallback(() => {
                _source.resource = newMusic.Resource;
                _source.Play();
            })
            .Append(_source.DOFade(newMusic.Volume, duration * .5f));

        _currentMusic = newMusic;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PauseMenu.Instance)
            PauseMenu.Instance.PauseStateChanged += OnPauseChanged;
    }

    private void OnPauseChanged()
    {
        var targetPitch = PauseMenu.Instance.IsPaused ? 0f : 1f;
        _source.pitch = targetPitch;
    }
}
