using UnityEngine;
using System.Collections.Generic;

public class PlayerContext : MonoBehaviour
{
    //PlayerContext class provides references to external systems to player systems
    public UIController UiController { get; private set; } //UIController wired to this player instance
    [field:SerializeField] public InventoryController InventoryController { get; private set; }
    [field:SerializeField] public PlayerInput PlayerInput { get; private set; }
    public List<StructureRecipeSO> structureRecipes;
    
    public void Initialize(UIController uiController)
    {
        this.UiController = uiController;
    }
}