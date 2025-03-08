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
        return new InventoryStorageService(_content, _root);
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
