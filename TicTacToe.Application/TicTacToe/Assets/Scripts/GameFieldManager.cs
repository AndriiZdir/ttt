using Assets.Scripts;
using Assets.Scripts.ApiModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFieldManager : Singleton<GameFieldManager>
{
    public string currentGameId;

    public float tileMargin = 1f;

    public GameFieldScript gameField;
    private GameStateModel lastGameState;
    private Rect gameBounds;

    private Dictionary<string, CubeScript> dictTiles;
    private CubeScript selectedTile;

    void Update()
    {
        
    }

    public static void StartGame(string gameId)
    {
        Instance.currentGameId = gameId;
        Instance.gameBounds = Rect.zero;
        Instance.dictTiles = new Dictionary<string, CubeScript>();
        Instance.selectedTile = null;

        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);

        Instance.StartCoroutine(Instance.UpdateGameFieldState());
    }

    public static void StopGame()
    {
        Instance.currentGameId = null;
        Instance.lastGameState = null;
        Instance.gameField = null;
        Instance.dictTiles = null;

        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);        
    }

    public void SelectTile(CubeScript tile)
    {
        if (Instance.selectedTile == null || Instance.selectedTile != tile)
        {
            Debug.Log("Select new tile " + tile.tileCoords);

            Instance.selectedTile = tile;

            gameField.CameraMoveToTile(tile.transform.position.x, tile.transform.position.z);
            gameField.CameraChangeZoom(20);
        }
        else
        {
            Debug.Log("This tile is already selected");
            gameField.CameraChangeZoom(60);
        }
    }

    IEnumerator UpdateGameFieldState()
    {
        while (currentGameId != null)
        {
            if (gameField == null)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

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



        lastGameState = gameState;
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
        var tile = Instantiate(gameField.fieldTile, new Vector3(x + x * tileMargin, 1, y + y * tileMargin), Quaternion.identity);
        tile.name = "tile_(" + x + ";" + y + ")";
        tile.tileCoords = new Vector2(x, y);

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
