using System.Collections;
using DG.Tweening;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite triggeredSprite;
    public AudioSource _sfxJumpPad;
    public float baseBouncePower = 20f;
    public float fallSpeedMultiplier = 1f;
    public float delayBeforeResetSprite = 0.2f;

    private Animator animator;
    private Sprite defaultSprite;
    private bool isBouncing;
    private Tween bounceTween;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        defaultSprite = spriteRenderer.sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isBouncing)
            return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (!rb) return;

        StartCoroutine(BounceCoroutine(rb));
    }

    private IEnumerator BounceCoroutine(Rigidbody2D rb)
    {
    	isBouncing = true;
        spriteRenderer.sprite = triggeredSprite;
        _sfxJumpPad.Play();

        bounceTween?.Complete(true);
        bounceTween = spriteRenderer.transform.DOPunchScale(Vector3.one * .25f, 0.2f, 7)
            .SetLink(spriteRenderer.gameObject);

    	float fallSpeed = Mathf.Abs(Mathf.Min(0, rb.linearVelocity.y));
    	float bouncePower = Mathf.Max(baseBouncePower, baseBouncePower + fallSpeed * fallSpeedMultiplier);
    	rb.linearVelocityY = bouncePower;

    	yield return new WaitForSeconds(delayBeforeResetSprite);
    	isBouncing = false;
        spriteRenderer.sprite = defaultSprite;
	}
}