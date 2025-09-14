﻿using OpenMir2;
using SystemModule;
using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    /// <summary>
    /// 设置怪物行动速度
    /// </summary>
    [Command("ChangeZenFastStep", "设置怪物行动速度", "速度", 10)]
    public class ChangeZenFastStepCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            string sFastStep = @params.Length > 0 ? @params[0] : "";
            int nFastStep = HUtil32.StrToInt(sFastStep, -1);
            if (string.IsNullOrEmpty(sFastStep) || nFastStep < 1 || !string.IsNullOrEmpty(sFastStep))
            {
                PlayerActor.SysMsg("设置怪物行动速度。", MsgColor.Red, MsgType.Hint);
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            SystemShare.Config.ZenFastStep = nFastStep;
            PlayerActor.SysMsg($"怪物行动速度: {nFastStep}", MsgColor.Green, MsgType.Hint);
        }
    }
}