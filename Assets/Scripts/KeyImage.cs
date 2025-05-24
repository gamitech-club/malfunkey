using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class KeyImage : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _filledSprite;
    [SerializeField] private ParticleSystem _fxDisabled;

    private Sprite _defaultSprite;
    private Tween _pressTween;
    private bool _isPressed;
    private bool _isDisabled;

    void Awake()
    {
        _defaultSprite = _image.sprite;
    }

    public void SetPressed(bool pressed)
    {
        if (pressed && _isDisabled)
            return;

        if (pressed == _isPressed)
            return;
        
        _pressTween?.Kill();
        _pressTween = _image.transform.DOScale(pressed ? 1.25f : 1f, 0.2f)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject);

        _image.sprite = pressed ? _filledSprite : _defaultSprite;
        _isPressed = pressed;
    }

    public void SetDisabled(bool disabled)
    {
        if (disabled == _isDisabled)
            return;

        if (disabled)
            _fxDisabled.Play();

        _image.color = disabled ? Color.gray : Color.white;
        _isDisabled = disabled;
    }
}
