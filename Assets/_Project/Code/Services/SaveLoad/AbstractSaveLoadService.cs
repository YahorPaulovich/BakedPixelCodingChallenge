using System.IO;
using UnityEngine;

public abstract class AbstractSaveLoadService : ISaveLoadService
{
    private string _directory;
    private string _filePath;
    
    protected const string KEY = "ggdPhkeOoiv6YMiPWa34kIuOdDUL7NwQFg6l1DVdwN8=";
    protected const string IV = "JZuM0HQsWSBVpRHTeRZMYQ==";
    
    public string FilePath => _filePath;

    public AbstractSaveLoadService()
    {
        _directory = Application.persistentDataPath + "/Saves/";
        Directory.CreateDirectory(_directory);

        _filePath = _directory + "Save.dat";
    }

    internal void SetFileName(string fileName)
    {
        _filePath = _directory + fileName;
    }
    
    protected abstract T ReadEncryptedData<T>(string path);
    protected abstract void WriteEncryptedData<T>(T data, FileStream stream);
    public abstract bool TrySave<T>(T data, bool encrypted = false);
    public abstract bool TryLoad<T>(out T data, bool encrypted = false);
    public abstract void SaveProgress();
    public abstract PlayerProgress LoadProgress();

    public virtual void Delete() 
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }
}
