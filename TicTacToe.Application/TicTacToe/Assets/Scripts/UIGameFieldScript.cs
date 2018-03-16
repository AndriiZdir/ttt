using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameFieldScript : MonoBehaviour
{
    [Header("Player List")]
    public GameObject pnlPlayerList;
    public char[] playerSigns;
    public Color[] playerSignsColors;
    public Color currentTurnPlayerColor;
    public Color skipTurnPlayerColor;
    public UIGameFieldPlayerScript gameFieldPlayerPrefab;

    public Dictionary<string, UIGameFieldPlayerScript> dictGameFieldPlayers;

    [Header("Bonus Panel")]
    public Image btnMine;
    public Text txtMineCount;
    public Color defaultMineButtonColor;
    public Color activeMineButtonColor;

    void Awake()
    {
        dictGameFieldPlayers = new Dictionary<string, UIGameFieldPlayerScript>();        
    }

    #region Player List
    public void AddPlayerToList(string playerId, byte sign, string playerName)
    {
        var item = Instantiate(gameFieldPlayerPrefab, pnlPlayerList.transform);
        item.name = "player_" + playerId;
        item.txtSign.text = playerSigns[sign].ToString();
        item.txtSign.color = playerSignsColors[sign];
        item.txtPlayerName.text = playerName;
        item.txtPoints.text = "";
        item.playerSign = sign;

        dictGameFieldPlayers.Add(playerId, item);
    }

    public void ClearPlayerList()
    {
        dictGameFieldPlayers.Clear();

        foreach (Transform item in pnlPlayerList.transform)
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
    #endregion

    #region Mine Button
    public void UpdateMineButton(bool? isActive = null, bool? isVisible = null, int? mineCount = null)
    {
        if (isVisible.HasValue)
        {
            btnMine.gameObject.SetActive(isVisible.Value);
        }

        if (mineCount.HasValue)
        {
            txtMineCount.text = mineCount.ToString();
        }

        if (isActive.HasValue)
        {
            btnMine.color = isActive.Value ? activeMineButtonColor : defaultMineButtonColor;
        }
    }
    #endregion

}
