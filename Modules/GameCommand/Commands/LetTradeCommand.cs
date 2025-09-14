﻿using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    [Command("Lettrade", "", "")]
    public class LetTradeCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(IPlayerActor PlayerActor)
        {
            PlayerActor.AllowDeal = !PlayerActor.AllowDeal;
            if (PlayerActor.AllowDeal)
            {
                PlayerActor.SysMsg(CommandHelp.EnableDealMsg, MsgColor.Green, MsgType.Hint);
            }
            else
            {
                PlayerActor.SysMsg(CommandHelp.DisableDealMsg, MsgColor.Green, MsgType.Hint);
            }
        }
    }
}