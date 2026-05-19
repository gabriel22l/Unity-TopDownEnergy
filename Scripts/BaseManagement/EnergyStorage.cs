using UnityEngine;

public class EnergyStorage : MonoBehaviour, IStructure
{
    public float storageAmount = 50f;
    public void Initialize(BaseManager baseManager)
    {
        baseManager.EnergyController.RegisterEnergyStorage(this);
    }
}
