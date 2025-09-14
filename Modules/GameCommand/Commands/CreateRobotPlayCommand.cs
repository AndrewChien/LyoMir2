﻿using OpenMir2;
using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    /// <summary>
    /// 增加AI玩家
    /// </summary>
    [Command("AddRobotPlay", "增加机器人玩家", "数量 地图 X Y")]
    public class CreateRobotPlayCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            if (@params[0] == "?")
            {
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            int userCount = HUtil32.StrToInt(@params[0], 1);
            //if (@params.Length >= 3)
            //{
            //    sMapName = string.IsNullOrEmpty(@params[1]) ? "" : @params[1];
            //    nX = HUtil32.StrToInt16(@params[2], 0);
            //    nY = HUtil32.StrToInt16(@params[3], 0);
            //    if (string.IsNullOrEmpty(sMapName) || (nX == 0 || nY == 0))
            //    {
            //        PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
            //        return;
            //    }
            //}
            //else
            //{
            //    sMapName = SystemShare.WorldEngine.GetHomeInfo(ref nX, ref nY);
            //}
            //for (var i = 0; i < userCount; i++)
            //{
            //    SystemShare.WorldEngine.AddRobotLogon(new RoBotLogon()
            //    {
            //        sChrName = RandomNumber.GetInstance().GenerateRandomNumber(RandomNumber.GetInstance().Random(4, 8)),
            //        sFilePath = M2Share.Config.EnvirDir,
            //        sConfigListFileName = M2Share.Config.sAIConfigListFileName,
            //        sHeroConfigListFileName = M2Share.Config.sHeroAIConfigListFileName,
            //        sMapName = sMapName,
            //        nX = nX,
            //        nY = nY
            //    });
            //}
            //PlayerActor.SysMsg($"已添加[{userCount}]个假人玩家,队列玩家:[{SystemShare.WorldEngine.RobotLogonQueue.Count}],当前共[{SystemShare.WorldEngine.RobotPlayerCount}]个假人玩家", MsgColor.Green, MsgType.Hint);

        }
    }
}