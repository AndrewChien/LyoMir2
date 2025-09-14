﻿using System.Text;
using SystemModule.Actors;

namespace CommandModule.Commands
{
    [Command("ServerInfo", "查看服务器信息", 10)]
    public class ServerInfoCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(string.Format("({0}) [{1}/{2}] [{3}/{4}] [{5}/{6}]", SystemShare.WorldEngine.MonsterCount,
            //       TRunSocket.g_nGateRecvMsgLenMin, TRunSocket.g_nGateRecvMsgLenMax, SystemShare.WorldEngine.OnlineIPlayerActor,
            //       SystemShare.WorldEngine.IPlayerActorCount, SystemShare.WorldEngine.LoadPlayCount, SystemShare.WorldEngine.m_IPlayerActorFreeList.Count));
            //sb.AppendLine(string.Format("Run({0}/{1}) Soc({2}/{3}) Usr({4}/{5})", M2Share.nRunTimeMin, M2Share.nRunTimeMax, Settings.g_nSockCountMin,
            //        Settings.g_nSockCountMax, Settings.g_nUsrTimeMin, Settings.g_nUsrTimeMax));
            //sb.AppendLine(string.Format("Hum{0}/{1} Usr{2}/{3} Mer{4}/{5} Npc{6}/{7}", Settings.g_nHumCountMin, Settings.g_nHumCountMax,
            //        M2Share.dwUsrRotCountMin, M2Share.dwUsrRotCountMax, SystemShare.WorldEngine.dwProcessMerchantTimeMin,
            //        SystemShare.WorldEngine.dwProcessMerchantTimeMax, SystemShare.WorldEngine.dwProcessNpcTimeMin, SystemShare.WorldEngine.dwProcessNpcTimeMax,
            //        Settings.g_nProcessHumanLoopTime));
            //sb.AppendLine(string.Format("MonG({0}/{1}/{2}) MonP({3}/{4}/{5}) ObjRun({6}/{7})", Settings.g_nMonGenTime, Settings.g_nMonGenTimeMin,
            //      Settings.g_nMonGenTimeMax, Settings.g_nMonProcTime, Settings.g_nMonProcTimeMin, Settings.g_nMonProcTimeMax, Settings.g_nBaseObjTimeMin,
            //      Settings.g_nBaseObjTimeMax));
            //if (M2Share.dwStartTimeTick == 0)
            //{
            //    M2Share.dwStartTimeTick = HUtil32.GetTickCount();
            //}
            //LogService.Error(sb.ToString());

            //TGateInfo GateInfo;
            //sb.Clear();
            //for (int i = TRunSocket.g_GateArr.GetLowerBound(0); i <= TRunSocket.g_GateArr..Length(0); i++)
            //{
            //    GateInfo = TRunSocket.g_GateArr[i];
            //    if (GateInfo.boUsed && (GateInfo.Socket != null))
            //    {
            //        sb.Append(string.Format("Gate [0] ", i));
            //        sb.Append(string.Format("Gate [{0}]:[{1}] ", GateInfo.sAddr, GateInfo.nPort));
            //        sb.Append(string.Format("Gate SendMessage:[{0}] ", GateInfo.nSendedMsgCount));
            //        sb.Append(string.Format("Gate SendRemain:[{0}] ", GateInfo.nSendRemainCount));
            //        if (GateInfo.nSendMsgBytes < 1024)
            //        {
            //            sb.Append(string.Format("Gate SendBytes:[{0}] b ", GateInfo.nSendMsgBytes));
            //        }
            //        else
            //        {
            //            sb.Append(string.Format("Gate SendBytes:[{0}] kb ", GateInfo.nSendMsgBytes / 1024));
            //        }
            //        if (GateInfo.UserList != null)
            //        {
            //            sb.Append(string.Format("Gate Users:[{0}] / [{1}] ", GateInfo.nUserCount, GateInfo.UserList.Count));
            //        }
            //        else
            //        {
            //            sb.Append(string.Format("Gate Users:[{0}] ", GateInfo.nUserCount));
            //        }
            //        sb.AppendLine(Environment.NewLine);
            //    }
            //}
            //if (sb.Length > 0)
            //{
            //    LogService.Error(sb.ToString());
            //}
        }
    }
}
