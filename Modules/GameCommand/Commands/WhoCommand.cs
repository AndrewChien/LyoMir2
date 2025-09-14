﻿using SystemModule.Actors;

namespace CommandModule.Commands
{
    /// <summary>
    /// 当前服务器在线人数
    /// </summary>
    [Command("Who", "查看在线人数", "统计服务器在线人数", 10)]
    public class WhoCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(IPlayerActor PlayerActor)
        {
            //PlayerActor.HearMsg($"当前服务器在线人数: {ModuleShare.WorldEngine.PlayObjectCount}({offlineCount}/{(ModuleShare.WorldEngine.PlayObjectCount - offlineCount)})");
        }
    }
}