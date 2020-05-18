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
        int packetTick = _packet.ReadInt();
        string msg = _packet.ReadString();

        ClientPacketSender.UdpTestReceived();

        Debug.Log(msg);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int packetTick = _packet.ReadInt();
        int id = _packet.ReadInt();

        if (GameManager.players.ContainsKey(id))
            if (GameManager.players[id].LastPlayerUpdateTick < packetTick)
            {
                GameManager.players[id].LastPlayerUpdateTick = packetTick;
                //Head
                Vector3 pos = _packet.ReadVector3();
                Quaternion rot = _packet.ReadQuaternion();

                //Left hand
                HandDataPacket leftHand = new HandDataPacket()
                {
                    HandPosition = _packet.ReadVector3(),
                    HandRotation = _packet.ReadQuaternion(),
                    Trigger = _packet.ReadFloat(),
                    Grip = _packet.ReadFloat(),
                    Velocity = _packet.ReadVector3(),
                    HandState = (HandState)_packet.ReadInt(),
                    TargetHandState = (HandState)_packet.ReadInt(),
                    StatePower = _packet.ReadFloat(),
                };

                //Right hand
                HandDataPacket rightHand = new HandDataPacket()
                {
                    HandPosition = _packet.ReadVector3(),
                    HandRotation = _packet.ReadQuaternion(),
                    Trigger = _packet.ReadFloat(),
                    Grip = _packet.ReadFloat(),
                    Velocity = _packet.ReadVector3(),
                    HandState = (HandState)_packet.ReadInt(),
                    TargetHandState = (HandState)_packet.ReadInt(),
                    StatePower = _packet.ReadFloat(),
                };

                GameManager.players[id].emulatedPlayer.Head.transform.position = pos;
                GameManager.players[id].emulatedPlayer.Head.transform.rotation = rot;
                GameManager.players[id].emulatedPlayer.EmulateHand(GameManager.players[id].emulatedPlayer.LeftHand, leftHand);
                GameManager.players[id].emulatedPlayer.EmulateHand(GameManager.players[id].emulatedPlayer.RightHand, rightHand);
            }
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

    public static void ProjectilePosition(Packet _packet)
    {
        int packetTick = _packet.ReadInt();
        int id = _packet.ReadInt();
        Vector3 pos = _packet.ReadVector3();
        Quaternion rot = _packet.ReadQuaternion();

        lock (Projectile.Projectiles)
            if (Projectile.Projectiles.ContainsKey(id))
            {
                try
                {
                    Projectile.Projectiles[id].Emulate(pos, rot);
                }
                catch
                {
                    Debug.Log("waaa");
                }
            }
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

    internal static void DespawnFireball(Packet _packet)
    {
        int id = _packet.ReadInt();
        bool explode = _packet.ReadBool();

        if (GameManager.EmulatedFireballs.ContainsKey(id))
        {
            ThreadManager.ExecuteOnMainThread(() =>
            {
                lock (GameManager.EmulatedFireballs)
                    GameManager.EmulatedFireballs[id].Despawn(explode);
            });

        }
    }

    internal static void UpdateFireball(Packet _packet)
    {
        int id = _packet.ReadInt();
        Vector3 position = _packet.ReadVector3();
        float size = _packet.ReadFloat();

        lock (GameManager.EmulatedFireballs)
            if (GameManager.EmulatedFireballs.ContainsKey(id))
            {
                try
                {
                    GameManager.EmulatedFireballs[id].Emulate(position, size);
                }
                catch
                {
                    Debug.Log("whaa");
                }
            }
    }

    public static void UpdateFireballs(Packet _packet)
    {
        int packetTick = _packet.ReadInt();

        int fireballCount = _packet.ReadInt();
        for (int i = 0; i < fireballCount; i++)
        {
            int id = _packet.ReadInt();

            lock (GameManager.EmulatedFireballs)
                if (GameManager.EmulatedFireballs.ContainsKey(id))
                {
                    if (GameManager.EmulatedFireballs[id]._lastUpdateTick < packetTick)
                    {
                        GameManager.EmulatedFireballs[id]._lastUpdateTick = packetTick;

                        Vector3 position = _packet.ReadVector3();
                        float size = _packet.ReadFloat();

                        try
                        {
                            GameManager.EmulatedFireballs[id].Emulate(position, size);
                        }
                        catch
                        {
                            Debug.Log("whaa");
                        }
                    }
                }
        }

    }

    internal static void SpawnFireball(Packet _packet)
    {
        int id = _packet.ReadInt();
        Vector3 position = _packet.ReadVector3();
        float size = _packet.ReadFloat();

        GameManager.instance.SpawnEmulatedFireball(id, position, size);
    }

    public static void DespawnProjectile(Packet _packet)
    {
        int id = _packet.ReadInt();
        ThreadManager.ExecuteOnMainThread(() =>
        {
            if (Projectile.Projectiles.ContainsKey(id))
            {
                lock (Projectile.Projectiles)
                    Projectile.Projectiles[id].Despawn();
            }
        });
    }

    internal static void SpawnProjectile(Packet _packet)
    {
        int id = _packet.ReadInt();
        Vector3 pos = _packet.ReadVector3();

        GameManager.instance.SpawnProjectile(pos, id);
    }
}



//public static void VRHeadData(Packet _packet)
//{
//    int id = _packet.ReadInt();
//    Vector3 pos = _packet.ReadVector3();
//    Quaternion rot = _packet.ReadQuaternion();

//    if (GameManager.players.ContainsKey(id))
//    {
//        GameManager.players[id].emulatedPlayer.Head.transform.position = pos;
//        GameManager.players[id].emulatedPlayer.Head.transform.rotation = rot;
//    }
//}

//public static void VRLeftHandData(Packet _packet)
//{
//    int id = _packet.ReadInt();
//    HandDataPacket leftHand = new HandDataPacket()
//    {
//        HandPosition = _packet.ReadVector3(),
//        HandRotation = _packet.ReadQuaternion(),
//        Trigger = _packet.ReadFloat(),
//        Grip = _packet.ReadFloat(),
//        Velocity = _packet.ReadVector3(),
//        HandState = (HandState)_packet.ReadInt(),
//        StatePower = _packet.ReadFloat()
//    };

//    if (GameManager.players.ContainsKey(id))
//        GameManager.players[id].emulatedPlayer.EmulateHand(GameManager.players[id].emulatedPlayer.LeftHand, leftHand);
//}

//public static void VRRightHandData(Packet _packet)
//{
//    int id = _packet.ReadInt();
//    HandDataPacket rightHand = new HandDataPacket()
//    {
//        HandPosition = _packet.ReadVector3(),
//        HandRotation = _packet.ReadQuaternion(),
//        Trigger = _packet.ReadFloat(),
//        Grip = _packet.ReadFloat(),
//        Velocity = _packet.ReadVector3(),
//        HandState = (HandState)_packet.ReadInt(),
//        StatePower = _packet.ReadFloat()
//    };

//    if (GameManager.players.ContainsKey(id))
//        GameManager.players[id].emulatedPlayer.EmulateHand(GameManager.players[id].emulatedPlayer.RightHand, rightHand);
//}
