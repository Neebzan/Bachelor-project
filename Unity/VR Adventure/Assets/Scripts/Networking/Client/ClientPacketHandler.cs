using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class ClientPacketHandler
{
    public static event Action OnConnectedToServer;

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
        OnConnectedToServer?.Invoke();
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

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();
        ThreadManager.ExecuteOnMainThread(() =>
        {
            PlayerManager player;
            if (GameManager.players.TryGetValue(_id, out player))
            {
                UnityEngine.Object.Destroy(player.gameObject);
                GameManager.players.Remove(_id);
            }
        });
    }

    public static void VRHeadData(Packet _packet)
    {
        int id = _packet.ReadInt();
        Vector3 pos = _packet.ReadVector3();
        Quaternion rot = _packet.ReadQuaternion();

        if (GameManager.players.ContainsKey(id))
        {
            GameManager.players[id].emulatedPlayer.Head.transform.position = pos;
            GameManager.players[id].emulatedPlayer.Head.transform.rotation = rot;
        }
    }

    internal static void ProjectilePosition(Packet _packet)
    {
        int id = _packet.ReadInt();
        Vector3 pos = _packet.ReadVector3();
        Quaternion rot = _packet.ReadQuaternion();

        Projectile.Projectiles[id].transform.position = pos;
        Projectile.Projectiles[id].transform.rotation = rot;
    }

    internal static void TimeSync(Packet _packet)
    {
        int oldClientTime = _packet.ReadInt();
        int serverTime = _packet.ReadInt();
        float RTT = DateTime.UtcNow.Millisecond - oldClientTime;
        float latency = RTT / 2f;

        Debug.Log($"Packet RTT: {RTT}ms - Latency: {latency}");
        Debug.Log($"Time sync diff would have been: {serverTime - (oldClientTime + latency)}ms");

    }

    public static void DespawnProjectile(Packet _packet)
    {
        int id = _packet.ReadInt();
        ThreadManager.ExecuteOnMainThread(() =>
        {
            if (Projectile.Projectiles.ContainsKey(id))
                Projectile.Projectiles[id].Despawn();
        });
    }

    internal static void SpawnProjectile(Packet _packet)
    {
        int id = _packet.ReadInt();
        Vector3 pos = _packet.ReadVector3();

        GameManager.instance.SpawnProjectile(pos, id);
    }

    public static void VRLeftHandData(Packet _packet)
    {
        int id = _packet.ReadInt();
        HandDataPacket leftHand = new HandDataPacket()
        {
            HandPosition = _packet.ReadVector3(),
            HandRotation = _packet.ReadQuaternion(),
            Trigger = _packet.ReadFloat(),
            Grip = _packet.ReadFloat(),
            Velocity = _packet.ReadVector3()
        };

        if (GameManager.players.ContainsKey(id))
            GameManager.players[id].emulatedPlayer.EmulateHand(GameManager.players[id].emulatedPlayer.LeftHand, leftHand);
    }

    public static void VRRightHandData(Packet _packet)
    {
        int id = _packet.ReadInt();
        HandDataPacket rightHand = new HandDataPacket()
        {
            HandPosition = _packet.ReadVector3(),
            HandRotation = _packet.ReadQuaternion(),
            Trigger = _packet.ReadFloat(),
            Grip = _packet.ReadFloat(),
            Velocity = _packet.ReadVector3()
        };

        if (GameManager.players.ContainsKey(id))
            GameManager.players[id].emulatedPlayer.EmulateHand(GameManager.players[id].emulatedPlayer.RightHand, rightHand);
    }
}
