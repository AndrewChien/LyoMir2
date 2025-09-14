﻿using OpenMir2;
using SystemModule;
using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    /// <summary>
    /// 刷指定怪物
    /// </summary>
    [Command("Mob", "刷指定怪物", "怪物名称 数量 等级(0-7)", 10)]
    public class MobCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            short nX = 0;
            short nY = 0;
            string sMonName = @params.Length > 0 ? @params[0] : "";//名称
            int nCount = @params.Length > 1 ? HUtil32.StrToInt(@params[1], 0) : 1;//数量
            byte nLevel = @params.Length > 2 ? (byte)HUtil32.StrToInt(@params[2], 0) : (byte)0;//怪物等级
            if (string.IsNullOrEmpty(sMonName))
            {
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            if (nCount <= 0)
            {
                nCount = 1;
            }
            if (!(nLevel <= 10))
            {
                nLevel = 0;
            }
            nCount = (byte)HUtil32._MIN(64, nCount);
            PlayerActor.GetFrontPosition(ref nX, ref nY);//刷在当前X，Y坐标
            for (int i = 0; i < nCount; i++)
            {
                IMonsterActor monster = (IMonsterActor)SystemShare.WorldEngine.RegenMonsterByName(PlayerActor.Envir.MapName, nX, nY, sMonName);
                if (monster != null)
                {
                    // monster.SlaveMakeLevel = nLevel;
                    monster.SlaveExpLevel = nLevel;
                    monster.RecalcAbilitys();
                    monster.RefNameColor();
                }
                else
                {
                    PlayerActor.SysMsg(CommandHelp.GameCommandMobMsg, MsgColor.Red, MsgType.Hint);
                    break;
                }
            }
        }
    }
}