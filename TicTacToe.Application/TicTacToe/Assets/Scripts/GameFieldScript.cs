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

    [Header("Game Field")]
    public CubeScript fieldTilePrefab;
    public Material[] cubeSignMaterials;
    public Material cubeDefaultMaterial;
    public Material cubeMineMaterial;
    public Material cubeUsedMineMaterial;
    public SelectBoxScript tileSelectBox;
    public CombinationScript combinationPrefab;
    public MineScript minePrefab;

    private Dictionary<Vector2, CubeScript> dictTiles;
    private CubeScript selectedTile;
    private Dictionary<Vector3, CombinationScript> dictCombinations;

    [Header("UI")]
    public GameObject buttonDeselect;
    public UIGameFieldScript gameFieldUI;

    [Header("Variables")]
    public float tileMargin = 1f;

    private string currentPlayerId;
    private string currentGameId;

    private bool _isMyTurnToMove;
    public bool IsMyTurnToMove
    {
        get
        {
            return _isMyTurnToMove;
        }

        private set
        {
            if (_isMyTurnToMove != value) //if True - turn was changed
            {
                _isMyTurnToMove = value;
                
                gameFieldUI.UpdateMineButton(false, _isMyTurnToMove && mineQuantityPlacedInCurrentTurn < mineQuantityCanBePlacedInCurrentTurn && mineQuantity > 0, mineQuantity);                
                mineQuantityPlacedInCurrentTurn = 0;
                DeselectTile();
            }
        }
    }

    private int mineQuantity;
    private readonly int mineQuantityCanBePlacedInCurrentTurn = 1;
    private int mineQuantityPlacedInCurrentTurn;

    void Start()
    {

        dictTiles = new Dictionary<Vector2, CubeScript>();
        dictCombinations = new Dictionary<Vector3, CombinationScript>();

        currentGameId = GameFieldManager.Instance.currentGameId;
        currentPlayerId = ApiService.Instance.PlayerInfo.PlayerId;
        
        IsMyTurnToMove = false;

        gameFieldUI.ClearPlayerList();

        if (GameFieldManager.Instance.gameDetails != null)
        {
            foreach (var player in GameFieldManager.Instance.gameDetails.Players)
            {
                gameFieldUI.AddPlayerToList(player.PlayerId, player.Sign, player.PlayerName);
            }

            mineQuantity = GameFieldManager.Instance.gameDetails.MinesQuantity;
        }

        DeselectTile();

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

    public void OnPlaceMine()
    {
        if (selectedTile != null && selectedTile.currentState == CubeState.Default)
        {
            StartCoroutine(ApiService.SetMineAsync(x => { }, currentGameId, (int)selectedTile.tileCoords.x, (int)selectedTile.tileCoords.y));
            DeselectTile();
        }
    }
    #endregion


    public void SelectTile(CubeScript tile)
    {
        if (!_isMyTurnToMove) { return; }

        gameCamera.StopAllCoroutines();

        if (selectedTile != tile)
        {
            selectedTile = tile;
            buttonDeselect.SetActive(true);
            gameFieldUI.UpdateMineButton(true);

            tileSelectBox.SetPosition(selectedTile.transform.position.x, selectedTile.transform.position.z);
            tileSelectBox.SetVisibility(true);

            var currZoom = gameCamera.GetCurrentZoom();

            if (currZoom > 50)
            {
                gameCamera.CameraMoveToTile(selectedTile.transform.position.x, selectedTile.transform.position.z);
                gameCamera.CameraChangeZoom(50);
            }
        }
        else
        {
            StartCoroutine(ApiService.SetPointAsync(x => { }, currentGameId, (int)tile.tileCoords.x, (int)tile.tileCoords.y));

            DeselectTile();
        }
    }

    public void DeselectTile()
    {        
        //gameCamera.StopAllCoroutines();
        //gameCamera.CameraChangeZoom(75);
        selectedTile = null;
        buttonDeselect.SetActive(false);
        tileSelectBox.SetVisibility(false);
        gameFieldUI.UpdateMineButton(false);
    }


    public byte GetSignForPlayer(string playerId)
    {
        var playerInfo = gameFieldUI.dictGameFieldPlayers[playerId];

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
            gameFieldUI.UpdatePlayerInformation(player.PlayerId, player.Points, isCurrentTurn, player.SkipsNextTurn);
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
            var startTile = GetTile(combination.X, combination.Y);
            InsertCombination(startTile.transform.position.x, startTile.transform.position.z, combination.Direction, combination.Length, combination.PlayerId);
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

        foreach (var comb in dictCombinations)
        {
            Destroy(comb.Value.gameObject);
        }        
        dictCombinations.Clear();

        gameBounds = Rect.zero;
        selectedTile = null;

        gameFieldUI.ClearPlayerList();
        gameFieldUI.AddPlayerToList("p1", 3, "Player 1");
        gameFieldUI.AddPlayerToList("p2", 1, "Player 2");

        ExpandField(Rect.MinMaxRect(-10, -10, 10, 10));

        InsertCombination(-6, 6, CombinationDirection.Horizontal, 5, "p1");
        InsertCombination(-6, 6, CombinationDirection.Vertical, 6, "p2");
        InsertCombination(-6, 6, CombinationDirection.UpDownDiagonal, 7, "p1");
        InsertCombination(-6, 6, CombinationDirection.DownUpDiagonal, 8, "p2");

        //dictTiles[new Vector2(-3, 3)].SetState(CubeState.Signed, "p1");
        //dictTiles[new Vector2(-3, 4)].SetState(CubeState.Signed, "p2");
        //dictTiles[new Vector2(-2, 3)].SetState(CubeState.Signed, "p1");
        //dictTiles[new Vector2(-2, 4)].SetState(CubeState.Signed, "p2");

        mineQuantity = 1;
        IsMyTurnToMove = true;        
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
        gameCamera.SetCameraMoveBounds(gameBounds.xMin * 2, gameBounds.yMin * 2, gameBounds.xMax * 2, gameBounds.yMax * 2);

        Debug.Log("Field expanded to " + gameBounds);
    }

    private CubeScript GetTile(float x, float z)
    {
        return GetTile(new Vector2(x, z));
    }

    private CubeScript GetTile(Vector2 coords)
    {
        CubeScript tile = null;

        dictTiles.TryGetValue(coords, out tile);

        return tile;
    }

    private CubeScript InsertTile(Vector2 coords)
    {
        var tile = Instantiate(fieldTilePrefab, new Vector3(coords.x + coords.x * tileMargin, 0, coords.y + coords.y * tileMargin), fieldTilePrefab.transform.rotation);
        tile.name = "tile " + coords;
        tile.tileCoords = coords;
        tile.gameField = this;
        tile.OnTileSelected += SelectTile;

        dictTiles.Add(coords, tile);

        return tile;
    }

    private CombinationScript InsertCombination(float x, float z, CombinationDirection direction, int length, string playerId)
    {
        var combination = new Vector3(x, (byte)direction, z);

        if (dictCombinations.ContainsKey(combination)) { return dictCombinations[combination]; }

        var obj = Instantiate(combinationPrefab);
        obj.name = "combination " + combination;
        obj.gameField = this;
        obj.UpdatePosition(x, z, direction, length);

        var playerSign = gameFieldUI.dictGameFieldPlayers[playerId].playerSign;
        var combColor = gameFieldUI.playerSignsColors[playerSign];
        combColor.r *= 0.5f;
        combColor.g *= 0.5f;
        combColor.b *= 0.5f;
        obj.SetColor(combColor);

        dictCombinations.Add(combination, obj);

        return obj;
    }

    public MineScript InsertMine(CubeScript tile, string playerId)
    {
        var obj = Instantiate(minePrefab);
        obj.name = "mine " + tile.tileCoords;
        obj.SetMineTile(tile, playerId == currentPlayerId);

        return obj;
    }
}
