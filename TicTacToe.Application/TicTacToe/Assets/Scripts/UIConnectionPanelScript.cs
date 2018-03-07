using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConnectionPanelScript : MonoBehaviour
{
    public GameObject pnlConnecting;
    public GameObject pnlError;

    public void ShowConnecting()
    {
        Show();
        pnlError.SetActive(false);
        pnlConnecting.SetActive(true);
    }

    public void ShowError()
    {
        Show();
        pnlConnecting.SetActive(false);
        pnlError.SetActive(true);        
    }

    public void Show()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }
}
