﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum ServerPackets
{
    Welcome
}

public enum ClientPackets
{

}


public class Packet : IDisposable
{
    private List<byte> buffer;
    private byte[] readableBuffer;
    private int readPos;


    public Packet()
    {
        buffer = new List<byte>();
        readPos = 0;
    }

    public Packet(int _packetId)
    {
        buffer = new List<byte>();
        readPos = 0;

        Write(_packetId);
    }

    public Packet(byte[] _data)
    {
        buffer = new List<byte>();
        readPos = 0;

        SetBytes(_data);
    }

    public void SetBytes(byte[] _data)
    {
        Write(_data);
        readableBuffer = buffer.ToArray();
    }

    /// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
    public void WriteLength()
    {
        buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); // Insert the byte length of the packet at the very beginning
    }

    public int Length()
    {
        return buffer.Count;
    }

    /// <summary>Gets the packet's content in array form.</summary>
    public byte[] ToArray()
    {
        readableBuffer = buffer.ToArray();
        return readableBuffer;
    }

    public void Reset(bool _reset)
    {
        if (_reset)
        {
            buffer.Clear();
            readableBuffer = null;
            readPos = 0;
        }
        else
            readPos -= 4; //"Unread" bytes for some reason?
    }

    #region WriteMethods
    public void Write(byte[] _value)
    {
        buffer.AddRange(_value);
    }
    public void Write(float _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }

    public void Write(string _value)
    {
        Write(_value.Length); //Write the length of the string
        buffer.AddRange(Encoding.ASCII.GetBytes(_value)); //Add the string
    }

    public void Write(int _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }

    public void Write(Vector3 _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
    }

    public void Write(Quaternion _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
        Write(_value.w);
    }

    #endregion

    #region ReadMethods
    public int ReadInt(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            //When there's unread bytes
            int _value = BitConverter.ToInt32(readableBuffer, readPos);
            if (_moveReadPos)
            {
                readPos += 4; //Size of int
            }
            return _value;
        }
        else
        {
            throw new Exception("Couldn't read value of type 'int'!");
        }
    }

    public string ReadString(bool _moveReadPos = true)
    {
        try
        {
            //First read the length of the incoming string
            int length = ReadInt();

            string _value = Encoding.ASCII.GetString(readableBuffer, readPos, length);

            //If the current read pos is suppose to be moved
            //and the length of  the string was more than ZERO
            if (_moveReadPos && _value.Length > 0)
            {
                readPos += length; //Size of the string
            }
            return _value;
        }
        catch
        {
            throw new Exception("Couldn't read value of type 'string'!");
        }
    }
    #endregion

    private bool disposed = false;

    protected virtual void Dispose(bool _disposing)
    {
        if (!disposed)
        {
            if (_disposing)
            {
                buffer = null;
                readableBuffer = null;
                readPos = 0;
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
