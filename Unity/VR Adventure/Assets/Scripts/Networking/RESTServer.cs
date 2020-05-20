using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RESTServer : MonoBehaviour
{

    private string serverToken = "";
    private string serverSecret = "ThisIsAVerySecretServerSecretThisIsAVerySecretServerSecretThisIsAVerySecretServerSecretThisIsAVerySecretServerSecret";
    private readonly string _baseUrl = "http://212.10.51.254:30830/api/";


    public IEnumerator GetServerToken()
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

            }
        }
    }
}
