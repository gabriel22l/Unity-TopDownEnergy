using UnityEngine;
public class HeldItem: MonoBehaviour
{
    public virtual void PrimaryAction(ItemActionContext ctx ){}
    public virtual void SecondaryAction(ItemActionContext ctx ){}
    public virtual void CancelAction(ItemActionContext ctx ){}
}