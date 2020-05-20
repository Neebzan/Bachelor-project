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
    public event Action TokenVerificationDone;

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
        if(verified)
        {
            JwtSecurityToken token = new JwtSecurityToken(accessToken);
            Client.instance.UserName = token.Subject;
        }
        Verified = verified;
        TokenVerificationDone?.Invoke();
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

            if (webRequest.responseCode == 200)
                callback(true);
            else
                callback(false);
        }
    }


}
