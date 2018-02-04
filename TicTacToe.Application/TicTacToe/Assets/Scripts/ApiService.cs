using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using Assets.Scripts.ApiModels;

namespace Assets.Scripts
{
    public class ApiService : Singleton<ApiService>
    {
        public string APIEndpoint = "https://localhost:44319";

        private static KeyValuePair<string, string> cookieHeader;

        private static string jsonLastResult;

        public static UnityWebRequestAsyncOperation LoginAsync(string name, string password)  //api/auth
        {
            WWWForm formData = new WWWForm();
            formData.AddField("name", name);
            formData.AddField("password", password);

            using (UnityWebRequest www = UnityWebRequest.Post(GetFullUrl("/api/auth"), formData))
            {
                return www.SendWebRequest();
            }

            //if (www.isNetworkError || www.isHttpError)
            //{
            //    Debug.Log(www.error);
            //}
            //else
            //{
            //    if (www.responseCode == 200)
            //    {
            //        var headerName = "cookie";
            //        var headerValue = www.GetResponseHeader(headerName);
            //        cookieHeader = new KeyValuePair<string, string>(headerName, headerValue);
            //        return true;
            //    }
            //}

            //return false;
        }

        public static IEnumerator FindGamesAsync(string search = null)  //api/lobby
        {
            if (search != null)
            {
                search = "?search=" + search;
            }

            UnityWebRequest www = UnityWebRequest.Get(GetFullUrl("/api/lobby" + search));

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
                    var jsonLastResult = www.downloadHandler.text;

                    Debug.Log(jsonLastResult);

                    yield break;
                }

                Debug.LogWarning(www.responseCode);
            }

            yield break;
        }

        public static bool Login(string name, string password)  //api/auth
        {
            WWWForm formData = new WWWForm();
            formData.AddField("name", name);
            formData.AddField("password", "password");
            
            UnityWebRequest www = UnityWebRequest.Post(GetFullUrl("/api/auth"), formData);
            var response = www.SendWebRequest();
            
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.responseCode == 200)
                {
                    var headerName = "cookie";
                    var headerValue = www.GetResponseHeader(headerName);
                    cookieHeader = new KeyValuePair<string, string>(headerName, headerValue);
                    return true;
                }
            }
            
            return false;
        }

        public static IEnumerable<LobbyGameListItem> FindGames(string search = null)  //api/lobby
        {
            if (search != null)
            {
                search = "?search=" + search;
            }

            UnityWebRequest www = UnityWebRequest.Get(GetFullUrl("/api/lobby" + search));

            if (cookieHeader.Value != null)
            {
                www.SetRequestHeader(cookieHeader.Key, cookieHeader.Value);
            }

            www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.responseCode == 200)
                {
                    var jsonResult = www.downloadHandler.text;
                    Debug.Log(jsonResult);
                    var model = JsonUtility.FromJson<IEnumerable<LobbyGameListItem>>(jsonResult);

                    return model;
                }

                Debug.Log(www.responseCode);
            }

            return null;
        }

        public static IEnumerable<T> GetListResult<T>()
        {
            var result = JsonUtility.FromJson<ListApiResult<T>>(jsonLastResult);
            return result.result;
        }

        public static T GetEntityResult<T>()
        {
            var result = JsonUtility.FromJson<EntityApiResult<T>>(jsonLastResult);
            return result.result;
        }

        private static string GetFullUrl(string path)
        {            
            return Instance.APIEndpoint + path;
        }
    }
}
