using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition;
using DG.Tweening;

public class Portal : MonoBehaviour
{
    [SerializeField] private TransitionSettings _transitionSettings;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !other.TryGetComponent(out Player player))
            return;
        
        var rb = other.attachedRigidbody;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        var otherTransform = other.transform;
        other.enabled = false;

        DOTween.Sequence()
            .Append(otherTransform.DOMove(transform.position, .8f))
            .Join(otherTransform.DOLocalRotate(new(0, 0, 360f), .8f, RotateMode.FastBeyond360))
            .Join(otherTransform.DOScale(Mathf.Epsilon, .8f))
            .OnComplete(GoToNextLevel)
            .SetLink(gameObject);
    }

    private void GoToNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        TransitionManager.Instance().Transition(nextScene, _transitionSettings, 0f);
    }
}
