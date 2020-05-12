using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


public class ServerPacketSender
{
    public static void WelcomeMessage(int client, string msg)
    {
        using (Packet packet = new Packet((int)ServerPackets.Welcome))
        {
            packet.Write(msg);
            packet.Write(client);

            SendTCPPacket(client, packet);
        }
        Console.WriteLine($"Welcome message sent to client {client}");
    }

    public static void UdpTestMessage(int client)
    {
        using (Packet packet = new Packet((int)ServerPackets.UpdTestSent))
        {
            packet.Write("This is a UDP test from the Server!");

            SendUDPPacket(client, packet);
        }
        Console.WriteLine($"UdpTest message sent to client {client}");
    }

    public static void PlayerPostion(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.PlayerPosition))
        {
            packet.Write(player.id);
            packet.Write(player.transform.position);

            SendUDPPacketAll(packet);
        }
    }

    public static void SpawnPlayer(int targetClient, Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.SpawnPlayer))
        {
            packet.Write(player.id);
            packet.Write(player.UserName);
            packet.Write(player.position);

            Console.WriteLine("Send SpawnPlayer message to client: " + targetClient);
            SendTCPPacket(targetClient, packet);
        }
    }



    private static void SendTCPPacket(int _clientId, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_clientId].tcp.SendData(_packet);
    }

    private static void SendUDPPacket(int _clientId, Packet _packet)
    {
        _packet.WriteLength();
        Server.SendUDPData(Server.clients[_clientId].udp.endPoint, _packet);
    }

    private static void SendUDPPacketAll(Packet _packet, int excludeId = -1)
    {
        _packet.WriteLength();
        foreach (var client in Server.clients.Values)
        {
            if (client.id != excludeId)
                Server.SendUDPData(client.udp.endPoint, _packet);

        }
        
    }
}

