using System;
using System.IO;
using System.Security.Cryptography;
using MemoryPack;
using UnityEngine;
using Zenject;

public sealed class BinarySaveLoadService : AbstractSaveLoadService
{
    [Inject] private readonly IPersistentProgressService _progressService;

    protected override T ReadEncryptedData<T>(string path)
    {
        using FileStream fileStream = new FileStream(path, FileMode.Open);
        byte[] fileBytes = new byte[fileStream.Length];
        fileStream.Read(fileBytes, 0, fileBytes.Length);

        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);

        using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(
            aesProvider.Key,
            aesProvider.IV
        );

        using MemoryStream decryptionStream = new MemoryStream(fileBytes);
        using CryptoStream cryptoStream = new CryptoStream(
            decryptionStream,
            cryptoTransform,
            CryptoStreamMode.Read
        );

        byte[] decryptedData = new byte[fileBytes.Length];
        int decryptedByteCount = cryptoStream.Read(decryptedData, 0, decryptedData.Length);

        return MemoryPackSerializer.Deserialize<T>(decryptedData.AsSpan(0, decryptedByteCount));
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
        if (!File.Exists(FilePath))
        {
            Debug.LogError($"Cannot load file at {FilePath}. File does not exist!");
            throw new FileNotFoundException($"{FilePath} does not exist!");
        }

        try
        {
            using FileStream fileStream = new FileStream(FilePath, FileMode.Open);
            if (encrypted)
            {
                data = ReadEncryptedData<T>(FilePath);
            }
            else
            {
                byte[] fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, fileBytes.Length);
                data = MemoryPackSerializer.Deserialize<T>(fileBytes);
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }

    public override void SaveProgress()
    {
        TrySave(_progressService.Progress, false);
    }

    public override PlayerProgress LoadProgress()
    {
        TryLoad(out PlayerProgress playerProgress, false);
        return playerProgress;
    }
}