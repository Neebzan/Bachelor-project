using DatabaseREST.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Canvas))]
public class MainMenu : MonoBehaviour {
    private Canvas _canvas;

    public List<GameObject> MenuPanels = new List<GameObject>();

    public GameObject [ ] HighscoreElements = new GameObject [ 10 ];

    private readonly string refreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBYmJvdHQ0NDM5U2NvdCIsImF1ZCI6InJlZnJlc2giLCJuYmYiOjE1ODg5MjYzMjYsImV4cCI6MTU5ODA4NDcyNiwiaWF0IjoxNTg4OTI2MzI2fQ.5RMfdQIWmAGMnRBSYgL43dhNWVuDqEo0a56yYCHaw14";
    private readonly string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBYmJvdHQ0NDM5U2NvdCIsImF1ZCI6ImFjY2VzcyIsIm5iZiI6MTU4ODkyNjMyNiwiZXhwIjoxNTk2ODc4NzI2LCJpYXQiOjE1ODg5MjYzMjZ9.LGSmOhSHO_JF_710ZL9zToXhg6niStyV7l5LRUTp2bI";

    private readonly string _baseUrl = "http://212.10.51.254:30830/api/";


    private void Awake () {
        _canvas = GetComponent<Canvas>();
        StartCoroutine(GetHighscores(accessToken, "Neebz"));
        NavigateTo(MenuPanels [ 1 ]);
    }

    public void PlayGame () {
        Debug.Log("Play game pressed");
    }

    public void NavigateTo (GameObject navigateToPanel) {
        foreach (GameObject panel in MenuPanels) {
            if (navigateToPanel == panel)
                panel.SetActive(true);
            else
                panel.SetActive(false);
        }
    }

    IEnumerator GetHighscores (string token, string id = "", int perPage = 10) {
        string url = _baseUrl + "highscore/xp/?perPage=" + perPage;

        if (id != "")
            url += "&id=" + id;


        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            webRequest.SetRequestHeader("token", token);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError) {
                Debug.Log(webRequest.error);
            }

            if (webRequest.isDone) {
                string json = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
                Debug.Log("JSON: " + json);
                List<HighscoreEntry> highscores = JsonConvert.DeserializeObject<List<HighscoreEntry>>(json);

                for (int i = 0; i < highscores.Count; i++) {
                    HighscoreElement element = HighscoreElements [ i ].GetComponent<HighscoreElement>();
                    element.Placement.text = highscores [ i ].Rank.ToString();
                    element.Username.text = highscores [ i ].Player.PlayerId;
                    element.Score.text = highscores [ i ].Player.Experience.ToString();
                    if (!String.IsNullOrEmpty(id) && highscores [ i ].Player.PlayerId == id) {
                        element.BackgroundImage.color = new Color(.9f,.82f,.4f);
                    }
                }
            }
        }
    }
}


