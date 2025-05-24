using UnityEngine;
using UnityEngine.UI;

public class KeyImage : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _filledSprite;

    private Sprite _defaultSprite;

    void Awake()
    {
        _defaultSprite = _image.sprite;
    }

    public void SetImageFill(bool fill)
    {
        _image.sprite = fill ? _filledSprite : _defaultSprite;
    }
}
