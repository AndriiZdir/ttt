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

namespace Assets.Scripts
{
    public class ApiService : Singleton<ApiService>
    {
        public string APIEndpoint = "https://localhost:44319";

        private static KeyValuePair<string, string> cookieHeader;

        public static IEnumerator LoginAsync(Action<IEnumerable<LobbyGameListItem>> callback, string name, string password)  //api/auth
        {
            WWWForm formData = new WWWForm();
            formData.AddField("name", name);
            formData.AddField("password", password);

            using (UnityWebRequest www = UnityWebRequest.Post(GetFullUrl("/api/auth"), formData))
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

        }

        public static IEnumerator FindGamesAsync(Action<IEnumerable<LobbyGameListItem>> callback, string search = null)  //api/lobby
        {
            if (search != null)
            {
                search = "?search=" + search;
            }

            using (UnityWebRequest www = UnityWebRequest.Get(GetFullUrl("/api/lobby" + search)))
            {
                if (cookieHeader.Value != null)
                {
                    www.SetRequestHeader(cookieHeader.Key, cookieHeader.Value);
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
    }
}
