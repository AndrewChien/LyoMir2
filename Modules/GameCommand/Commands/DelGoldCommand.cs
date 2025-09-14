﻿using OpenMir2;
using SystemModule;
using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    /// <summary>
    /// 调整指定玩家游戏币
    /// </summary>
    [Command("DelGold", "调整指定玩家游戏币", help: "人物名称 数量", 10)]
    public class DelGoldCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            string sHumName = @params.Length > 0 ? @params[0] : "";
            int nCount = @params.Length > 1 ? HUtil32.StrToInt(@params[1], 0) : 0;
            if (string.IsNullOrEmpty(sHumName) || nCount <= 0)
            {
                return;
            }
            IPlayerActor mIPlayerActor = SystemShare.WorldEngine.GetPlayObject(sHumName);
            if (string.IsNullOrEmpty(sHumName) || nCount <= 0)
            {
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            mIPlayerActor = SystemShare.WorldEngine.GetPlayObject(sHumName);
            if (mIPlayerActor != null)
            {
                if (mIPlayerActor.Gold > nCount)
                {
                    mIPlayerActor.Gold -= nCount;
                }
                else
                {
                    nCount = mIPlayerActor.Gold;
                    mIPlayerActor.Gold = 0;
                }
                mIPlayerActor.GoldChanged();
                PlayerActor.SysMsg(sHumName + "的金币已减少" + nCount + ".", MsgColor.Green, MsgType.Hint);
                if (SystemShare.GameLogGold)
                {
                    //SystemShare.Mediator.Publish(new GameMessageEvent() { EventType = 13, Event = "123" });
                    // M2Share.EventSource.AddEventLog(13, PlayerActor.MapName + "\09" + PlayerActor.CurrX + "\09" + PlayerActor.CurrY + "\09"
                    //                                     + PlayerActor.ChrName + "\09" + Grobal2.StringGoldName + "\09" + nCount + "\09" + "1" + "\09" + sHumName);
                }
            }
            else
            {
                int nServerIndex = 0;
                if (SystemShare.WorldEngine.FindOtherServerUser(sHumName, ref nServerIndex))
                {
                    PlayerActor.SysMsg(sHumName + "现在" + nServerIndex + "号服务器上", MsgColor.Green, MsgType.Hint);
                }
                else
                {
                    //M2Share.FrontEngine.AddChangeGoldList(PlayerActor.ChrName, sHumName, -nCount);
                    PlayerActor.SysMsg(sHumName + "现在不在线，等其上线时金币将自动减少", MsgColor.Green, MsgType.Hint);
                }
            }
        }
    }
}