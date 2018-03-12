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
    //public string PlayerId;
    public GameFieldScript gameField;

    private MeshRenderer _meshRenderer;

    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void UpdatePosition(float x, float y, CombinationDirection direction, int length)
    {
        Vector3 position = new Vector3(x, transform.position.y, y);
        Quaternion rotation;
        Vector3 scale = transform.localScale;

        scale.x = length * 2 - 1;

        float offset = length - 1;

        switch (direction)
        {
            case CombinationDirection.Horizontal:
                rotation = Quaternion.identity;
                position.x -= offset;
                break;
            case CombinationDirection.Vertical:
                rotation = Quaternion.Euler(0, 90, 0);
                position.z -= offset;
                break;
            case CombinationDirection.DownUpDiagonal:
                rotation = Quaternion.Euler(0, 45, 0);
                scale.x *= SQRT2;
                position.x -= offset;
                position.z += offset;
                break;
            case CombinationDirection.UpDownDiagonal:
                rotation = Quaternion.Euler(0, 135, 0);
                scale.x *= SQRT2;
                position.x -= offset;
                position.z -= offset;
                break;

            case CombinationDirection.Undefined:
            default: throw new System.NotSupportedException();
        }

        transform.SetPositionAndRotation(position, rotation);
        transform.localScale = scale;
    }

    public void SetColor(Color color)
    {
        _meshRenderer.material.color = color;
    }
}
