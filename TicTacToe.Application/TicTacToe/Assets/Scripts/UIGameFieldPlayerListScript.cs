using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameFieldPlayerListScript : MonoBehaviour
{

    public char[] playerSigns;
    public Color[] playerSignsColors;
    public Color currentTurnPlayerColor;
    public Color skipTurnPlayerColor;

    public UIGameFieldPlayerScript gameFieldPlayerPrefab;

    public Dictionary<string, UIGameFieldPlayerScript> dictGameFieldPlayers;


    // Use this for initialization
    void Start()
    {
        dictGameFieldPlayers = new Dictionary<string, UIGameFieldPlayerScript>();        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddPlayerToList(string playerId, byte sign, string playerName)
    {
        var item = Instantiate(gameFieldPlayerPrefab, this.transform);
        item.name = "player_" + playerId;
        item.txtSign.text = playerSigns[sign].ToString();
        item.txtSign.color = playerSignsColors[sign];
        item.txtPlayerName.text = playerName;
        item.txtPoints.text = "";

        dictGameFieldPlayers.Add(playerId, item);
    }

    public void ClearPlayerList()
    {
        dictGameFieldPlayers.Clear();

        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }
    }

    public void ShowCurrentTurnPlayer(string playerId)
    {
        foreach (var item in dictGameFieldPlayers)
        {
            var player = item.Value;

            if (item.Key == playerId)
            {
                player.txtPlayerNameOutline.effectColor = Color.clear;
            }
            else
            {
                player.txtPlayerNameOutline.effectColor = Color.yellow;
            }
        }
    }

    public void SetPlayerPoints(string playerId, int points)
    {
        dictGameFieldPlayers[playerId].txtPoints.text = points.ToString();
    }
}
