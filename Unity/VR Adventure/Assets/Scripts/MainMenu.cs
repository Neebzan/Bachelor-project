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

    public GameObject HighscoreElementPrefab;
    public GameObject HighscorePanel;

    private List<Highscore> highscores = new List<Highscore>();
    private readonly string refreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBYmJvdHQ0NDM5U2NvdCIsImF1ZCI6InJlZnJlc2giLCJuYmYiOjE1ODg5MjYzMjYsImV4cCI6MTU5ODA4NDcyNiwiaWF0IjoxNTg4OTI2MzI2fQ.5RMfdQIWmAGMnRBSYgL43dhNWVuDqEo0a56yYCHaw14";
    private readonly string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBYmJvdHQ0NDM5U2NvdCIsImF1ZCI6ImFjY2VzcyIsIm5iZiI6MTU4ODkyNjMyNiwiZXhwIjoxNTk2ODc4NzI2LCJpYXQiOjE1ODg5MjYzMjZ9.LGSmOhSHO_JF_710ZL9zToXhg6niStyV7l5LRUTp2bI";

    private readonly string _baseUrl = "http://212.10.51.254:30830/api/";



    private void Awake () {
        _canvas = GetComponent<Canvas>();

        for (int i = 0; i < 10; i++) {
            highscores.Add(new Highscore());
        }

        highscores = highscores.OrderByDescending(x => x.score).ToList();

        for (int i = 0; i < highscores.Count; i++) {
            GameObject element = GameObject.Instantiate(HighscoreElementPrefab, HighscorePanel.transform);
            HighscoreElement highscoreElement = element.GetComponent<HighscoreElement>();
            highscoreElement.Placement.text = i.ToString();
            highscoreElement.Username.text = highscores [ i ].username;
            highscoreElement.Score.text = highscores [ i ].score.ToString();
        }

        StartCoroutine(GetHighscores("Neebz", accessToken));
    }

    public void PlayGame () {
        Debug.Log("Play game pressed");
    }


    private class Highscore {
        public readonly int score;
        public readonly string username;

        public Highscore () {
            score = UnityEngine.Random.Range(0, 9999);
            username = "user " + UnityEngine.Random.Range(0, 9999).ToString();
        }
    }

    [Serializable]
    public class PlaceholderHighscore {
        public int rank;
        public PlaceholderPlayer player;
    }

    [Serializable]
    public class PlaceholderPlayer {
        public string playerId;
        public int experience;
    }

    [Serializable]
    public class PlaceholderHighscores {
        public List<PlaceholderHighscore> highscores;
    }



    IEnumerator GetHighscores (string id, string token, int perPage = 10) {

        string url = _baseUrl + "highscore/xp/?perPage=" + perPage + "&id=" + id;

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
                PlaceholderHighscores highscores = JsonConvert.DeserializeObject<PlaceholderHighscores>(json);

                //if (highscore.rank != 0) {

                //}
            }
        }
    }
}


