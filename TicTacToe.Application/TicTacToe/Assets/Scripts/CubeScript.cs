using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public Texture[] cubSignTextures;

    public CubeState State = CubeState.Default;
    public Vector2 tileCoords;
    //public GameFieldScript gameField;

    void Awake()
    {
        
    }

    void Update()
    {
        switch (State)
        {
            case CubeState.Default:
            default:
                break;

            case CubeState.Activating:
                //_thisTransform.position += Vector3.Slerp(Vector3.up * 0, Vector3.up * 1f, 0.5f * Time.deltaTime);
                break;
            case CubeState.Activated:
                break;

            case CubeState.Deactivating:
                break;
            case CubeState.Deactivated:
                break;

            case CubeState.Loading:
                break;
        }
    }

    public void OnTapEnable()
    {
        if (State == CubeState.Default)
        {
            StartCoroutine(Activate());
        }
        else if (State == CubeState.Activated)
        {
            State = CubeState.Loading;
        }        
    }

    IEnumerator Activate()
    {
        State = CubeState.Activating;
        yield return new WaitForSeconds(0.5f);
        State = CubeState.Activated;
        yield break;
    }

    private void OnMouseDown()
    {
        GameFieldManager.Instance.SelectTile(this);
        Debug.Log(tileCoords);
    }    
}

public enum CubeState
{
    Default,
    Activating,
    Activated,
    Deactivating,
    Deactivated,
    Loading
}
