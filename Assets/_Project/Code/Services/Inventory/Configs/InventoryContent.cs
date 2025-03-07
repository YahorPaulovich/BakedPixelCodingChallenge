using UnityEngine;

[CreateAssetMenu(fileName = "InventoryContent", menuName = "Configs/Inventory/InventoryContent", order = 2)]
public sealed class InventoryContent : ScriptableObject
{
    [field: SerializeField] public InitialInventoryItem[] Content { get; set; }
}
