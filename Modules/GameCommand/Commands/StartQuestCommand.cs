﻿using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    [Command("StartQuest", "", "问答名称", 10)]
    public class StartQuestCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            string sQuestName = @params.Length > 0 ? @params[0] : "";
            if (string.IsNullOrEmpty(sQuestName))
            {
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            //SystemShare.WorldEngine.SendQuestMsg(sQuestName);
        }
    }
}