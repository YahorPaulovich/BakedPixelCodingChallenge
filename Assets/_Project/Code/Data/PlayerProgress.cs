using MemoryPack;

[MemoryPackable]
public partial class PlayerProgress
{
    public InventoryData InventoryData { get; set; }

    public PlayerProgress()
    {
        InventoryData = new InventoryData();
    }

    public override string ToString()
    {
        return $"PlayerProgress: {InventoryData}";
    }
}