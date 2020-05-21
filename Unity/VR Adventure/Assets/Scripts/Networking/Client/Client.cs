using DatabaseREST.Models;
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
    private int score;

    public int Score {
        get { return score; }
        set {
            score = value;
            PlayerScoreUpdated?.Invoke();
        }
    }

    private bool _ready;

    public bool Ready {
        get { return _ready; }
        set { _ready = value;
            PlayerReadyUpdated?.Invoke();
        }
    }


    public event Action PlayerScoreUpdated;
    public event Action PlayerLatencyUpdated;
    public event EventHandler PlayerDisconnected;
    public Action PlayerReadyUpdated;

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
            foreach (var item in PingHistory) {
                lat += item;
            }
            if (PingHistory.Count > 0)
                return lat / PingHistory.Count;
            else {
                return 0;
            }
        }
    }


    public Queue<int> PingHistory = new Queue<int>();

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

    public void Ping (long oldTimeStamp) {
        PingHistory.Enqueue((int)(Timer.ElapsedMilliseconds - oldTimeStamp));
        if (PingHistory.Count > 5)
            PingHistory.Dequeue();

        PlayerLatencyUpdated?.Invoke();
    }

    public void ReadyUp (bool ready) {
        Ready = ready;
        ClientPacketSender.ReadyStatusUpdate(ready);
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
        GameManager.instance.SpawnPlayerUI(this);
    }

    private void OnConnectionChanged (ClientConnectionEventArgs e) {
        ConnectedToServer?.Invoke(this, e);
    }

    public void Disconnect () {
        if (isConnected) {
            GameManager.HandleClientDisconnect();

            isConnected = false;

            tcp.Disconnect();
            udp.Disconnect();

            UnityEngine.Debug.Log("Disconnected from server.");

            OnConnectionChanged(new ClientConnectionEventArgs() {
                Success = !isConnected,
                Type = ClientConnectionEvent.Disconnect
            });
            PlayerDisconnected?.Invoke(this, EventArgs.Empty);
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

