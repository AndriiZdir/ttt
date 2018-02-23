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
    public int tileCount = 5;

    public GameFieldScript gameField;
    private GameStateModel lastGameState;
    private Rect gameBounds;

    void Update()
    {
        
    }

    public static void StartGame(string gameId)
    {
        Instance.currentGameId = gameId;
        Instance.gameBounds = Rect.zero;

        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);

        Instance.StartCoroutine(Instance.UpdateGameFieldState());
    }

    public static void StopGame()
    {
        Instance.currentGameId = null;
        Instance.lastGameState = null;
        Instance.gameField = null;

        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);        
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

        if(gameBounds != gameStateBounds)
        {
            ExpandField(gameStateBounds);
        }
        

        lastGameState = gameState;
    }

    private void ExpandField(Rect bounds)
    {        
        for (float x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (float y = bounds.yMin; y <= bounds.yMax; y++)
            {
                if (!gameBounds.Contains(new Vector2(x, y)))
                {

                    var tile = Instantiate(gameField.fieldTile, new Vector3(x + x * tileMargin, 1, y + y * tileMargin), Quaternion.identity);
                    //var go = SpawnFieldTile(new Vector3(x + x * tileMargin, 1, y + y * tileMargin), gameField.fieldTile.gameObject);
                    tile.name = "tile_(" + x + ";" + y + ")";
                    tile.tileCoords = new Vector2(x, y);
                }
            }
        }

        gameBounds = bounds;

        Debug.Log("Field expanded to " + gameBounds);
    }

    //public static GameObject SpawnFieldTile(Vector3 position, GameObject fieldTile, Transform rootTransform = null)
    //{
    //    var obj = PoolManager.SpawnObject(fieldTile, position, Quaternion.identity);
        
    //    obj.transform.parent = rootTransform;
    //    obj.SetActive(true);

    //    return obj;
    //}

    // Use this for initialization
    //void Start()
    //{
    //    camAnimator = mainCamera.GetComponent<Animator>();

    //    for (int x = -tileCount; x <= tileCount; x++)
    //    {
    //        for (int z = -tileCount; z <= tileCount; z++)
    //        {
    //            //var go = SpawnFieldTile(new Vector3(x + x * tileMargin, 1, z + z * tileMargin));
    //            //go.name = "tile_(" + x + ";" + z + ")";
    //        }
    //    }

    //    StartCoroutine(Zoom());
    //}

    //IEnumerator Zoom()
    //{
    //    yield return new WaitForSeconds(3);
    //    camAnimator.Play("cameraAnimation_In");
    //    yield return new WaitForSeconds(3);
    //    camAnimator.Play("cameraAnimation_Out");
    //}

    // Update is called once per frame
}
