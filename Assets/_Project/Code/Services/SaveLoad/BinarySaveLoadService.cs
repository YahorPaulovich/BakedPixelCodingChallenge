using System;
using System.IO;
using System.Security.Cryptography;
using MemoryPack;
using UnityEngine;
using Zenject;

public sealed class BinarySaveLoadService : AbstractSaveLoadService
{
    [Inject] private readonly IPersistentProgressService _persistentProgress;

    protected override T ReadEncryptedData<T>(string path)
    {
        try
        {
            byte[] fileBytes = File.ReadAllBytes(path);

            using Aes aesProvider = Aes.Create();
            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);

            using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor();
            using MemoryStream memoryStream = new MemoryStream(fileBytes);
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);

            using MemoryStream decryptedStream = new MemoryStream();
            cryptoStream.CopyTo(decryptedStream);
            decryptedStream.Position = 0;

            return MemoryPackSerializer.Deserialize<T>(decryptedStream.ToArray());
        }
        catch (CryptographicException ex)
        {
            Debug.LogError($"Decryption failed: {ex.Message}");
            return default;
        }
    }

    protected override void WriteEncryptedData<T>(T data, FileStream stream)
    {
        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);

        using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
        using CryptoStream cryptoStream = new CryptoStream(
            stream,
            cryptoTransform,
            CryptoStreamMode.Write
        );

        byte[] serializedData = MemoryPackSerializer.Serialize(data);
        cryptoStream.Write(serializedData, 0, serializedData.Length);
    }

    public override bool TrySave<T>(T data, bool encrypted = false)
    {
        try
        {
            using FileStream fileStream = new FileStream(FilePath, FileMode.OpenOrCreate);
            if (encrypted)
            {
                WriteEncryptedData(data, fileStream);
            }
            else
            {
                byte[] serializedData = MemoryPackSerializer.Serialize(data);
                fileStream.Write(serializedData, 0, serializedData.Length);
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    public override bool TryLoad<T>(out T data, bool encrypted = false)
    {
        data = default;

        if (!File.Exists(FilePath))
        {
            Debug.LogWarning($"Cannot load file at {FilePath}. File does not exist!");
            return false;
        }

        try
        {
            if (encrypted)
            {
                data = ReadEncryptedData<T>(FilePath);
            }
            else
            {
                using FileStream fileStream = new FileStream(FilePath, FileMode.Open);
                byte[] fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, fileBytes.Length);
                data = MemoryPackSerializer.Deserialize<T>(fileBytes);
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    public override void SaveProgress()
    {
        TrySave(_persistentProgress.Progress, true);
    }

    public override PlayerProgress LoadProgress()
    {
        PlayerProgress playerProgress = null;

        if (TryLoad(out playerProgress, true))
        {
            Debug.Log("Progress loaded successfully.");
        }
        else
        {
            Debug.LogWarning("Failed to load progress. Initializing new progress.");
        }

        return playerProgress;
    }
}