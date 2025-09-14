﻿using SystemModule.Actors;

namespace CommandModule.Commands
{
    /// <summary>
    /// 设置地图模式
    /// </summary>
    [Command("SetMapMode", "设置地图模式", 10)]
    public class SetMapModeCommamd : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            string sMapName = @params.Length > 0 ? @params[0] : "";
            string sMapMode = @params.Length > 1 ? @params[1] : "";
            string sParam1 = @params.Length > 2 ? @params[2] : "";
            string sParam2 = @params.Length > 3 ? @params[3] : "";
            if (PlayerActor.Permission < 6)
            {
                return;
            }

            //if ((string.IsNullOrEmpty(sMapName)) || (string.IsNullOrEmpty(sMapMode)))
            //{
            //    if (Settings.Config.boGMShowFailMsg)
            //    {
            //        PlayerActor.SysMsg("命令格式: @" + this.Attributes.Name + " 地图号 模式", MsgColor.c_Red, MsgType.t_Hint);
            //    }
            //    return;
            //}
            //Envir = Settings.g_MapMgr.FindMap(sMapName);
            //if ((Envir == null))
            //{
            //    PlayerActor.SysMsg   SysMsg(sMapName + " 不存在!!!", MsgColor.c_Red, MsgType.t_Hint);
            //    return;
            //}
            //if ((sMapMode).CompareTo(("SAFE")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boSAFE = true;
            //    }
            //    else
            //    {
            //        Envir.m_boSAFE = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("DARK")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boDARK = true;
            //    }
            //    else
            //    {
            //        Envir.m_boDARK = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("DARK")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boDARK = true;
            //    }
            //    else
            //    {
            //        Envir.m_boDARK = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("FIGHT")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boFightZone = true;
            //    }
            //    else
            //    {
            //        Envir.m_boFightZone = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("FIGHT2")) == 0) // PK掉装备地图
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boFight2Zone = true;
            //    }
            //    else
            //    {
            //        Envir.m_boFight2Zone = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("FIGHT3")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boFight3Zone = true;
            //    }
            //    else
            //    {
            //        Envir.m_boFight3Zone = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("FIGHT4")) == 0) // 挑战地图
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boFight4Zone = true;
            //    }
            //    else
            //    {
            //        Envir.m_boFight4Zone = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("DAY")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boDAY = true;
            //    }
            //    else
            //    {
            //        Envir.m_boDAY = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("QUIZ")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boQUIZ = true;
            //    }
            //    else
            //    {
            //        Envir.m_boQUIZ = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("NORECONNECT")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boNORECONNECT = true;
            //        Envir.sNoReconnectMap = sParam1;
            //    }
            //    else
            //    {
            //        Envir.m_boNORECONNECT = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("MUSIC")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boMUSIC = true;
            //        Envir.m_nMUSICID = HUtil32.StrToInt(sParam1, -1);
            //    }
            //    else
            //    {
            //        Envir.m_boMUSIC = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("EXPRATE")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boEXPRATE = true;
            //        Envir.m_nEXPRATE = HUtil32.StrToInt(sParam1, -1);
            //    }
            //    else
            //    {
            //        Envir.m_boEXPRATE = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("PKWINLEVEL")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boPKWINLEVEL = true;
            //        Envir.m_nPKWINLEVEL = HUtil32.StrToInt(sParam1, -1);
            //    }
            //    else
            //    {
            //        Envir.m_boPKWINLEVEL = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("PKWINEXP")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boPKWINEXP = true;
            //        Envir.m_nPKWINEXP = HUtil32.StrToInt(sParam1, -1);
            //    }
            //    else
            //    {
            //        Envir.m_boPKWINEXP = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("PKLOSTLEVEL")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boPKLOSTLEVEL = true;
            //        Envir.m_nPKLOSTLEVEL = HUtil32.StrToInt(sParam1, -1);
            //    }
            //    else
            //    {
            //        Envir.m_boPKLOSTLEVEL = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("PKLOSTEXP")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boPKLOSTEXP = true;
            //        Envir.m_nPKLOSTEXP = HUtil32.StrToInt(sParam1, -1);
            //    }
            //    else
            //    {
            //        Envir.m_boPKLOSTEXP = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("DECHP")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))) && (if(!string.IsNullOrEmpty(sParam2))))
            //    {
            //        Envir.m_boDECHP = true;
            //        Envir.m_nDECHPTIME = HUtil32.StrToInt(sParam1, -1);
            //        Envir.m_nDECHPPOINT = HUtil32.StrToInt(sParam2, -1);
            //    }
            //    else
            //    {
            //        Envir.m_boDECHP = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("DECGAMEGOLD")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))) && (if(!string.IsNullOrEmpty(sParam2))))
            //    {
            //        Envir.m_boDecGameGold = true;
            //        Envir.m_nDECGAMEGOLDTIME = HUtil32.StrToInt(sParam1, -1);
            //        Envir.m_nDecGameGold = HUtil32.StrToInt(sParam2, -1);
            //    }
            //    else
            //    {
            //        Envir.m_boDecGameGold = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("KILLFUNC")) == 0)
            //{
            //    // 20080415 地图杀人触发
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boKILLFUNC = true;
            //        Envir.m_nKILLFUNC = HUtil32.StrToInt(sParam1, -1);
            //    }
            //    else
            //    {
            //        Envir.m_boKILLFUNC = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("INCGAMEGOLD")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))) && (if(!string.IsNullOrEmpty(sParam2))))
            //    {
            //        Envir.m_boIncGameGold = true;
            //        Envir.m_nINCGAMEGOLDTIME = HUtil32.StrToInt(sParam1, -1);
            //        Envir.m_nIncGameGold = HUtil32.StrToInt(sParam2, -1);
            //    }
            //    else
            //    {
            //        Envir.m_boIncGameGold = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("INCGAMEPOINT")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))) && (if(!string.IsNullOrEmpty(sParam2))))
            //    {
            //        Envir.m_boINCGAMEPOINT = true;
            //        Envir.m_nINCGAMEPOINTTIME = HUtil32.StrToInt(sParam1, -1);
            //        Envir.m_nINCGAMEPOINT = HUtil32.StrToInt(sParam2, -1);
            //    }
            //    else
            //    {
            //        Envir.m_boINCGAMEPOINT = false;
            //    }
            //}
            //// ------------------------------------------------------------------------------
            //else if ((sMapMode).CompareTo(("NEEDLEVELTIME")) == 0)
            //{
            //    // 雪域地图传送,判断等级,地图时间 20081228
            //    if ((if(!string.IsNullOrEmpty(sParam1))) && (if(!string.IsNullOrEmpty(sParam2))))
            //    {
            //        Envir.m_boNEEDLEVELTIME = true;
            //        Envir.m_nNEEDLEVELPOINT = HUtil32.StrToInt(sParam1, 0);
            //        // 进地图最低等级
            //    }
            //    else
            //    {
            //        Envir.m_boNEEDLEVELTIME = false;
            //    }
            //}
            //// 20080124 禁止召唤英雄
            //else if ((sMapMode).CompareTo(("NOCALLHERO")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boNOCALLHERO = true;
            //    }
            //    else
            //    {
            //        Envir.m_boNOCALLHERO = false;
            //    }
            //}
            //// 禁止英雄守护 20080629
            //else if ((sMapMode).CompareTo(("NOHEROPROTECT")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boNOHEROPROTECT = true;
            //    }
            //    else
            //    {
            //        Envir.m_boNOHEROPROTECT = false;
            //    }
            //}
            //// 20080503 禁止死亡掉物品
            //else if ((sMapMode).CompareTo(("NODROPITEM")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boNODROPITEM = true;
            //    }
            //    else
            //    {
            //        Envir.m_boNODROPITEM = false;
            //    }
            //}
            //// 20080124 不允许使用任何物品和技能
            //else if ((sMapMode).CompareTo(("MISSION")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boMISSION = true;
            //    }
            //    else
            //    {
            //        Envir.m_boMISSION = false;
            //    }

            //    // ------------------------------------------------------------------------------
            //}
            //else if ((sMapMode).CompareTo(("RUNHUMAN")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boRUNHUMAN = true;
            //    }
            //    else
            //    {
            //        Envir.m_boRUNHUMAN = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("RUNMON")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boRUNMON = true;
            //    }
            //    else
            //    {
            //        Envir.m_boRUNMON = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("NEEDHOLE")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boNEEDHOLE = true;
            //    }
            //    else
            //    {
            //        Envir.m_boNEEDHOLE = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("NORECALL")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boNORECALL = true;
            //    }
            //    else
            //    {
            //        Envir.m_boNORECALL = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("NOGUILDRECALL")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boNOGUILDRECALL = true;
            //    }
            //    else
            //    {
            //        Envir.m_boNOGUILDRECALL = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("NODEARRECALL")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boNODEARRECALL = true;
            //    }
            //    else
            //    {
            //        Envir.m_boNODEARRECALL = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("NOMASTERRECALL")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boNOMASTERRECALL = true;
            //    }
            //    else
            //    {
            //        Envir.m_boNOMASTERRECALL = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("NORANDOMMOVE")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boNORANDOMMOVE = true;
            //    }
            //    else
            //    {
            //        Envir.m_boNORANDOMMOVE = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("NODRUG")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boNODRUG = true;
            //    }
            //    else
            //    {
            //        Envir.m_boNODRUG = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("MINE")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boMINE = true;
            //    }
            //    else
            //    {
            //        Envir.m_boMINE = false;
            //    }
            //}
            //else if ((sMapMode).CompareTo(("NOPOSITIONMOVE")) == 0)
            //{
            //    if ((if(!string.IsNullOrEmpty(sParam1))))
            //    {
            //        Envir.m_boNOPOSITIONMOVE = true;
            //    }
            //    else
            //    {
            //        Envir.m_boNOPOSITIONMOVE = false;
            //    }
            //}
            //sMsg = "地图模式: " + Envir.GetEnvirInfo();
            //PlayerActor.SysMsg(sMsg, MsgColor.c_Blue, MsgType.t_Hint);
        }
    }
}