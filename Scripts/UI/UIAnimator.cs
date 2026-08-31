using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimator : MonoBehaviour
{
    [SerializeField] private float duration = 0.1f;
    [SerializeField] private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private void Awake() => canvasGroup = GetComponent<CanvasGroup>();

    public void FadeIn()  => Fade(1f);
    public void FadeOut() => Fade(0f);

    // Starts the AnimateAlpha Coroutine to the target alpha.
    // If the GameObject is inactive, it will stop the fade and reset the coroutine
    private void Fade(float target)
    {
        if (canvasGroup == null)
        {
            Debug.LogWarning("null CanvasGroup at the time Fade is called");
            return;
        }
        if (!canvasGroup.gameObject.activeInHierarchy)
        {
            fadeCoroutine = null;
            StopAllCoroutines();
            return;
        }
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(AnimateAlpha(canvasGroup.alpha, target));
    }

    // Coroutine that animates the alpha of the CanvasGroup from startAlpha to targetAlpha over the duration.
    private IEnumerator AnimateAlpha(float startAlpha, float targetAlpha)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        fadeCoroutine = null;
    }
}