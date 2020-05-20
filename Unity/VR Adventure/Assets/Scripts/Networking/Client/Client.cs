﻿using DatabaseREST.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using UnityEngine;

public enum ClientConnectionEvent { Connect, Disconnect }

public class ClientConnectionEventArgs : EventArgs {
    public bool Success { get; set; }
    public ClientConnectionEvent Type { get; set; }
}

public class Client : MonoBehaviour {
    public static Client instance;

    public string ip = "127.0.0.1";
    public int port = 27000;
    public int ID;
    public string Username = "NotSet";
    public int Score;

    public TCP tcp;
    public UDP udp;

    public bool isConnected = false;

    [HideInInspector]
    public Players PlayerInfo;
    [HideInInspector]
    public RESTClient Rest = new RESTClient();

    public int Latency {
        get {
            int lat = 0;
            foreach (var item in pingHistory) {
                lat += item;
            }
            if (pingHistory.Count > 0)
                return lat / pingHistory.Count;
            else {
                return 0;
            }
        }
    }

    private Queue<int> pingHistory = new Queue<int>();

    public Queue<int> PingHistory {
        get { return pingHistory; }
        set {
            pingHistory = value;
            LatencyUpdated?.Invoke();
        }
    }
    public Stopwatch Timer = new Stopwatch();

    public event EventHandler<ClientConnectionEventArgs> ConnectedToServer;
    public event Action LatencyUpdated;

    private void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this) {
            UnityEngine.Debug.Log("Instance already exists!");
            Destroy(this);
        }
        ClientPacketHandler.OnClientConnectedToServer += OnClientConnectedToServer;
    }

    private void OnClientConnectedToServer () {
        isConnected = true;
        StartCoroutine(AutoTimeSync());

        OnConnectionChanged(new ClientConnectionEventArgs() {
            Success = isConnected,
            Type = ClientConnectionEvent.Connect
        });
    }

    public void ConnectToServer (string _userName) {
        Username = _userName;
        tcp = new TCP();
        udp = new UDP(PacketHandlers.Client);
        tcp.Connect(ip, port, PacketHandlers.Client);
    }

    private void OnConnectionChanged (ClientConnectionEventArgs e) {
        ConnectedToServer?.Invoke(this, e);
    }

    public void Disconnect () {
        if (isConnected) {
            isConnected = false;

            tcp.Disconnect();
            udp.Disconnect();

            foreach (ClientConnectedPlayer player in GameManager.instance.EmulatedPlayers.Values)
                GameObject.Destroy(player.gameObject);
            GameManager.instance.EmulatedPlayers.Clear();

            foreach (EmulatedFireball fireball in GameManager.instance.EmulatedFireballs.Values)
                GameObject.Destroy(fireball.gameObject);
            GameManager.instance.EmulatedFireballs.Clear();

            UnityEngine.Debug.Log("Disconnected from server.");

            OnConnectionChanged(new ClientConnectionEventArgs() {
                Success = !isConnected,
                Type = ClientConnectionEvent.Disconnect
            });
        }
    }
    private void OnApplicationQuit () {
        if (isConnected) {
            Disconnect();
        }
    }

    IEnumerator AutoTimeSync () {
        Timer.Start();
        while (isConnected) {
            yield return new WaitForSeconds(1);
            ClientPacketSender.TimeSync(Timer.ElapsedMilliseconds);
        }
        Timer.Stop();
    }
}

