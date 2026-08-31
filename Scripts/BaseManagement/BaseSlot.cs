using UnityEngine;

public class BaseSlot : MonoBehaviour
{
    public Transform slotTransform;
    public StructureData StructureData { get; private set; }
    public bool IsEmpty => StructureData == null;

    private GameObject currentStructure;

    private void Awake()
    {
       if(slotTransform == null) slotTransform = transform;
    }
    public IStructure Build(StructureData structureData)
    {
        this.StructureData = structureData;
        currentStructure = Instantiate(structureData.structurePrefab, 
        slotTransform.position,
        Quaternion.identity, slotTransform);
        AnimateBuiltStructure(currentStructure);
        
        currentStructure.TryGetComponent(out IStructure structure);
        if (structure == null)
        {
            Debug.LogError($"No IStructure found in {currentStructure.name}");
            return null;
        }
        return structure;
    }
    private void AnimateBuiltStructure(GameObject structure)
    {
        if(!structure.TryGetComponent(out Animator animator)) return;
        animator.SetTrigger("Built");
    }
}
