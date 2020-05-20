using DatabaseREST.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RESTClient : MonoBehaviour
{
    private string refreshToken = "";
    private string accessToken = "";

    private readonly string _baseUrl = "http://212.10.51.254:30830/api/";

    public bool Verified = false;
    public event Action OnTokenVerificationDone;
    public event Action OnNewAccessTokenFailed;


    private void Awake()
    {
        GetCommandlineTokens();
        StartCoroutine(VerifyToken(accessToken, ((bool result) => VerifyCallback(result))));
    }

    public void GetCommandlineTokens()
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-rt")
                refreshToken = args[i + 1];
            else if (args[i] == "-at")
                accessToken = args[i + 1];
        }
    }

    private void VerifyCallback(bool verified)
    {
        if (verified)
        {
            JwtSecurityToken token = new JwtSecurityToken(accessToken);
            Client.instance.Username = token.Subject;
        }
        Verified = verified;
        OnTokenVerificationDone?.Invoke();
    }

    public IEnumerator VerifyToken(string token, Action<bool> callback)
    {
        string url = _baseUrl + "token/verify";
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, ""))
        {
            webRequest.SetRequestHeader("token", token);

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
                callback(false);
            }
            if (webRequest.isDone)
            {
                if (webRequest.responseCode == 200)
                    callback(true);
                else
                    callback(false);
            }
        }
        callback(false);
    }

    public IEnumerator RequestAccessToken(IEnumerator retryCallback = null)
    {
        string url = _baseUrl + "token";
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, ""))
        {
            webRequest.SetRequestHeader("token", refreshToken);

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
            }

            if (webRequest.isDone)
            {
                if (webRequest.responseCode == 200)
                {
                    string json = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
                    TokenModel tokens = JsonConvert.DeserializeObject<TokenModel>(json);
                    accessToken = tokens.AccessToken;
                    if (retryCallback != null)
                        yield return retryCallback;
                }
                else
                    OnNewAccessTokenFailed?.Invoke();
            }
        }
    }

    public IEnumerator GetPlayerInfo()
    {
        string url = _baseUrl + "players";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("token", accessToken);

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
            }

            if (webRequest.isDone)
            {
                if (webRequest.responseCode == 200)
                {
                    string json = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
                    Players player = JsonConvert.DeserializeObject<Players>(json);
                    Client.instance.PlayerInfo = player;
                }
                else if(webRequest.responseCode == 401)
                {
                    yield return RequestAccessToken(GetPlayerInfo());
                }
            }
        }
    }

}
