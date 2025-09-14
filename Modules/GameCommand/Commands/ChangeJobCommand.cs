﻿using OpenMir2.Enums;
using SystemModule;
using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    /// <summary>
    /// 调整指定玩家职业
    /// </summary>
    [Command("ChangeJob", "调整指定玩家职业", CommandHelp.GameCommandChangeJobHelpMsg, 10)]
    public class ChangeJobCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            string sHumanName = @params.Length > 0 ? @params[0] : "";
            string sJobName = @params.Length > 1 ? @params[1] : "";
            if (string.IsNullOrEmpty(sHumanName) || string.IsNullOrEmpty(sJobName))
            {
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            IPlayerActor mIPlayerActor = SystemShare.WorldEngine.GetPlayObject(sHumanName);
            if (mIPlayerActor != null)
            {
                if (string.Compare(sJobName, "Warr", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    mIPlayerActor.Job = PlayerJob.Warrior;
                }
                if (string.Compare(sJobName, "Wizard", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    mIPlayerActor.Job = PlayerJob.Wizard;
                }
                if (string.Compare(sJobName, "Taos", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    mIPlayerActor.Job = PlayerJob.Taoist;
                }
                mIPlayerActor.HasLevelUp(1);
                mIPlayerActor.SysMsg(CommandHelp.GameCommandChangeJobHumanMsg, MsgColor.Green, MsgType.Hint);
                PlayerActor.SysMsg(string.Format(CommandHelp.GameCommandChangeJobMsg, sHumanName), MsgColor.Green, MsgType.Hint);
            }
            else
            {
                PlayerActor.SysMsg(string.Format(CommandHelp.NowNotOnLineOrOnOtherServer, sHumanName), MsgColor.Red, MsgType.Hint);
            }
        }
    }
}