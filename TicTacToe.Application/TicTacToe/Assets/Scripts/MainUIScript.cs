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
        SceneManager.LoadScene("GameField", LoadSceneMode.Single);
    }
}
