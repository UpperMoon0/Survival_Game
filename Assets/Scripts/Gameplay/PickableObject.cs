using UnityEngine;

public class PickableObject : MonoBehaviour
{
    private int type = -1;
    public int Type { get => type; set => type = value; }
    private void Start()
    {
        Instantiate(PrefabList.GetPickablePrefab(type), transform);
    }
}
