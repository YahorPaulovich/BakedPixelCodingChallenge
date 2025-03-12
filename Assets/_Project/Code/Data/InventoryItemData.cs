using MemoryPack;

[MemoryPackable]
public partial class InventoryItemData
{
    public string ItemID { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public int OccupiedSlotIndex { get; set; }

    public override string ToString()
    {
        return $"Name: {Name}, Count: {Count}, Slot: {OccupiedSlotIndex}";
    }
}
