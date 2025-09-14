﻿using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    [Command("Letguild", "加入公会", "")]
    public class LetGuildCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(IPlayerActor PlayerActor)
        {
            PlayerActor.AllowGuild = !PlayerActor.AllowGuild;
            if (PlayerActor.AllowGuild)
            {
                PlayerActor.SysMsg(CommandHelp.EnableJoinGuild, MsgColor.Green, MsgType.Hint);
            }
            else
            {
                PlayerActor.SysMsg(CommandHelp.DisableJoinGuild, MsgColor.Green, MsgType.Hint);
            }
            return;
        }
    }
}