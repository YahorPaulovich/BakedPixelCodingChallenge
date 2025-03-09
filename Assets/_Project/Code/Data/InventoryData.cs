using System.Collections.Generic;
using MemoryPack;

[MemoryPackable]
public partial class InventoryData
{
    public List<InventoryStorageItem> Items { get; set; }

    public InventoryData()
    {
        Items = new List<InventoryStorageItem>();
    }
}
