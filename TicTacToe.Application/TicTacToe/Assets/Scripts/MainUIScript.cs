using Assets.Scripts.ApiModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIScript : MonoBehaviour
{
    public void ClickExit()
    {
        Application.Quit();
    }

    public void ClickFindGames()
    {
        StartCoroutine(Assets.Scripts.ApiService.FindGamesAsync());

        var result = Assets.Scripts.ApiService.GetListResult<LobbyGameListItem>();

        Debug.Log(result);

        //SceneManager.LoadScene("GameField", LoadSceneMode.Single);
    }
}
