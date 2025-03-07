using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public sealed class InventoryStorageService : IInventoryStorageService
{
    public InventoryStorageService() { }
    public InventoryStorageService(InventoryContent productCatalog)
    {
        Initialize(productCatalog);
    }

    public void Initialize(InventoryContent productCatalog)
    {
        var content = productCatalog.Content;

        if (content == null)
        {
            return;
        }

        foreach (var item in content)
        {
            if (item.Count <= 0)
            {
                Debug.LogError($"Invalid count {item.Count} in inventory of '{this}'.");
            }
            else
            {
                Add(item.Item, item.Count);
            }
        }
    }
    
    private sealed class Comparer : IComparer<InventoryItem>
    {
        public static readonly Comparer Instance = new Comparer();
        public int Compare(InventoryItem a, InventoryItem b)
        {
            if (a.LessThan(b))
            {
                return -1;
            }
            if (b.LessThan(a))
            {
                return 1;
            }

            int iidA = a.GetInstanceID();
            int iidB = b.GetInstanceID();

            if (iidA < iidB)
            {
                return -1;
            }
            if (iidA > iidB)
            {
                return 1;
            }

            return 0;
        }
    }

    private readonly SortedDictionary<InventoryItem, int> _items = new SortedDictionary<InventoryItem, int>(Comparer.Instance);

    public int Count => _items.Count;

    public IEnumerable<InventoryStorageItem> Items
    {
        get
        {
            foreach (var item in _items)
            {
                yield return new InventoryStorageItem(item.Key, item.Value);
            }
        }
    }

    public int CountOf(InventoryItem item)
    {
        _items.TryGetValue(item, out int count);
        return count;
    }

public void Add(InventoryItem item, int amount)
{
    if (amount <= 0)
    {
        Debug.LogError($"Attempted to add {amount} of '{item.Name}' into the inventory.");
        return;
    }

    if (item.MaxStackSize > 1)
    {
        if (_items.TryGetValue(item, out int count))
        {
            int spaceLeft = item.MaxStackSize - count;
            int amountToAdd = math.min(amount, spaceLeft);

            if (amountToAdd > 0)
            {
                _items[item] = count + amountToAdd;
                amount -= amountToAdd;
            }

            if (amount > 0)
            {
                _items[item] = amount;
            }
        }
        else
        {
            _items[item] = math.min(amount, item.MaxStackSize);
            amount -= _items[item];

            if (amount > 0)
            {
                _items[item] = amount;
            }
        }
    }
    else
    {
        for (int i = 0; i < amount; i++)
        {
            _items[item] = 1;
        }
    }

    OnChanged?.Invoke();
}

    public bool Remove(InventoryItem item, int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError($"Attemped to remove {amount} of '{item.Name}' from the inventory.");
            return false;
        }

        if (!_items.TryGetValue(item, out int count) || count < amount)
        {
            return false;
        }

        count -= amount;
        if (count > 0)
        {
            _items[item] = count;
        }
        else
        {
            if (!_items.Remove(item))
            {
                return false;
            }
        }

        OnChanged?.Invoke();
        return true;
    }

    public void Clear()
    {
        _items.Clear();
        OnChanged?.Invoke();
    }

    public event Action OnChanged;
}
