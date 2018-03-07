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
        public string APIEndpoint = "http://tictactoe.me";

        public const string COOKIE_HEADER_NAME = "cookie";
        public const string SETCOOKIE_HEADER_NAME = "set-cookie";

        private string authInfoFile;         

        public GameAuthModel PlayerInfo { get; private set; }

        private void Awake()
        {
            authInfoFile = Application.persistentDataPath + "/auth.json";

            var readAuthResult = TryReadAuthorization();   //Try to read saved auth data

            if (!readAuthResult)
            {
                PlayerInfo = new GameAuthModel();
            }            
        }

        #region Auth
        public static IEnumerator GetPlayerInfoAsync(Action<AuthPlayerInfoResultModel> callback)  //api/auth/info
        {
            using (UnityWebRequest www = UnityWebRequest.Get(GetFullUrl("/api/auth/info")))
            {
                if (Instance.IsAuthenticated())
                {
                    www.SetRequestHeader(COOKIE_HEADER_NAME, Instance.PlayerInfo.AuthCookie);
                }               

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    if (callback != null)
                    {
                        callback(null);
                    }

                    Debug.LogWarning(www.responseCode);
                }
                else
                {
                    var result = GetEntityResult<AuthPlayerInfoResultModel>(www.downloadHandler.text);

                    if (result.PlayerId == null)
                    {
                        Instance.PlayerInfo.AuthCookie = null;
                        Instance.PlayerInfo.PlayerName = null;
                    }
                    else
                    {
                        Instance.PlayerInfo.PlayerId = result.PlayerId;
                        Instance.PlayerInfo.PlayerName = result.PlayerName;
                    }                    

                    Instance.SaveAuthorization();

                    if (callback != null)
                    {
                        callback(result);
                    }
                }
            }

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
                    Debug.LogWarning(www.url + "\r\n" + www.error);

                    if (callback != null)
                    {
                        callback(false);
                    }
                }
                else
                {
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
        #endregion

        #region Lobby        

        public static IEnumerator FindGamesAsync(Action<IEnumerable<LobbyGameListItem>> callback, string search = null)  //api/lobby
        {
            if (search != null)
            {
                search = "?search=" + search;
            }

            using (UnityWebRequest www = UnityWebRequest.Get(GetFullUrl("/api/lobby" + search)))
            {
                if (Instance.IsAuthenticated())
                {
                    www.SetRequestHeader(COOKIE_HEADER_NAME, Instance.PlayerInfo.AuthCookie);
                }

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogWarning(www.url + "\r\n" + www.error);

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

            using (UnityWebRequest www = UnityWebRequest.Get(GetFullUrl("/api/lobby/details/" + gameId)))
            {
                if (Instance.IsAuthenticated())
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

                    if (callback != null)
                    {
                        callback(result);
                    }
                }

            }
        }

        public static IEnumerator InitGameAsync(Action<string> callback)  //api/lobby/init
        {

            using (UnityWebRequest www = UnityWebRequest.Post(GetFullUrl("/api/lobby/init"), string.Empty))
            {
                if (Instance.IsAuthenticated())
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
                    var gameId = JsonConvert.DeserializeObject<string>(www.downloadHandler.text);

                    if (callback != null)
                    {
                        callback(gameId);
                    }
                }

            }
        }

        public static IEnumerator JoinGameAsync(Action<bool> callback, string gameId/*, string password = null*/)  //api/lobby/join/{roomid}
        {

            using (UnityWebRequest www = UnityWebRequest.Post(GetFullUrl("/api/lobby/join/" + gameId), string.Empty))
            {
                if (Instance.IsAuthenticated())
                {
                    www.SetRequestHeader(COOKIE_HEADER_NAME, Instance.PlayerInfo.AuthCookie);
                }

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

        public static IEnumerator KickPlayerAsync(Action<bool> callback, string gameId, string playerId)  //api/lobby/kick/{roomid}/{playerid}
        {

            using (UnityWebRequest www = UnityWebRequest.Post(GetFullUrl("/api/lobby/kick/" + gameId + "/" + playerId), string.Empty))
            {
                if (Instance.IsAuthenticated())
                {
                    www.SetRequestHeader(COOKIE_HEADER_NAME, Instance.PlayerInfo.AuthCookie);
                }

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

        public static IEnumerator LeaveGameAsync(Action<bool> callback)  //api/lobby/leave
        {

            using (UnityWebRequest www = UnityWebRequest.Post(GetFullUrl("/api/lobby/leave"), string.Empty))
            {
                if (Instance.IsAuthenticated())
                {
                    www.SetRequestHeader(COOKIE_HEADER_NAME, Instance.PlayerInfo.AuthCookie);
                }

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

        public static IEnumerator SetReadyAsync(Action<bool> callback, string gameId)  //api/lobby/ready/{roomid}
        {

            using (UnityWebRequest www = UnityWebRequest.Post(GetFullUrl("/api/lobby/ready/" + gameId), string.Empty))
            {
                if (Instance.IsAuthenticated())
                {
                    www.SetRequestHeader(COOKIE_HEADER_NAME, Instance.PlayerInfo.AuthCookie);
                }

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


        #endregion

        #region Game
        public static IEnumerator StartGameAsync(Action<bool> callback, string gameId)  //api/game/start/{roomid}
        {

            using (UnityWebRequest www = UnityWebRequest.Post(GetFullUrl("/api/game/start/" + gameId), string.Empty))
            {
                if (Instance.IsAuthenticated())
                {
                    www.SetRequestHeader(COOKIE_HEADER_NAME, Instance.PlayerInfo.AuthCookie);
                }

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

        public static IEnumerator GetGameStateAsync(Action<GameStateModel> callback, string gameId)  //api/game/state/{roomid}
        {

            using (UnityWebRequest www = UnityWebRequest.Get(GetFullUrl("/api/game/state/" + gameId)))
            {
                if (Instance.IsAuthenticated())
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
                    var result = GetEntityResult<GameStateModel>(www.downloadHandler.text);

                    if (callback != null)
                    {
                        callback(result);
                    }
                }

            }
        }
        #endregion

        #region Utilities
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
        #endregion


        public bool TryReadAuthorization()
        {
            if (!File.Exists(authInfoFile)) { return false; }

            var json = File.ReadAllText(authInfoFile);

            PlayerInfo = JsonConvert.DeserializeObject<GameAuthModel>(json);

            return true;
        }

        public bool IsAuthenticated()
        {
            return (PlayerInfo != null && PlayerInfo.AuthCookie != null);
        }
    }
}
