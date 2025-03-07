using System;
using System.Collections.Generic;

public interface IInventoryStorageService
{
    public void Initialize(InventoryContent productCatalog);
    public IEnumerable<InventoryStorageItem> Items { get; }
    public int CountOf(InventoryItem item);
    public void Add(InventoryItem item, int amount = 1);
    public bool Remove(InventoryItem item, int amount = 1);
    public void Clear();
    public event Action OnChanged;
}
