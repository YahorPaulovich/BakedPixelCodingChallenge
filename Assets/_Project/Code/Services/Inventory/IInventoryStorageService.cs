using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryStorageService
{
    public int TotalSlots { get; set; }
    public int UnlockedSlots { get; set; }
    public RectTransform Root { get; }
    public InitialInventoryItem[] Content { get; }
    public void Initialize(InventoryContent productCatalog, RectTransform root);
    public IEnumerable<InventoryStorageItem> Items { get; }
    public bool ContainsItem(InventoryItem item);
    public void UpdateItemCount(InventoryItem item, int value = 1);
    public int CountOf(InventoryItem item);
    public void Add(InventoryItem item, int amount = 1);
    public bool Remove(InventoryItem item, int amount = 1);
    public void Clear();
    public event Action OnChanged;
}
