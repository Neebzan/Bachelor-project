using DatabaseREST.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using UnityEngine;


public class Client : MonoBehaviour
{
    public static Client instance;

    public string ip = "127.0.0.1";
    public int port = 27000;
    public int id;
    [HideInInspector]
    public string userName = "NotSet";

    public TCP tcp;
    public UDP udp;

    public bool isConnected = false;

    public int Latency
    {
        get
        {
            int lat = 0;
            foreach (var item in pingHistory)
            {
                lat += item;
            }
            return lat / pingHistory.Count;
        }
    }
    public Queue<int> pingHistory = new Queue<int>();
    public Stopwatch Timer = new Stopwatch();


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            UnityEngine.Debug.Log("Instance already exists!");
            Destroy(this);
        }
    }

    public void ConnectToServer(string _userName)
    {
        tcp = new TCP();
        udp = new UDP(PacketHandlers.Client);
        isConnected = true;
        userName = _userName;
        tcp.Connect(ip, port, PacketHandlers.Client);
        StartCoroutine(AutoTimeSync());
    }

    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.client.Close();
            udp.client.Close();

            UnityEngine.Debug.Log("Disconnected from server.");
        }
    }
    private void OnApplicationQuit()
    {
        Disconnect();
    }

    IEnumerator AutoTimeSync()
    {
        Timer.Start();
        while (isConnected)
        {
            yield return new WaitForSeconds(1);
            ClientPacketSender.TimeSync(Timer.ElapsedMilliseconds);
        }
        Timer.Stop();
    }
}

