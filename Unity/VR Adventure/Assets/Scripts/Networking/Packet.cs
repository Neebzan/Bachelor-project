using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


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

    /// <summary>Gets the length of the unread data contained in the packet.</summary>
    public int UnreadLength()
    {
        return Length() - readPos; // Return the remaining length (unread)
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

    /// <summary>
    /// Insert an int into the buffer at the given position (Default 0)
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="pos"></param>
    public void InsertInt(int _value, int pos = 0)
    {
        buffer.InsertRange(pos, BitConverter.GetBytes(_value));
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
    public void Write(bool _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }

    #endregion

    #region ReadMethods
    public byte[] ReadBytes(int _length, bool _moveReadPos = true)
    {
        if(buffer.Count > readPos)
        {
            //Get the bytes from the current read position in the buffer, to the amount of "_length"
            byte[] _value = buffer.GetRange(readPos, _length).ToArray();
            if(_moveReadPos)
            {
                //If we're moving the current read position
                readPos += _length;
            }
            return _value;
        }
        else
        {
            throw new Exception("Couldn't read value of type 'byte[]'!");
        }
    }
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

    public bool ReadBool(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            //When there's unread bytes
            bool _value = BitConverter.ToBoolean(readableBuffer, readPos);
            if (_moveReadPos)
            {
                readPos += 1; // size of bool
            }
            return _value;
        }
        else
        {
            throw new Exception("Could not read value of type 'bool'!");
        }
    }
    public float ReadFloat(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            //When there's unread bytes
            float _value = BitConverter.ToSingle(readableBuffer, readPos);
            if (_moveReadPos)
            {
                readPos += 4; //Size of int
            }
            return _value;
        }
        else
        {
            throw new Exception("Couldn't read value of type 'float'!");
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

    public Vector3 ReadVector3(bool _moveReadPos = true)
    {
        return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
    }

    public Quaternion ReadQuaternion(bool _moveReadPos = true)
    {
        return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
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
