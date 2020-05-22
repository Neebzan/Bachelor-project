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

    public int Latency;

    public ServerClient(int _id)
    {
        id = _id;
        tcp = new TCP(id);
        udp = new UDP(id);
    }

    public void Connect(TcpClient tcpClient)
    {
        tcp.Disconnected += Server.DisconnectClient;
        tcp.Connect(tcpClient, PacketHandlers.Server);
        isConnected = true;
    }

    public void SpawnPlayer(string _userName, int score = -1)
    {
        player = ServerManager.instance.SpawnPlayer();
        if (score < 0)
        {
            player.Initialize(id, _userName);
            Console.WriteLine("New player spawned!");
        }
        else
        {
            player.Initialize(id, _userName, score);
            Console.WriteLine("Existing player spawned!");
        }


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
                //If not this player
                if (client.id != id)
                    ServerPacketSender.SpawnPlayer(client.id, player);

        }

        //Send all existing fireballs to the connecting player
        foreach (var fireball in ServerManager.instance.Fireballs.Values)
        {
            ServerPacketSender.SpawnFireball(id, fireball);
        }

    }

    public void Disconnect()
    {
        if (isConnected)
        {
            ServerManager.instance.PlayerDisconnected(player);

            tcp.Disconnected -= Server.DisconnectClient;
            isConnected = false;

            ThreadManager.ExecuteOnMainThread(() =>
            {
                UnityEngine.Object.Destroy(player.gameObject);
                player = null;
            });

            ServerPacketSender.PlayerDisonnected(id);

            tcp.Disconnect();
            udp.Disconnect();
            Debug.Log("Disconnected from server.");
        }
    }

}
