﻿using SystemModule;
using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    /// <summary>
    /// 将指定地图所有玩家随机移动
    /// </summary>
    [Command("MapMoveHuman", "将指定地图所有玩家随机移动", CommandHelp.GameCommandMapMoveHelpMsg, 10)]
    public class MapMoveHumanCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            string sSrcMap = @params.Length > 0 ? @params[0] : "";
            string sDenMap = @params.Length > 1 ? @params[1] : "";
            if (string.IsNullOrEmpty(sDenMap) || string.IsNullOrEmpty(sSrcMap) ||
                !string.IsNullOrEmpty(sSrcMap) && sSrcMap[0] == '?')
            {
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            SystemModule.Maps.IEnvirnoment srcEnvir = SystemShare.MapMgr.FindMap(sSrcMap);
            SystemModule.Maps.IEnvirnoment denEnvir = SystemShare.MapMgr.FindMap(sDenMap);
            if (srcEnvir == null)
            {
                PlayerActor.SysMsg(string.Format(CommandHelp.GameCommandMapMoveMapNotFound, sSrcMap), MsgColor.Red,
                    MsgType.Hint);
                return;
            }
            if (denEnvir == null)
            {
                PlayerActor.SysMsg(string.Format(CommandHelp.GameCommandMapMoveMapNotFound, sDenMap), MsgColor.Red,
                    MsgType.Hint);
                return;
            }
            IList<IPlayerActor> humanList = new List<IPlayerActor>();
            SystemShare.WorldEngine.GetMapRageHuman(srcEnvir, srcEnvir.Width / 2, srcEnvir.Height / 2, 1000, ref humanList, true);
            for (int i = 0; i < humanList.Count; i++)
            {
                IPlayerActor moveHuman = humanList[i];
                if (moveHuman != PlayerActor)
                {
                    moveHuman.MapRandomMove(sDenMap, 0);
                }
            }
        }
    }
}