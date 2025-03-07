using UnityEngine;

[CreateAssetMenu(fileName = "InventoryBodyItem", menuName = "Configs/Inventory/InventoryBodyItem", order = 0)]
public class InventoryBodyItem : InventoryItem
{
    [field: SerializeField, Range(0, 10000000)] public ulong Protection { get; private set; }
}
