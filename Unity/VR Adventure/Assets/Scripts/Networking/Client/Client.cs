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
    public int id;
    [HideInInspector]
    public string userName = "NotSet";

    public TCP tcp;
    public UDP udp;

    public bool isConnected = false;

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
    public Queue<int> pingHistory = new Queue<int>();
    public Stopwatch Timer = new Stopwatch();

    public event EventHandler<ClientConnectionEventArgs> ConnectedToServer;

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
        userName = _userName;
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

            for (int i = 0; i < GameManager.EmulatedPlayers.Count; i++)
                GameObject.Destroy(GameManager.EmulatedPlayers [ i ]);
            GameManager.EmulatedPlayers.Clear();

            for (int i = 0; i < GameManager.EmulatedFireballs.Count; i++)
                GameObject.Destroy(GameManager.EmulatedFireballs [ i ]);
            GameManager.EmulatedFireballs.Clear();


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

