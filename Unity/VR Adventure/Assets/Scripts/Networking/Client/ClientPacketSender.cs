using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ClientPacketSender : MonoBehaviour
{
    public static void WelcomeReceived()
    {
        using(Packet _packet = new Packet((int)ClientPackets.WelcomeReceived))
        {
            _packet.Write(Client.instance.id);
            _packet.Write("TestUserName");

            SendTCPData(_packet);
        }
    }

    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength(); //Add a length to the packet
        Client.instance.tcp.SendData(_packet);
    }
}

