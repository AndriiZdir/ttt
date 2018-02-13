using Assets.Scripts;
using Assets.Scripts.ApiModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameDetailsScript : MonoBehaviour
{
    public string gameId;
    public bool IsCreatedByMe;
    public float UpdateRate = 2f;

    public Text txtGameId;
    //public Text txtUsers;
    //public Text txtBombs;
    //public GameObject IsWithPassword;

    public RectTransform gamePlayersContent;
    public UIGamePlayerScript gamePlayerPrefab;

    public Button btnReady;
    public Button btnStart;
    public Button btnJoin;
    public Button btnLeave;

    private string myPlayerId;

    private void OnEnable()
    {
        txtGameId.text = "Loading...";
        myPlayerId = ApiService.Instance.PlayerInfo.PlayerId;

        foreach (Transform child in gamePlayersContent.transform)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(UpdateCoroutine());
    }

    private void OnDisable()
    {
        gameId = null;        
    }

    IEnumerator UpdateCoroutine()
    {
        while (gameId != null)
        {
            StartCoroutine(ApiService.GetGameDetailsAsync(GameDetailsCallback, gameId));

            yield return new WaitForSeconds(UpdateRate);
        }
    }

    private void GameDetailsCallback(LobbyGameDetailsModel model)
    {
        bool isMeJoined = false;
        bool isMeReady = false;
        bool isAllReady = true;

        if (model.GameState == GameRoomState.Closed)
        {
            OnBtnLeave();
            return;
        }

        if (model.GameState == GameRoomState.Started)
        {
            if (isMeJoined && isMeReady)
            {
                Debug.Log("Load game field scene.");
            }
            return;
        }

        foreach (Transform child in gamePlayersContent.transform)
        {
            Destroy(child.gameObject);
        }
        
        txtGameId.text = model.GameState + " game #" + model.GameId.Substring(24);

        isAllReady = model.JoinedPlayers >= 2;

        foreach (var gamePlayer in model.Players)
        {
            var item = GameObject.Instantiate(gamePlayerPrefab, gamePlayersContent.transform);
            item.transform.localScale = Vector3.one;

            item.txtPlayerName.text = gamePlayer.PlayerName;
            item.playerSign.sprite = item.playerSigns[gamePlayer.Sign];

            item.readyStatusObject.SetActive(gamePlayer.IsReady);
            item.waitingStatusObject.SetActive(!gamePlayer.IsReady);

            if (!isMeJoined 
                && gamePlayer.PlayerId == myPlayerId)
            {
                isMeJoined = true;
            }

            if (!isMeReady 
                && isMeJoined
                && gamePlayer.PlayerId == myPlayerId 
                && gamePlayer.IsReady)
            {
                isMeReady = true;
            }

            isAllReady = isAllReady && gamePlayer.IsReady;
        }

        btnStart.gameObject.SetActive(model.CreatedBy == myPlayerId);
        btnStart.interactable = isAllReady;
        btnReady.gameObject.SetActive(!isMeReady && isMeJoined);
        btnJoin.gameObject.SetActive(!isMeJoined);
        btnLeave.gameObject.SetActive(isMeJoined);
    }

    public void OnBtnReady()
    {

    }

    public void OnBtnStart()
    {

    }

    public void OnBtnJoin()
    {

    }

    public void OnBtnLeave()
    {
        gameObject.SetActive(false);
    }
}
