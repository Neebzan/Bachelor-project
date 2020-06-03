using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class ClientPacketHandler {
    public static event Action OnClientConnectedToServer;


    public static void WelcomeMessage (Packet packet) {
        string msg = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"Welcome message from server: {msg}");

        Client.instance.ID = id;

        //Send a response to server here
        ClientPacketSender.WelcomeReceived();

        //Setup udp client on same port as TCP client
        Client.instance.udp.Connect(Client.instance.ip, Client.instance.port, ((IPEndPoint)Client.instance.tcp.client.Client.LocalEndPoint).Port);
        OnClientConnectedToServer?.Invoke();
    }

    public static void SpawnPlayer (Packet packet) {
        Debug.Log("Attempting to spawn player!");

        int _id = packet.ReadInt();
        string _username = packet.ReadString();
        Vector3 _position = packet.ReadVector3();

        GameManager.instance.SpawnPlayer(_id, _username, _position, Quaternion.identity);
    }

    //public static void UdpReceiveMessageTest (Packet _packet) {
    //    long packetTick = _packet.ReadLong();
    //    string msg = _packet.ReadString();

    //    ClientPacketSender.UdpTestReceived();

    //    Debug.Log(msg);
    //}

    public static void PlayerPosition (Packet _packet) {
        long packetTick = _packet.ReadLong();
        int id = _packet.ReadInt();

        if (GameManager.instance.EmulatedPlayers.ContainsKey(id))
            if (GameManager.instance.EmulatedPlayers [ id ].LastPlayerUpdateTick < packetTick) {
                GameManager.instance.EmulatedPlayers [ id ].LastPlayerUpdateTick = packetTick;

                //Head
                Vector3 pos = _packet.ReadVector3();
                Quaternion rot = _packet.ReadQuaternion();

                //Left hand
                HandDataPacket leftHand = new HandDataPacket() {
                    HandPosition = _packet.ReadVector3(),
                    HandRotation = _packet.ReadQuaternion(),
                    Trigger = _packet.ReadFloat(),
                    Grip = _packet.ReadFloat(),
                    HandState = (HandState)_packet.ReadInt(),
                    StatePower = _packet.ReadFloat(),
                };

                //Right hand
                HandDataPacket rightHand = new HandDataPacket() {
                    HandPosition = _packet.ReadVector3(),
                    HandRotation = _packet.ReadQuaternion(),
                    Trigger = _packet.ReadFloat(),
                    Grip = _packet.ReadFloat(),
                    HandState = (HandState)_packet.ReadInt(),
                    StatePower = _packet.ReadFloat(),
                };

                GameManager.instance.EmulatedPlayers [ id ].Tick(pos, rot, leftHand, rightHand);
            }
    }

    public static void PlayerDisconnected (Packet _packet) {
        int _id = _packet.ReadInt();
        GameManager.instance.PlayerDisconnected(_id);
    }

    //public static void ProjectilePosition(Packet _packet)
    //{
    //    long packetTick = _packet.ReadLong();
    //    int id = _packet.ReadInt();
    //    Vector3 pos = _packet.ReadVector3();
    //    Quaternion rot = _packet.ReadQuaternion();

    //    lock (Projectile.Projectiles)
    //        if (Projectile.Projectiles.ContainsKey(id))
    //        {
    //            try
    //            {
    //                Projectile.Projectiles[id].Emulate(pos, rot);
    //            }
    //            catch
    //            {
    //                Debug.Log("waaa");
    //            }
    //        }
    //}

    internal static void TimeSync (Packet _packet) {
        //https://docs.gamesparks.com/tutorials/real-time-services/clock-synchronization-and-network-programming.html

        long oldTimeStamp = _packet.ReadLong();
        DateTime serverTime = new DateTime(_packet.ReadLong());

        Client.instance.Ping(oldTimeStamp);




        //Read how many other players latency is included
        int latencyCount = _packet.ReadInt();

        for (int i = 0; i < latencyCount; i++) {
            int playerID = _packet.ReadInt();
            if (GameManager.instance.EmulatedPlayers.ContainsKey(playerID))
                GameManager.instance.EmulatedPlayers [ playerID ].Ping = _packet.ReadInt();
        }

        //Client.instance.Latency = (int)RTT / 2;

        //Debug.Log($"Packet RTT/Latency: {Client.instance.Latency}ms");
        //TimeSpan ts =  serverTime - DateTime.UtcNow;
        //Debug.Log($"Time sync diff: {ts.TotalMilliseconds + (Client.instance.Latency/2)}ms");
        //Debug.Log($"Time sync diff would have been: {serverTime - (oldTimeStamp + Client.instance.Latency)}ms");

    }

    public static void PlayerScoreUpdated (Packet _packet) {
        int playerID = _packet.ReadInt();
        int newScore = _packet.ReadInt();

        if (GameManager.instance.EmulatedPlayers.ContainsKey(playerID)) {
            GameManager.instance.EmulatedPlayers [ playerID ].Score = newScore;
        }
        else if (Client.instance.ID == playerID) {
            Client.instance.Score = newScore;
        }
    }

    internal static void DespawnFireball (Packet _packet) {
        int id = _packet.ReadInt();
        bool explode = _packet.ReadBool();

        ThreadManager.ExecuteOnMainThread(() => {
            if (GameManager.instance.EmulatedFireballs.ContainsKey(id)) {
                lock (GameManager.instance.EmulatedFireballs)
                    GameManager.instance.EmulatedFireballs [ id ].Despawn(explode);
            }
        });

    }

    internal static void UpdateFireball (Packet _packet) {
        int id = _packet.ReadInt();
        Vector3 position = _packet.ReadVector3();
        float size = _packet.ReadFloat();

        lock (GameManager.instance.EmulatedFireballs)
            if (GameManager.instance.EmulatedFireballs.ContainsKey(id)) {
                try {
                    GameManager.instance.EmulatedFireballs [ id ].Emulate(position, size);
                }
                catch {
                    Debug.Log("whaa");
                }
            }
    }

    public static void PlayerReadyStateUpdated(Packet _packet) {
        int clientID = _packet.ReadInt();
        bool readyState = _packet.ReadBool();

        if (GameManager.instance.EmulatedPlayers.ContainsKey(clientID)){
            GameManager.instance.EmulatedPlayers [ clientID ].Ready = readyState;
        }
    }

    public static void UpdateFireballs (Packet _packet) {
        long packetTick = _packet.ReadLong();

        int fireballCount = _packet.ReadInt();
        for (int i = 0; i < fireballCount; i++) {
            int id = _packet.ReadInt();

            lock (GameManager.instance.EmulatedFireballs)
                if (GameManager.instance.EmulatedFireballs.ContainsKey(id)) {
                    if (GameManager.instance.EmulatedFireballs [ id ]._lastUpdateTick < packetTick) {
                        GameManager.instance.EmulatedFireballs [ id ]._lastUpdateTick = packetTick;

                        Vector3 position = _packet.ReadVector3();
                        float size = _packet.ReadFloat();

                        try {
                            GameManager.instance.EmulatedFireballs [ id ].Emulate(position, size);
                        }
                        catch {
                            Debug.Log("whaa");
                        }
                    }
                }
        }

    }

    internal static void SpawnFireball (Packet _packet) {
        int id = _packet.ReadInt();
        Vector3 position = _packet.ReadVector3();
        float size = _packet.ReadFloat();

        GameManager.instance.SpawnEmulatedFireball(id, position, size);
    }

    internal static void GameServerCreated(Packet _packet)
    {
        string messageJSON = _packet.ReadString();
        GameserverInstance configuration = JsonConvert.DeserializeObject<GameserverInstance>(messageJSON);

        Client.instance.ConnectToServer(configuration);
    }

    internal static void ServerList(Packet _packet)
    {
        string messageJSON = _packet.ReadString();
        List<GameserverInstance> servers = JsonConvert.DeserializeObject<List<GameserverInstance>>(messageJSON);

        //Do something with servers   
    }

    //public static void DespawnProjectile(Packet _packet)
    //{
    //    int id = _packet.ReadInt();
    //    ThreadManager.ExecuteOnMainThread(() =>
    //    {
    //        if (Projectile.Projectiles.ContainsKey(id))
    //        {
    //            lock (Projectile.Projectiles)
    //                Projectile.Projectiles[id].Despawn();
    //        }
    //    });
    //}

    //internal static void SpawnProjectile(Packet _packet)
    //{
    //    int id = _packet.ReadInt();
    //    Vector3 pos = _packet.ReadVector3();

    //    GameManager.instance.SpawnProjectile(pos, id);
    //}
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
