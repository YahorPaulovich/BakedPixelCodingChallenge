#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class InventoryContentEditor
{
    [MenuItem("Assets/Fill Inventory Content")]
    private static void FillInventoryContent()
    {
        var selected = Selection.activeObject as InventoryContent;

        if (selected == null)
        {
            Debug.LogError("Please select an InventoryContent asset.");
            return;
        }

        var items = Resources.FindObjectsOfTypeAll<InventoryItem>();
        var initialItems = new InitialInventoryItem[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            initialItems[i] = new InitialInventoryItem
            {
                Item = items[i],
                Count = 1
            };
        }
        selected.Content = initialItems;

        EditorUtility.SetDirty(selected);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"InventoryContent '{selected.name}' filled with all items in the project.");
    }

    [MenuItem("Assets/Fill Inventory Content", true)]
    private static bool ValidateFillInventoryContent()
    {
        return Selection.activeObject is InventoryContent;
    }
}
#endif