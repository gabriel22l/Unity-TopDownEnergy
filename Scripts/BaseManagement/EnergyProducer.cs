using UnityEngine;

public class EnergyProducer : MonoBehaviour, IStructure
{
    [SerializeField] protected float energyPerTick = 1f;
    [SerializeField] protected float energyStorage = 10f;
    public float EnergyStorage => energyStorage;

    public virtual void Initialize(BaseManager baseManager)
    {
        baseManager.EnergyController.RegisterEnergyProducer(this);
    }
    public virtual float ProduceEnergy()
    {
        return energyPerTick;
    }
}