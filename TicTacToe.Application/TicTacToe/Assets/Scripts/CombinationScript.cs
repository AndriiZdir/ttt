using Assets.Scripts.ApiModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationScript : MonoBehaviour
{
    const float SQRT2 = 1.414213f;

    public Vector2 Position;
    public CombinationDirection Direction;
    public int Length;
    public string PlayerId;
    public GameFieldScript gameField;

    private MeshRenderer _meshRenderer;

    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Use this for initialization
    void Start()
    {
        _meshRenderer.material.color = Color.cyan;
    }

    public void UpdatePosition(float x, float y, CombinationDirection direction, int length)
    {
        Vector3 position = new Vector3(0f, 1.1f, 0f);
        Vector3 scale = new Vector3(1f, 0.25f, 0.25f);
        Quaternion rotation;

        float scaleStep = 1;
        
        switch (direction)
        {
            case CombinationDirection.Horizontal:
                rotation = Quaternion.identity;
                scaleStep = 4;
                break;
            case CombinationDirection.Vertical:
                rotation = Quaternion.Euler(0, 90, 0);
                scaleStep = 4;
                break;
            case CombinationDirection.UpDownDiagonal:
                rotation = Quaternion.Euler(0, 45, 0);
                scaleStep = 4 * SQRT2;
                break;
            case CombinationDirection.DownUpDiagonal:
                rotation = Quaternion.Euler(0, 135, 0);
                scaleStep = 4 * SQRT2;
                break;

            case CombinationDirection.Undefined:
            default: throw new System.NotSupportedException();
        }

        if (length % 2 == 0)
        {

        }
        else
        {

        }

        transform.SetPositionAndRotation(position, rotation);
        transform.localScale = scale;
    }


}
