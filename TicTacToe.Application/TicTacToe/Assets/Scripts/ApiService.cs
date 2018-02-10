using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using Assets.Scripts.ApiModels;
using Newtonsoft.Json;
using System.IO;

namespace Assets.Scripts
{
    public class ApiService : Singleton<ApiService>
    {
        public string APIEndpoint = "https://localhost:44319";

        public const string COOKIE_HEADER_NAME = "cookie";
        public const string SETCOOKIE_HEADER_NAME = "set-cookie";

        private string authInfoFile;         

        public GameAuthModel PlayerInfo { get; private set; }

        private void Awake()
        {
            authInfoFile = Application.persistentDataPath + "/auth.json";
        }

        public static IEnumerator SignInAsync(Action<bool> callback, string name, string password)  //api/auth/signin
        {
            WWWForm formData = new WWWForm();
            formData.AddField("name", name);
            formData.AddField("password", password);

            using (UnityWebRequest www = UnityWebRequest.Post(GetFullUrl("/api/auth/signin"), formData))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogWarning(www.error);

                    if (callback != null)
                    {
                        callback(false);
                    }
                }
                else
                {
                    if (Instance.PlayerInfo == null)
                    {
                        Instance.PlayerInfo = new GameAuthModel();
                    }

                    var cookieValue = www.GetResponseHeader(SETCOOKIE_HEADER_NAME);

                    Instance.PlayerInfo.AuthCookie = cookieValue.Split(';')[0];

                    Debug.Log(ApiService.Instance.PlayerInfo.AuthCookie);

                    Instance.SaveAuthorization();

                    if (callback != null)
                    {
                        callback(true);
                    }
                }
            }

        }

        public static IEnumerator SignUpAsync(Action<bool> callback, string name, string password)  //api/auth/signup
        {
            WWWForm formData = new WWWForm();
            formData.AddField("name", name);
            formData.AddField("password", password);

            using (UnityWebRequest www = UnityWebRequest.Post(GetFullUrl("/api/auth/signup"), formData))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogWarning(www.error);

                    if (callback != null)
                    {
                        callback(false);
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        callback(true);
                    }
                }
            }

        }

        public static IEnumerator SignOutAsync(Action<bool> callback)  //api/auth/signout
        {
            using (UnityWebRequest www = UnityWebRequest.Post(GetFullUrl("/api/auth/signout"), string.Empty))
            {
                www.SetRequestHeader(COOKIE_HEADER_NAME, Instance.PlayerInfo.AuthCookie);

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogWarning(www.error);

                    if (callback != null)
                    {
                        callback(false);
                    }
                }
                else
                {
                    if (Instance.PlayerInfo != null)
                    {
                        Instance.PlayerInfo.AuthCookie = null;

                        Instance.SaveAuthorization();
                    }                   

                    if (callback != null)
                    {
                        callback(true);
                    }
                }
            }

        }


        public static IEnumerator GetPlayerInfoAsync(Action<AuthPlayerInfoResultModel> callback)  //api/auth/info
        {
            if (Instance.PlayerInfo == null)
            {
                if (callback != null)
                {
                    callback(null);
                }

                yield break;
            }

            using (UnityWebRequest www = UnityWebRequest.Get(GetFullUrl("/api/auth/info")))
            {
                www.SetRequestHeader(COOKIE_HEADER_NAME, Instance.PlayerInfo.AuthCookie);

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    if (callback != null)
                    {
                        callback(null);
                    }

                    Instance.PlayerInfo = null;

                    Debug.LogWarning(www.responseCode);
                }
                else
                {
                    var result = GetEntityResult<AuthPlayerInfoResultModel>(www.downloadHandler.text);

                    Instance.PlayerInfo.PlayerId = result.PlayerId;
                    Instance.PlayerInfo.PlayerName = result.PlayerName;

                    Instance.SaveAuthorization();

                    if (callback != null)
                    {
                        callback(result);
                    }
                }
            }

        }

        public static IEnumerator FindGamesAsync(Action<IEnumerable<LobbyGameListItem>> callback, string search = null)  //api/lobby
        {
            if (search != null)
            {
                search = "?search=" + search;
            }

            using (UnityWebRequest www = UnityWebRequest.Get(GetFullUrl("/api/lobby" + search)))
            {
                if (Instance.PlayerInfo != null)
                {
                    www.SetRequestHeader(COOKIE_HEADER_NAME, Instance.PlayerInfo.AuthCookie);
                }

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogWarning(www.error);

                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    var result = GetListResult<LobbyGameListItem>(www.downloadHandler.text);

                    if (callback != null)
                    {
                        callback(result);
                    }
                }

            }

            yield break;
        }

        public static IEnumerator GetGameDetailsAsync(Action<LobbyGameDetailsModel> callback, string gameId/*, string password = null*/)  //api/lobby/details/{roomid}
        {

            using (UnityWebRequest www = UnityWebRequest.Get(GetFullUrl("api/lobby/details/" + gameId)))
            {
                if (Instance.PlayerInfo != null)
                {
                    www.SetRequestHeader(COOKIE_HEADER_NAME, Instance.PlayerInfo.AuthCookie);
                }

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogWarning(www.error);

                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    var result = GetEntityResult<LobbyGameDetailsModel>(www.downloadHandler.text);

                    Debug.Log(www.downloadHandler.text);

                    if (callback != null)
                    {
                        callback(result);
                    }
                }

            }
        }


        private static IEnumerable<T> GetListResult<T>(string json)
        {
            var result = JsonConvert.DeserializeObject<ListApiResult<T>>(json);
            return result.result;
        }

        private static T GetEntityResult<T>(string json)
        {
            var result = JsonConvert.DeserializeObject<EntityApiResult<T>>(json);
            return result.result;
        }

        private static string GetFullUrl(string path)
        {            
            return Instance.APIEndpoint + path;
        }

        private void SaveAuthorization()
        {
            var json = JsonConvert.SerializeObject(Instance.PlayerInfo);

            File.WriteAllText(authInfoFile, json);
        }

        public bool ReadAuthorization()
        {
            if (!File.Exists(authInfoFile)) { return false; }

            var json = File.ReadAllText(authInfoFile);

            PlayerInfo = JsonConvert.DeserializeObject<GameAuthModel>(json);

            return IsAuthenticated();
        }

        public bool IsAuthenticated()
        {
            return (PlayerInfo != null && PlayerInfo.AuthCookie != null);
        }
    }
}
