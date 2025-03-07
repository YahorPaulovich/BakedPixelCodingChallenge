using System;

[Serializable]
public struct InventoryStorageItem
{
    public InventoryItem Item;
    public int Count;

    public InventoryStorageItem(InventoryItem item, int count)
    {
        Item = item;
        Count = count;
    }
}
