using UnityEngine;

[CreateAssetMenu(fileName = "StructureDataSO", menuName = "Scriptable Objects/StructureDataSO")]
public class StructureDataSO : ScriptableObject
{
    public int structureID;
    public string structureName;
    public GameObject structurePrefab;
    public Sprite structureSprite;
    public bool uniqueStructure;
}
