using Assets.Scripts;
using Assets.Scripts.ApiModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUIScript : MonoBehaviour
{
    public GameObject mainPanel = null;
    public GameObject gameListPanel = null;
    public GameObject gameListContent = null;
    public Button gameListButton = null;

    private void Start()
    {
        mainPanel.SetActive(true);
    }

    public void ClickExit()
    {
        Application.Quit();
    }

    public void ClickFindGames()
    {
        System.Action<IEnumerable<LobbyGameListItem>> callback = (gameList) => {
            mainPanel.SetActive(false);            

            foreach (Transform child in gameListContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            foreach (var game in gameList)
            {
                var newButton = GameObject.Instantiate(gameListButton, gameListContent.transform);
                newButton.name = game.GameId;
                newButton.transform.localScale = Vector3.one;
            }

            gameListPanel.SetActive(true);
        };

        StartCoroutine(ApiService.FindGamesAsync(callback));

        //SceneManager.LoadScene("GameField", LoadSceneMode.Single);
    }
}
