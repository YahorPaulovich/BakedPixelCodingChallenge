using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class InventoryView : MonoBehaviour
{
    [SerializeField]
    private Transform _content;
    private readonly List<InventoryItemView> _itemViews = new List<InventoryItemView>();
    [Inject] private InventoryItemView.Factory _itemFactory = default;
    [Inject] private IInventoryStorageService _inventoryStorage;

    [SerializeField] private int _totalSlots = 30;

    private void OnEnable()
    {
        _inventoryStorage.OnChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        _inventoryStorage.OnChanged -= Refresh;
        DestroyItems();
    }

    private void Refresh()
    {
        DestroyItems();
        FillItems();
        ResetScrollPosition(-200f);
    }

    private void FillItems()
    {
        int filledSlots = 0;

        foreach (var item in _inventoryStorage.Items)
        {
            if (filledSlots >= _totalSlots)
            {
                break;
            }

            InventoryItemView itemView = _itemFactory.Create(item.Item, item.Count);
            itemView.transform.SetParent(_content, false);
            _itemViews.Add(itemView);
            filledSlots++;
        }

        for (int i = filledSlots; i < _totalSlots; i++)
        {
            InventoryItemView emptySlot = _itemFactory.Create(null, 0);
            emptySlot.transform.SetParent(_content, false);
            _itemViews.Add(emptySlot);
        }
    }

    private void ResetScrollPosition(float y)
    {
        var position = _content.transform.position;
        position.y = y;
        _content.transform.position = position;
    }

    private void DestroyItems()
    {
        foreach (var itemView in _itemViews)
        {
            itemView.Dispose();
        }
        _itemViews.Clear();
    }
}