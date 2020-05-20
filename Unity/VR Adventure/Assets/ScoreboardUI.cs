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
        player.PlayerPingUpdated += scoreboardEntry.OnConnectedPlayerPingUpdated;
        player.PlayerScoreUpdated += scoreboardEntry.OnConnectedPlayerScoreUpdated;
        player.PlayerDisconnected += scoreboardEntry.OnConnectedPlayerDisconnected;
    }
    public void AddScoreboardEntry (Client client) {
        ScoreboardEntry scoreboardEntry = Instantiate(ScoreboardEntryPrefab, ScoreboardEntryPanel.transform).GetComponent<ScoreboardEntry>();
        scoreboardEntry.Username = client.Username;
        client.LatencyUpdated += scoreboardEntry.OnClientLatencyUpdated;
        client.PlayerScoreUpdated += scoreboardEntry.OnClientScoreUpdated;
    }
}

