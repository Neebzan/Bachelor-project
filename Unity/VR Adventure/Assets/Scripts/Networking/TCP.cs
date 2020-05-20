using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;


public class TCP
{
    public int dataBufferSize = 4096;

    public TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer;

    private Packet incomingPacket;

    public event EventHandler<int> Disconnected;

    private readonly int id;

    public bool isConnected = false;

    private static Dictionary<int, PacketHandler> packetHandlers;

    public TCP(int _id)
    {
        id = _id;
    }

    public TCP()
    {

    }

    public void Connect(string _ip, int _port, Dictionary<int, PacketHandler> _packetHandlers)
    {
        client = new TcpClient()
        {
            SendBufferSize = dataBufferSize,
            ReceiveBufferSize = dataBufferSize
        };

        packetHandlers = _packetHandlers;

        //Initialize the packet class
        incomingPacket = new Packet();

        receiveBuffer = new byte[dataBufferSize];
        client.BeginConnect(_ip, _port, ConnectCallback, client);
    }

    public void Connect(TcpClient _client, Dictionary<int, PacketHandler> _packetHandlers)
    {
        client = _client;
        client.ReceiveBufferSize = dataBufferSize;

        packetHandlers = _packetHandlers;

        stream = client.GetStream();

        receiveBuffer = new byte[dataBufferSize];

        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

        //Initialize the packet class
        incomingPacket = new Packet();

        ServerPacketSender.WelcomeMessage(id, "This is a welcome message!");
    }

    public void SendData(Packet _packet)
    {
        try
        {
            if (client != null)
            {
                stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Couldn't sent data via TCP: {e}");
        }
    }

    private void ConnectCallback(IAsyncResult _result)
    {
        try
        {
            client.EndConnect(_result);

            Debug.Log("Connecting!");

            if (!client.Connected)
                return;

            Debug.Log("Connected!");

            stream = client.GetStream();



            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }
        catch (Exception e)
        {

            Debug.Log(e);
        }
    }

    private void ReceiveCallback(IAsyncResult _result)
    {
        try
        {
            int bytesReceived = stream.EndRead(_result);
            //
            if (bytesReceived <= 0)
            {
                Disconnect();
                return;
            }

            byte[] data = new byte[bytesReceived];
            Array.Copy(receiveBuffer, data, bytesReceived);

            incomingPacket.Reset(HandleData(data));

            //Wait for next data
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Disconnect();
        }
    }

    private bool HandleData(byte[] _data)
    {
        int packetLength = 0;

        incomingPacket.SetBytes(_data);

        //Check if there is 4 or more bytes in the packet (size of int is 4)
        if (incomingPacket.UnreadLength() >= 4)
        {
            //Read the length of the incoming packet
            packetLength = incomingPacket.ReadInt();
            if (packetLength <= 0)
            {
                //If the length is 0, return
                return true;
            }
        }

        //Keep reading until there is no more data left to read for this specific packet
        //We keep this in a while loop here because one packet might be made up of several packets
        while (packetLength > 0 && packetLength <= incomingPacket.UnreadLength())
        {
            byte[] bytes = incomingPacket.ReadBytes(packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(bytes))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });

            packetLength = 0;

            //Check if there 4 or more bytes in the packet (size of int is 4)
            if (incomingPacket.UnreadLength() >= 4)
            {
                //Read the length of the incoming packet
                packetLength = incomingPacket.ReadInt();
                if (packetLength <= 0)
                {
                    //If the length is 0, return
                    return true;
                }

            }
        }

        if (packetLength <= 1)
        {
            return true;
        }

        return false;
    }

    public void Disconnect()
    {
        Disconnected?.Invoke(this, id);
        client.Close();
        incomingPacket = null;
        receiveBuffer = null;
        client.Close();
    }
}
