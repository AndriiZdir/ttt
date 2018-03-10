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
    public Material cubeDefaultMaterial;
    public Material cubeMineMaterial;
    public Material cubeUsedMineMaterial;
    public CombinationScript combinationPrefab;

    private Dictionary<Vector2, CubeScript> dictTiles;
    private CubeScript selectedTile;
    private Dictionary<Vector3, CombinationScript> dictCombinations;

    [Header("UI")]
    public GameObject buttonDeselect;
    public UIGameFieldPlayerListScript uiGameFieldPlayerList;

    [Header("Variables")]
    public float tileMargin = 1f;

    private string currentPlayerId;
    private string currentGameId;
    private bool IsMyTurnToMove;

    void Start()
    {                

        dictTiles = new Dictionary<Vector2, CubeScript>();
        dictCombinations = new Dictionary<Vector3, CombinationScript>();

        buttonDeselect.SetActive(false);

        uiGameFieldPlayerList.ClearPlayerList();

        if (GameFieldManager.Instance.gameDetails != null)
        {
            foreach (var player in GameFieldManager.Instance.gameDetails.Players)
            {
                uiGameFieldPlayerList.AddPlayerToList(player.PlayerId, player.Sign, player.PlayerName);
            }
        }

        currentGameId = GameFieldManager.Instance.currentGameId;
        currentPlayerId = ApiService.Instance.PlayerInfo.PlayerId;

        GameFieldManager.Instance.StartCoroutine(UpdateGameFieldState());
    }

    #region OnEvents
    public void OnBtnDeselect()
    {
        DeselectTile();
    }

    public void OnBtnTest()
    {
        GenerateTestGame();
    }
    #endregion
    

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
            if (IsMyTurnToMove)
            {
                StartCoroutine(ApiService.SetPointAsync(x => { }, currentGameId, (int)tile.tileCoords.x, (int)tile.tileCoords.y));                
            }

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
        gameCamera.CameraChangeZoom(75);
        selectedTile = null;
        buttonDeselect.SetActive(false);
    }


    public byte GetSignForPlayer(string playerId)
    {
        var playerInfo = uiGameFieldPlayerList.dictGameFieldPlayers[playerId];

        return playerInfo.playerSign;
    }


    IEnumerator UpdateGameFieldState()
    {
        while (currentGameId != null)
        {
            yield return ApiService.GetGameStateAsync(GameStateCallback, currentGameId);
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

        IsMyTurnToMove = (gameState.CurrentTurnPlayerId == currentPlayerId);

        //Points
        foreach (var point in gameState.Points)
        {
            var tile = GetTile(point.X, point.Y);

            if (tile == null)
            {
                Debug.LogWarningFormat("Tile {0};{1} is null", point.X, point.Y);
                continue;                
            }

            tile.SetState((CubeState)point.Type, point.PlayerId);
        }

        //Combinations
        foreach (var combination in gameState.Combinations)
        {
            InsertCombination(combination.X, combination.Y, combination.Direction, combination.Length, combination.PlayerId);
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
                var coords = new Vector2(x, y);
                
                if (!dictTiles.ContainsKey(coords))
                {
                    InsertTile(coords);
                }
            }
        }

        gameBounds = bounds;

        Debug.Log("Field expanded to " + gameBounds);
    }

    private CubeScript InsertTile(Vector2 coords)
    {
        var tile = Instantiate(fieldTilePrefab, new Vector3(coords.x + coords.x * tileMargin, 1, coords.y + coords.y * tileMargin), fieldTilePrefab.transform.rotation);
        tile.name = "tile_(" + coords + ")";
        tile.tileCoords = coords;
        tile.gameField = this;
        tile.OnTileSelected += SelectTile;

        dictTiles.Add(coords, tile);

        return tile;
    }

    private CubeScript GetTile(float x, float y)
    {
        return GetTile(new Vector2(x, y));
    }

    private CubeScript GetTile(Vector2 coords)
    {
        CubeScript tile = null;

        dictTiles.TryGetValue(coords, out tile);

        return tile;
    }

    private CombinationScript InsertCombination(float x, float y, CombinationDirection direction, int length, string playerId)
    {
        var combination = new Vector3(x, y, (byte)direction);

        if (dictCombinations.ContainsKey(combination)) { return dictCombinations[combination]; }

        var obj = Instantiate(combinationPrefab);        
        obj.name = "combination_(" + combination + ")";
        obj.gameField = this;
        obj.UpdatePosition(x, y, direction, length);

        dictCombinations.Add(combination, obj);

        return obj;
    }
}
