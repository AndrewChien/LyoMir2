﻿using SystemModule.Actors;

namespace CommandModule.Commands
{
    [Command("Authally", "", "")]
    internal class AuthallyCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(IPlayerActor PlayerActor)
        {
            if (PlayerActor.IsGuildMaster())
            {
                //PlayerActor.MyGuild.EnableAuthAlly = !PlayerActor.MyGuild.EnableAuthAlly;
                //if (PlayerActor.MyGuild.EnableAuthAlly)
                //{
                //    PlayerActor.SysMsg(CommandHelp.EnableAuthAllyGuild, MsgColor.Green, MsgType.Hint);
                //}
                //else
                //{
                //    PlayerActor.SysMsg(CommandHelp.DisableAuthAllyGuild, MsgColor.Green, MsgType.Hint);
                //}
            }
            return;
        }
    }
}