using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDP
{
    public UdpClient client;
    public IPEndPoint endPoint;
    private readonly int id;

    private static Dictionary<int, PacketHandler> packetHandlers;

    public UDP(Dictionary<int, PacketHandler> _packetHandlers)
    {
        packetHandlers = _packetHandlers;
    }

    public UDP(int _id)
    {
        id = _id;
    }

    public void Connect(IPEndPoint _endPoint, Dictionary<int, PacketHandler> _packetHandlers)
    {
        packetHandlers = _packetHandlers;
        endPoint = _endPoint;
        //ServerPacketSender.UdpTestMessage(id);
    }

    public void Connect(string _ip, int _port, int _localPort)
    {
        endPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);

        client = new UdpClient(_localPort);

        client.Connect(endPoint);

        client.BeginReceive(ReceiveCallback, null);

        //Send a packet to the server to initialize communication
        using (Packet _packet = new Packet())
        {
            //Packet containing only the sender ID (Part of the inital communication setup)
            SendData(_packet);
        }
    }

    public void SendData(Packet _packet)
    {
        try
        {
            //Add the client's id so that the server knows who sent the packet
            _packet.InsertInt(Client.instance.ID);
            if (client != null)
            {
                client.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Console.WriteLine(e);
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            byte[] _data = client.EndReceive(ar, ref endPoint);
            client.BeginReceive(ReceiveCallback, null);

            if (_data.Length < 4)
            {
                Disconnect();
                return;
            }

            HandleData(_data);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Console.WriteLine(e);
            Disconnect();
        }
    }

    private void HandleData(byte[] data)
    {
        using (Packet _packet = new Packet(data))
        {
            int _packetLength = _packet.ReadInt();
            data = _packet.ReadBytes(_packetLength);
        }

        ThreadManager.ExecuteOnMainThread(() =>
        {
            using (Packet _packet = new Packet(data))
            {
                int _packetId = _packet.ReadInt();
                packetHandlers[_packetId](_packet);
            }
        });
    }

    public void HandleData(Packet _packet)
    {
        int _packetLength = _packet.ReadInt();
        byte[] bytes = _packet.ReadBytes(_packetLength);

        ThreadManager.ExecuteOnMainThread(() =>
        {
            using (Packet packet = new Packet(bytes))
            {
                int _packetId = packet.ReadInt();
                packetHandlers[_packetId](packet);
            }
        });
    }

    public void Disconnect()
    {
        endPoint = null;
        client?.Close();
    }
}
