using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour {
    public GameObject ScoreboardEntryPanel;
    public GameObject ScoreboardEntryPrefab;
    public void AddScoreboardEntry (ClientConnectedPlayer player) {        
        ScoreboardEntry scoreboardEntry = Instantiate(ScoreboardEntryPrefab, ScoreboardEntryPanel.transform).GetComponent<ScoreboardEntry>();
        scoreboardEntry.Username = player.Username;
        player.PlayerPing += scoreboardEntry.OnConnectedPlayerPing;
        player.PlayerScore += scoreboardEntry.OnConnectedPlayerScore;
    }
    public void AddScoreboardEntry (Client client) {
        ScoreboardEntry scoreboardEntry = Instantiate(ScoreboardEntryPrefab, ScoreboardEntryPanel.transform).GetComponent<ScoreboardEntry>();
        client.LatencyUpdated += scoreboardEntry.OnClientLatencyUpdated;
        scoreboardEntry.Username = client.Username;
    }
}

