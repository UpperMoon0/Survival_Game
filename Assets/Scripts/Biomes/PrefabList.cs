using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PrefabList
{
    private static List<GameObject> prefabList = new List<GameObject>();
    private static List<GameObject> pickablePrefabList = new List<GameObject>();
    static PrefabList()
    {
        var biomePrefabAssetBundle
            = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "biomeprefabs"));
        if (biomePrefabAssetBundle == null)
        {
            Debug.Log("Failed to load biomeprefabs AssetBundle!");
            return;
        }

        prefabList.Add(biomePrefabAssetBundle.LoadAsset<GameObject>("Tree1"));
        pickablePrefabList.Add(biomePrefabAssetBundle.LoadAsset<GameObject>("Flint"));
        pickablePrefabList.Add(biomePrefabAssetBundle.LoadAsset<GameObject>("Stick"));
    }
    public static GameObject GetPrefab(int id)
    {
            return prefabList[id];
    }
    public static GameObject GetPickablePrefab(int id)
    {
            return pickablePrefabList[id];
    }
}
