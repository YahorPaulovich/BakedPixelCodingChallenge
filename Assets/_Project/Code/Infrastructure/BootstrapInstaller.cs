using Zenject;

public sealed class BootstrapInstaller : MonoInstaller
{
    private PersistentProgressService _persistentProgress;
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
        _persistentProgress = new PersistentProgressService();

        Container
            .Bind<IPersistentProgressService>()
            .To<PersistentProgressService>()
            .FromInstance(_persistentProgress)
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
