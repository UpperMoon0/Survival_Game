using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemDict
{
    private static Dictionary<int, Sprite> itemTexturesDict = new Dictionary<int, Sprite>();
    static ItemDict()
    {
        var itemsAssetBundle
            = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "items"));
        if (itemsAssetBundle == null)
        {
            Debug.Log("Failed to load items AssetBundle!");
            return;
        }

        itemTexturesDict.Add(0, itemsAssetBundle.LoadAsset<Sprite>("Wood"));
        itemTexturesDict.Add(1, itemsAssetBundle.LoadAsset<Sprite>("Stone"));
        itemTexturesDict.Add(2, itemsAssetBundle.LoadAsset<Sprite>("Fiber"));
    }
    public static Sprite GetItemTexture(int id)
    {
        Sprite spr;
        itemTexturesDict.TryGetValue(id, out spr);
        return spr;
    }
}
