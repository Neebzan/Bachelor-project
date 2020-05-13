using DatabaseREST.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;


public class Client : MonoBehaviour {
    public static Client instance;

    public string ip = "127.0.0.1";
    public int port = 13000;
    public int id;
    [HideInInspector]
    public string userName = "NotSet";

    public TCP tcp;
    public UDP udp;

    public bool isConnected = false;

    private void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this) {
            Debug.Log("Instance already exists!");
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start () {
        //InitClientData();
        //tcp = new TCP();
        //udp = new UDP(PacketHandlers.Client);
    }

    public void ConnectToServer (string _userName) {
        tcp = new TCP();
        udp = new UDP(PacketHandlers.Client);
        isConnected = true;
        userName = _userName;
        tcp.Connect(ip, port, PacketHandlers.Client);
    }


    private void Disconnect () {
        if (isConnected) {
            isConnected = false;
            tcp.client.Close();
            udp.client.Close();

            Debug.Log("Disconnected from server.");
        }
    }

    private void OnApplicationQuit () {
        Disconnect();
    }
}

