﻿namespace ScriptSystem.Consts
{
    /// <summary>
    /// 执行指令编码定义
    /// </summary>
    public enum ExecutionCode : short
    {
        [ScriptDefName("Set")]
        Set = 1,
        [ScriptDefName("Take")]
        Take,
        [ScriptDefName("GIVE")]
        Give,
        [ScriptDefName("TAKEW")]
        Takew,
        [ScriptDefName("CLOSE")]
        Close,
        [ScriptDefName("RESET")]
        ReSet,
        [ScriptDefName("SETOPEN")]
        SetOpen,
        [ScriptDefName("SETUNIT")]
        SetUnit,
        [ScriptDefName("RESETUNIT")]
        ResetUnit,
        [ScriptDefName("BREAK")]
        Break,
        [ScriptDefName("TIMERECALL")]
        TimereCall,
        [ScriptDefName("PARAM1")]
        Param1,
        [ScriptDefName("PARAM2")]
        Param2,
        [ScriptDefName("PARAM3")]
        Param3,
        [ScriptDefName("PARAM4")]
        Param4,
        [ScriptDefName("EXEACTION")]
        Exeaction,
        [ScriptDefName("MAPMOVE")]
        MapMove,
        [ScriptDefName("MAP")]
        Map,
        [ScriptDefName("TAKECHECKITEM")]
        TakecheckItem,
        [ScriptDefName("MONGEN")]
        MonGen,
        [ScriptDefName("MONGENP")]
        MonGenp,
        [ScriptDefName("MONCLEAR")]
        MonClear,
        [ScriptDefName("MOV")]
        Mov,
        [ScriptDefName("INC")]
        Inc,
        [ScriptDefName("DEC")]
        Dec,
        [ScriptDefName("SUM")]
        Sum,
        [ScriptDefName("DIV")]
        Div,
        [ScriptDefName("MUL")]
        Mul,
        [ScriptDefName("PERCENT")]
        Percent,
        [ScriptDefName("BREAKTIMERECALL")]
        BreakTimereCall,
        [ScriptDefName("SENDMSG")]
        SendMsg,
        [ScriptDefName("CHANGEMODE")]
        ChangeMode,
        [ScriptDefName("PKPOINT")]
        PkPoint,
        [ScriptDefName("CHANGEXP")]
        ChangExp,
        [ScriptDefName("RECALLMOB")]
        ReCallMob,
        [ScriptDefName("MOVR")]
        Movr,
        [ScriptDefName("EXCHANGEMAP")]
        ExchangeMap,
        [ScriptDefName("RECALLMAP")]
        ReCallMap,
        [ScriptDefName("ADDBATCH")]
        AddBatch,
        [ScriptDefName("BATCHDELAY")]
        BatchDelay,
        [ScriptDefName("BATCHMOVE")]
        BatchMove,
        [ScriptDefName("PLAYDICE")]
        PlayDice,
        [ScriptDefName("PASTEMAP")]
        PasteMap,
        [ScriptDefName("LOADGEN")]
        LoadGen,
        [ScriptDefName("ADDNAMELIST")]
        AddNameList,
        [ScriptDefName("DELNAMELIST")]
        DelNameList,
        [ScriptDefName("ADDGUILDLIST")]
        AddGuildList,
        [ScriptDefName("DELGUILDLIST")]
        DelGuildList,
        [ScriptDefName("ADDACCOUNTLIST")]
        AddAccountList,
        [ScriptDefName("DELACCOUNTLIST")]
        DelAccountList,
        [ScriptDefName("ADDIPLIST")]
        AddIpList,
        [ScriptDefName("DELIPLIST")]
        DelIpList,
        [ScriptDefName("GOQUEST")]
        GoQuest,
        [ScriptDefName("ENDQUEST")]
        EndQuest,
        [ScriptDefName("GOTO")]
        Goto,
        [ScriptDefName("HAIRCOLOR")]
        HairColor,
        [ScriptDefName("WEARCOLOR")]
        WearColor,
        [ScriptDefName("HAIRSTYLE")]
        Hairstyle,
        [ScriptDefName("MONRECALL")]
        MonreCall,
        [ScriptDefName("HORSECALL")]
        HorseCall,
        [ScriptDefName("HAIRRNDCOL")]
        Hairrndcol,
        [ScriptDefName("RANDSETDAILYQUEST")]
        Randsetdailyquest,
        [ScriptDefName("REFINEWEAPON")]
        RefineWeapon,
        [ScriptDefName("RECALLGROUPMEMBERS")]
        ReCallgroupMembers,
        [ScriptDefName("MAPTING")]
        Mapting,
        [ScriptDefName("WRITEWEAPONNAME")]
        WriteWeaponName,
        [ScriptDefName("ENABLECMD")]
        EnableCmd,
        [ScriptDefName("LINEMSG")]
        LineMsg,
        [ScriptDefName("EVENTMSG")]
        EventMsg,
        [ScriptDefName("SOUNDMSG")]
        SoundMsg,
        [ScriptDefName("SETMISSION")]
        SetMission,
        [ScriptDefName("CLEARMISSION")]
        ClearMission,
        [ScriptDefName("MONPWR")]
        Monpwr,
        [ScriptDefName("ENTER_OK")]
        EnterOk,
        [ScriptDefName("ENTER_FAIL")]
        EnterFail,
        [ScriptDefName("MONADDITEM")]
        MonAddItem,
        [ScriptDefName("CHANGEWEATHER")]
        Changeweather,
        [ScriptDefName("CHANGEWEAPONATOM")]
        ChangeWeaponatom,
        [ScriptDefName("GETREPAIRCOST")]
        GetrepairCost,
        [ScriptDefName("KILLHORSE")]
        KillHorse,
        [ScriptDefName("REPAIRITEM")]
        RepairItem,
        [ScriptDefName("USEREMERGENCYCLOSE")]
        UseremerGencyClose,
        [ScriptDefName("BUILDGUILD")]
        BuildGuild,
        [ScriptDefName("GUILDWAR")]
        Guildwar,
        [ScriptDefName("CHANGEUSERNAME")]
        ChangeUserName,
        [ScriptDefName("CHANGEMONLEVEL")]
        ChangemonLevel,
        [ScriptDefName("DROPITEMMAP")]
        DropItemMap,
        [ScriptDefName("CLEARITEMMAP")]
        ClearItemMap,
        [ScriptDefName("PROPOSECASTLEWAR")]
        Proposecastlewar,
        [ScriptDefName("FINISHCASTLEWAR")]
        Finishcastlewar,
        [ScriptDefName("MOVENPC")]
        MoveNpc,
        [ScriptDefName("SPEAK")]
        Speak,
        [ScriptDefName("SENDCMD")]
        SendCmd,
        [ScriptDefName("INCFAME")]
        IncFame,
        [ScriptDefName("DECFAME")]
        DecFame,
        [ScriptDefName("CAPTURECASTLEFLAG")]
        CapturecastleFlag,
        [ScriptDefName("MAKESHOOTER")]
        MakeShooter,
        [ScriptDefName("KILLSHOOTER")]
        KillShooter,
        [ScriptDefName("LEAVESHOOTER")]
        LeaveShooter,
        [ScriptDefName("CHANGEMAPATTR")]
        ChangeMapAttr,
        [ScriptDefName("RESETMAPATTR")]
        ResetMapAttr,
        [ScriptDefName("MAKECASTLEDOOR")]
        MakeCastleDoor,
        [ScriptDefName("REPAIRCASTLEDOOR")]
        RepairCastleDoor,
        [ScriptDefName("CHARGESHOOTER")]
        ChargeShooter,
        [ScriptDefName("SETAREAATTR")]
        SetareaAttr,
        [ScriptDefName("TESTFLAG")]
        TestFlag,
        [ScriptDefName("APPLYFLAG")]
        ApplyFlag,
        [ScriptDefName("PASTEFLAG")]
        PasteFlag,
        [ScriptDefName("GETBACKCASTLEGOLD")]
        GetBackCastleGold,
        [ScriptDefName("GETBACKUPGITEM")]
        GetBackUpgItem,
        [ScriptDefName("TINGWAR")]
        TingWar,
        [ScriptDefName("SAVEPASSWD")]
        SavePasswd,
        [ScriptDefName("CREATENPC")]
        CreateNpc,
        [ScriptDefName("TAKEBONUS")]
        TakeBonus,
        [ScriptDefName("SYSMSG")]
        SysMsg,
        [ScriptDefName("LOADVALUE")]
        LoadValue,
        [ScriptDefName("SAVEVALUE")]
        SaveValue,
        [ScriptDefName("SAVELOG")]
        SaveLog,
        [ScriptDefName("GETMARRIED")]
        GetMarried,
        [ScriptDefName("DIVORCE")]
        Divorce,
        [ScriptDefName("CAPTURESAYING")]
        CaptureSaying,
        [ScriptDefName("CANCELMARRIAGERING")]
        CancelMarriagering,
        [ScriptDefName("OPENUSERMARKET")]
        OpenUserMarket,
        [ScriptDefName("SETTYPEUSERMARKET")]
        SettypeUserMarket,
        [ScriptDefName("CHECKSOLDITEMSUSERMARKET")]
        CheckSoldItemsUserMarket,
        [ScriptDefName("SETGMEMAP")]
        SetGmeMap,
        [ScriptDefName("SETGMEPOINT")]
        SetGmePoint,
        [ScriptDefName("SETGMETIME")]
        SetGmetTime,
        [ScriptDefName("STARTNEWGME")]
        Startnewgme,
        [ScriptDefName("MOVETOGMEMAP")]
        MoveToGmeMap,
        [ScriptDefName("FINISHGME")]
        FinishgMe,
        [ScriptDefName("CONTINUEGME")]
        Continuegme,
        [ScriptDefName("SETGMEPLAYTIME")]
        SetgmeplayTime,
        [ScriptDefName("SETGMEPAUSETIME")]
        SetgmepauseTime,
        [ScriptDefName("SETGMELIMITUSER")]
        SetgmeLimitUser,
        [ScriptDefName("SETEVENTMAP")]
        SeteventMap,
        [ScriptDefName("RESETEVENTMAP")]
        ReseteventMap,
        [ScriptDefName("TESTREFINEPOINTS")]
        TestrefinePoints,
        [ScriptDefName("RESETREFINEWEAPON")]
        ResetrefineWeapon,
        [ScriptDefName("TESTREFINEACCESSORIES")]
        Testrefineaccessories,
        [ScriptDefName("REFINEACCESSORIES")]
        RefineAccessories,
        [ScriptDefName("APPLYMONMISSION")]
        ApplyMonMission,
        [ScriptDefName("MAPMOVER")]
        MapMover,
        [ScriptDefName("ADDSTR")]
        AddStr,
        [ScriptDefName("SETEVENTDAMAGE")]
        SeteventDamage,
        [ScriptDefName("FORMATSTR")]
        FormatStr,
        [ScriptDefName("CLEARPATH")]
        ClearPath,
        [ScriptDefName("ADDPATH")]
        AddPath,
        [ScriptDefName("APPLYPATH")]
        ApplyPath,
        [ScriptDefName("MAPSPELL")]
        MapSpell,
        [ScriptDefName("GIVEEXP")]
        GiveExp,
        [ScriptDefName("GROUPMOVE")]
        GroupMove,
        [ScriptDefName("GIVEEXPMAP")]
        GiveExpMap,
        [ScriptDefName("APPLYMONEX")]
        ApplyMonex,
        [ScriptDefName("CLEARNAMELIST")]
        ClearNameList,
        [ScriptDefName("TINGCASTLEVISITOR")]
        TingCastleVisitor,
        [ScriptDefName("MAKEHEALZONE")]
        MakeHealZone,
        [ScriptDefName("MAKEDAMAGEZONE")]
        MakeDamageZone,
        [ScriptDefName("CLEARZONE")]
        ClearZone,
        [ScriptDefName("READVALUESQL")]
        ReadValueSql,
        [ScriptDefName("READSTRINGSQL")]
        ReadStringSql,
        [ScriptDefName("WRITEVALUESQL")]
        WriteValueSql,
        [ScriptDefName("INCVALUESQL")]
        IncValueSql,
        [ScriptDefName("DECVALUESQL")]
        DecValueSql,
        [ScriptDefName("UPDATEVALUESQL")]
        UpDateValueSql,
        [ScriptDefName("KILLSLAVE")]
        KillSlave,
        [ScriptDefName("SETITEMEVENT")]
        SetItemEvent,
        [ScriptDefName("REMOVEITEMEVENT")]
        ReMoveItemEvent,
        [ScriptDefName("RETURN")]
        Return,
        [ScriptDefName("CLEARCASTLEOWNER")]
        ClearCastleOwner,
        [ScriptDefName("DISSOLUTIONGUILD")]
        DissolutionGuild,
        [ScriptDefName("CHANGEGENDER")]
        ChangeGender,
        [ScriptDefName("SETFAME")]
        SetFame,
        [ScriptDefName("CHANGELEVEL")]
        ChangeLevel,
        [ScriptDefName("MARRY")]
        Marry,
        [ScriptDefName("UNMARRY")]
        UnMarry,
        [ScriptDefName("GETMARRY")]
        GetMarry,
        [ScriptDefName("GETMASTER")]
        GetMaster,
        [ScriptDefName("CLEARSKILL")]
        ClearSkill,
        [ScriptDefName("DELNOJOBSKILL")]
        DelnoJobSkill,
        [ScriptDefName("DELSKILL")]
        DelSkill,
        [ScriptDefName("ADDSKILL")]
        AddSkill,
        [ScriptDefName("SKILLLEVEL")]
        SkillLevel,
        [ScriptDefName("CHANGEPKPOINT")]
        ChangePkPoint,
        [ScriptDefName("CHANGEEXP")]
        ChangeExp,
        [ScriptDefName("CHANGEJOB")]
        ChangeJob,
        [ScriptDefName("MISSION")]
        Mission,
        [ScriptDefName("MOBPLACE")]
        MobPlace,
        [ScriptDefName("SETMEMBERTYPE")]
        SetMemberType,
        [ScriptDefName("SETMEMBERLEVEL")]
        SetMemberLevel,
        [ScriptDefName("GAMEGOLD")]
        GameGold,
        [ScriptDefName("AUTOADDGAMEGOLD")]
        AutoAddGameGold,
        [ScriptDefName("AUTOSUBGAMEGOLD")]
        AutoSubGameGold,
        [ScriptDefName("CHANGENAMECOLOR")]
        ChangeNameColor,
        [ScriptDefName("CLEARPASSWORD")]
        ClearPassword,
        [ScriptDefName("RENEWLEVEL")]
        Renewlevel,
        [ScriptDefName("KILLMONEXPRATE")]
        KillMonExpRate,
        [ScriptDefName("POWERRATE")]
        PowerRate,
        [ScriptDefName("CHANGEPERMISSION")]
        ChangePerMission,
        [ScriptDefName("KILL")]
        Kill,
        [ScriptDefName("KICK")]
        Kick,
        [ScriptDefName("BONUSPOINT")]
        BonusPoint,
        [ScriptDefName("RESTRENEWLEVEL")]
        Restrenewlevel,
        [ScriptDefName("DELMARRY")]
        DelMarry,
        [ScriptDefName("DELMASTER")]
        DelMaster,
        [ScriptDefName("MASTER")]
        Master,
        [ScriptDefName("UNMASTER")]
        UnMaster,
        [ScriptDefName("CREDITPOINT")]
        CreditPoint,
        [ScriptDefName("CLEARNEEDITEMS")]
        ClearNeedItems,
        [ScriptDefName("CLEARMAKEITEMS")]
        ClearMakeItems,
        [ScriptDefName("SETSENDMSGFLAG")]
        SetSendMsgFlag,
        [ScriptDefName("UPGRADEITEM")]
        UpgradeItems,
        [ScriptDefName("UPGRADEITEMEX")]
        UpgradeItemSex,
        [ScriptDefName("MONGENEX")]
        MonGenex,
        [ScriptDefName("CLEARMAPMON")]
        ClearMapMon,
        [ScriptDefName("SETMPAMODE")]
        SetMapMode,
        [ScriptDefName("GAMEPOINT")]
        GamePoint,
        [ScriptDefName("PKZONE")]
        PvpZone,
        [ScriptDefName("RESTBONUSPOINT")]
        RestBonusPoint,
        [ScriptDefName("TAKECASTLEGOLD")]
        TakeCastleGold,
        [ScriptDefName("HUMANHP")]
        HumanHp,
        [ScriptDefName("HUMANMP")]
        HumanMp,
        [ScriptDefName("GUILDBUILDPOINT")]
        BuildPoint,
        [ScriptDefName("GUILDAURAEPOINT")]
        AuraePoint,
        [ScriptDefName("GUILDSTABILITYPOINT")]
        StabilityPoint,
        [ScriptDefName("GUILDFLOURISHPOINT")]
        FlourishPoint,
        [ScriptDefName("OPENITEMBOX")]
        OpenMagicbox,
        [ScriptDefName("SETRANKLEVELNAME")]
        SetRankLevelName,
        [ScriptDefName("GMEXECUTE")]
        GmExecute,
        [ScriptDefName("GUILDCHIEFITEMCOUNT")]
        GuildChiefItemCount,
        [ScriptDefName("ADDNAMEDATELIST")]
        AddNameDateList,
        [ScriptDefName("DELNAMEDATELIST")]
        DelNameDateList,
        [ScriptDefName("MOBFIREBURN")]
        MobFireburn,
        [ScriptDefName("MESSAGEBOX")]
        MessageBox,
        [ScriptDefName("SETSCRIPTFLAG")]
        SetscriptFlag,
        [ScriptDefName("SETAUTOGETEXP")]
        SetautogetExp,
        [ScriptDefName("VAR")]
        Var,
        [ScriptDefName("LOADVAR")]
        LoadVar,
        [ScriptDefName("SAVEVAR")]
        SaveVar,
        [ScriptDefName("CALCVAR")]
        CalcVar,
        [ScriptDefName("OFFLINEPLAY")]
        OffLinePlay,
        [ScriptDefName("KICKOFFLINE")]
        KickOffline,
        [ScriptDefName("STARTTAKEGOLD")]
        StarttakeGold,
        [ScriptDefName("CLEARDELAYGOTO")]
        ClearDelayGoto,
        [ScriptDefName("CHANGERECOMMENDGAMEGOLD")]
        ChangereCommendGameGold,
        [ScriptDefName("ANSIREPLACETEXT")]
        AnsirePlaceText,
        [ScriptDefName("ENCODETEXT")]
        EncodeText,
        [ScriptDefName("DECODETEXT")]
        DecodeText,
        [ScriptDefName("ADDTEXTLIST")]
        AddTextList,
        [ScriptDefName("DELTEXTLIST")]
        DelTextList,
        [ScriptDefName("GROUPMAPMOVE")]
        GroupMapMove,
        [ScriptDefName("RECALLHUMAN")]
        ReCallHuman,
        [ScriptDefName("REGOTO")]
        Regoto,
        [ScriptDefName("INTTOSTR")]
        IntToStr,
        [ScriptDefName("STRTOINT")]
        StrToInt,
        [ScriptDefName("GUILDMOVE")]
        GuildMove,
        [ScriptDefName("GUILDMAPMOVE")]
        GuildMapMove,
        [ScriptDefName("RANDOMMOVE")]
        RandomMove,
        [ScriptDefName("USEBONUSPOINT")]
        UseBonusPoint,
        [ScriptDefName("TAKEONITEM")]
        TakeOnItem,
        [ScriptDefName("TAKEOFFITEM")]
        TakeOffItem,
        [ScriptDefName("GIVEEX")]
        GiveEx,
        [ScriptDefName("TIMEOPEN")]
        TimeOpen,
        [ScriptDefName("TIMECLOSE")]
        TimeClose,
        [ScriptDefName("GUILDMEMBERMAXLIMIT")]
        GuildMemberMaxLimit,
        [ScriptDefName("ADDGUILDNAMEDATELIST")]
        AddGuildNamedateList,
        [ScriptDefName("DELGUILDNAMEDATELIST")]
        DelGuildNamedateList,
        [ScriptDefName("GOHOME")]
        GoHome,
        [ScriptDefName("ADDBLOCKIPLIST")]
        AddBlockIpList,
        [ScriptDefName("MOVEDATA")]
        MovData,
        [ScriptDefName("SENDCOLORMSG")]
        SendColorMsg,
        [ScriptDefName("ADDRANDOMMAPGATE")]
        AddRandomMapGate,
        [ScriptDefName("DELRANDOMMAPGATE")]
        DelRandomMapGate,
        [ScriptDefName("GETRANDOMMAPGATE")]
        GetRandommApGate,
        [ScriptDefName("OPENBOOK")]
        OpenBook,
        [ScriptDefName("OPENBOX")]
        OpenBox,
        [ScriptDefName("CHANGEITEMS")]
        ChangeItems,
        [ScriptDefName("CLEARREMEMBER")]
        ClearreMember,
        [ScriptDefName("SENDMOVEMSG")]
        SendMoveMsg,
        [ScriptDefName("SETITEMSLIGHT")]
        SetItemsLight,
        [ScriptDefName("READRANDOMSTR")]
        ReadrandomStr,
        [ScriptDefName("CHANGERANGEMONPOS")]
        ChangeRangeMonPos,
        [ScriptDefName("READ")]
        Read,
        [ScriptDefName("WRITE")]
        Write,
        [ScriptDefName("CHANGEITEMNEWADDVALUE")]
        ChangeItemNewAddValue,
        [ScriptDefName("OPENHOMEPAGE")]
        OpenHomePage,
        [ScriptDefName("PKZONEX")]
        PvpZoneEx,
        [ScriptDefName("SETITEMSLIGHTEX")]
        SetItemslightex,
        [ScriptDefName("SPELL")]
        Spell,
        [ScriptDefName("ADDMAPMAGICEVENT")]
        AddMapMagicEvent,
        [ScriptDefName("RANDOMADDMAPMAGICEVENT")]
        RandomAddMapMagicEvent,
        [ScriptDefName("SNOW")]
        SNow,
        [ScriptDefName("RANDOMUPGRADEITEM")]
        RandoMupgradeItem,
        [ScriptDefName("LOCK")]
        Lock,
        [ScriptDefName("UNLOCK")]
        Unlock,
        [ScriptDefName("GETDUELITEMS")]
        GetdUelItems,
        [ScriptDefName("CANUSEITEM")]
        CanUseItem,
        [ScriptDefName("AUTOGOTOXY")]
        AutoGotoxy,
        [ScriptDefName("CHANGENEWSTATUS")]
        ChangeNewStatus,
        [ScriptDefName("SENDDELAYMSG")]
        SendDelayMsg,
        [ScriptDefName("ACTVARLIST")]
        ActvarList,
        [ScriptDefName("SETMASKED")]
        SetMasked,
        [ScriptDefName("SENDCENTERMSG")]
        SendCenterMsg,
        [ScriptDefName("CHANGEUSEITEMSTARSLEVEL")]
        ChangeUseItemStarsLevel,
        [ScriptDefName("CHANGEBAGITEMSTARSLEVEL")]
        ChangeBagItemStarsLevel,
        [ScriptDefName("BINDBAGITEM")]
        BindbagItem,
        [ScriptDefName("SETONTIMER")]
        SetOnTimer,
        [ScriptDefName("SETOFFTIMER")]
        SETOFFTIMER,
        [ScriptDefName("KILLSCTIMER")]
        KillScTimer,
        [ScriptDefName("PLAYSOUND")]
        PlaySound,
        [ScriptDefName("SHOWEFFECT")]
        ShowEffect,
        [ScriptDefName("CHANGEITEMVALUE")]
        ChangeItemvalue,
        [ScriptDefName("VIBRATION")]
        Vibration,
        [ScriptDefName("OPENBIGDIALOGBOX")]
        OpenbigDialogBox,
        [ScriptDefName("CLOSEBIGDIALOGBOX")]
        CloseBigDialogBox,
        [ScriptDefName("TAGMAPMOVE")]
        TagMapMove,
        [ScriptDefName("TAGMAPINFO")]
        TagMapinfo,
        [ScriptDefName("AISTART")]
        AiStart,
        [ScriptDefName("AISTOP")]
        AiStop,
        [ScriptDefName("CHANGEIPADDRESS")]
        ChangeIpAddress,
        [ScriptDefName("CHANGEATTATCKMODE")]
        ChangeAttatckMode,
        [ScriptDefName("AILOGON")]
        AiLogon,
        [ScriptDefName("AILOGONEX")]
        AiLogonEx,
        [ScriptDefName("KICKALL")]
        KickAll,
        [ScriptDefName("TAKEITEMLIST")]
        TakeItemList,
        [ScriptDefName("SHOWRANKLEVLNAME")]
        ShowRankLevlName,
        [ScriptDefName("LOADROBOTCONFIG")]
        LoadRobotConfig,
        [ScriptDefName("ACTMISSION")]
        ActMission,
        [ScriptDefName("GUILDRECALL")]
        GuildReCall,
        [ScriptDefName("GROUPADDLIST")]
        GroupAddList,
        [ScriptDefName("CLEARLIST")]
        ClearList,
        [ScriptDefName("GROUPRECALL")]
        GroupReCall,
        [ScriptDefName("GROUPMOVEMAP")]
        GroupMoveMap,
        [ScriptDefName("SAVESLAVES")]
        SaveSlaves,
        /// <summary>
        /// 检查会员时间
        /// </summary>
        [ScriptDefName("CHECKUSERDATE")]
        CheckUserDate,
        /// <summary>
        /// 加入会员人物及时间
        /// </summary>
        [ScriptDefName("ADDUSERDATE")]
        AddUserDate,
        /// <summary>
        /// 删除会员人物及时间
        /// </summary>
        [ScriptDefName("DELUSERDATE")]
        DelUserDate,
        /// <summary>
        /// 挂机
        /// </summary>
        [ScriptDefName("OFFLINE")]
        OffLine,
        /// <summary>
        /// 特修身上所有装备
        /// </summary>
        [ScriptDefName("REPAIRALL")]
        RepairAll,
        /// <summary>
        /// 产生一个随机数字
        /// </summary>
        [ScriptDefName("SETRANDOMNO")]
        SetRandomNo,
        /// <summary>
        /// 刷新包裹物品命令
        /// </summary>
        [ScriptDefName("QUERYBAGITEMS")]
        QueryBagItems,
        /// <summary>
        /// 将指定物品刷新到指定地图坐标范围内
        /// </summary>
        [ScriptDefName("THROWITEM")]
        ThrowItem,
        /// <summary>
        /// 开通元宝交易
        /// </summary>
        [ScriptDefName("OPENYBDEAL")]
        OpenYbDeal,
        /// <summary>
        /// 查询可以购买的物品
        /// </summary>        
        [ScriptDefName("QUERYYBSELL")]
        QueryYbSell,
        /// <summary>
        /// 查询元宝交易
        /// </summary>
        [ScriptDefName("QUERYYBDEAL")]
        QueryYbDeal,
        /// <summary>
        /// 延时跳转
        /// </summary>
        [ScriptDefName("DELAYGOTO")]
        DelayGoto,
        /// <summary>
        /// 延时跳转
        /// </summary>
        [ScriptDefName("DELAYCALL")]
        DelayCall,
        /// <summary>
        /// 获取客户端输入值
        /// </summary>
        [ScriptDefName("QUERYVALUE")]
        QueryValue,
        /// <summary>
        /// 杀死所有宠物
        /// </summary>
        [ScriptDefName("KILLSLAVENAME")]
        KillSlaveName,
        /// <summary>
        /// 查询物品信息
        /// </summary>
        [ScriptDefName("QUERYITEMDLG")]
        QueryItemDlg,
        /// <summary>
        /// 升级对话框中的物品
        /// </summary>
        [ScriptDefName("UPGRADEDLGITEM")]
        UpgradeDlgItem,
        /// <summary>
        /// 获取装备指定属性到变量中
        /// </summary>
        [ScriptDefName("GETDLGITEMVALUE")]
        GetDlgItemValue,
        /// <summary>
        /// 回收对话框中的物品
        /// </summary>
        [ScriptDefName("TAKEDLGITEM")]
        TakeDlgItem,

        [ScriptDefName("GAMEDIAMOND")]
        GAMEDIAMOND,

        [ScriptDefName("OPENMERCHANTBIGDLG")]
        OPENMERCHANTBIGDLG,

        [ScriptDefName("GIVEFENGHAO")]
        GIVEFENGHAO,

        [ScriptDefName("NOT")]
        NOT,

        [ScriptDefName("CHANGENGEXP")]
        CHANGENGEXP,

        [ScriptDefName("OPENLASTSKILL")]
        OPENLASTSKILL,

        [ScriptDefName("OPENUPGRADEDLG")]
        OPENUPGRADEDLG,

        [ScriptDefName("RECLAIMITEM")]
        RECLAIMITEM,

        [ScriptDefName("SETFLUTECOUNT")]
        SETFLUTECOUNT,

        [ScriptDefName("KILLMONBURSTRATE")]
        KILLMONBURSTRATE,

        [ScriptDefName("GETRANDOMLINETEXT")]
        GETRANDOMLINETEXT,

        [ScriptDefName("GOLDCOUNT")]
        GOLDCOUNT,

        [ScriptDefName("READSKILLNG")]
        READSKILLNG,

        [ScriptDefName("DETOXIFCATION")]
        DETOXIFCATION,

        [ScriptDefName("SHOWPHANTOM")]
        SHOWPHANTOM,

        [ScriptDefName("SETNEWITEMVALUE")]
        SETNEWITEMVALUE,

        [ScriptDefName("SETNPCIMAGE")]
        SETNPCIMAGE,

        [ScriptDefName("SUPERMOVEMSG")]
        SUPERMOVEMSG,

        [ScriptDefName("OPENSTORAGEVIEW")]
        OPENSTORAGEVIEW,

        [ScriptDefName("OPENUPGRADEDIALOG")]
        OPENUPGRADEDIALOG,

        [ScriptDefName("LOOPGOTO")]
        LOOPGOTO,

        [ScriptDefName("GETDBITEMFIELDVALUE")]
        GETDBITEMFIELDVALUE,

        [ScriptDefName("EXTBAGPAGECOUNT")]
        EXTBAGPAGECOUNT,

        [ScriptDefName("EXTBAGOPENITEMCOUNT")]
        EXTBAGOPENITEMCOUNT,

        [ScriptDefName("SHOWGODBLESS")]
        SHOWGODBLESS,

        [ScriptDefName("OPENGODBLESS")]
        OPENGODBLESS,

        [ScriptDefName("GIVEGAMEPET")]
        GIVEGAMEPET,

        [ScriptDefName("ACTIVATIONCASKET")]
        ACTIVATIONCASKET,

        [ScriptDefName("CLEARALLPULSE")]
        CLEARALLPULSE,

        [ScriptDefName("EXITGAME")]
        EXITGAME,

        [ScriptDefName("GIVESTATEITEM")]
        GIVESTATEITEM,

        [ScriptDefName("GETRANDOMTEXT")]
        GETRANDOMTEXT,

        [ScriptDefName("ADDGUILDMEMBER")]
        ADDGUILDMEMBER,

        [ScriptDefName("RECALLGAMEPET")]
        RECALLGAMEPET,

        [ScriptDefName("SETGAMEPETATTACKHUMPOWERRATE")]
        SETGAMEPETATTACKHUMPOWERRATE,

        [ScriptDefName("SETGAMEPETENABLEPICK")]
        SETGAMEPETENABLEPICK,

        [ScriptDefName("GAMEGIRD")]
        GAMEGIRD,

        [ScriptDefName("CHANGESTATE")]
        CHANGESTATE,

        [ScriptDefName("CHANGESLAVELEVEL")]
        CHANGESLAVELEVEL,

        [ScriptDefName("REALIVE")]
        REALIVE,

        [ScriptDefName("ADDMPPER")]
        ADDMPPER,

        [ScriptDefName("GETLISTSTRING")]
        GETLISTSTRING,

        [ScriptDefName("SETSTRINGBLANK")]
        SETSTRINGBLANK,

        [ScriptDefName("GETSTRINGPOS")]
        GETSTRINGPOS,

        [ScriptDefName("SORTVARTOLIST")]
        SORTVARTOLIST,

        [ScriptDefName("CHANGEMODEEX")]
        CHANGEMODEEX,

        [ScriptDefName("ADDHPPER")]
        ADDHPPER,

        [ScriptDefName("TAKEON")]
        TAKEON,

        [ScriptDefName("ABILITYADD")]
        ABILITYADD,

        [ScriptDefName("READRANDOMLINE")]
        READRANDOMLINE,

        [ScriptDefName("CHANGEATTACKMODE")]
        CHANGEATTACKMODE,

        [ScriptDefName("RECALLHERO")]
        RECALLHERO,

        [ScriptDefName("OFFLINEPLAYEX")]
        OFFLINEPLAYEX,

        [ScriptDefName("SETSCTIMER")]
        SETSCTIMER,

        [ScriptDefName("DELLINELIST")]
        DELLINELIST,

        [ScriptDefName("SENDSCROLLMSG")]
        SENDSCROLLMSG,

        [ScriptDefName("WEBBROWSER")]
        WEBBROWSER,

        [ScriptDefName("SETOFFLINEPLAY")]
        SETOFFLINEPLAY,

        [ScriptDefName("ADDLINELIST")]
        ADDLINELIST,
    }
}