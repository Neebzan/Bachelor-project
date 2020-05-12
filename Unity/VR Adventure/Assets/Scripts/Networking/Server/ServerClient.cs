using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class ServerClient
{
    public int id;
    public Player player;

    public TCP tcp;
    public UDP udp;

    public bool isConnected = false;

    public ServerClient(int _id)
    {
        id = _id;
        tcp = new TCP(id);
        udp = new UDP(id);     
    }

    ~ServerClient()
    {
       
    }

    public void Connect(TcpClient tcpClient)
    {
        tcp.Disconnected += Server.DisconnectClient;
        tcp.Connect(tcpClient, PacketHandlers.Server);
        isConnected = true;
    }

    public void SpawnPlayer(string _userName)
    {
        player = ServerManager.instance.InstantiatePlayer();
        player.Initialize(id, _userName);

        //Send all existing players to the client
        foreach (var client in Server.clients.Values)
        {
            //if player exists
            if (client.player != null)
            {
                //If not this player
                if (client.id != id)
                    ServerPacketSender.SpawnPlayer(id, client.player);
            }
        }

        //Send the new player, to all existing player
        foreach (var client in Server.clients.Values)
        {
            if (client.player != null)
                ServerPacketSender.SpawnPlayer(client.id, player);

        }

    }

    public void Disconnect()
    {
        if (isConnected)
        {
            tcp.Disconnected -= Server.DisconnectClient;
            isConnected = false;
            player = null;
            tcp.Disconnect();
            udp.Disconnect();
            Debug.Log("Disconnected from server.");
        }
    }

}
