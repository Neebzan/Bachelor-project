using DatabaseREST.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HighscorePanel : MonoBehaviour
{
    public GameObject [ ] HighscoreElements = new GameObject [ 10 ];

    private readonly string refreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBYmJvdHQ0NDM5U2NvdCIsImF1ZCI6InJlZnJlc2giLCJuYmYiOjE1ODg5MjYzMjYsImV4cCI6MTU5ODA4NDcyNiwiaWF0IjoxNTg4OTI2MzI2fQ.5RMfdQIWmAGMnRBSYgL43dhNWVuDqEo0a56yYCHaw14";
    private readonly string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBYmJvdHQ0NDM5U2NvdCIsImF1ZCI6ImFjY2VzcyIsIm5iZiI6MTU4ODkyNjMyNiwiZXhwIjoxNTk2ODc4NzI2LCJpYXQiOjE1ODg5MjYzMjZ9.LGSmOhSHO_JF_710ZL9zToXhg6niStyV7l5LRUTp2bI";

    private readonly string _baseUrl = "http://212.10.51.254:30830/api/";


    private int _currentPage = -1;
    private int _totalPages = -1;
    private string _userID = "Neebz";
    private bool isUpdatingHighscores = false;

    private void OnEnable () {
        StartCoroutine(GetHighscores(accessToken, _userID));        
    }

    public void ChangePage (int pages) {
        if (!isUpdatingHighscores) {
            if (_totalPages != -1) {
                if (_currentPage + pages > 0 && _currentPage + pages <= _totalPages) {
                    _currentPage += pages;
                    StartCoroutine(GetHighscores(accessToken));
                }
            }
        }
    }

    IEnumerator GetHighscores (string token, string id = "", int perPage = 10) {
        isUpdatingHighscores = true;
        string url = _baseUrl + "highscore/xp/?perPage=" + perPage;

        if (id != "")
            url += "&id=" + id;

        else if (_currentPage >= 1) {
            url += "&page=" + _currentPage;
        }

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            webRequest.SetRequestHeader("token", token);

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
                Debug.Log(webRequest.error);

            if (webRequest.isDone) {
                string json = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
                HighscorePage highscores = JsonConvert.DeserializeObject<HighscorePage>(json);

                for (int i = 0; i < highscores?.Entries?.Count; i++) {
                    HighscoreElement element = HighscoreElements [ i ].GetComponent<HighscoreElement>();
                    element.Placement.text = highscores.Entries [ i ].Rank.ToString();
                    element.Username.text = highscores.Entries [ i ].Player.PlayerId;
                    element.Score.text = highscores.Entries [ i ].Player.Experience.ToString();

                    if (!String.IsNullOrEmpty(_userID) && highscores.Entries [ i ].Player.PlayerId == _userID)
                        element.Highlight(true);
                    else
                        element.Highlight(false);

                }

                if (highscores.Entries.Count < perPage) {
                    for (int i = highscores.Entries.Count; i < perPage; i++) {
                        HighscoreElement element = HighscoreElements [ i ].GetComponent<HighscoreElement>();
                        element.Placement.text = "";
                        element.Username.text = "";
                        element.Score.text = "";

                        element.Highlight(false);
                    }
                }

                _currentPage = highscores.CurrentPage;
                _totalPages = highscores.TotalPageCount;
            }
        }
        isUpdatingHighscores = false;
    }
}
