using Zenject;

public sealed class InputInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindInputService();
    }

    private void BindInputService()
    {
        Container
            .BindInterfacesAndSelfTo<InputService>()
            .AsSingle();
    }
}