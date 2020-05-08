using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MainMenu : MonoBehaviour {
    private Canvas _canvas;

    public GameObject HighscoreElementPrefab;
    public GameObject HighscorePanel;

    private List<Highscore> highscores = new List<Highscore>();




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
    }

    public void PlayGame () {
        Debug.Log("Play game pressed");
    }


    private class Highscore {
        public readonly int score;
        public readonly string username;

        public Highscore () {
            score = Random.Range(0, 9999);
            username = "user " + Random.Range(0, 9999).ToString();
        }
    }
}


