using Assets.Scripts;
using Assets.Scripts.ApiModels;
using System;
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

    public InputField signName;
    public InputField signPassword;

    private void Start()
    {       

        Action<AuthPlayerInfoResultModel> callback = (x) => {
            playerName.text = x.PlayerName;
            ShowAuthWidget();
        };

        StartCoroutine(Assets.Scripts.ApiService.GetPlayerInfoAsync(callback));
        
    }

    public void SignIn()
    {        

        StartCoroutine(Assets.Scripts.ApiService.LoginAsync((x) => { if (x) { sigInPanel.SetActive(false); } }, signName.text, signPassword.text));

        Action<AuthPlayerInfoResultModel> callback = (x) => {            
            ShowAuthWidget();
        };

        StartCoroutine(Assets.Scripts.ApiService.GetPlayerInfoAsync(callback));
    }

    public void SignUp()
    {
        throw new System.NotImplementedException("SignOut");
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

        if(ApiService.Instance.PlayerInfo == null)
        {
            playerName.text = null;
            ifSignedIn.gameObject.SetActive(false);
            ifNotSignedIn.gameObject.SetActive(true);
        }
        else
        {            
            playerName.text = ApiService.Instance.PlayerInfo.PlayerName;
            ifSignedIn.gameObject.SetActive(true);
            ifNotSignedIn.gameObject.SetActive(false);
        }


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
