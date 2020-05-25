using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPacketHandler
{
    public static void WelcomeReceived(Packet _packet)
    {
        int _clientId = _packet.ReadInt();
        string _username = _packet.ReadString();

        if (Server.DisconnectedPlayersScore.ContainsKey(_username))
        {
            Console.WriteLine("Reconnecting player");
            int existingScore = Server.DisconnectedPlayersScore[_username];
            Server.clients[_clientId].SpawnPlayer(_username, existingScore);
            Server.DisconnectedPlayersScore.Remove(_username);
        }
        else
        {
            Console.WriteLine("spawning new player");
            Server.clients[_clientId].SpawnPlayer(_username);
        }
    }

    public static void PlayerMovement(Packet _packet)
    {
        Console.WriteLine("Player movement received!");
        long packetTick = _packet.ReadLong();
        int id = _packet.ReadInt();

        if (Server.clients.ContainsKey(id))
        {
            if (Server.clients[id].player.LastUpdateTick < packetTick)
            {
                Server.clients[id].player.LastUpdateTick = packetTick;
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


                Server.clients[id].player.SetHead(pos, rot);
                Server.clients[id].player.SetHand(leftHand, true);
                Server.clients[id].player.SetHand(rightHand);
            }
        }
    }

    internal static void Configure(Packet _packet)
    {
        string messageJSON = _packet.ReadString();
        GameserverInstance configuration = JsonConvert.DeserializeObject<GameserverInstance>(messageJSON);
        Server.GameserverInstance = configuration;
        Console.WriteLine("Server configuration recieved");
        Console.WriteLine("Configuring server..");
        Console.WriteLine("Server configured");
        Console.WriteLine("\n");

        //Send ready state
        ServerPacketSender.SendReadyState();
    }

    public static void PlayerReadyStateUpdated(Packet _packet)
    {
        int clientID = _packet.ReadInt();
        bool readyState = _packet.ReadBool();

        if (Server.clients.ContainsKey(clientID))
        {
            Server.clients[clientID].player.IsReady = readyState;

            // Send player ready updated to all other players
            ServerPacketSender.PlayerReadyStateUpdated(clientID, Server.clients[clientID].player.IsReady);
        }
    }

    public static void TimeSync(Packet _packet)
    {
        int id = _packet.ReadInt();
        long clientTimeStamp = _packet.ReadLong();
        int clientLatency = _packet.ReadInt();
        //save of the client
        Server.clients[id].Latency = clientLatency;
        ServerPacketSender.TimeSync(id, clientTimeStamp, DateTime.UtcNow.Ticks);
    }

    public static void UdpTestReceived(Packet packet)
    {
        long packetTick = packet.ReadLong();
        int _id = packet.ReadInt();
        string _msg = packet.ReadString();
    }
}
