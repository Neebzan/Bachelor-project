using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour {
    public TextMeshProUGUI PingText;
    public TextMeshProUGUI UsernameText;
    public TextMeshProUGUI ExpText;

    void Start () {
    }

    private void FixedUpdate () {
        if (Client.instance.isConnected) {
            PingText.text = Client.instance.Latency.ToString();
        }
    }



    //IEnumerator GetPlayerStats (string token, string id = "") {
    //isUpdatingHighscores = true;
    //string url = _baseUrl + "highscore/xp/?perPage=" + perPage;

    //if (id != "")
    //    url += "&id=" + id;

    //else if (_currentPage >= 1) {
    //    url += "&page=" + _currentPage;
    //}

    //using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
    //    webRequest.SetRequestHeader("token", token);

    //    yield return webRequest.SendWebRequest();

    //    if (webRequest.isNetworkError || webRequest.isHttpError)
    //        Debug.Log(webRequest.error);

    //    if (webRequest.isDone) {
    //        string json = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
    //        HighscorePage highscores = JsonConvert.DeserializeObject<HighscorePage>(json);

    //        for (int i = 0; i < highscores.Entries.Count; i++) {
    //            HighscoreElement element = HighscoreElements [ i ].GetComponent<HighscoreElement>();
    //            element.Placement.text = highscores.Entries [ i ].Rank.ToString();
    //            element.Username.text = highscores.Entries [ i ].Player.PlayerId;
    //            element.Score.text = highscores.Entries [ i ].Player.Experience.ToString();

    //            if (!String.IsNullOrEmpty(_userID) && highscores.Entries [ i ].Player.PlayerId == _userID)
    //                element.Highlight(true);
    //            else
    //                element.Highlight(false);

    //        }

    //        if (highscores.Entries.Count < perPage) {
    //            for (int i = highscores.Entries.Count; i < perPage; i++) {
    //                HighscoreElement element = HighscoreElements [ i ].GetComponent<HighscoreElement>();
    //                element.Placement.text = "";
    //                element.Username.text = "";
    //                element.Score.text = "";

    //                element.Highlight(false);
    //            }
    //        }

    //        _currentPage = highscores.CurrentPage;
    //        _totalPages = highscores.TotalPageCount;
    //    }
    //}
    //isUpdatingHighscores = false;
  //}
}

