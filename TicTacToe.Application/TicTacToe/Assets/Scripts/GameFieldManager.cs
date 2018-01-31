using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldManager : Singleton<GameFieldManager>
{
    public Transform root;
    public Transform fieldTile;
    public Camera mainCamera;

    public float tileMargin = 0.1f;
    public int tileCount = 20;

    // Use this for initialization
    void Start()
    {
        for (int x = -5; x <= 5; x++)
        {
            for (int y = -5; y <= 5; y++)
            {
                SpawnFieldTile(new Vector3(x + x * tileMargin, 1, y + y * tileMargin));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static GameObject SpawnFieldTile(Vector3 position)
    {
        var obj = PoolManager.SpawnObject(Instance.fieldTile.gameObject, position, Quaternion.identity);

        obj.transform.parent = Instance.root;
        obj.SetActive(true);

        return obj;
    }
}
