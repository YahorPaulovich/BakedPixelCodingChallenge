using UnityEngine;
using Zenject;

public sealed class InventorySystemInstaller : MonoInstaller
{
    [Header("InventoryItem")]
    [SerializeField] private int _inventoryItemViewPoolSize = 64;
    [SerializeField] private InventoryItemView _inventoryItemViewPrefab;
    [SerializeField] private InventoryContent _content;

    public override void InstallBindings()
    {
        BindInventoryStorageService();
        BindInventoryItemViewFactory();
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
        return new InventoryStorageService(_content);
    }

    private void BindInventoryItemViewFactory()
    {
        Container.BindFactory<InventoryItem, int, InventoryItemView, InventoryItemView.Factory>()
            .FromMonoPoolableMemoryPool(options => options
            .WithInitialSize(_inventoryItemViewPoolSize)
            .FromComponentInNewPrefab(_inventoryItemViewPrefab)
            .UnderTransform(transform));
    }
}
