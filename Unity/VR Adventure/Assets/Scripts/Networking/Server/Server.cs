using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }

    public static Dictionary<int, ServerClient> clients = new Dictionary<int, ServerClient>();

    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;

    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        Port = _port;

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

        InitServer();

        Console.WriteLine("Server started!");
    }

    private static void TcpConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        //Accept next client
        tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);

        Console.WriteLine("Incoming connection from " + _client.Client.RemoteEndPoint);

        //clients.Add()
        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.client == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }

        Console.WriteLine("Server is full!");

    }

    //public static void DisconnectClient(string id)
    //{
    //    Client _client;
    //    if(clients.TryGetValue(id, out _client))
    //    {
    //        _client.tcp.Disconnect();
    //        clients.Remove(id);
    //        Debug.Log($"Client: {id} - Disconnected successfully!");
    //    }
    //    else
    //    {
    //        Debug.Log($"Attempted to disconnect client: {id} - but the client wasen't found!");
    //    }
    //}

    private static void InitServer()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new ServerClient(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived }
        };
    }

    public static void Stop()
    {
        tcpListener.Stop();
    }
}
