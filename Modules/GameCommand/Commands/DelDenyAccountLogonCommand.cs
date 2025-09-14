﻿using SystemModule;
using SystemModule.Actors;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    [Command("DelDenyAccountLogon", "", "登录帐号", 10)]
    public class DelDenyAccountLogonCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null)
            {
                return;
            }
            string sAccount = @params.Length > 0 ? @params[0] : "";
            string sFixDeny = @params.Length > 1 ? @params[1] : "";
            if (string.IsNullOrEmpty(sAccount))
            {
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            bool boDelete = false;
            for (int i = 0; i < SystemShare.DenyAccountList.Count; i++)
            {
                //if ((sAccount).CompareTo((M2Share.g_DenyAccountList[i])) == 0)
                //{
                //    //if (((int)M2Share.g_DenyAccountList[i]) != 0)
                //    //{
                //    //    M2Share.SaveDenyAccountList();
                //    //}
                //    M2Share.g_DenyAccountList.RemoveAt(i);
                //    PlayerActor.SysMsg(sAccount + "已从禁止登录帐号列表中删除。", MsgColor.c_Green, MsgType.t_Hint);
                //    boDelete = true;
                //    break;
                //}
            }
            if (!boDelete)
            {
                PlayerActor.SysMsg(sAccount + "没有被禁止登录。", MsgColor.Green, MsgType.Hint);
            }
        }
    }
}