using Zenject;

public sealed class BootstrapInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindInputService();
        BindPersistentDataService();
        BindSaveLoadService();
    }

    private void BindInputService()
    {
        Container
            .BindInterfacesAndSelfTo<InputService>()
            .AsSingle();
    }
    
    private void BindPersistentDataService()
    {
        Container
            .Bind<IPersistentProgressService>()
            .To<PersistentProgressService>()
            .AsSingle();
    }
    
    private void BindSaveLoadService()
    {
        Container
            .Bind<ISaveLoadService>()
            .To<BinarySaveLoadService>()
            .AsSingle();
    }
}
