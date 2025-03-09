public interface ISaveLoadService : IService
{
    public string FilePath { get; }
    public bool TrySave<T>(T data, bool encrypted = false);
    public bool TryLoad<T>(out T data, bool encrypted = false);
    public void Delete();
    public void SaveProgress();
    public PlayerProgress LoadProgress();
}
