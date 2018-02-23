using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldScript : MonoBehaviour
{
    public Transform rootObject;
    public CubeScript fieldTile;

    public Camera gameCamera;
    public Animator gameCameraAnimator;

    // Use this for initialization
    void Start()
    {
        GameFieldManager.Instance.gameField = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
