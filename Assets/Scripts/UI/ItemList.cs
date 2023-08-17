using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemList
{
    private static Dictionary<string, int> itemNameDict = new Dictionary<string, int>()
    {
        { "Wood", 0 },
        { "Stone", 1 },
        { "Fiber", 2 },
        { "Flint", 3 },
        { "Stick", 4 },
        { "Flint Axe", 5 }
    };
    private static Dictionary<int, int> pickableToItemDict = new Dictionary<int, int>()
    {
        { 0, itemNameDict["Flint"] },
        { 1, itemNameDict["Stick"] }
    };
    private static List<Sprite> itemTexturesList = new List<Sprite>();

    static ItemList()
    {
        var itemsAssetBundle
            = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "items"));
        if (itemsAssetBundle == null)
        {
            Debug.Log("Failed to load items AssetBundle!");
            return;
        }

        foreach (var item in itemNameDict)
        {
            itemTexturesList.Add(itemsAssetBundle.LoadAsset<Sprite>(RemoveEmptySpaces(item.Key)));
        }
    }
    public static Sprite GetItemTexture(int id)
    {
        return itemTexturesList[id];
    }
    public static int GetItemIDFromPickable(int pickableID)
    {
        return pickableToItemDict[pickableID];
    }
    public static int GetItemIDFromName(string name)
    {
        return itemNameDict[name];
    }
    public static string GetItemNameFromID(int id)
    {
        foreach (var item in itemNameDict)
        {
            if (item.Value == id)
            {
                return item.Key;
            }
        }
        return null;
    }
    private static string RemoveEmptySpaces(string input)
    {
        return input.Replace(" ", string.Empty);
    }
}
