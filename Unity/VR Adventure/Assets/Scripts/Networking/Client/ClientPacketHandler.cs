using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientPacketHandler : MonoBehaviour
{
    public static void WelcomeMessage(Packet packet)
    {
        string msg = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"Welcome message from server: {msg}");

        Client.instance.id = id;

        //Send a response to server here
        ClientPacketSender.WelcomeReceived();

        //Setup udp client on same port as TCP client
        Client.instance.udp.Connect(Client.instance.ip, Client.instance.port, ((IPEndPoint)Client.instance.tcp.client.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet packet)
    {
        Debug.Log("Attempting to spawn player!");

        int _id = packet.ReadInt();
        string _username = packet.ReadString();
        Vector3 _position = packet.ReadVector3();

        GameManager.instance.SpawnPlayer(_id, _username, _position, Quaternion.identity);
    }

    public static void UdpReceiveMessageTest(Packet _packet)
    {
        string msg = _packet.ReadString();

        ClientPacketSender.UdpTestReceived();

        Debug.Log(msg);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _pos = _packet.ReadVector3();

        if (GameManager.players.ContainsKey(_id))
            GameManager.players[_id].transform.position = _pos;
        //Debug.Log($"{_pos} is new position for player {_id}");
    }
}
