using UnityEngine;

[RequireComponent(typeof(Animator))]
public class JumpPad : MonoBehaviour
{
    public float baseBounceForce = 20f;
    public float velocityMultiplier = 1f;
    public float delayBeforeBounce = 0.15f;

    private Animator animator;
    private bool isBouncing = false;
    
    void Start()
    {
        animator = GetComponent<Animator>();
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

    private System.Collections.IEnumerator BounceAfterStretch(Rigidbody2D rb)
    {
    	isBouncing = true;
    	animator.SetTrigger("Stretch");

    	yield return new WaitForSeconds(delayBeforeBounce);

    	float fallSpeed = Mathf.Abs(rb.linearVelocity.y);
    	float bouncePower = baseBounceForce + (fallSpeed * velocityMultiplier);

    	rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    	rb.AddForce(Vector2.up * bouncePower, ForceMode2D.Impulse);

    	isBouncing = false;
    	
	}
}