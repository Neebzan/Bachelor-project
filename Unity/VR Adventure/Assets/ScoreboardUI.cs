﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour {
    public GameObject ScoreboardEntryPanel;
    public GameObject ScoreboardEntryPrefab;
    public void AddScoreboardEntry (ClientConnectedPlayer player) {        
        ScoreboardEntry scoreboardEntry = Instantiate(ScoreboardEntryPrefab, ScoreboardEntryPrefab.transform).GetComponent<ScoreboardEntry>();
        player.PlayerTick += scoreboardEntry.OnPlayerTick;
    }
}

