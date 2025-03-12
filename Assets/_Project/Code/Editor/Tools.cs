using System.IO;
using UnityEditor;
using UnityEngine;

public class Tools
{
    [MenuItem("Tools/Save System/Clear Saves")]
    public static void ClearSaves()
    {
        ClearPrefs();

        var filePath = Application.persistentDataPath + "/Saves/";

        if (Directory.Exists(filePath))
        {
            Debug.Log(filePath);
            Directory.Delete(filePath, true);
        }

        Debug.Log("Saves have been deleted");
    }

    public static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
