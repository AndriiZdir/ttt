using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TilePressAction(CubeScript selectedTile);

public class CubeScript : MonoBehaviour
{
    public CubeState currentState;
    public byte currentSign;
    public Vector2 tileCoords;
    public GameFieldScript gameField;
    public string playerId;
    public MineScript mine;

    public event TilePressAction OnTileSelected;

    private MeshRenderer thisMeshRenderer;

    void Start()
    {
        currentState = CubeState.Default;
        thisMeshRenderer = GetComponent<MeshRenderer>();

        //thisMeshRenderer.material = gameField.cubeSignMaterials[Random.Range(0, gameField.cubeSignMaterials.Length)];
    }

    void OnMouseUpAsButton()
    {
        bool isUnderUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        if (!isUnderUI && OnTileSelected != null)
        {
            OnTileSelected(this);
        }
    }

    public void SetState(CubeState state, string playerId)
    {
        if (currentState == state) { return; }

        if (state == CubeState.Default)
        {
            thisMeshRenderer.material = gameField.cubeDefaultMaterial;
        }
        else if (state == CubeState.Signed)
        {
            thisMeshRenderer.material = gameField.cubeSignMaterials[gameField.GetSignForPlayer(playerId)];
        }
        else if (state == CubeState.Mine)
        {
            mine = gameField.InsertMine(this, playerId);
        }
        else if (state == CubeState.MineUsed)
        {
            mine.Expolde();
        }
        else
        {
            throw new System.NotImplementedException("[SetState] Unknown cube state " + state);
        }

        currentState = state;
    }
}

public enum CubeState : byte
{
    Default,
    //Selected,
    Signed = 1,

    Mine = 128,
    MineUsed = 129
}
