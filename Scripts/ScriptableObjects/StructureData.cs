using UnityEngine;

[CreateAssetMenu(fileName = "StructureData", menuName = "Scriptable Objects/StructureData")]
public class StructureData : ScriptableObject
{
    public int structureID;
    public string structureName;
    public GameObject structurePrefab;
    public Sprite structureSprite;
    public bool uniqueStructure;
}
