using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Extensions;
using Assets.Scripts;
using Assets.Scripts.ApiModels;

public class GameFieldScript : MonoBehaviour
{
    [Header("Camera")]
    public Camera gameCamera;
    public Animator gameCameraAnimator;
        
    private GameStateModel lastGameState;
    private Rect gameBounds;

    [Header("Tiles")]
    public CubeScript fieldTilePrefab;
    private Dictionary<string, CubeScript> dictTiles;
    private CubeScript selectedTile;

    [Header("UI")]
    public GameObject buttonDeselect;
    public UIGameFieldPlayerListScript uiGameFieldPlayerList;

    [Header("Variables")]
    public float cameraAimTime = 1.0f;
    public float tileMargin = 1f;

    void Start()
    {
        dictTiles = new Dictionary<string, CubeScript>();

        buttonDeselect.SetActive(false);

        uiGameFieldPlayerList.ClearPlayerList();

        foreach (var player in GameFieldManager.Instance.gameDetails.Players)
        {
            uiGameFieldPlayerList.AddPlayerToList(player.PlayerId, player.Sign, player.PlayerName);
        }

        GameFieldManager.Instance.StartCoroutine(UpdateGameFieldState());
    }

    // Update is called once per frame
    void Update()
    {

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
        StopAllCoroutines();

        if (selectedTile == null)
        {            
            CameraMoveToTile(tile.transform.position.x, tile.transform.position.z);
            CameraChangeZoom(20);            
        }
        else
        {
            CameraMoveToTile(tile.transform.position.x, tile.transform.position.z, 0.5f);
        }

        Debug.Log("Select new tile " + tile.tileCoords);
        selectedTile = tile;
        buttonDeselect.SetActive(true);
    }

    public void DeselectTile()
    {
        Debug.Log("Deselected..");
        StopAllCoroutines();
        CameraChangeZoom(60);
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

        uiGameFieldPlayerList.ShowCurrentTurnPlayer(gameState.CurrentTurnPlayerId);

        foreach (var player in gameState.Players)
        {
            uiGameFieldPlayerList.SetPlayerPoints(player.PlayerId, player.Points);
        }

        foreach (var point in gameState.Points)
        {
                        
        }

        lastGameState = gameState;
    }


    private void GenerateTestGame()
    {
        gameBounds = Rect.zero;
        dictTiles = new Dictionary<string, CubeScript>();
        selectedTile = null;

        ExpandField(Rect.MinMaxRect(-16, -16, 16, 16));
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
        var tile = Instantiate(fieldTilePrefab, new Vector3(x + x * tileMargin, 1, y + y * tileMargin), Quaternion.identity);
        tile.name = "tile_(" + x + ";" + y + ")";
        tile.tileCoords = new Vector2(x, y);
        tile.gameField = this;

        dictTiles.Add(x + ";" + y, tile);

        return tile;
    }

    private CubeScript GetTile(float x, float y)
    {
        CubeScript tile = null;

        dictTiles.TryGetValue(x + ";" + y, out tile);

        return tile;
    }

    private void CameraMoveToTile(float x, float y, float timeCoef = 1)
    {
        var currentCamPosition = gameCamera.transform.position;

        StartCoroutine(gameCamera.gameObject.MoveOverSeconds(new Vector3(x - 16, currentCamPosition.y, y - 16), cameraAimTime * timeCoef));
    }

    private void CameraChangeZoom(float zoom)
    {
        StartCoroutine(gameCamera.SmoothChangeCameraFOV(zoom, cameraAimTime));
    }

}
