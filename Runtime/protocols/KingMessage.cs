using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketClientPackage.Runtime.protocols
{
public class KingMessage
{
    private int _writeOffset = 0;
    private int _readOffset = 0;
    private byte[] _buffer;
    private bool _isBigEndian = false;
    private int _controllerId = 0;
    private int _requestId = 0;

    public KingMessage(byte[] buffer = null, int size = 2048, bool isBigEndian = false)
    {
        if (buffer != null)
        {
            _buffer = buffer;
        }
        else
        {
            _buffer = new byte[size];
        }
        _isBigEndian = isBigEndian;
    }

    public void SetDirection(int controllerId, int requestId, bool isWrite = true, bool isRead = true)
    {
        byte[] aBuffer = new byte[8];
        
        if (_isBigEndian)
        {
            WriteInt32BE(aBuffer, controllerId, 0);
            WriteInt32BE(aBuffer, requestId, 4);
        }
        else
        {
            WriteInt32LE(aBuffer, controllerId, 0);
            WriteInt32LE(aBuffer, requestId, 4);
        }

        byte[] newBuffer = new byte[aBuffer.Length + _buffer.Length];
        Buffer.BlockCopy(aBuffer, 0, newBuffer, 0, aBuffer.Length);
        Buffer.BlockCopy(_buffer, 0, newBuffer, aBuffer.Length, _buffer.Length);
        _buffer = newBuffer;

        if (isWrite) _writeOffset += 8;
        if (isRead) _readOffset += 8;

        _controllerId = controllerId;
        _requestId = requestId;
    }

    public byte[] GetFinalBuffer()
    {
        if (_writeOffset < _buffer.Length)
        {
            byte[] finalBuffer = new byte[_writeOffset];
            Buffer.BlockCopy(_buffer, 0, finalBuffer, 0, _writeOffset);
            return finalBuffer;
        }
        return _buffer;
    }

    public byte[] GetBuffer()
    {
        return _buffer;
    }

    public void Reset(int bufferSize = 2048)
    {
        _writeOffset = 0;
        _readOffset = 0;
        _buffer = new byte[bufferSize];
        _controllerId = 0;
        _requestId = 0;
    }

    public void WriteKMsg(KingMessage kMsg, bool isBehind)
    {
        byte[] kMsgBuffer = kMsg.GetFinalBuffer();
        if (isBehind)
        {
            byte[] newBuffer = new byte[GetFinalBuffer().Length + kMsgBuffer.Length];
            Buffer.BlockCopy(GetFinalBuffer(), 0, newBuffer, 0, GetFinalBuffer().Length);
            Buffer.BlockCopy(kMsgBuffer, 0, newBuffer, GetFinalBuffer().Length, kMsgBuffer.Length);
            _buffer = newBuffer;
        }
        else
        {
            byte[] newBuffer = new byte[kMsgBuffer.Length + _buffer.Length];
            Buffer.BlockCopy(kMsgBuffer, 0, newBuffer, 0, kMsgBuffer.Length);
            Buffer.BlockCopy(_buffer, 0, newBuffer, kMsgBuffer.Length, _buffer.Length);
            _buffer = newBuffer;
            _requestId = kMsg.GetRequestId();
            _controllerId = kMsg.GetControllerId();
        }
        _writeOffset += kMsg.GetWriteOffset();
    }

    public void WriteBuffer(byte[] buffer, bool isTop = false)
    {
        // Note: isTop parameter not fully implemented as in original JS
        byte[] newBuffer = new byte[_buffer.Length + buffer.Length];
        Buffer.BlockCopy(_buffer, 0, newBuffer, 0, _buffer.Length);
        Buffer.BlockCopy(buffer, 0, newBuffer, _buffer.Length, buffer.Length);
        _buffer = newBuffer;
        _writeOffset += buffer.Length;
    }

    // Write methods for various types
    public void WriteInt8(sbyte val, int offset = -1)
    {
        if (offset == -1)
        {
            offset = _writeOffset;
            _writeOffset += 1;
        }
        _buffer[offset] = (byte)val;
    }

    public void WriteInt8Arr(sbyte[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (sbyte val in arr)
        {
            WriteInt8(val);
        }
    }

    public void WriteUInt8(byte val, int offset = -1)
    {
        if (offset == -1)
        {
            offset = _writeOffset;
            _writeOffset += 1;
        }
        _buffer[offset] = val;
    }

    public void WriteUInt8Arr(byte[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (byte val in arr)
        {
            WriteUInt8(val);
        }
    }

    public void WriteInt16(short val, int offset = -1)
    {
        if (offset == -1)
        {
            offset = _writeOffset;
            _writeOffset += 2;
        }
        if (_isBigEndian)
            WriteInt16BE(_buffer, val, offset);
        else
            WriteInt16LE(_buffer, val, offset);
    }

    public void WriteInt16Arr(short[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (short val in arr)
        {
            WriteInt16(val);
        }
    }

    public void WriteUInt16(ushort val, int offset = -1)
    {
        if (offset == -1)
        {
            offset = _writeOffset;
            _writeOffset += 2;
        }
        if (_isBigEndian)
            WriteUInt16BE(_buffer, val, offset);
        else
            WriteUInt16LE(_buffer, val, offset);
    }

    public void WriteUInt16Arr(ushort[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (ushort val in arr)
        {
            WriteUInt16(val);
        }
    }

    public void WriteInt32(int val, int offset = -1)
    {
        if (offset == -1)
        {
            offset = _writeOffset;
            _writeOffset += 4;
        }
        if (_isBigEndian)
            WriteInt32BE(_buffer, val, offset);
        else
            WriteInt32LE(_buffer, val, offset);
    }

    public void WriteInt32Arr(int[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (int val in arr)
        {
            WriteInt32(val);
        }
    }

    public void WriteUInt32(uint val, int offset = -1)
    {
        if (offset == -1)
        {
            offset = _writeOffset;
            _writeOffset += 4;
        }
        if (_isBigEndian)
            WriteUInt32BE(_buffer, val, offset);
        else
            WriteUInt32LE(_buffer, val, offset);
    }

    public void WriteUInt32Arr(uint[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (uint val in arr)
        {
            WriteUInt32(val);
        }
    }

    public void WriteInt(long val, int offset = -1)
    {
        if (offset == -1)
        {
            offset = _writeOffset;
            _writeOffset += 6;
        }
        if (_isBigEndian)
            WriteIntBE(_buffer, val, offset, 6);
        else
            WriteIntLE(_buffer, val, offset, 6);
    }

    public void WriteIntArr(long[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (long val in arr)
        {
            WriteInt(val);
        }
    }

    public void WriteUInt64(ulong val, int offset = -1)
    {
        if (offset == -1)
        {
            offset = _writeOffset;
            _writeOffset += 8;
        }
        if (_isBigEndian)
        {
            WriteUInt32BE(_buffer, (uint)(val >> 32), offset);
            WriteUInt32BE(_buffer, (uint)(val & 0xFFFFFFFF), offset + 4);
        }
        else
        {
            WriteUInt32LE(_buffer, (uint)(val & 0xFFFFFFFF), offset);
            WriteUInt32LE(_buffer, (uint)(val >> 32), offset + 4);
        }
    }

    public void WriteUInt64Arr(ulong[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (ulong val in arr)
        {
            WriteUInt64(val);
        }
    }

    public void WriteInt64(long val, int offset = -1)
    {
        if (offset == -1)
        {
            offset = _writeOffset;
            _writeOffset += 8;
        }
        if (_isBigEndian)
        {
            WriteInt32BE(_buffer, (int)(val >> 32), offset);
            WriteInt32BE(_buffer, (int)(val & 0xFFFFFFFF), offset + 4);
        }
        else
        {
            WriteInt32LE(_buffer, (int)(val & 0xFFFFFFFF), offset);
            WriteInt32LE(_buffer, (int)(val >> 32), offset + 4);
        }
    }

    public void WriteInt64Arr(long[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (long val in arr)
        {
            WriteInt64(val);
        }
    }

    public void WriteBool(bool val, int offset = -1)
    {
        if (offset == -1)
        {
            offset = _writeOffset;
            _writeOffset += 1;
        }
        byte byteVal = val ? (byte)1 : (byte)0;
        _buffer[offset] = byteVal;
    }

    public void WriteBoolArr(bool[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (bool val in arr)
        {
            WriteBool(val);
        }
    }

    public void WriteFloat(float val, int offset = -1)
    {
        if (offset == -1)
        {
            offset = _writeOffset;
            _writeOffset += 4;
        }
        byte[] bytes = BitConverter.GetBytes(val);
        if (_isBigEndian != BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        Buffer.BlockCopy(bytes, 0, _buffer, offset, 4);
    }

    public void WriteFloatArr(float[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (float val in arr)
        {
            WriteFloat(val);
        }
    }

    public void WriteDouble(double val, int offset = -1)
    {
        if (offset == -1)
        {
            offset = _writeOffset;
            _writeOffset += 8;
        }
        byte[] bytes = BitConverter.GetBytes(val);
        if (_isBigEndian != BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        Buffer.BlockCopy(bytes, 0, _buffer, offset, 8);
    }

    public void WriteDoubleArr(double[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (double val in arr)
        {
            WriteDouble(val);
        }
    }

    public void WriteString(string val)
    {
        int length = Encoding.UTF8.GetByteCount(val);
        WriteUInt16((ushort)length);
        if (length <= 0) return;

        byte[] stringBytes = Encoding.UTF8.GetBytes(val);
        Buffer.BlockCopy(stringBytes, 0, _buffer, _writeOffset, length);
        _writeOffset += length;
    }

    public void WriteUtf8StringRrr(string[] arr)
    {
        WriteUInt8((byte)arr.Length);
        foreach (string val in arr)
        {
            WriteString(val);
        }
    }

    // Read methods
    public uint ReadUInt32()
    {
        uint val;
        if (_isBigEndian)
            val = ReadUInt32BE(_buffer, _readOffset);
        else
            val = ReadUInt32LE(_buffer, _readOffset);
        _readOffset += 4;
        return val;
    }

    public uint[] ReadUInt32Arr()
    {
        byte length = ReadUInt8();
        uint[] arr = new uint[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = ReadUInt32();
        }
        return arr;
    }

    public int ReadInt32()
    {
        int val;
        if (_isBigEndian)
            val = ReadInt32BE(_buffer, _readOffset);
        else
            val = ReadInt32LE(_buffer, _readOffset);
        _readOffset += 4;
        return val;
    }

    public int[] ReadInt32Arr()
    {
        byte length = ReadUInt8();
        int[] arr = new int[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = ReadInt32();
        }
        return arr;
    }

    public float ReadFloat()
    {
        byte[] bytes = new byte[4];
        Buffer.BlockCopy(_buffer, _readOffset, bytes, 0, 4);
        if (_isBigEndian != BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        float val = BitConverter.ToSingle(bytes, 0);
        _readOffset += 4;
        return val;
    }

    public float[] ReadFloatArr()
    {
        byte length = ReadUInt8();
        float[] arr = new float[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = ReadFloat();
        }
        return arr;
    }

    public double ReadDouble()
    {
        byte[] bytes = new byte[8];
        Buffer.BlockCopy(_buffer, _readOffset, bytes, 0, 8);
        if (_isBigEndian != BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        double val = BitConverter.ToDouble(bytes, 0);
        _readOffset += 8;
        return val;
    }

    public double[] ReadDoubleArr()
    {
        byte length = ReadUInt8();
        double[] arr = new double[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = ReadDouble();
        }
        return arr;
    }

    public byte ReadUInt8()
    {
        byte val = _buffer[_readOffset];
        _readOffset += 1;
        return val;
    }

    public byte[] ReadUInt8Arr()
    {
        byte length = ReadUInt8();
        byte[] arr = new byte[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = ReadUInt8();
        }
        return arr;
    }

    public sbyte ReadInt8()
    {
        sbyte val = (sbyte)_buffer[_readOffset];
        _readOffset += 1;
        return val;
    }

    public sbyte[] ReadInt8Arr()
    {
        byte length = ReadUInt8();
        sbyte[] arr = new sbyte[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = ReadInt8();
        }
        return arr;
    }

    public ushort ReadUInt16()
    {
        ushort val;
        if (_isBigEndian)
            val = ReadUInt16BE(_buffer, _readOffset);
        else
            val = ReadUInt16LE(_buffer, _readOffset);
        _readOffset += 2;
        return val;
    }

    public ushort[] ReadUInt16Arr()
    {
        byte length = ReadUInt8();
        ushort[] arr = new ushort[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = ReadUInt16();
        }
        return arr;
    }

    public short ReadInt16()
    {
        short val;
        if (_isBigEndian)
            val = ReadInt16BE(_buffer, _readOffset);
        else
            val = ReadInt16LE(_buffer, _readOffset);
        _readOffset += 2;
        return val;
    }

    public short[] ReadInt16Arr()
    {
        byte length = ReadUInt8();
        short[] arr = new short[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = ReadInt16();
        }
        return arr;
    }

    public bool ReadBool()
    {
        byte val = _buffer[_readOffset];
        _readOffset += 1;
        return val != 0;
    }

    public bool[] ReadBoolArr()
    {
        byte length = ReadUInt8();
        bool[] arr = new bool[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = ReadBool();
        }
        return arr;
    }

    public string ReadUtf8StringFixedLength(int length)
    {
        byte[] strBuf = new byte[length];
        Buffer.BlockCopy(_buffer, _readOffset, strBuf, 0, length);
        
        int breakIndex = -1;
        for (int i = 0; i < length; i++)
        {
            if (strBuf[i] == 0)
            {
                breakIndex = i;
                break;
            }
        }

        if (breakIndex != -1)
        {
            byte[] temp = new byte[breakIndex];
            Buffer.BlockCopy(strBuf, 0, temp, 0, breakIndex);
            strBuf = temp;
        }

        _readOffset += length;
        return Encoding.UTF8.GetString(strBuf);
    }

    public string ReadUtf8String()
    {
        ushort length = ReadUInt16();
        if (length == 0) return string.Empty;

        byte[] strBuf = new byte[length];
        Buffer.BlockCopy(_buffer, _readOffset, strBuf, 0, length);
        
        int breakIndex = -1;
        for (int i = 0; i < length; i++)
        {
            if (strBuf[i] == 0)
            {
                breakIndex = i;
                break;
            }                                                                  
        }

        if (breakIndex != -1)
        {
            byte[] temp = new byte[breakIndex];
            Buffer.BlockCopy(strBuf, 0, temp, 0, breakIndex);
            strBuf = temp;
        }

        _readOffset += length;
        return Encoding.UTF8.GetString(strBuf);
    }

    public string[] ReadUtf8StringArr()
    {
        byte length = ReadUInt8();
        string[] arr = new string[length];
        for (int i = 0; i < length; i++)
        {
            arr[i] = ReadUtf8String();
        }
        return arr;
    }

    // Utility methods
    public int GetLength()
    {
        return _buffer.Length;
    }

    public bool IsBigEndian()
    {
        return _isBigEndian;
    }

    public void SetBigEndian(bool value)
    {
        _isBigEndian = value;
    }

    public int GetWriteOffset()
    {
        return _writeOffset;
    }

    public void SetWriteOffset(int value)
    {
        _writeOffset = value;
    }

    public int GetReadOffset()
    {
        return _readOffset;
    }

    public void SetReadOffset(int value)
    {
        _readOffset = value;
    }

    public void SetBuffer(byte[] value)
    {
        _buffer = value;
    }

    public int GetControllerId()
    {
        return _controllerId;
    }

    public void SetControllerId(int value)
    {
        _controllerId = value;
    }

    public int GetRequestId()
    {
        return _requestId;
    }

    public void SetRequestId(int value)
    {
        _requestId = value;
    }

    public bool IsCanRead()
    {
        return _buffer.Length > _readOffset;
    }

    // Helper methods for endian-aware reading/writing
    private static void WriteInt16BE(byte[] buffer, short value, int offset)
    {
        buffer[offset] = (byte)(value >> 8);
        buffer[offset + 1] = (byte)value;
    }

    private static void WriteInt16LE(byte[] buffer, short value, int offset)
    {
        buffer[offset] = (byte)value;
        buffer[offset + 1] = (byte)(value >> 8);
    }

    private static void WriteUInt16BE(byte[] buffer, ushort value, int offset)
    {
        buffer[offset] = (byte)(value >> 8);
        buffer[offset + 1] = (byte)value;
    }

    private static void WriteUInt16LE(byte[] buffer, ushort value, int offset)
    {
        buffer[offset] = (byte)value;
        buffer[offset + 1] = (byte)(value >> 8);
    }

    private static void WriteInt32BE(byte[] buffer, int value, int offset)
    {
        buffer[offset] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)value;
    }

    private static void WriteInt32LE(byte[] buffer, int value, int offset)
    {
        buffer[offset] = (byte)value;
        buffer[offset + 1] = (byte)(value >> 8);
        buffer[offset + 2] = (byte)(value >> 16);
        buffer[offset + 3] = (byte)(value >> 24);
    }

    private static void WriteUInt32BE(byte[] buffer, uint value, int offset)
    {
        buffer[offset] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)value;
    }

    private static void WriteUInt32LE(byte[] buffer, uint value, int offset)
    {
        buffer[offset] = (byte)value;
        buffer[offset + 1] = (byte)(value >> 8);
        buffer[offset + 2] = (byte)(value >> 16);
        buffer[offset + 3] = (byte)(value >> 24);
    }

    private static void WriteIntBE(byte[] buffer, long value, int offset, int bytes)
    {
        for (int i = 0; i < bytes; i++)
        {
            buffer[offset + i] = (byte)(value >> (8 * (bytes - 1 - i)));
        }
    }

    private static void WriteIntLE(byte[] buffer, long value, int offset, int bytes)
    {
        for (int i = 0; i < bytes; i++)
        {
            buffer[offset + i] = (byte)(value >> (8 * i));
        }
    }

    private static short ReadInt16BE(byte[] buffer, int offset)
    {
        return (short)((buffer[offset] << 8) | buffer[offset + 1]);
    }

    private static short ReadInt16LE(byte[] buffer, int offset)
    {
        return (short)(buffer[offset] | (buffer[offset + 1] << 8));
    }

    private static ushort ReadUInt16BE(byte[] buffer, int offset)
    {
        return (ushort)((buffer[offset] << 8) | buffer[offset + 1]);
    }

    private static ushort ReadUInt16LE(byte[] buffer, int offset)
    {
        return (ushort)(buffer[offset] | (buffer[offset + 1] << 8));
    }

    private static int ReadInt32BE(byte[] buffer, int offset)
    {
        return (buffer[offset] << 24) | (buffer[offset + 1] << 16) | (buffer[offset + 2] << 8) | buffer[offset + 3];
    }

    private static int ReadInt32LE(byte[] buffer, int offset)
    {
        return buffer[offset] | (buffer[offset + 1] << 8) | (buffer[offset + 2] << 16) | (buffer[offset + 3] << 24);
    }

    private static uint ReadUInt32BE(byte[] buffer, int offset)
    {
        return (uint)((buffer[offset] << 24) | (buffer[offset + 1] << 16) | (buffer[offset + 2] << 8) | buffer[offset + 3]);
    }

    private static uint ReadUInt32LE(byte[] buffer, int offset)
    {
        return (uint)(buffer[offset] | (buffer[offset + 1] << 8) | (buffer[offset + 2] << 16) | (buffer[offset + 3] << 24));
    }
}
}



