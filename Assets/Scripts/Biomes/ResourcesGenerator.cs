using System.Collections.Generic;
using UnityEngine;

public class ResourcesGenerator : MonoBehaviour
{
    public GameObject pickableObject;

    private List<Vector3> objPos;
    void Awake()
    {
        objPos = new List<Vector3>();
        // Spawn trees
        for (int i = 0; i < 40; i++)
        {
            SpawnObject(50, 50, (Vector3 pos, Quaternion rotation) =>
            {
                Instantiate(PrefabList.GetPrefab(0), pos, rotation);
            });
        }
        // Spawn flints
        for (int i = 0; i < 15; i++)
        {
            SpawnObject(50, 50, (Vector3 pos, Quaternion rotation) =>
            {
                GameObject obj = Instantiate(pickableObject, pos, rotation);
                obj.GetComponent<PickableObject>().Type = 0;
            });
        }
        // Spawn sticks
        for (int i = 0; i < 20; i++)
        {
            SpawnObject(50, 50, (Vector3 pos, Quaternion rotation) =>
            {
                GameObject obj = Instantiate(pickableObject, pos, rotation);
                obj.GetComponent<PickableObject>().Type = 1;
            });
        }
    }
    private void SpawnObject(float xRange, float zRange, System.Action<Vector3, Quaternion> spawnMethod)
    {
        float x = Random.Range(-xRange, xRange);
        float z = Random.Range(-zRange, zRange);
        Vector3 position = new Vector3(x, 0, z);
        bool tooClose = false;
        foreach (Vector3 pos in objPos)
        {
            if (Vector3.Distance(position, pos) < 2.0f)
            {
                tooClose = true;
                break;
            }
        }
        if (!tooClose)
        {
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            spawnMethod(position, rotation);
            objPos.Add(position);
        }
    }
}
