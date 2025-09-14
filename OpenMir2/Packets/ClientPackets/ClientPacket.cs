﻿using System;
using System.IO;
using System.Text;

namespace OpenMir2.Packets.ClientPackets
{
    public abstract class ClientPacket
    {
        private readonly BinaryReader binaryReader;

        public ClientPacket() { }

        public ClientPacket(byte[] segment)
        {
            if (segment == null)
            {
                throw new Exception("segment is null");
            }
            binaryReader = new BinaryReader(new MemoryStream(segment));
        }

        public string ReadPascalString(int size)
        {
            byte packegeLen = binaryReader.ReadByte();
            if (size < packegeLen)
            {
                size = packegeLen;
            }
            byte[] strbuff = binaryReader.ReadBytes(size);
            return Encoding.GetEncoding("gb2312").GetString(strbuff, 0, packegeLen);
        }

        public int ReadInt32()
        {
            return binaryReader.ReadInt32();
        }

        public string ReadString()
        {
            return binaryReader.ReadString();
        }

        public uint ReadUInt32()
        {
            return binaryReader.ReadUInt32();
        }

        public short ReadInt16()
        {
            return binaryReader.ReadInt16();
        }

        public ushort ReadUInt16()
        {
            return binaryReader.ReadUInt16();
        }

        public double ReadDouble()
        {
            return binaryReader.ReadDouble();
        }

        public byte ReadByte()
        {
            return binaryReader.ReadByte();
        }

        public char ReadChar()
        {
            return binaryReader.ReadChar();
        }

        public int PeekChar()
        {
            return binaryReader.PeekChar();
        }

        public bool ReadBoolean()
        {
            return binaryReader.ReadBoolean();
        }

        public byte[] ReadBytes(int size)
        {
            return binaryReader.ReadBytes(size);
        }

        public ushort[] ReadUInt16(int size)
        {
            ushort[] shortarr = new ushort[size];
            for (int i = 0; i < shortarr.Length; i++)
            {
                shortarr[i] = binaryReader.ReadUInt16();
            }
            return shortarr;
        }

        public MemoryStream GetStream()
        {
            using MemoryStream stream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream);
            WritePacket(writer);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public byte[] GetBuffer()
        {
            using MemoryStream stream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream);
            WritePacket(writer);
            stream.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            return data;
        }

        public int GetPacketSize()
        {
            using MemoryStream stream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream);
            WritePacket(writer);
            stream.Seek(0, SeekOrigin.Begin);
            return (int)stream.Length;
        }

        public static T ToPacket<T>(Span<byte> rawBytes) where T : ClientPacket, new()
        {
            ClientPacket packet = Activator.CreateInstance<T>();
            using MemoryStream stream = new MemoryStream(rawBytes.ToArray());
            using BinaryReader reader = new BinaryReader(stream);
            try
            {
                if (packet == null)
                {
                    return null;
                }

                packet.ReadPacket(reader);
            }
            catch
            {
                return null;
            }
            return (T)packet;
        }

        public static T ToPacket<T>(byte[] rawBytes) where T : ClientPacket, new()
        {
            ClientPacket packet = Activator.CreateInstance<T>();
            using MemoryStream stream = new MemoryStream(rawBytes, 0, rawBytes.Length);
            using BinaryReader reader = new BinaryReader(stream);
            try
            {
                if (packet == null)
                {
                    return null;
                }

                packet.ReadPacket(reader);
            }
            catch
            {
                return null;
            }
            return (T)packet;
        }

        protected abstract void ReadPacket(BinaryReader reader);

        protected abstract void WritePacket(BinaryWriter writer);
    }
}