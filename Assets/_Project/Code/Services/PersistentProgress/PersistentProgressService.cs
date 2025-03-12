public sealed class PersistentProgressService : IPersistentProgressService
{ 
    public PlayerProgress Progress { get; set; }

    public override string ToString()
    {
        return $"PersistentProgress: {Progress}";
    }
}
