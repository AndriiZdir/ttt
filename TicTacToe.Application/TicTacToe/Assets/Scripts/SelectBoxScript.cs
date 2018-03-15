using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBoxScript : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
    }

    public void SetColor(Color color)
    {
        _meshRenderer.material.color = color;
    }

    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(x, 0f, y);
    }

    public void SetPosition(Vector2 position)
    {
        SetPosition(position.x, position.y);
    }

    public void SetVisibility(bool isVisible)
    {
        _meshRenderer.enabled = isVisible;
    }
}
