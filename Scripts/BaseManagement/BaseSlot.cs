using UnityEngine;

public class BaseSlot : MonoBehaviour
{
    public Transform slotTransform;
    public StructureDataSO StructureData { get; private set; }
    [SerializeField] private float groundYOffset = 0.5f;
    public bool IsEmpty => StructureData == null;

    private GameObject currentStructure;

    private void Awake()
    {
       if(slotTransform == null) slotTransform = transform;
    }
    public IStructure Build(StructureDataSO structureData)
    {
        this.StructureData = structureData;
        currentStructure = Instantiate(structureData.structurePrefab, 
        new Vector3(slotTransform.position.x, slotTransform.position.y - groundYOffset, slotTransform.position.z), 
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
