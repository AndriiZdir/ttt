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

    public void UpdatePlayerInformation(string playerId, int? points = null, bool? isCurrentTurn = null, bool? isSkiping = null)
    {
        var player = dictGameFieldPlayers[playerId];

        if (isCurrentTurn.HasValue)
        {
            player.txtPlayerNameOutline.effectColor = isCurrentTurn.Value ? currentTurnPlayerColor : Color.clear;
        }

        if (isSkiping.HasValue)
        {
            player.txtPlayerName.color = isSkiping.Value ? skipTurnPlayerColor : Color.white;
        }

        if (points.HasValue)
        {
            player.txtPoints.text = points.Value.ToString();
        }
    }

}
