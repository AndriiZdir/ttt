using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AuthorizationScript : MonoBehaviour
{
    public Text playerName;
    public GameObject authWidget;
    public GameObject sigInPanel;

    private void Start()
    {
        Assets.Scripts.ApiService.
        ShowAuthWidget();        
    }

    public void SignOut()
    {
        throw new System.NotImplementedException("SignOut");
    }

    public void ShowAuthWidget()
    {
        var ifNotSignedIn = authWidget.transform.GetChild(0);
        var ifSignedIn = authWidget.transform.GetChild(1);

        if (!authWidget.activeInHierarchy)
        {
            authWidget.SetActive(true);
        }

        ifSignedIn.gameObject.SetActive(false);
        ifNotSignedIn.gameObject.SetActive(true);
    }

    public void ShowSigInPanel()
    {
        sigInPanel.SetActive(true);
    }

    public void HideSigInPanel()
    {
        sigInPanel.SetActive(false);
    }
}
