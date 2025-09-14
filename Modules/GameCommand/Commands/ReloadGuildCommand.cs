﻿using SystemModule;
using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    /// <summary>
    /// 重新读取行会
    /// </summary>
    [Command("ReloadGuild", "重新读取指定行会", 10)]
    public class ReloadGuildCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            string sParam1 = string.Empty;
            if (@params.Length > 0)
            {
                sParam1 = @params.Length > 0 ? @params[0] : "";
                if (string.IsNullOrEmpty(sParam1))
                {
                    PlayerActor.SysMsg(string.Format(CommandHelp.GameCommandParamUnKnow, this.Command.Name, CommandHelp.GameCommandReloadGuildHelpMsg), MsgColor.Red, MsgType.Hint);
                    return;
                }
            }
            if (SystemShare.ServerIndex != 0)
            {
                PlayerActor.SysMsg(CommandHelp.GameCommandReloadGuildOnMasterserver, MsgColor.Red, MsgType.Hint);
                return;
            }
            SystemModule.Castles.IGuild guild = SystemShare.GuildMgr.FindGuild(sParam1);
            if (guild == null)
            {
                PlayerActor.SysMsg(string.Format(CommandHelp.GameCommandReloadGuildNotFoundGuildMsg, sParam1), MsgColor.Red, MsgType.Hint);
                return;
            }
            guild.LoadGuild();
            PlayerActor.SysMsg(string.Format(CommandHelp.GameCommandReloadGuildSuccessMsg, sParam1), MsgColor.Red, MsgType.Hint);
            // WorldEngine.SendServerGroupMsg(SS_207, nServerIndex, sParam1);
        }
    }
}