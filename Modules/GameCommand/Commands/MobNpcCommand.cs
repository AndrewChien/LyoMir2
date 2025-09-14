﻿using OpenMir2;
using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    /// <summary>
    /// 在当前XY坐标创建NPC
    /// </summary>
    [Command("MobNpc", "在当前XY坐标创建NPC", CommandHelp.GameCommandMobNpcHelpMsg, 10)]
    public class MobNpcCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            string sParam1 = @params.Length > 0 ? @params[0] : "";
            string sParam2 = @params.Length > 1 ? @params[1] : "";
            string sParam3 = @params.Length > 2 ? @params[2] : "";
            string sParam4 = @params.Length > 3 ? @params[3] : "";
            if (string.IsNullOrEmpty(sParam1) || string.IsNullOrEmpty(sParam2) || (!string.IsNullOrEmpty(sParam1)) && sParam1[0] == '?')
            {
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            int nAppr = HUtil32.StrToInt(sParam3, 0);
            bool boIsCastle = HUtil32.StrToInt(sParam4, 0) == 1;
            if (string.IsNullOrEmpty(sParam1))
            {
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            //var merchant = new Merchant();
            //merchant.ChrName = sParam1;
            //merchant.MapName = PlayerActor.MapName;
            //merchant.Envir = PlayerActor.Envir;
            //merchant.Appr = (ushort)nAppr;
            //merchant.NpcFlag = 0;
            //merchant.CastleMerchant = boIsCastle;
            //merchant.ScriptName = sParam2;
            //PlayerActor.GetFrontPosition(ref nX, ref nY);
            //merchant.CurrX = nX;
            //merchant.CurrY = nY;
            //merchant.Initialize();
            //merchant.OnEnvirnomentChanged();
            //SystemShare.WorldEngine.AddMerchant(merchant);
        }
    }
}