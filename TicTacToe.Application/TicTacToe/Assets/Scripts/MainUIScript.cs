using Assets.Scripts;
using Assets.Scripts.ApiModels;
using System;
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
    public UIActiveGameScript activeGameItem = null;

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
                string gameId = game.GameId;

                var item = GameObject.Instantiate(activeGameItem, gameListContent.transform);
                item.txtGameId.text = "Game #" + gameId.Substring(24);
                item.txtUsers.text = string.Format("{0}/{1}", game.JoinedUsers, game.MaxUsers);
                item.txtBombs.text = game.MinesQuantity.ToString();
                item.IsWithPassword.SetActive(game.IsWithPassword);
                item.transform.localScale = Vector3.one;
                
                item.me.onClick.AddListener(() => { ShowGameDetails(gameId); });
            }

            gameListPanel.SetActive(true);            
        };

        StartCoroutine(ApiService.FindGamesAsync(callback));        
    }

    public void ShowGameDetails(string gameId)
    {


        //SceneManager.LoadScene("GameField", LoadSceneMode.Single);
    }

    public void OnScroll(Vector2 value)
    {
        if (value.y > 1)
        {
            Debug.Log("Load: " + value.y);
        }
    }

    public void OnScroll2(float value)
    {
        Debug.Log("Load: " + value);
    }
}
