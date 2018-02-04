using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldManager : Singleton<GameFieldManager>
{
    public Transform root;
    public Transform fieldTile;
    public Camera mainCamera;
    public Animator camAnimator;

    public string gameFieldId;
    public float tileMargin = 1f;
    public int tileCount = 5;

    // Use this for initialization
    void Start()
    {
        camAnimator = mainCamera.GetComponent<Animator>();

        for (int x = -tileCount; x <= tileCount; x++)
        {
            for (int z = -tileCount; z <= tileCount; z++)
            {
                var go = SpawnFieldTile(new Vector3(x + x * tileMargin, 1, z + z * tileMargin));
                go.name = "tile_(" + x + ";" + z + ")";
            }
        }

        StartCoroutine(Zoom());
    }

    IEnumerator Zoom()
    {
        yield return new WaitForSeconds(3);
        camAnimator.Play("cameraAnimation_In");
        yield return new WaitForSeconds(3);
        camAnimator.Play("cameraAnimation_Out");
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
