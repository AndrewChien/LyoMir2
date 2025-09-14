﻿using OpenMir2;
using SystemModule.Actors;

namespace CommandModule.Commands
{
    [Command("ClearItemMap", "清除指定地图范围物品", "地图编号", 10)]
    public class ClearItemMapCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            string sMap = @params.Length > 0 ? @params[0] : "";
            string sItemName = @params.Length > 1 ? @params[1] : "";
            int nX = @params.Length > 2 ? HUtil32.StrToInt(@params[2], 0) : 0;
            int nY = @params.Length > 3 ? HUtil32.StrToInt(@params[3], 0) : 0;
            int nRange = @params.Length > 4 ? HUtil32.StrToInt(@params[4], 0) : 0;
            if (string.IsNullOrEmpty(sMap) || string.IsNullOrEmpty(sItemName) || nX < 0 || nY < 0 || nRange < 0 || !string.IsNullOrEmpty(sItemName) && sItemName[0] == '?')
            {
                //PlayerActor.SysMsg(string.Format(Settings.GameCommandParamUnKnow, this.Attributes.Name, Settings.GameCommandCLEARITEMMAPHelpMsg), MsgColor.c_Red, MsgType.t_Hint);
                return;
            }
            if (sItemName == "ALL")
            {
            }
            // TMapItem MapItem = null;
            // var ItemList = new List<TMapItem>();
            // var Envir = Settings.g_MapMgr.FindMap(sMap);// 查找地图
            // if (Envir != null)
            // {
            //     ItemList = new List<TMapItem>();
            //     Envir.GetMapItem(nX, nY, nRange, ItemList);// 取地图上指定范围的物品
            //     if (!boClearAll)// /清除指定物品
            //     {
            //         if (ItemList.Count > 0)
            //         {
            //             for (int i = 0; i < ItemList.Count; i++)
            //             {
            //                 MapItem = ItemList[i];
            //                 if ((string.Compare(MapItem.Name, sItemName, StringComparison.OrdinalIgnoreCase) == 0))
            //                 {
            //                     for (int nXX = nX - nRange; nXX <= nX + nRange; nXX++)
            //                     {
            //                         for (int nYY = nY - nRange; nYY <= nY + nRange; nYY++)
            //                         {
            //                             Envir.DeleteFromMap(nXX, nYY, CellType.OS_ITEMOBJECT, MapItem);
            //                             if (MapItem == null)
            //                             {
            //                                 break;
            //                             }
            //                         }
            //                     }
            //                 }
            //             }
            //         }
            //     }
            //     else
            //     {
            //         //清除全部物品
            //         if (ItemList.Count > 0)
            //         {
            //             for (int i = 0; i < ItemList.Count; i++)
            //             {
            //                 MapItem = ItemList[i];
            //                 for (int nXX = nX - nRange; nXX <= nX + nRange; nXX++)
            //                 {
            //                     for (int nYY = nY - nRange; nYY <= nY + nRange; nYY++)
            //                     {
            //                         Envir.DeleteFromMap(nXX, nYY, CellType.OS_ITEMOBJECT, MapItem);
            //                         if (MapItem == null)
            //                         {
            //                             break;
            //                         }
            //                     }
            //                 }
            //             }
            //         }
            //     }
            //     ItemList = null;
            // }
        }
    }
}