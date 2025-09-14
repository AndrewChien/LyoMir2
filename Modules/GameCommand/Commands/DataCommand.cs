﻿using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    /// <summary>
    /// 查看时间日期
    /// </summary>
    [Command("Date", "查看时间日期", "")]
    public class DataCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(IPlayerActor PlayerActor)
        {
            PlayerActor.SysMsg(CommandHelp.NowCurrDateTime + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), MsgColor.Blue, MsgType.Hint);
        }
    }
}