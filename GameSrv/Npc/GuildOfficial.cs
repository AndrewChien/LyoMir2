﻿using M2Server.Npc;
using OpenMir2.Enums;
using SystemModule.Actors;
using SystemModule.Castles;
using SystemModule.Const;
using SystemModule.Enums;

namespace GameSrv.Npc
{
    /// <summary>
    /// 行会NPC类
    /// 行会管理NPC 如：比奇国王
    /// </summary>
    public class GuildOfficial : NormNpc
    {
        public GuildOfficial() : base()
        {
            RaceImg = ActorRace.Merchant;
            Appr = 8;
        }

        protected override void GetVariableText(IPlayerActor PlayObject, string sVariable, ref string sMsg)
        {
            base.GetVariableText(PlayObject, sVariable, ref sMsg);
            if (sVariable == "$REQUESTCASTLELIST")
            {
                string sText = "";
                IList<string> List = new List<string>();
                SystemShare.CastleMgr.GetCastleNameList(List);
                for (int i = 0; i < List.Count; i++)
                {
                    sText = sText + Format("<{0}/@requestcastlewarnow{1}> {2}", List[i], i, sText);
                }
                sText = sText + "\\ \\";
                sMsg = ReplaceVariableText(sMsg, "<$REQUESTCASTLELIST>", sText);
            }
        }

        public override void Run()
        {
            if (M2Share.RandomNumber.Random(40) == 0)
            {
                TurnTo(M2Share.RandomNumber.RandomByte(8));
            }
            else
            {
                if (M2Share.RandomNumber.Random(30) == 0)
                {
                    SendRefMsg(Messages.RM_HIT, Dir, CurrX, CurrY, 0, "");
                }
            }
            base.Run();
        }

        public override void UserSelect(IPlayerActor PlayObject, string sData)
        {
            string sLabel = string.Empty;
            base.UserSelect(PlayObject, sData);
            try
            {
                if (!string.IsNullOrEmpty(sData) && sData.StartsWith("@"))
                {
                    string sMsg = HUtil32.GetValidStr3(sData, ref sLabel, '\r');
                    bool boCanJmp = PlayObject.LableIsCanJmp(sLabel);
                    M2Share.ScriptEngine.GotoLable(PlayObject, this.ActorId, sLabel, !boCanJmp);
                    if (!boCanJmp)
                    {
                        return;
                    }
                    if (string.Compare(sLabel, ScriptFlagConst.sBUILDGUILDNOW, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ReQuestBuildGuild(PlayObject, sMsg);
                    }
                    else if (string.Compare(sLabel, ScriptFlagConst.sSCL_GUILDWAR, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ReQuestGuildWar(PlayObject, sMsg);
                    }
                    else if (string.Compare(sLabel, ScriptFlagConst.sDONATE, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        DoNate(PlayObject);
                    }
                    else if (HUtil32.CompareLStr(sLabel, ScriptFlagConst.sREQUESTCASTLEWAR))
                    {
                        ReQuestCastleWar(PlayObject, sLabel[ScriptFlagConst.sREQUESTCASTLEWAR.Length..]);
                    }
                    else if (string.Compare(sLabel, ScriptFlagConst.sEXIT, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        PlayObject.SendMsg(this, Messages.RM_MERCHANTDLGCLOSE, 0, ActorId, 0, 0);
                    }
                    else if (string.Compare(sLabel, ScriptFlagConst.sBACK, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (string.IsNullOrEmpty(PlayObject.ScriptGoBackLable))
                        {
                            PlayObject.ScriptGoBackLable = ScriptFlagConst.sMAIN;
                        }
                        M2Share.ScriptEngine.GotoLable(PlayObject, this.ActorId, PlayObject.ScriptGoBackLable);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
            }
        }

        /// <summary>
        /// 请求建立行会
        /// </summary>
        /// <param name="PlayObject"></param>
        /// <param name="sGuildName"></param>
        /// <returns></returns>
        private int ReQuestBuildGuild(IPlayerActor PlayObject, string sGuildName)
        {
            int result = 0;
            sGuildName = sGuildName.Trim();
            UserItem UserItem = null;
            if (string.IsNullOrEmpty(sGuildName))
            {
                result = -4;
            }
            if (PlayObject.MyGuild == null)
            {
                if (PlayObject.Gold >= SystemShare.Config.BuildGuildPrice)
                {
                    UserItem = PlayObject.CheckItems(SystemShare.Config.WomaHorn);
                    if (UserItem == null)
                    {
                        result = -3;// '你没有准备好需要的全部物品。'
                    }
                }
                else
                {
                    result = -2;// '缺少创建费用。'
                }
            }
            else
            {
                result = -1;// '您已经加入其它行会。'
            }
            if (result == 0)
            {
                if (SystemShare.GuildMgr.AddGuild(sGuildName, PlayObject.ChrName))
                {
                    SystemShare.WorldEngine.SendServerGroupMsg(Messages.SS_205, M2Share.ServerIndex, sGuildName + '/' + PlayObject.ChrName);
                    PlayObject.SendDelItems(UserItem);
                    PlayObject.DelBagItem(UserItem.MakeIndex, SystemShare.Config.WomaHorn);
                    PlayObject.DecGold(SystemShare.Config.BuildGuildPrice);
                    PlayObject.GoldChanged();
                    PlayObject.MyGuild = SystemShare.GuildMgr.MemberOfGuild(PlayObject.ChrName);
                    if (PlayObject.MyGuild != null)
                    {
                        short rankNo = 0;
                        PlayObject.GuildRankName = PlayObject.MyGuild.GetRankName(PlayObject, ref rankNo);
                        PlayObject.GuildRankNo = rankNo;
                        RefShowName();
                    }
                }
                else
                {
                    result = -4;
                }
            }
            if (result >= 0)
            {
                PlayObject.SendMsg(this, Messages.RM_BUILDGUILD_OK, 0, 0, 0, 0);
            }
            else
            {
                PlayObject.SendMsg(this, Messages.RM_BUILDGUILD_FAIL, 0, result, 0, 0);
            }
            return result;
        }

        /// <summary>
        /// 请求行会战争
        /// </summary>
        /// <param name="PlayObject"></param>
        /// <param name="sGuildName"></param>
        /// <returns></returns>
        private static void ReQuestGuildWar(IPlayerActor PlayObject, string sGuildName)
        {
            if (SystemShare.GuildMgr.FindGuild(sGuildName) != null)
            {
                if (PlayObject.Gold >= SystemShare.Config.GuildWarPrice)
                {
                    PlayObject.DecGold(SystemShare.Config.GuildWarPrice);
                    PlayObject.GoldChanged();
                    PlayObject.ReQuestGuildWar(sGuildName);
                }
                else
                {
                    PlayObject.SysMsg("你没有足够的金币!!!", MsgColor.Red, MsgType.Hint);
                }
            }
            else
            {
                PlayObject.SysMsg("行会 " + sGuildName + " 不存在!!!", MsgColor.Red, MsgType.Hint);
            }
        }

        private void DoNate(IPlayerActor PlayObject)
        {
            PlayObject.SendMsg(this, Messages.RM_DONATE_OK, 0, 0, 0, 0);
        }

        private void ReQuestCastleWar(IPlayerActor PlayObject, string sIndex)
        {
            int nIndex = HUtil32.StrToInt(sIndex, -1);
            if (nIndex < 0)
            {
                nIndex = 0;
            }
            IUserCastle Castle = SystemShare.CastleMgr.GetCastle(nIndex);
            if (PlayObject.IsGuildMaster() && !Castle.IsMember(PlayObject))
            {
                UserItem UserItem = PlayObject.CheckItems(SystemShare.Config.ZumaPiece);
                if (UserItem != null)
                {
                    if (Castle.AddAttackerInfo(PlayObject.MyGuild))
                    {
                        PlayObject.SendDelItems(UserItem);
                        PlayObject.DelBagItem(UserItem.MakeIndex, SystemShare.Config.ZumaPiece);
                        M2Share.ScriptEngine.GotoLable(PlayObject, this.ActorId, "~@request_ok");
                    }
                    else
                    {
                        PlayObject.SysMsg("你现在无法请求攻城!!!", MsgColor.Red, MsgType.Hint);
                    }
                }
                else
                {
                    PlayObject.SysMsg("你没有" + SystemShare.Config.ZumaPiece + "!!!", MsgColor.Red, MsgType.Hint);
                }
            }
            else
            {
                PlayObject.SysMsg("你的请求被取消!!!", MsgColor.Red, MsgType.Hint);
            }
        }
    }
}