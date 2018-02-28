using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TilePressAction(CubeScript selectedTile);

public class CubeScript : MonoBehaviour
{
    public CubeState state;
    public byte sign;
    public Vector2 tileCoords;
    public GameFieldScript gameField;
    public string playerId;

    public event TilePressAction OnTileSelected;

    private MeshRenderer thisMeshRenderer;

    void Start()
    {
        state = CubeState.Default;
        thisMeshRenderer = GetComponent<MeshRenderer>();

        //thisMeshRenderer.material = gameField.cubeSignMaterials[Random.Range(0, gameField.cubeSignMaterials.Length)];
    }

    void OnMouseUpAsButton()
    {
        if (OnTileSelected != null)
        {
            OnTileSelected(this);
        }        
    }

    
}

public enum CubeState
{
    Default,
    Selected,
    Signed,
    Mine,
    MineUsed
}
