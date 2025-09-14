﻿using OpenMir2;
using OpenMir2.NativeList.Utils;

namespace SystemModule.Data
{
    public struct MapCellInfo
    {
        /// <summary>
        /// 对象数量
        /// </summary>
        public readonly int Count => ObjList == null ? 0 : ObjList.Count;
        /// <summary>
        /// 地图对象列表
        /// </summary>
        public NativeList<CellObject> ObjList { get; set; }
        /// <summary>
        /// 是否可以移动
        /// </summary>
        public readonly bool Valid => Attribute == CellAttribute.Walk;
        /// <summary>
        /// 移动标识
        /// </summary>
        public CellAttribute Attribute = CellAttribute.Walk;

        public MapCellInfo()
        {
            ObjList = null;
        }

        public readonly bool IsAvailable => ObjList?.Count > 0;

        public readonly void Add(CellObject cell)
        {
            ObjList.Add(cell);
        }

        public readonly void Update(int index, ref CellObject cell)
        {
            cell.AddTime = HUtil32.GetTickCount();
            ObjList[index] = cell;
        }

        public readonly void Remove(CellObject index)
        {
            //todo 异步通知处理并移除
            ObjList.Remove(index);
        }

        public readonly void Remove(int index)
        {
            ObjList.RemoveAt(index);
        }

        public void SetAttribute(CellAttribute cellAttribute)
        {
            Attribute = cellAttribute;
        }

        public readonly void Clear()
        {
            ObjList.Clear();
            //ObjList = null;
        }
    }
}