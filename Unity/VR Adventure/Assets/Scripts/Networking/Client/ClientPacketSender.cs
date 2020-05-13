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
        using(Packet _packet = new Packet((int)ClientPackets.WelcomeReceived))
        {
            _packet.Write(Client.instance.id);
            _packet.Write(Client.instance.userName);

            SendTCPData(_packet);
        }
    }

    public static void UdpTestReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.UdpTestReceived))
        {
            _packet.Write(Client.instance.id);
            _packet.Write("Yo Server bruh, I got that UDP packet!");

            SendUDPData(_packet);
        }
    }

    public static void PlayerMovement(bool[] inputs)
    {
        //Debug.Log("Sending inputs!");
        using (Packet _packet = new Packet((int)ClientPackets.PlayerMovement))
        {
            _packet.Write(inputs.Length);
            _packet.Write(Client.instance.id);
            foreach (var item in inputs)
            {
                _packet.Write(item);
            }

            SendUDPData(_packet);
        }
    }

    public static void VRRightHandData(HandDataPacket dataPacket)
    {

        using (Packet _packet = new Packet((int)ClientPackets.VRRightHandData))
        {            
            _packet.Write(Client.instance.id);
            _packet.Write(dataPacket.HandPosition);
            _packet.Write(dataPacket.HandRotation);
            _packet.Write(dataPacket.Trigger);
            _packet.Write(dataPacket.Grip);
            _packet.Write(dataPacket.Velocity);

            SendUDPData(_packet);
        }
    }

    public static void VRLeftHandData(HandDataPacket dataPacket)
    {
        using (Packet _packet = new Packet((int)ClientPackets.VRLeftHandData))
        {
            _packet.Write(Client.instance.id);
            _packet.Write(dataPacket.HandPosition);
            _packet.Write(dataPacket.HandRotation);
            _packet.Write(dataPacket.Trigger);
            _packet.Write(dataPacket.Grip);
            _packet.Write(dataPacket.Velocity);

            SendUDPData(_packet);
        }
    }

    public static void HeadData(Vector3 pos, Quaternion rot)
    {
        using (Packet _packet = new Packet((int)ClientPackets.VRLeftHandData))
        {
            _packet.Write(Client.instance.id);
            _packet.Write(pos);
            _packet.Write(rot);
            
            SendUDPData(_packet);
        }
    }

    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength(); //Add a length to the packet
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength(); //Add a length to the packet
        Client.instance.udp.SendData(_packet);
    }
}

