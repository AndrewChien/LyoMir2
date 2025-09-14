﻿using MemoryPack;

namespace OpenMir2.Data
{
    [MemoryPackable]
    public partial struct RecordHeader
    {
        public string sAccount { get; set; }
        public string Name { get; set; }
        public int SelectID { get; set; }
        public double dCreateDate { get; set; }
        public bool Deleted { get; set; }
        public double UpdateDate { get; set; }
        public double CreateDate { get; set; }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}