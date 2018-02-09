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

        var isAuthenticated = ApiService.Instance.ReadAuthorization();

        if (isAuthenticated)
        {
            Action<AuthPlayerInfoResultModel> callback = (playerInfoResult) =>
            {
                UpdateAuthWidget();
            };

            StartCoroutine(ApiService.GetPlayerInfoAsync(callback));
        }
        else
        {
            UpdateAuthWidget();
        }

    }

    public void SignIn()
    {

        StartCoroutine(ApiService.SignInAsync((signInResult) =>
        {
            if (signInResult)
            {
                Action<AuthPlayerInfoResultModel> callback = (playerInfoResult) =>
                {
                    UpdateAuthWidget();
                    HideSigInPanel();
                };

                StartCoroutine(ApiService.GetPlayerInfoAsync(callback));
            }
        },
        signName.text, signPassword.text));
    }

    public void SignUp()
    {
        StartCoroutine(ApiService.SignUpAsync((signUpResult) =>
        {
            if (signUpResult)
            {
                SignIn();
            }
        },
        signName.text, signPassword.text));
    }

    public void SignOut()
    {
        StartCoroutine(ApiService.SignOutAsync((signOutResult) =>
        {
            if (signOutResult)
            {
                UpdateAuthWidget();
            }
        }));
    }

    public void UpdateAuthWidget()
    {
        var ifNotSignedIn = authWidget.transform.GetChild(0);
        var ifSignedIn = authWidget.transform.GetChild(1);

        if (!authWidget.activeInHierarchy)
        {
            authWidget.SetActive(true);
        }

        if (!ApiService.Instance.IsAuthenticated())
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
        signName.text = ApiService.Instance.PlayerInfo.PlayerName;
        sigInPanel.SetActive(true);
    }

    public void HideSigInPanel()
    {
        sigInPanel.SetActive(false);
        signName.text = null;
        signPassword.text = null;
    }
}
