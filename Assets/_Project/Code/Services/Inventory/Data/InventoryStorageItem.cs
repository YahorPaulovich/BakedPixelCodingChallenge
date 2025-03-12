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

    public override string ToString()
    {
        if (Item == null)
        {
            return $"InventoryStorageItem: [Item is null, Count = {Count}]";
        }

        return $"InventoryStorageItem: [Item: {Item}, Count: {Count}]";
    }
}
