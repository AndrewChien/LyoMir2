﻿using OpenMir2;
using SystemModule;
using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    /// <summary>
    /// 调整指定玩家能量值
    /// </summary>
    [Command("Hunger", "调整指定玩家能量值", "人物名称 能量值", 10)]
    public class HungerCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            string sHumanName = @params.Length > 0 ? @params[0] : "";
            int nHungerPoint = @params.Length > 1 ? HUtil32.StrToInt(@params[1], 0) : -1;
            if (PlayerActor.Permission < 6)
            {
                return;
            }
            if (string.IsNullOrEmpty(sHumanName) || nHungerPoint < 0)
            {
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            IPlayerActor mIPlayerActor = SystemShare.WorldEngine.GetPlayObject(sHumanName);
            if (mIPlayerActor != null)
            {
                mIPlayerActor.HungerStatus = nHungerPoint;
                mIPlayerActor.SendMsg(PlayerActor, Messages.RM_MYSTATUS, 0, 0, 0, 0);
                mIPlayerActor.RefMyStatus();
                PlayerActor.SysMsg(sHumanName + " 的能量值已改变。", MsgColor.Green, MsgType.Hint);
            }
            else
            {
                PlayerActor.SysMsg(sHumanName + "没有在线!!!", MsgColor.Red, MsgType.Hint);
            }
        }
    }
}