﻿using MemoryPack;
using System.Runtime.InteropServices;

namespace OpenMir2.Packets.ServerPackets
{
    [MemoryPackable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial record struct MessageBodyW
    {
        public ushort Param1;
        public ushort Param2;
        public ushort Tag1;
        public ushort Tag2;
    }
}