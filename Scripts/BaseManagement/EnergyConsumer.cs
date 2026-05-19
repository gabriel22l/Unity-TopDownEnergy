using UnityEngine;

public class EnergyConsumer : MonoBehaviour, IStructure
{
    [SerializeField] protected float consumptionPerTick = 1f;
    public float  ConsumptionPerTick => consumptionPerTick;

    public bool IsConsuming { get; protected set; } = true;
    
    protected bool isActive;
    public virtual void Initialize(BaseManager baseManager)
    {
        baseManager.EnergyController.RegisterEnergyConsumer(this);
    }
    public virtual void Power(){}
    public virtual void UnPower(){}
}
