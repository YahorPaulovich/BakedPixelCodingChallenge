using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using BurstLinq;
using UnityEngine;
using Zenject;
using R3;
using Unity.Mathematics;

public sealed class InventoryView : MonoBehaviour
{
    [SerializeField] private Transform _content;

    private readonly List<InventorySlot> _slots = new List<InventorySlot>();
    private readonly List<InventoryItemView> _itemViews = new List<InventoryItemView>();

    [Inject] private InventorySlot.Factory _slotFactory;
    [Inject] private InventoryItemView.Factory _itemFactory;
    [Inject] private IInventoryStorageService _inventoryStorage;

    [Inject] private ISaveLoadService _saveLoad;
    [Inject] private IPersistentProgressService _persistentProgress;

    private Dictionary<InventoryStorageItem, int> _occupiedSlots = new Dictionary<InventoryStorageItem, int>();
    private CompositeDisposable _disposables = new CompositeDisposable();

    private void Awake()
    {
        LoadProgressOrInitNew();
    }

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

        SaveProgress();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Refresh()
    {
        DestroyItems();
        FillSlots();
        ResetScrollPosition(-200f);

        SaveProgress();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ResetScrollPosition(float y)
    {
        var position = _content.transform.position;
        position.y = y;
        _content.transform.position = position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CreateSlots()
    {
        for (int i = 0; i < _inventoryStorage.TotalSlots; i++)
        {
            var slot = _slotFactory.Create();
            slot.transform.SetParent(_content, false);

            if (i >= _inventoryStorage.UnlockedSlots)
            {
                slot.Lock();
            }

            _slots.Add(slot);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FillSlots()
    {
        int slotIndex = 0;

        foreach (var item in _inventoryStorage.Items)
        {
            if (slotIndex >= _inventoryStorage.UnlockedSlots)
            {
                Debug.LogWarning("Not enough unlocked slots to display all items.");
                break;
            }

            var itemView = _itemFactory.Create(item.Item, item.Count);
            var draggableItem = itemView.DraggableItem;
            draggableItem.SetRoot(_inventoryStorage.Root);

            draggableItem.OnItemMovedAsObservable()
                .Subscribe(newIndex => OnItemMoved(item, newIndex))
                .AddTo(_disposables);

            var existingItem = _occupiedSlots.FirstOrDefault(x => x.Key.Item.ItemID == item.Item.ItemID);
            if (_occupiedSlots.TryGetValue(existingItem.Key, out var index))
            {
                DropItemView(item, itemView, index);
            }
            else
            {
                DropItemView(item, itemView, slotIndex);
            }

            _itemViews.Add(itemView);
            slotIndex++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DropItemView(InventoryStorageItem item, InventoryItemView view, int index)
    {
        int foundIndex = FindFreeSlotIndex(startIndex: index);

        if (foundIndex >= 0)
        {
            view.transform.SetParent(_slots[foundIndex].transform, false);
            _occupiedSlots[item] = foundIndex;
        }
        else
        {
            Debug.LogError($"No free slots available for item {item.Item.Name}!");
            view.Dispose();
        }
    }

    private int FindFreeSlotIndex(int startIndex = 0)
    {
        int maxIndex = math.min(_inventoryStorage.UnlockedSlots, _slots.Count) - 1;
        startIndex = math.clamp(startIndex, 0, maxIndex);

        for (int i = startIndex; i <= maxIndex; i++)
        {
            if (IsSlotFree(_slots[i]))
            {
                return i;
            }
        }

        for (int i = 0; i < startIndex; i++)
        {
            if (IsSlotFree(_slots[i]))
            {
                return i;
            }
        }

        return -1;
    }
    
    private bool IsSlotFree(InventorySlot slot)
    {
        return !slot.IsLocked && slot.transform.childCount <= 1;
    }

    private void OnItemMoved(InventoryStorageItem item, int newIndex)
    {
        var key = _occupiedSlots.Keys.FirstOrDefault(k =>
            k.Item.ItemID == item.Item.ItemID
        );

        _occupiedSlots[key] = newIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        _disposables.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnlockSlots(int numberOfSlots)
    {
        _inventoryStorage.UnlockedSlots += numberOfSlots;

        for (int i = 0; i < _inventoryStorage.UnlockedSlots && i < _slots.Count; i++)
        {
            _slots[i].Unlock();
        }

        Refresh();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnlockSlotByIndex(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Count)
        {
            Debug.LogError($"Invalid slot index: {slotIndex}");
            return;
        }

        _slots[slotIndex].Unlock();

        if (slotIndex >= _inventoryStorage.UnlockedSlots)
        {
            _inventoryStorage.UnlockedSlots = slotIndex + 1;
        }

        Refresh();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LoadProgressOrInitNew()
    {
        _persistentProgress.Progress = _saveLoad.LoadProgress() ?? NewProgress();

        ApplyLoadedInventoryData(_persistentProgress.Progress.InventoryData);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private PlayerProgress NewProgress()
    {
        var progress = new PlayerProgress();
        progress.InventoryData = new InventoryData();

        return progress;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ApplyLoadedInventoryData(InventoryData inventoryData)
    {
        var savedItemIds = new HashSet<string>(inventoryData.Items.Select(x => x.ItemID));
        var itemsToRemove = _inventoryStorage.Items
            .Where(storageItem => !savedItemIds.Contains(storageItem.Item.ItemID))
            .ToList();
        foreach (var item in itemsToRemove)
        {
            _inventoryStorage.Remove(item.Item, item.Count);
        }

        _occupiedSlots.Clear();
        foreach (var item in inventoryData.Items)
        {
            var inventoryItem = _inventoryStorage.Items
                .FirstOrDefault(x => x.Item.ItemID == item.ItemID);

            if (inventoryItem.Item != null)
            {
                _inventoryStorage.UpdateItemCount(inventoryItem.Item, item.Count);
                _occupiedSlots[inventoryItem] = item.OccupiedSlotIndex;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SaveProgress()
    {
        var inventoryData = _persistentProgress.Progress.InventoryData;
        inventoryData.Items.Clear();

        int slotIndex = 0;
        foreach (var inventoryStorageItem in _inventoryStorage.Items)
        {
            var slotEntry = _occupiedSlots.FirstOrDefault(x =>
                x.Key.Item.ItemID == inventoryStorageItem.Item.ItemID
            );

            var itemData = new InventoryItemData
            {
                ItemID = inventoryStorageItem.Item.ItemID,
                Name = inventoryStorageItem.Item.Name,
                Count = inventoryStorageItem.Count,
                OccupiedSlotIndex = slotEntry.Equals(default(KeyValuePair<InventoryStorageItem, int>))
                    ? slotIndex
                    : slotEntry.Value
            };
            inventoryData.Items.Add(itemData);
            slotIndex++;
        }

        _saveLoad.SaveProgress();
        Debug.Log("Progress saved successfully.");
    }
}