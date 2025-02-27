﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class ClientPacketSender
{
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.WelcomeReceived))
        {
            _packet.Write(Client.instance.ID);
            _packet.Write(Client.instance.Username);

            SendTCPData(_packet);
            Debug.Log("Welcome sent");
        }
    }

    public static void UdpTestReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.UdpTestReceived))
        {
            _packet.Write(Client.instance.ID);
            _packet.Write("Yo Server bruh, I got that UDP packet!");

            SendUDPData(_packet);
        }
    }

    public static void PlayerMovement(Vector3 headPosition, Quaternion headRotation, HandDataPacket LeftHandData, HandDataPacket rightHandData)
    {
        using (Packet _packet = new Packet((int)ClientPackets.PlayerMovement))
        {
            _packet.Write(Client.instance.ID);
            //Head
            _packet.Write(headPosition);
            _packet.Write(headRotation);

            //Left hand
            _packet.Write(LeftHandData.HandPosition);
            _packet.Write(LeftHandData.HandRotation);
            _packet.Write(LeftHandData.Trigger);
            _packet.Write(LeftHandData.Grip);
            _packet.Write(LeftHandData.Velocity);
            _packet.Write((int)LeftHandData.HandState);
            _packet.Write((int)LeftHandData.TargetHandState);
            _packet.Write(LeftHandData.StatePower);

            //Right hand
            _packet.Write(rightHandData.HandPosition);
            _packet.Write(rightHandData.HandRotation);
            _packet.Write(rightHandData.Trigger);
            _packet.Write(rightHandData.Grip);
            _packet.Write(rightHandData.Velocity);
            _packet.Write((int)rightHandData.HandState);
            _packet.Write((int)rightHandData.TargetHandState);
            _packet.Write(rightHandData.StatePower);

            //Send
            SendUDPData(_packet);
            //Debug.Log("Player movement sent");
        }
    }

    public static void TimeSync(long currentTimestamp)
    {
        using (Packet _packet = new Packet((int)ClientPackets.TimeSync))
        {
            _packet.Write(Client.instance.ID);
            _packet.Write(currentTimestamp); //Send current local time stamp
            _packet.Write(Client.instance.Latency); //Send previously calculated latency

            SendTCPData(_packet);
        }
    }

    public static void ReadyStatusUpdate(bool ready)
    {
        using (Packet _packet = new Packet((int)ClientPackets.ReadyStateUpdated))
        {
            _packet.Write(Client.instance.ID);
            _packet.Write(ready);

            SendTCPData(_packet);
        }
    }

    public static void CreateServerRequest(GameserverInstance gameserverInstance)
    {

        string JSON = JsonConvert.SerializeObject(gameserverInstance);
        using (Packet packet = new Packet((int)ServerManagerPackets.Create))
        {
            packet.Write(JSON);
            SendTCPDataServerManager(packet);
        }
    }

    public static void RequestServerList()
    {
        using (Packet packet = new Packet((int)ServerManagerPackets.LiveServers))
        {
            SendTCPDataServerManager(packet);
        }
    }

    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength(); //Add a length to the packet
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendTCPDataServerManager(Packet _packet)
    {
        _packet.WriteLength(); //Add a length to the packet
        Client.instance.ServerManagerTCP.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLengthAndTimeStamp();
        Client.instance.udp.SendData(_packet);
    }
}

//public static void VRRightHandData(HandDataPacket dataPacket)
//{

//    using (Packet _packet = new Packet((int)ClientPackets.VRRightHandData))
//    {            
//        _packet.Write(Client.instance.id);
//        _packet.Write(dataPacket.HandPosition);
//        _packet.Write(dataPacket.HandRotation);
//        _packet.Write(dataPacket.Trigger);
//        _packet.Write(dataPacket.Grip);
//        _packet.Write(dataPacket.Velocity);
//        _packet.Write((int)dataPacket.HandState);
//        _packet.Write((int)dataPacket.TargetHandState);
//        _packet.Write(dataPacket.StatePower);

//        SendUDPData(_packet);
//    }
//}

//public static void VRLeftHandData(HandDataPacket dataPacket)
//{
//    using (Packet _packet = new Packet((int)ClientPackets.VRLeftHandData))
//    {
//        _packet.Write(Client.instance.id);
//        _packet.Write(dataPacket.HandPosition);
//        _packet.Write(dataPacket.HandRotation);
//        _packet.Write(dataPacket.Trigger);
//        _packet.Write(dataPacket.Grip);
//        _packet.Write(dataPacket.Velocity);
//        _packet.Write((int)dataPacket.HandState);
//        _packet.Write((int)dataPacket.TargetHandState);
//        _packet.Write(dataPacket.StatePower);

//        SendUDPData(_packet);
//    }
//}

//public static void HeadData(Vector3 pos, Quaternion rot)
//{
//    using (Packet _packet = new Packet((int)ClientPackets.VRHeadData))
//    {
//        _packet.Write(Client.instance.id);
//        _packet.Write(pos);
//        _packet.Write(rot);

//        SendUDPData(_packet);
//    }
//}