﻿using OpenMir2;
using SystemModule;
using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    /// <summary>
    /// 设置怪物集中目标
    /// </summary>
    [Command("Mission", "设置怪物集中目标", " X Y", 10)]
    public class MissionCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            string sX = @params.Length > 0 ? @params[0] : "";
            string sY = @params.Length > 1 ? @params[1] : "";
            if (string.IsNullOrEmpty(sX) || string.IsNullOrEmpty(sY))
            {
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            short nX = HUtil32.StrToInt16(sX, 0);
            short nY = HUtil32.StrToInt16(sY, 0);
            SystemShare.BoMission = true;
            SystemShare.MissionMap = PlayerActor.MapName;
            SystemShare.MissionX = nX;
            SystemShare.MissionY = nY;
            PlayerActor.SysMsg("怪物集中目标已设定为: " + PlayerActor.MapName + '(' + SystemShare.MissionX + ':' + SystemShare.MissionY + ')', MsgColor.Green, MsgType.Hint);
        }
    }
}