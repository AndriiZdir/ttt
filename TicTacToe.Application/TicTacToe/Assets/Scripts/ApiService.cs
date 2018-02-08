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
        private static readonly string authInfoFile = Application.persistentDataPath + "/auth.json";

        public static GameAuthModel PlayerInfo { get; private set; }

        public static IEnumerator LoginAsync(Action<bool> callback, string name, string password)  //api/auth/signin
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
                }
                else
                {
                    if (www.responseCode == 200)
                    {                       

                        if (PlayerInfo == null)
                        {
                            PlayerInfo = new GameAuthModel();
                        }

                        PlayerInfo.AuthCookie = www.GetResponseHeader(COOKIE_HEADER_NAME);

                        if (callback != null)
                        {
                            callback(true);
                        }
                    }
                    else
                    {
                        if (callback != null)
                        {
                            callback(false);
                        }

                        Debug.LogWarning(www.responseCode);
                    }
                }
            }

        }

        public static IEnumerator GetPlayerInfoAsync(Action<AuthPlayerInfoResultModel> callback)  //api/auth/info
        {
            ReadAuthorization();

            if (PlayerInfo == null)
            {
                if (callback != null)
                {
                    callback(null);
                }

                yield break;
            }

            using (UnityWebRequest www = UnityWebRequest.Get(GetFullUrl("/api/auth/info")))
            {
                www.SetRequestHeader(COOKIE_HEADER_NAME, PlayerInfo.AuthCookie);

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogWarning(www.error);
                }
                else
                {
                    if (www.responseCode == 200)
                    {
                        var result = GetEntityResult<AuthPlayerInfoResultModel>(www.downloadHandler.text);

                        PlayerInfo.PlayerId = result.PlayerId;
                        PlayerInfo.PlayerName = result.PlayerName;

                        SaveAuthorization();

                        if (callback != null)
                        {
                            callback(result);
                        }
                    }
                    else
                    {
                        if (callback != null)
                        {
                            callback(null);
                        }

                        PlayerInfo = null;

                        Debug.LogWarning(www.responseCode);
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
                if (PlayerInfo != null)
                {
                    www.SetRequestHeader(COOKIE_HEADER_NAME, PlayerInfo.AuthCookie);
                }

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogWarning(www.error);
                }
                else
                {
                    if (www.responseCode == 200)
                    {

                        var result = GetListResult<LobbyGameListItem>(www.downloadHandler.text);

                        if (callback != null)
                        {
                            callback(result);
                        }

                        yield break;
                    }

                    Debug.LogWarning(www.responseCode);
                }
            }

            yield break;
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

        private static void SaveAuthorization()
        {
            var json = JsonConvert.SerializeObject(PlayerInfo);

            File.WriteAllText(authInfoFile, json);
        }

        public static bool ReadAuthorization()
        {
            var json = File.ReadAllText(authInfoFile);

            PlayerInfo = JsonConvert.DeserializeObject<GameAuthModel>(json);

            return (PlayerInfo != null && PlayerInfo.AuthCookie != null);
        }
    }
}
