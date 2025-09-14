using MemoryPack;
using System.Runtime.InteropServices;

namespace OpenMir2.Packets.ServerPackets
{
    //[MemoryPackable(GenerateType.CircularReference)]
    [MemoryPackable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct ServerDataPacket
    {
        /// <summary>
        /// 封包标识码
        /// </summary>
        public uint PacketCode { get; set; }
        /// <summary>
        /// 封包总长度
        /// </summary>
        public ushort PacketLen { get; set; }

        /// <summary>
        /// 消息头固定大小
        /// </summary>
        public const int FixedHeaderLen = 6;
    }

    //CircularReference适用于AOT
    [MemoryPackable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct ServerDataMessage
    {
        private ServerDataType _type;
        public ServerDataType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        private string _socketId;
        public string SocketId
        {
            get { return _socketId; }
            set { _socketId = value; }
        }
        private short _dataLen;
        public short DataLen
        {
            get { return _dataLen; }
            set { _dataLen = value; }
        }
        private byte[] _data;
        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }
    }

    public enum ServerDataType : byte
    {
        Enter = 0,
        Leave = 1,
        Data = 2,
        KeepAlive = 3
    }
}