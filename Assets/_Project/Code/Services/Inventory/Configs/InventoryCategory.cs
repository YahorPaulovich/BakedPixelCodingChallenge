using UnityEngine;

[CreateAssetMenu(fileName = "InventoryCategory", menuName = "Configs/Inventory/InventoryCategory", order = 1)]
public class InventoryCategory : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
}
