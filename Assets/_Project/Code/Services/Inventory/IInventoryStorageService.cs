using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryStorageService
{
    public RectTransform Root { get; }
    public void Initialize(InventoryContent productCatalog, RectTransform root);
    public IEnumerable<InventoryStorageItem> Items { get; }
    public int CountOf(InventoryItem item);
    public void Add(InventoryItem item, int amount = 1);
    public bool Remove(InventoryItem item, int amount = 1);
    public void Clear();
    public event Action OnChanged;
}
