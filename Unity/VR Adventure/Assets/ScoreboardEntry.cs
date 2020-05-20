﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardEntry : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI _pingText;
    [SerializeField]
    private TextMeshProUGUI _usernameText;
    [SerializeField]
    private TextMeshProUGUI _scoreText;

    private int ping;
    public int Ping {
        get { return ping; }
        set {
            ping = value;
            _pingText.text = ping.ToString();
        }
    }

    private int score;
    public int Score {
        get { return score; }
        set {
            score = value;
            _scoreText.text = score.ToString();
        }
    }

    private string username;
    public string Username {
        get { return username; }
        set {
            username = value;
            _usernameText.text = username;
        }
    }

    public void OnConnectedPlayerTick (object sender, EventArgs e) {
        Username = (sender as ClientConnectedPlayer).Username;
        // Ping
        // Score
    }

    public void OnClientLatencyUpdated () {
        Ping = Client.instance.Latency;
    }
    public void OnPlayerTick (object sender, EventArgs e) {
        Username = (sender as Player).Username;
        Score = (sender as Player).Score;
    }
}
