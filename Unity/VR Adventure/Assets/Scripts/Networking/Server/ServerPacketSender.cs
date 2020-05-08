using System;
using System.Collections.Generic;
using System.Linq;
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
    }

    private static void SendTCPPacket(int _clientId, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_clientId].tcp.SendData(_packet);
    }
}

