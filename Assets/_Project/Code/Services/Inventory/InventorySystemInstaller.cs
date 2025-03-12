using UnityEngine;
using Zenject;

public sealed class InventorySystemInstaller : MonoInstaller
{
    [Header("Inventory Item")]
    [SerializeField] private int _inventoryItemViewPoolSize = 64;
    [SerializeField] private InventoryItemView _inventoryItemViewPrefab;

    [Header("Inventory Slot")]
    [SerializeField] private int _inventorySlotPoolSize = 64;
    [SerializeField] private InventorySlot _inventorySlotPrefab;

    [Header("Transform")]
    [SerializeField] private InventoryContent _content;
    [SerializeField] private RectTransform _root;

    [Header("Capacity")]
    [SerializeField] private int _totalSlots = 30;
    [SerializeField] private int _unlockedSlots = 15;

    private InventoryStorageService _inventoryStorage = null;

    public override void InstallBindings()
    {
        BindInventoryStorageService();
        BindInventoryItemViewFactory();
        BindInventorySlotFactory();
    }

    private void BindInventoryStorageService()
    {
        Container
            .Bind<IInventoryStorageService>()
            .To<InventoryStorageService>()
            .FromMethod(Initialize)
            .AsSingle();
    }

    private InventoryStorageService Initialize(InjectContext context)
    {
        _inventoryStorage = new InventoryStorageService(_content, _root);
        _inventoryStorage.TotalSlots = _totalSlots;
        _inventoryStorage.UnlockedSlots = _unlockedSlots;

        return _inventoryStorage;
    }

    private void BindInventoryItemViewFactory()
    {
        Container.BindFactory<InventoryItem, int, InventoryItemView, InventoryItemView.Factory>()
            .FromMonoPoolableMemoryPool(options => options
            .WithInitialSize(_inventoryItemViewPoolSize)
            .FromComponentInNewPrefab(_inventoryItemViewPrefab)
            .UnderTransform(transform));
    }

    private void BindInventorySlotFactory()
    {
        Container.BindFactory<InventorySlot, InventorySlot.Factory>()
            .FromMonoPoolableMemoryPool(options => options
            .WithInitialSize(_inventorySlotPoolSize)
            .FromComponentInNewPrefab(_inventorySlotPrefab)
            .UnderTransform(transform));
    }
}
