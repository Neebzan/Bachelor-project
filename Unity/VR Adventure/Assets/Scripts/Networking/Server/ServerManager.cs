﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public static ServerManager instance;

    public GameObject playerPrefab;
    public GameObject vrPlayerPrefab;
    public GameObject projectilePrefab;
    public GameObject FireballPrefab;

    public Dictionary<int, Fireball> Fireballs = new Dictionary<int, Fireball>();

    public RESTServer REST;
    public Match match;

    public event EventHandler OnPlayerSpawned;
    public event EventHandler OnPlayerDisconnected;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance already exists!");
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        match = gameObject.AddComponent<Match>();
        REST.GetServerToken();

        //Server.Start(10, 27000);
        Server.Start(10);

    }

    private void FixedUpdate()
    {
        ServerPacketSender.UpdateFireballsCollection();
        ServerPacketSender.PlayerPositions();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player SpawnPlayer()
    {
        Player player = Instantiate(vrPlayerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Player>();
        OnPlayerSpawned?.Invoke(player, EventArgs.Empty);
        return player;
    }

    public Fireball SpawnFireball (int playerID) {
        Fireball fireball = Instantiate(FireballPrefab).GetComponent<Fireball>();
        fireball.PlayerID = playerID;
        Fireballs.Add(fireball.ID, fireball);
        
        return fireball;
    }

    public void PlayerDisconnected(Player player)
    {
        OnPlayerDisconnected?.Invoke(player, EventArgs.Empty);
    }

}
