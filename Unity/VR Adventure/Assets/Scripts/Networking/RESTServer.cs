using DatabaseREST.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class RESTServer : MonoBehaviour
{

    private string serverToken = "";
    private string serverSecret = "ThisIsAVerySecretServerSecretThisIsAVerySecretServerSecretThisIsAVerySecretServerSecretThisIsAVerySecretServerSecret";
    private readonly string _baseUrl = "http://212.10.51.254:30830/api/";
    //private readonly string _baseUrl = "https://localhost:5001/api/";


    public event Action<bool> OnGetServerTokenDone;

    public void GetServerToken()
    {
        StartCoroutine(StartGetServerToken());
    }
    private IEnumerator StartGetServerToken()
    {
        string url = _baseUrl + "token/server";

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, ""))
        {
            webRequest.SetRequestHeader("serverSecret", serverSecret);

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
            }
            if (webRequest.isDone)
            {
                if (webRequest.responseCode == 200)
                {
                    serverToken = Encoding.Default.GetString(webRequest.downloadHandler.data);
                    OnGetServerTokenDone?.Invoke(true);
                    Console.WriteLine("Server token successfully received!");

                    //Test
                    //PlayedMatch t1 = new PlayedMatch()
                    //{
                    //    Score = 99,
                    //    PlayerId = "Abbotts996Harvey"
                    //};

                    //PlayedMatch t2 = new PlayedMatch()
                    //{
                    //    Score = 250,
                    //    PlayerId = "Abie186Barbra"
                    //};

                    //Matches match = new Matches()
                    //{
                    //    Begun = DateTime.UtcNow,
                    //    Difficulty = 52,
                    //    Ended = DateTime.UtcNow.AddMinutes(5),
                    //    MapName = "map1"
                    //};
                    //match.PlayedMatch.Add(t1);
                    //match.PlayedMatch.Add(t2);
                    //UploadMatch(match);

                }
                else
                {
                    OnGetServerTokenDone?.Invoke(false);
                    Console.WriteLine("Server token could NOT be received!");
                }
            }
        }
    }

    public void UploadMatch(Matches match)
    {
        StartCoroutine(StartUploadMatchResult(match));
    }

    IEnumerator StartUploadMatchResult(Matches match)
    {
        string url = _baseUrl + "matches";

        string json = JsonConvert.SerializeObject(match);

        WWWForm form = new WWWForm();
        form.AddField("match", json, Encoding.Default);
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("match", json));


        using (UnityWebRequest webRequest = new UnityWebRequest(url))
        {
            webRequest.method = "POST";
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            webRequest.uploadHandler.contentType = "application/json";
            //webRequest.uploadHandler.data = Encoding.ASCII.GetBytes(json);

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("serverToken", serverToken);
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
            }
            else if (webRequest.isDone)
            {
                if (webRequest.responseCode == 201)
                {
                    Console.WriteLine("Match was uploaded!");
                }
                else if (webRequest.responseCode == 401)
                {
                    Console.WriteLine("Unauthorized token!");
                }
                else
                {
                    Console.WriteLine("Something else went wrong when uploading match!");
                }
            }
        }
    }

    //public IEnumerator GetMaps()
    //{
    //    string url = _baseUrl + "token/server";

    //    using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
    //    {
    //        yield return webRequest.SendWebRequest();
    //    }
    //}
}
