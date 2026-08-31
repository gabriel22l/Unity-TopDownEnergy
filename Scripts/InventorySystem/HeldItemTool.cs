using UnityEngine;
public class HeldItemTool : HeldItem
{
    private float swingAngle = 90f;
    private float swingBackDuration = 0.1f;
    private float swingForwardDuration = 0.1f;
    private float returnIdleDuration = 0.2f;

    private Coroutine swingCoroutine;
    private bool isSwinging = false;
    
    [SerializeField] private DamageHandler damageHandler;
    public override void PrimaryAction(ItemActionContext ctx)
    {
        if (isSwinging) return;
        if (swingCoroutine == null)
        {
            swingCoroutine = StartCoroutine(SwingAnimation());
        }
    }
    public override void CancelAction(ItemActionContext ctx)
    {
        StopAllCoroutines();
    }

    #region Animation
    private System.Collections.IEnumerator SwingAnimation()
    {
        isSwinging = true;

        yield return RotateTo(swingAngle, swingBackDuration);
        yield return ActiveSwing();
        yield return RotateTo(0f, returnIdleDuration);

        swingCoroutine = null;
        isSwinging = false;
    }
    private System.Collections.IEnumerator RotateTo(float targetZ, float duration)
    {
        float startZ = transform.localEulerAngles.z;
        if (startZ > 180f) startZ -= 360f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            
            float z = Mathf.Lerp(startZ, targetZ, t / duration);
            
            transform.localEulerAngles = new Vector3(0f, 0f, z);
            yield return null;
        }
        transform.localEulerAngles = new Vector3(0f, 0f, targetZ);
    }
    private System.Collections.IEnumerator ActiveSwing()
    {
        //enable damage dealer
        damageHandler?.EnableHitCollider();
        yield return RotateTo(-swingAngle, swingForwardDuration);
        damageHandler?.DisableHitCollider();
    }
    #endregion
}