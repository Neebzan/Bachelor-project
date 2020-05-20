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
        player.PlayerTick += scoreboardEntry.OnConnectedPlayerTick;
    }
    public void AddScoreboardEntry (Player player) {
        ScoreboardEntry scoreboardEntry = Instantiate(ScoreboardEntryPrefab, ScoreboardEntryPanel.transform).GetComponent<ScoreboardEntry>();
        //player.PlayerTick += scoreboardEntry.OnPlayerTick;
        Client.instance.LatencyUpdated += scoreboardEntry.OnClientLatencyUpdated;
    }
}

