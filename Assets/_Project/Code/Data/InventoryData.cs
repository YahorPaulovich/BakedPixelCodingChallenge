using System.Collections.Generic;
using System.Text;
using MemoryPack;

[MemoryPackable]
public partial class InventoryData
{
    public List<InventoryItemData> Items { get; set; }

    public InventoryData()
    {
        Items = new List<InventoryItemData>();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("InventoryData:");
        sb.AppendLine("Items:");

        if (Items != null && Items.Count > 0)
        {
            foreach (var item in Items)
            {
                sb.AppendLine($"  - Name: {item.Name}, Count: {item.Count}, Slot: {item.OccupiedSlotIndex}");
            }
        }
        else
        {
            sb.AppendLine("  No items");
        }

        return sb.ToString().TrimEnd();
    }
}
