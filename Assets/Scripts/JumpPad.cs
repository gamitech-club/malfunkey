using System.Collections;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float baseBouncePower = 20f;
    public float fallSpeedMultiplier = 1f;
    public float delayBeforeResetSprite = 0.2f;
    public Sprite triggeredSprite;

    private Animator animator;
    private Sprite defaultSprite;
    private bool isBouncing;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        defaultSprite = spriteRenderer.sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isBouncing) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null && rb.linearVelocity.y <= 0)
        {
        	StartCoroutine(BounceAfterStretch(rb));
        }
    }

    private IEnumerator BounceAfterStretch(Rigidbody2D rb)
    {
    	isBouncing = true;
        spriteRenderer.sprite = triggeredSprite;

    	float fallSpeed = Mathf.Abs(rb.linearVelocity.y);
    	float bouncePower = Mathf.Max(baseBouncePower, baseBouncePower + fallSpeed * fallSpeedMultiplier);
    	rb.linearVelocityY = bouncePower;

    	yield return new WaitForSeconds(delayBeforeResetSprite);
    	isBouncing = false;
        spriteRenderer.sprite = defaultSprite;
	}
}