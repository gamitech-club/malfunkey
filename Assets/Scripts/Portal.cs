using UnityEngine;
using UnityEngine.SceneManagement;
using EditorAttributes;
using EasyTransition;
using DG.Tweening;

public class Portal : MonoBehaviour
{
    [SerializeField, Required] private TransitionSettings _transitionSettings;
    [SerializeField, Required] private AudioSource _sfxEnter;

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

        _sfxEnter.Play();

        DOTween.Sequence()
            .Append(otherTransform.DOMove(transform.position, .8f))
            .Join(otherTransform.DOLocalRotate(new(0, 0, 360f), .8f, RotateMode.FastBeyond360))
            .Join(otherTransform.DOScale(Mathf.Epsilon, .8f))
            .AppendInterval(.7f)
            .OnComplete(GoToNextLevel)
            .SetLink(gameObject);
    }

    private void GoToNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

        // if last level, go back to main menu
        if (nextScene >= SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0;
        }
        
        TransitionManager.Instance().Transition(nextScene, _transitionSettings, 0f);
    }
}
