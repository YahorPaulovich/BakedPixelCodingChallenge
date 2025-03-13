using System.Linq;
using BurstLinq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = Unity.Mathematics.Random;

public class InventoryPlayerActionsView : MonoBehaviour
{
    [SerializeField] private Button _shootButton;
    [SerializeField] private Button _addAmmoButton;
    [SerializeField] private Button _addItemButton;
    [SerializeField] private Button _deleteItemButton;

    [Inject] private readonly IInventoryStorageService _inventoryStorage;

    private Random _random;

    private void Awake()
    {
        _random = new Random((uint)System.DateTime.Now.Ticks);
    }

    private void OnEnable()
    {
        if (_shootButton != null)
            _shootButton.onClick.AddListener(OnShootButtonClicked);

        if (_addAmmoButton != null)
            _addAmmoButton.onClick.AddListener(OnAddAmmoButtonClicked);

        if (_addItemButton != null)
            _addItemButton.onClick.AddListener(OnAddItemButtonClicked);

        if (_deleteItemButton != null)
            _deleteItemButton.onClick.AddListener(OnDeleteItemButtonClicked);
    }

    private void OnDisable()
    {
        if (_shootButton != null)
            _shootButton.onClick.RemoveListener(OnShootButtonClicked);

        if (_addAmmoButton != null)
            _addAmmoButton.onClick.RemoveListener(OnAddAmmoButtonClicked);

        if (_addItemButton != null)
            _addItemButton.onClick.RemoveListener(OnAddItemButtonClicked);

        if (_deleteItemButton != null)
            _deleteItemButton.onClick.RemoveListener(OnDeleteItemButtonClicked);
    }

    private void OnShootButtonClicked()
    {
        var ammoItems = _inventoryStorage.Content
            .Where(item => item.Item.Category.Name == "Consumables")
            .ToList();

        if (ammoItems.Count == 0)
        {
            Debug.LogError("No ammo available to shoot.");
            return;
        }

        var randomAmmo = ammoItems[_random.NextInt(0, ammoItems.Count)];

        if (!_inventoryStorage.Remove(randomAmmo.Item, 1))
        {
            Debug.LogError("Failed to remove ammo.");
        }
        else
        {
            Debug.Log($"Shot with {randomAmmo.Item.Name}");
        }
    }

    private void OnAddAmmoButtonClicked()
    {
        var ammoTypes = _inventoryStorage.Content
            .Select(item => item.Item)
            .Where(item => item.Category.Name == "Consumables")
            .Distinct()
            .ToList();

        if (ammoTypes.Count == 0)
        {
            Debug.LogError("No ammo types found in inventory content.");
            return;
        }

        foreach (var ammo in ammoTypes)
        {
            _inventoryStorage.Add(ammo, ammo.MaxStackSize);
        }

        Debug.Log("Added full stacks of all ammo types.");
    }

    private void OnAddItemButtonClicked()
    {
        var weaponItem = GetRandomItemByCategory("Weapons");
        var headItem = GetRandomItemByCategory("Head");
        var torsoItem = GetRandomItemByCategory("Torso");

        InventoryItem[] items = { weaponItem, headItem, torsoItem };
        
        foreach (var item in items)
        {
            if (item != null)
            {
                _inventoryStorage.Add(item, 1);
            }
        }

        Debug.Log("Added random items of each type.");
    }

    private InventoryItem GetRandomItemByCategory(string categoryName)
    {
        var items = _inventoryStorage.Content
            .Select(item => item.Item)
            .Where(item => item.Category.Name == categoryName)
            .ToList();

        if (items.Count == 0)
        {
            Debug.LogError($"No items found for category: {categoryName}");
            return null;
        }

        return items[_random.NextInt(0, items.Count)];
    }

    private void OnDeleteItemButtonClicked()
    {
        var nonEmptySlots = _inventoryStorage.Items
            .Where(item => item.Count > 0)
            .ToList();

        if (nonEmptySlots.Count == 0)
        {
            Debug.LogError("All slots are empty.");
            return;
        }

        var randomSlot = nonEmptySlots[_random.NextInt(0, nonEmptySlots.Count)];

        if (!_inventoryStorage.Remove(randomSlot.Item, randomSlot.Count))
        {
            Debug.LogError("Failed to remove items from slot.");
        }
        else
        {
            Debug.Log($"Removed all items of type {randomSlot.Item.Name} from inventory.");
        }
    }
}