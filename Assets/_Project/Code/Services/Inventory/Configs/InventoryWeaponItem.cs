using UnityEngine;

[CreateAssetMenu(fileName = "InventoryWeaponItem", menuName = "Configs/Inventory/InventoryWeaponItem", order = 0)]
public class InventoryWeaponItem : InventoryItem
{
    [field: SerializeField] public InventoryItem BulletType { get; private set; }
    [field: SerializeField, Range(0f, 10000000f)] public float DamagePerShot { get; private set; }
}
