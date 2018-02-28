using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Extensions;
using Assets.Scripts;
using Assets.Scripts.ApiModels;

public class GameFieldScript : MonoBehaviour
{
    [Header("Camera")]
    public CameraScript gameCamera;
        
    private GameStateModel lastGameState;
    private Rect gameBounds;

    [Header("Tiles")]
    public CubeScript fieldTilePrefab;
    public Material[] cubeSignMaterials;
    public Material cubeMineMaterial;
    public Material cubeUsedMineMaterial;
    
    private Dictionary<string, CubeScript> dictTiles;
    private CubeScript selectedTile;    

    [Header("UI")]
    public GameObject buttonDeselect;
    public UIGameFieldPlayerListScript uiGameFieldPlayerList;

    [Header("Variables")]
    public float tileMargin = 1f;

    void Start()
    {                

        dictTiles = new Dictionary<string, CubeScript>();

        buttonDeselect.SetActive(false);

        uiGameFieldPlayerList.ClearPlayerList();

        if (GameFieldManager.Instance.gameDetails != null)
        {
            foreach (var player in GameFieldManager.Instance.gameDetails.Players)
            {
                uiGameFieldPlayerList.AddPlayerToList(player.PlayerId, player.Sign, player.PlayerName);
            }
        }

        GameFieldManager.Instance.StartCoroutine(UpdateGameFieldState());
    }

    public void OnBtnDeselect()
    {
        DeselectTile();
    }

    public void OnBtnTest()
    {
       GenerateTestGame();
    }

    public void SelectTile(CubeScript tile)
    {
        gameCamera.StopAllCoroutines();

        if (selectedTile == null)
        {
            gameCamera.CameraMoveToTile(tile.transform.position.x, tile.transform.position.z);
            gameCamera.CameraChangeZoom(50);
        }
        else if (selectedTile == tile)
        {
            DeselectTile();
            return;
        }
        else
        {
            gameCamera.CameraMoveToTile(tile.transform.position.x, tile.transform.position.z, 0.5f);
        }

        Debug.Log("Select new tile " + tile.tileCoords);
        selectedTile = tile;
        buttonDeselect.SetActive(true);
    }

    public void DeselectTile()
    {
        Debug.Log("Deselected..");
        gameCamera.StopAllCoroutines();
        gameCamera.CameraChangeZoom(90);
        selectedTile = null;
        buttonDeselect.SetActive(false);
    }


    IEnumerator UpdateGameFieldState()
    {
        while (GameFieldManager.Instance.currentGameId != null)
        {
            yield return ApiService.GetGameStateAsync(GameStateCallback, GameFieldManager.Instance.currentGameId);
        }
    }

    private void GameStateCallback(GameStateModel gameState)
    {
        if (lastGameState == null)
        {
            InitGameField(gameState);
            return;
        }

        //Field Bounds
        var gameStateBounds = Rect.MinMaxRect(gameState.MoveBounds.Left, gameState.MoveBounds.Top, gameState.MoveBounds.Right, gameState.MoveBounds.Bottom);
        ExpandField(gameStateBounds);

        //Player List
        foreach (var player in gameState.Players)
        {
            bool isCurrentTurn = (gameState.CurrentTurnPlayerId == player.PlayerId);
            uiGameFieldPlayerList.UpdatePlayerInformation(player.PlayerId, player.Points, isCurrentTurn, player.SkipsNextTurn);
        }

        //Points
        foreach (var point in gameState.Points)
        {
            
        }

        lastGameState = gameState;
    }


    private void GenerateTestGame()
    {
        foreach (var tile in dictTiles)
        {
            Destroy(tile.Value.gameObject);
        }

        dictTiles.Clear();

        gameBounds = Rect.zero;        
        selectedTile = null;

        ExpandField(Rect.MinMaxRect(-6, -6, 6, 6));
    }

    private void InitGameField(GameStateModel gameState)
    {
        var gameStateBounds = Rect.MinMaxRect(gameState.MoveBounds.Left, gameState.MoveBounds.Top, gameState.MoveBounds.Right, gameState.MoveBounds.Bottom);

        ExpandField(gameStateBounds);

        lastGameState = gameState;
    }

    private void ExpandField(Rect bounds)
    {
        if (gameBounds == bounds)
        {
            return;
        }

        for (float x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (float y = bounds.yMin; y <= bounds.yMax; y++)
            {
                if (!gameBounds.Contains(new Vector2(x, y)))
                {
                    InsertTile(x, y);
                }
            }
        }

        gameBounds = bounds;

        Debug.Log("Field expanded to " + gameBounds);
    }

    private CubeScript InsertTile(float x, float y)
    {
        var tile = Instantiate(fieldTilePrefab, new Vector3(x + x * tileMargin, 1, y + y * tileMargin), fieldTilePrefab.transform.rotation);
        tile.name = "tile_(" + x + ";" + y + ")";
        tile.tileCoords = new Vector2(x, y);
        tile.gameField = this;
        tile.OnTileSelected += SelectTile;

        dictTiles.Add(x + ";" + y, tile);

        return tile;
    }

    private CubeScript GetTile(float x, float y)
    {
        CubeScript tile = null;

        dictTiles.TryGetValue(x + ";" + y, out tile);

        return tile;
    }

}
