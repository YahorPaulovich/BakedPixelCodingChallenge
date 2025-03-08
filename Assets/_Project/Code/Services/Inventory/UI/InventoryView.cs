using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class InventoryView : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private int _totalSlots = 30;
    [SerializeField] private int _unlockedSlots = 15;

    private readonly List<InventorySlot> _slots = new List<InventorySlot>();
    private readonly List<InventoryItemView> _itemViews = new List<InventoryItemView>();

    [Inject] private InventorySlot.Factory _slotFactory;
    [Inject] private InventoryItemView.Factory _itemFactory;
    [Inject] private IInventoryStorageService _inventoryStorage;

    private void Start()
    {
        CreateSlots();
        Refresh();
    }

    private void OnEnable()
    {
        _inventoryStorage.OnChanged += Refresh;
    }

    private void OnDisable()
    {
        _inventoryStorage.OnChanged -= Refresh;
        DestroyItems();
    }

    private void Refresh()
    {
        DestroyItems();
        FillSlots();
        ResetScrollPosition(-200f);
    }

    private void ResetScrollPosition(float y)
    {
        var position = _content.transform.position;
        position.y = y;
        _content.transform.position = position;
    }

    private void CreateSlots()
    {
        for (int i = 0; i < _totalSlots; i++)
        {
            var slot = _slotFactory.Create();
            slot.transform.SetParent(_content, false);

            if (i >= _unlockedSlots)
            {
                slot.Lock();
            }

            _slots.Add(slot);
        }
    }

    private void FillSlots()
    {
        int slotIndex = 0;

        foreach (var item in _inventoryStorage.Items)
        {
            if (slotIndex >= _unlockedSlots)
            {
                Debug.LogWarning("Not enough unlocked slots to display all items.");
                break;
            }

            var itemView = _itemFactory.Create(item.Item, item.Count);
            itemView.transform.SetParent(_slots[slotIndex].transform, false);
            itemView.GetComponentInChildren<DraggableItem>().SetRoot(_inventoryStorage.Root);
            _itemViews.Add(itemView);

            slotIndex++;
        }
    }

    private void DestroyItems()
    {
        foreach (var itemView in _itemViews)
        {
            if (itemView != null)
            {
                itemView.Dispose();
            }
        }
        _itemViews.Clear();
    }

    public void UnlockSlots(int numberOfSlots)
    {
        _unlockedSlots += numberOfSlots;

        for (int i = 0; i < _unlockedSlots && i < _slots.Count; i++)
        {
            _slots[i].Unlock();
        }

        Refresh();
    }

    public void UnlockSlotByIndex(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Count)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        _slots[slotIndex].Unlock();

        if (slotIndex >= _unlockedSlots)
        {
            _unlockedSlots = slotIndex + 1;
        }

        Refresh();
    }
}