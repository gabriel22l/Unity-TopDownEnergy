using UnityEngine;
using System.Collections;

public class PickUpHandler :  MonoBehaviour
{
    private Coroutine PickUpCoroutine;

    public void PlayPickUpAndDestroy(Transform target)
    {
        if(PickUpCoroutine != null)  return;
        StartCoroutine(PlayAndDestroy(target));
    }
    private IEnumerator PlayAndDestroy(Transform target, float duration = 0.2f)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        float timer = 0;
        Vector3 startPosition = transform.position;
        Vector3 startScale = transform.localScale;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.position = Vector3.Lerp(startPosition, target.position, t);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        Destroy(gameObject);
    }
    
}