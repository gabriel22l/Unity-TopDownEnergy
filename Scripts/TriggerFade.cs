using System;
using UnityEngine;

public class TriggerFade : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float alphaMinimumValue = 0.4f;
    [SerializeField] private float alphaMaximumValue = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    private bool shouldFade;
    private float currentValue;

    private void Start()
    {
        currentValue = spriteRenderer.color.a;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            shouldFade = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            shouldFade = false;
        }
    }
    private void Update()
    {   
        float target = shouldFade ? alphaMinimumValue : alphaMaximumValue;
        //minvalue = 0.3, maxvalue = 1; amounttomove = 0.7; speed = 0.7/duration seconds; 0.7/second
        currentValue = Mathf.MoveTowards(currentValue, target, ((alphaMaximumValue - alphaMinimumValue) / fadeDuration) * Time.deltaTime);
        Color newColor = spriteRenderer.color; //cannot modify alpha value directly
        newColor.a = currentValue;
        spriteRenderer.color = newColor;
    }
}
