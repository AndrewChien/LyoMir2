﻿using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    /// <summary>
    /// 重新加载物品数据库
    /// </summary>
    [Command("ReloadMagicDB", "重新加载技能数据库", 10)]

    public class ReloadMagicDbCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(IPlayerActor PlayerActor)
        {
            //ModuleShare.CommonDb.LoadMagicDB();
            PlayerActor.SysMsg("魔法数据库重新加载完成。", MsgColor.Green, MsgType.Hint);
        }
    }
}