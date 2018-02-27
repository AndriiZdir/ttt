using Assets.Scripts;
using Assets.Scripts.ApiModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFieldManager : Singleton<GameFieldManager>
{
    public string currentGameId;
    public LobbyGameDetailsModel gameDetails;

    public static void StartGame(string gameId, LobbyGameDetailsModel gameDetails)
    {
        Instance.currentGameId = gameId;
        Instance.gameDetails = gameDetails;

        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    public static void StopGame()
    {
        Instance.StopAllCoroutines();

        Instance.currentGameId = null;
        Instance.gameDetails = null;

        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);        
    }

}
