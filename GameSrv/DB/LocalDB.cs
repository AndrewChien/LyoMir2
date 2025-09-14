using M2Server.Npc;
using ScriptSystem.Consts;
using System.Collections;
using SystemModule.Actors;
using SystemModule.Const;
using SystemModule.Maps;

namespace GameSrv.DB
{
    /// <summary>
    /// 加载txt数据
    /// </summary>
    public class LocalDb
    {
        private static readonly char[] TextSplitConst = { ' ', '\t' };
        private static readonly char[] MonsterSpitConst = { ' ', '/', '\t' };
        private static readonly string[] divider = new[] { "/", "\\", " ", "\t" };

        /// <summary>
        /// 加载管理员列表AdminList.txt
        /// </summary>
        /// <returns></returns>
        public static bool LoadAdminList()
        {
            string sIPaddr = string.Empty;
            string sChrName = string.Empty;
            string sData = string.Empty;
            string sfilename = M2Share.GetEnvirFilePath("AdminList.txt");
            if (!File.Exists(sfilename))
            {
                return false;
            }
            SystemShare.WorldEngine.AdminList.Clear();
            using StringList loadList = new StringList();
            loadList.LoadFromFile(sfilename);
            for (int i = 0; i < loadList.Count; i++)
            {
                string sLineText = loadList[i];
                int nLv = -1;
                if (!string.IsNullOrEmpty(sLineText) && sLineText[0] != ';')
                {
                    if (sLineText[0] == '*')
                    {
                        nLv = 10;
                    }
                    else if (sLineText[0] == '1')
                    {
                        nLv = 9;
                    }
                    else if (sLineText[0] == '2')
                    {
                        nLv = 8;
                    }
                    else if (sLineText[0] == '3')
                    {
                        nLv = 7;
                    }
                    else if (sLineText[0] == '4')
                    {
                        nLv = 6;
                    }
                    else if (sLineText[0] == '5')
                    {
                        nLv = 5;
                    }
                    else if (sLineText[0] == '6')
                    {
                        nLv = 4;
                    }
                    else if (sLineText[0] == '7')
                    {
                        nLv = 3;
                    }
                    else if (sLineText[0] == '8')
                    {
                        nLv = 2;
                    }
                    else if (sLineText[0] == '9')
                    {
                        nLv = 1;
                    }
                    if (nLv > 0)
                    {
                        sLineText = HUtil32.GetValidStrCap(sLineText, ref sData, divider);
                        sLineText = HUtil32.GetValidStrCap(sLineText, ref sChrName, new[] { "/", "\\", " ", "\t" });
                        sLineText = HUtil32.GetValidStrCap(sLineText, ref sIPaddr, new[] { "/", "\\", " ", "\t" });
                        if (string.IsNullOrEmpty(sChrName) || string.IsNullOrEmpty(sIPaddr))
                        {
                            continue;
                        }
                        AdminInfo adminInfo = new AdminInfo
                        {
                            Level = (byte)nLv,
                            ChrName = sChrName,
                            IPaddr = sIPaddr
                        };
                        SystemShare.WorldEngine.AdminList.Add(adminInfo);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 读取守卫配置GuardList.txt
        /// </summary>
        public static void LoadGuardList()
        {
            try
            {
                string monName = string.Empty;
                string mapName = string.Empty;
                string cX = string.Empty;
                string cY = string.Empty;
                string direction = string.Empty;
                string sFileName = M2Share.GetEnvirFilePath("GuardList.txt");
                if (File.Exists(sFileName))
                {
                    StringList guardList = new StringList();
                    guardList.LoadFromFile(sFileName);
                    for (int i = 0; i < guardList.Count; i++)
                    {
                        string sLine = guardList[i];
                        if (!string.IsNullOrEmpty(sLine) && sLine[0] != ';')
                        {
                            sLine = HUtil32.GetValidStrCap(sLine, ref monName, ' ');
                            if (!string.IsNullOrEmpty(monName) && monName[0] == '\"')
                            {
                                HUtil32.ArrestStringEx(monName, "\"", "\"", ref monName);
                            }
                            sLine = HUtil32.GetValidStr3(sLine, ref mapName, ' ');
                            sLine = HUtil32.GetValidStr3(sLine, ref cX, new[] { ' ', ',' });
                            sLine = HUtil32.GetValidStr3(sLine, ref cY, new[] { ' ', ',', ':' });
                            sLine = HUtil32.GetValidStr3(sLine, ref direction, new[] { ' ', ':' });
                            if (!string.IsNullOrEmpty(monName) && !string.IsNullOrEmpty(mapName) && !string.IsNullOrEmpty(direction))
                            {
                                IActor guard = SystemShare.WorldEngine.RegenMonsterByName(mapName, HUtil32.StrToInt16(cX, 0), HUtil32.StrToInt16(cY, 0), monName);
                                if (guard != null)
                                {
                                    guard.Dir = (byte)HUtil32.StrToInt(direction, 0);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// 读取物品合成配置MakeItem.txt
        /// </summary>
        public void LoadMakeItem()
        {
            string sSubName = string.Empty;
            string sItemName = string.Empty;
            IList<MakeItem> list28 = null;
            string sFileName = M2Share.GetEnvirFilePath("MakeItem.txt");
            if (File.Exists(sFileName))
            {
                using StringList loadList = new StringList();
                loadList.LoadFromFile(sFileName);
                for (int i = 0; i < loadList.Count; i++)
                {
                    string sLine = loadList[i].Trim();
                    if (string.IsNullOrEmpty(sLine) || sLine.StartsWith(';'))
                    {
                        continue;
                    }
                    if (sLine.StartsWith('['))
                    {
                        if (list28 != null)
                        {
                            M2Share.MakeItemList.Add(sItemName, list28);
                        }
                        list28 = new List<MakeItem>();
                        HUtil32.ArrestStringEx(sLine, "[", "]", ref sItemName);
                    }
                    else
                    {
                        if (list28 != null)
                        {
                            sLine = HUtil32.GetValidStr3(sLine, ref sSubName, TextSplitConst);
                            int nItemCount = HUtil32.StrToInt(sLine.Trim(), 1);
                            list28.Add(new MakeItem() { ItemName = sSubName, ItemCount = nItemCount });
                        }
                    }
                }
                if (list28 != null)
                {
                    M2Share.MakeItemList.Add(sItemName, list28);
                }
            }
        }

        /// <summary>
        /// 读取Market_Def/QFunction-0.txt
        /// </summary>
        private static void QFunctionNpc()
        {
            try
            {
                string sScriptFile = M2Share.GetEnvirFilePath(ScriptFlagConst.sMarket_Def, "QFunction-0.txt");
                string sScritpDir = M2Share.GetEnvirFilePath(ScriptFlagConst.sMarket_Def);
                if (!Directory.Exists(sScritpDir))
                {
                    Directory.CreateDirectory(sScritpDir);
                }
                if (!File.Exists(sScriptFile))
                {
                    using StringList saveList = new StringList();
                    saveList.Add(";此脚为功能脚本，用于实现各种与脚本有关的功能");
                    saveList.SaveToFile(sScriptFile);
                }
                if (File.Exists(sScriptFile))
                {
                    SystemShare.FunctionNPC = new Merchant
                    {
                        MapName = "0",
                        CurrX = 0,
                        CurrY = 0,
                        ChrName = "QFunction",
                        NpcFlag = 0,
                        Appr = 0,
                        FilePath = ScriptFlagConst.sMarket_Def,
                        ScriptName = "QFunction",
                        IsHide = true,
                        IsQuest = false
                    };
                    SystemShare.WorldEngine.AddMerchant(SystemShare.FunctionNPC);
                }
                else
                {
                    SystemShare.FunctionNPC = null;
                }
            }
            catch
            {
                SystemShare.FunctionNPC = null;
            }
        }

        /// <summary>
        /// 读取MapQuest_def/QManage.txt
        /// </summary>
        private static void QMangeNpc()
        {
            try
            {
                string sScriptFile = M2Share.GetEnvirFilePath("MapQuest_def", "QManage.txt");
                string sScritpDir = M2Share.GetEnvirFilePath("MapQuest_def");
                if (!Directory.Exists(sScritpDir))
                {
                    Directory.CreateDirectory(sScritpDir);
                }
                if (!File.Exists(sScriptFile))
                {
                    string sShowFile = HUtil32.ReplaceChar(sScriptFile, '\\', '/');
                    StringList saveList = new StringList();
                    saveList.Add(";此脚为登录脚本，人物每次登录时都会执行此脚本，所有人物初始设置都可以放在此脚本中。");
                    saveList.Add(";修改脚本内容，可用@ReloadManage命令重新加载该脚本，不须重启程序。");
                    saveList.Add("[@Login]");
                    saveList.Add("#if");
                    saveList.Add("#act");
                    saveList.Add(";设置10倍杀怪经验");
                    saveList.Add(";CANGETEXP 1 10");
                    saveList.Add("#say");
                    saveList.Add("游戏登录脚本运行成功，欢迎进入本游戏!!!\\ \\");
                    saveList.Add("<关闭/@exit> \\ \\");
                    saveList.Add("登录脚本文件位于: \\");
                    saveList.Add(sShowFile + '\\');
                    saveList.Add("脚本内容请自行按自己的要求修改。");
                    saveList.SaveToFile(sScriptFile);
                    saveList = null;
                }
                if (File.Exists(sScriptFile))
                {
                    SystemShare.ManageNPC = new Merchant
                    {
                        MapName = "0",
                        CurrX = 0,
                        CurrY = 0,
                        ChrName = "QManage",
                        NpcFlag = 0,
                        Appr = 0,
                        FilePath = "MapQuest_def",
                        IsHide = true,
                        IsQuest = false
                    };
                    SystemShare.WorldEngine.AddQuestNpc(SystemShare.ManageNPC);
                }
                else
                {
                    SystemShare.ManageNPC = null;
                }
            }
            catch
            {
                SystemShare.ManageNPC = null;
            }
        }

        /// <summary>
        /// 读取Robot_def/RobotManage.txt
        /// </summary>
        private static void RobotNpc()
        {
            try
            {
                string sScriptFile = M2Share.GetEnvirFilePath("Robot_def", "RobotManage.txt");
                string sScritpDir = M2Share.GetEnvirFilePath("Robot_def");
                if (!Directory.Exists(sScritpDir))
                {
                    Directory.CreateDirectory(sScritpDir);
                }
                if (!File.Exists(sScriptFile))
                {
                    StringList tSaveList = new StringList();
                    tSaveList.Add(";此脚为机器人专用脚本，用于机器人处理功能用的脚本。");
                    tSaveList.SaveToFile(sScriptFile);
                    tSaveList = null;
                }
                if (File.Exists(sScriptFile))
                {
                    SystemShare.RobotNPC = new Merchant
                    {
                        MapName = "0",
                        CurrX = 0,
                        CurrY = 0,
                        ChrName = "RobotManage",
                        NpcFlag = 0,
                        Appr = 0,
                        FilePath = "Robot_def",
                        IsHide = true,
                        IsQuest = false
                    };
                    SystemShare.WorldEngine.AddQuestNpc(SystemShare.RobotNPC);
                }
                else
                {
                    SystemShare.RobotNPC = null;
                }
            }
            catch
            {
                SystemShare.RobotNPC = null;
            }
        }

        /// <summary>
        /// 读取地图任务配置MapQuest.txt
        /// </summary>
        /// <returns></returns>
        public int LoadMapQuest()
        {
            int result = 1;
            string sMap = string.Empty;
            string s1C = string.Empty;
            string s20 = string.Empty;
            string sMonName = string.Empty;
            string sItem = string.Empty;
            string sQuest = string.Empty;
            string s30 = string.Empty;
            string s34 = string.Empty;
            string sFileName = M2Share.GetEnvirFilePath("MapQuest.txt");
            if (File.Exists(sFileName))
            {
                StringList tMapQuestList = new StringList();
                tMapQuestList.LoadFromFile(sFileName);
                for (int i = 0; i < tMapQuestList.Count; i++)
                {
                    string tStr = tMapQuestList[i];
                    if (!string.IsNullOrEmpty(tStr) && tStr[0] != ';')
                    {
                        tStr = HUtil32.GetValidStr3(tStr, ref sMap, TextSplitConst);
                        tStr = HUtil32.GetValidStr3(tStr, ref s1C, TextSplitConst);
                        tStr = HUtil32.GetValidStr3(tStr, ref s20, TextSplitConst);
                        tStr = HUtil32.GetValidStr3(tStr, ref sMonName, TextSplitConst);
                        if (!string.IsNullOrEmpty(sMonName) && sMonName[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(sMonName, "\"", "\"", ref sMonName);
                        }
                        tStr = HUtil32.GetValidStr3(tStr, ref sItem, TextSplitConst);
                        if (!string.IsNullOrEmpty(sItem) && sItem[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(sItem, "\"", "\"", ref sItem);
                        }
                        tStr = HUtil32.GetValidStr3(tStr, ref sQuest, TextSplitConst);
                        tStr = HUtil32.GetValidStr3(tStr, ref s30, TextSplitConst);
                        if (!string.IsNullOrEmpty(sMap) && !string.IsNullOrEmpty(sMonName) && !string.IsNullOrEmpty(sQuest))
                        {
                            IEnvirnoment map = SystemShare.MapMgr.FindMap(sMap);
                            if (map != null)
                            {
                                HUtil32.ArrestStringEx(s1C, "[", "]", ref s34);
                                int n38 = HUtil32.StrToInt(s34, 0);
                                int n3C = HUtil32.StrToInt(s20, 0);
                                bool boGrouped = HUtil32.CompareLStr(s30, "GROUP");
                                if (!GameShare.QuestManager.CreateQuest(n38, n3C, sMonName, sItem, sQuest, boGrouped))
                                {
                                    result = -i;
                                }
                            }
                            else
                            {
                                result = -i;
                            }
                        }
                        else
                        {
                            result = -i;
                        }
                    }
                }
            }
            QMangeNpc();
            QFunctionNpc();
            RobotNpc();
            return result;
        }

        /// <summary>
        /// 读取交易商人配置Merchant.txt
        /// </summary>
        public void LoadMerchant()
        {
            string sScript = string.Empty;
            string sMapName = string.Empty;
            string sX = string.Empty;
            string sY = string.Empty;
            string sName = string.Empty;
            string sFlag = string.Empty;
            string sAppr = string.Empty;
            string sIsCalste = string.Empty;
            string sCanMove = string.Empty;
            string sMoveTime = string.Empty;
            string sFileName = M2Share.GetEnvirFilePath("Merchant.txt");
            if (File.Exists(sFileName))
            {
                StringList tMerchantList = new StringList();
                tMerchantList.LoadFromFile(sFileName);
                for (int i = 0; i < tMerchantList.Count; i++)
                {
                    string sLineText = tMerchantList[i].Trim();
                    if (!string.IsNullOrEmpty(sLineText) && sLineText[0] != ';')
                    {
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sScript, TextSplitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sMapName, TextSplitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sX, TextSplitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sY, TextSplitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sName, TextSplitConst);
                        if (!string.IsNullOrEmpty(sName) && sName[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(sName, "\"", "\"", ref sName);
                        }
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sFlag, TextSplitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sAppr, TextSplitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sIsCalste, TextSplitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sCanMove, TextSplitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sMoveTime, TextSplitConst);
                        if (!string.IsNullOrEmpty(sScript) && !string.IsNullOrEmpty(sMapName) && !string.IsNullOrEmpty(sAppr))
                        {
                            IMerchant merchantOldNpc = new Merchant()
                            {
                                ScriptName = sScript,
                                MapName = sMapName,
                                CurrX = HUtil32.StrToInt16(sX, 0),
                                CurrY = HUtil32.StrToInt16(sY, 0),
                                ChrName = sName,
                                NpcFlag = HUtil32.StrToInt16(sFlag, 0),
                                Appr = (ushort)HUtil32.StrToInt(sAppr, 0),
                                MoveTime = HUtil32.StrToInt(sMoveTime, 0)
                            };
                            if (HUtil32.StrToInt(sIsCalste, 0) != 0)
                            {
                                merchantOldNpc.CastleMerchant = true;
                            }
                            if (HUtil32.StrToInt(sCanMove, 0) != 0 && merchantOldNpc.MoveTime > 0)
                            {
                                merchantOldNpc.BoCanMove = true;
                            }
                            SystemShare.WorldEngine.AddMerchant(merchantOldNpc);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 读取MonGen文件夹下所有怪物信息文件
        /// </summary>
        /// <param name="monGenList"></param>
        /// <param name="sFileName"></param>
        private static void LoadMapGen(StringList monGenList, string sFileName)
        {
            string sFileDir = M2Share.GetEnvirFilePath("MonGen");
            if (!Directory.Exists(sFileDir))
            {
                Directory.CreateDirectory(sFileDir);
            }
            string sFilePatchName = sFileDir + sFileName;
            if (!File.Exists(sFilePatchName))
            {
                return;
            }

            using StringList loadList = new StringList();
            loadList.LoadFromFile(sFilePatchName);
            for (int i = 0; i < loadList.Count; i++)
            {
                monGenList.Add(loadList[i]);
            }
        }

        /// <summary>
        /// 读取怪物刷新配置信息MonGen.txt
        /// </summary>
        /// <returns></returns>
        public int LoadMonGen(out int mongenCount)
        {
            string sLineText = string.Empty;
            string sData = string.Empty;
            int i;
            int result = 0;
            mongenCount = 0;
            string sFileName = M2Share.GetEnvirFilePath("MonGen.txt");
            if (File.Exists(sFileName))
            {
                using StringList loadList = new StringList();
                loadList.LoadFromFile(sFileName);
                i = 0;
                while (true)
                {
                    if (i >= loadList.Count)
                    {
                        break;
                    }
                    if (HUtil32.CompareLStr("loadgen", loadList[i]))
                    {
                        string sMapGenFile = HUtil32.GetValidStr3(loadList[i], ref sLineText, TextSplitConst);
                        loadList.RemoveAt(i);
                        if (!string.IsNullOrEmpty(sMapGenFile))
                        {
                            LoadMapGen(loadList, sMapGenFile);
                        }
                    }
                    i++;
                }
                MonGenInfo monGenInfo = null;
                for (i = 0; i < loadList.Count; i++)
                {
                    sLineText = loadList[i];
                    if (!string.IsNullOrEmpty(sLineText) && sLineText[0] != ';')
                    {
                        monGenInfo = new MonGenInfo();
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSplitConst);
                        monGenInfo.MapName = sData;
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSplitConst);
                        monGenInfo.X = (short)HUtil32.StrToInt(sData, 0);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSplitConst);
                        monGenInfo.Y = (short)HUtil32.StrToInt(sData, 0);
                        sLineText = HUtil32.GetValidStrCap(sLineText, ref sData, TextSplitConst);
                        if (!string.IsNullOrEmpty(sData) && sData[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(sData, "\"", "\"", ref sData);
                        }
                        monGenInfo.MonName = sData;
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSplitConst);
                        monGenInfo.Range = (byte)HUtil32.StrToInt(sData, 0);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSplitConst);
                        monGenInfo.Count = (ushort)HUtil32.StrToInt(sData, 0);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSplitConst);
                        monGenInfo.ZenTime = HUtil32.StrToInt(sData, -1) * 60 * 1000;
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSplitConst);
                        monGenInfo.MissionGenRate = (byte)HUtil32.StrToInt(sData, 0);// 集中座标刷新机率 1 -100
                        if (!string.IsNullOrEmpty(monGenInfo.MapName) && !string.IsNullOrEmpty(monGenInfo.MonName) 
                            && monGenInfo.ZenTime > 0 
                            && SystemShare.MapMgr.GetMapInfo(M2Share.ServerIndex, monGenInfo.MapName) != null)
                        {
                            monGenInfo.CertList = new List<IMonsterActor>();
                            monGenInfo.Envir = SystemShare.MapMgr.FindMap(monGenInfo.MapName);
                            if (monGenInfo.Envir != null)
                            {
                                SystemShare.WorldEngine.AddMonGenList(monGenInfo);
                            }
                            else
                            {
                                monGenInfo = null;
                            }
                        }
                    }
                }
                monGenInfo = new MonGenInfo
                {
                    CertList = new List<IMonsterActor>(),
                    Envir = null
                };
                if (SystemShare.WorldEngine.CheckMonGenInfoThreadMap(0))
                {
                    SystemShare.WorldEngine.AddMonGenInfoThreadMap(0, monGenInfo);
                }
                else
                {
                    SystemShare.WorldEngine.CreateMonGenInfoThreadMap(0, new List<MonGenInfo>() { monGenInfo });
                }
                result = 1;
                mongenCount = SystemShare.WorldEngine.MonGenCount;
            }
            return result;
        }

        /// <summary>
        /// 读取MonItems文件夹下怪物物品掉落配置
        /// </summary>
        /// <returns></returns>
        public void LoadMonitems(string monName, ref IList<MonsterDropItem> itemList)
        {
            string sData = string.Empty;
            string monFileName = M2Share.GetEnvirFilePath("MonItems", $"{monName}.txt");
            if (File.Exists(monFileName))
            {
                if (itemList != null)
                {
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        itemList[i] = default;
                    }
                    itemList.Clear();
                }
                if (itemList == null)
                {
                    itemList = new List<MonsterDropItem>();
                }
                using StringList loadList = new StringList();
                loadList.LoadFromFile(monFileName);
                for (int i = 0; i < loadList.Count; i++)
                {
                    string s28 = loadList[i];
                    if (!string.IsNullOrEmpty(s28) && s28[0] != ';')
                    {
                        s28 = HUtil32.GetValidStr3(s28, ref sData, MonsterSpitConst);
                        int n18 = HUtil32.StrToInt(sData, -1);
                        s28 = HUtil32.GetValidStr3(s28, ref sData, MonsterSpitConst);
                        int n1C = HUtil32.StrToInt(sData, -1);
                        s28 = HUtil32.GetValidStr3(s28, ref sData, TextSplitConst);
                        if (!string.IsNullOrEmpty(sData))
                        {
                            if (sData[0] == '\"')
                            {
                                HUtil32.ArrestStringEx(sData, "\"", "\"", ref sData);
                            }
                        }
                        string itemName = sData;
                        s28 = HUtil32.GetValidStr3(s28, ref sData, TextSplitConst);
                        int itemCount = HUtil32.StrToInt(sData, 1);
                        if (n18 > 0 && n1C > 0 && !string.IsNullOrEmpty(itemName))
                        {
                            MonsterDropItem monItem = new MonsterDropItem
                            {
                                SelPoint = n18 - 1,
                                MaxPoint = n1C,
                                ItemName = itemName,
                                Count = itemCount
                            };
                            itemList.Add(monItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 读取管理NPC配置Npcs.txt
        /// </summary>
        public void LoadNpcs()
        {
            string chrName = string.Empty;
            string type = string.Empty;
            string mapName = string.Empty;
            string cX = string.Empty;
            string cY = string.Empty;
            string flag = string.Empty;
            string appr = string.Empty;
            string sFileName = M2Share.GetEnvirFilePath("Npcs.txt");
            if (File.Exists(sFileName))
            {
                using StringList loadList = new StringList();
                loadList.LoadFromFile(sFileName);
                for (int i = 0; i < loadList.Count; i++)
                {
                    string sData = loadList[i].Trim();
                    if (!string.IsNullOrEmpty(sData) && sData[0] != ';')
                    {
                        sData = HUtil32.GetValidStrCap(sData, ref chrName, TextSplitConst);
                        if (!string.IsNullOrEmpty(chrName) && chrName[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(chrName, "\"", "\"", ref chrName);
                        }
                        sData = HUtil32.GetValidStr3(sData, ref type, TextSplitConst);
                        sData = HUtil32.GetValidStr3(sData, ref mapName, TextSplitConst);
                        sData = HUtil32.GetValidStr3(sData, ref cX, TextSplitConst);
                        sData = HUtil32.GetValidStr3(sData, ref cY, TextSplitConst);
                        sData = HUtil32.GetValidStr3(sData, ref flag, TextSplitConst);
                        sData = HUtil32.GetValidStr3(sData, ref appr, TextSplitConst);
                        if (!string.IsNullOrEmpty(chrName) && !string.IsNullOrEmpty(mapName) && !string.IsNullOrEmpty(appr))
                        {
                            NormNpc npc = null;
                            switch (HUtil32.StrToInt(type, 0))
                            {
                                case 0:
                                    npc = new Merchant();
                                    break;
                                case 1:
                                    npc = new GuildOfficial();
                                    break;
                                case 2:
                                    npc = new CastleOfficial();
                                    break;
                            }
                            if (npc != null)
                            {
                                npc.MapName = mapName;
                                npc.CurrX = HUtil32.StrToInt16(cX, 0);
                                npc.CurrY = HUtil32.StrToInt16(cY, 0);
                                npc.ChrName = chrName;
                                npc.NpcFlag = HUtil32.StrToInt16(flag, 0);
                                npc.Appr = (ushort)HUtil32.StrToInt(appr, 0);
                                SystemShare.WorldEngine.AddQuestNpc(npc);
                            }
                        }
                    }
                }
            }
        }

        private static string LoadQuestDiaryList(int nIndex)
        {
            string result;
            if (nIndex >= 1000)
            {
                result = nIndex.ToString();
                return result;
            }
            if (nIndex >= 100)
            {
                result = nIndex.ToString() + '0';
                return result;
            }
            result = nIndex + "00";
            return result;
        }

        /// <summary>
        /// 读取QuestDiary文件夹下所有自定义配置文件
        /// </summary>
        /// <returns></returns>
        public int LoadQuestDiary()
        {
            int result = 1;
            string s18 = string.Empty;
            string s20 = string.Empty;
            bool bo2D = false;
            int nC = 1;
            //M2Share.QuestDiaryList.Clear();
            while (true)
            {
                IList<TQDDinfo> qdDinfoList = null;
                string sFileName = M2Share.GetEnvirFilePath("QuestDiary", LoadQuestDiaryList(nC) + ".txt");
                if (File.Exists(sFileName))
                {
                    s18 = string.Empty;
                    TQDDinfo qdDinfo = null;
                    using StringList loadList = new StringList();
                    loadList.LoadFromFile(sFileName);
                    for (int i = 0; i < loadList.Count; i++)
                    {
                        string s1C = loadList[i];
                        if (!string.IsNullOrEmpty(s1C) && s1C[0] != ';')
                        {
                            if (s1C[0] == '[' && s1C.Length > 2)
                            {
                                if (string.IsNullOrEmpty(s18))
                                {
                                    HUtil32.ArrestStringEx(s1C, "[", "]", ref s18);
                                    qdDinfoList = new List<TQDDinfo>();
                                    qdDinfo = new TQDDinfo
                                    {
                                        n00 = nC,
                                        s04 = s18,
                                        sList = new ArrayList()
                                    };
                                    qdDinfoList.Add(qdDinfo);
                                    bo2D = true;
                                }
                                else
                                {
                                    if (s1C[0] != '@')
                                    {
                                        s1C = HUtil32.GetValidStr3(s1C, ref s20, TextSplitConst);
                                        HUtil32.ArrestStringEx(s20, "[", "]", ref s20);
                                        qdDinfo = new TQDDinfo
                                        {
                                            n00 = HUtil32.StrToInt(s20, 0),
                                            s04 = s1C,
                                            sList = new ArrayList()
                                        };
                                        qdDinfoList.Add(qdDinfo);
                                        bo2D = true;
                                    }
                                    else
                                    {
                                        bo2D = false;
                                    }
                                }
                            }
                            else
                            {
                                if (bo2D)
                                {
                                    qdDinfo.sList.Add(s1C);
                                }
                            }
                        }
                    }
                }
                if (qdDinfoList != null)
                {
                    // M2Share.QuestDiaryList.Add(qdDinfoList);
                }
                else
                {
                    //M2Share.QuestDiaryList.Add(null);
                }
                nC++;
                if (nC >= 105)
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 读取安全区配置StartPoint.txt
        /// </summary>
        public void LoadStartPoint()
        {
            string mapName = string.Empty;
            string cX = string.Empty;
            string cY = string.Empty;
            string allSay = string.Empty;
            string range = string.Empty;
            string type = string.Empty;
            string zone = string.Empty;
            string fire = string.Empty;
            string sFileName = M2Share.GetEnvirFilePath("StartPoint.txt");
            if (File.Exists(sFileName))
            {
                M2Share.StartPointList.Clear();
                using StringList loadList = new StringList();
                loadList.LoadFromFile(sFileName);
                for (int i = 0; i < loadList.Count; i++)
                {
                    string sLine = loadList[i].Trim();
                    if (!string.IsNullOrEmpty(sLine) && sLine[0] != ';')
                    {
                        sLine = HUtil32.GetValidStr3(sLine, ref mapName, TextSplitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref cX, TextSplitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref cY, TextSplitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref allSay, TextSplitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref range, TextSplitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref type, TextSplitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref zone, TextSplitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref fire, TextSplitConst);
                        if (!string.IsNullOrEmpty(mapName) && !string.IsNullOrEmpty(cX) && !string.IsNullOrEmpty(cY))
                        {
                            StartPoint startPoint = new StartPoint
                            {
                                MapName = mapName,
                                CurrX = HUtil32.StrToInt16(cX, 0),
                                CurrY = HUtil32.StrToInt16(cY, 0),
                                NotAllowSay = Convert.ToBoolean(HUtil32.StrToInt(allSay, 0)),
                                Range = HUtil32.StrToInt(range, 0),
                                Type = HUtil32.StrToInt(type, 0),
                                PkZone = HUtil32.StrToInt(zone, 0),
                                PkFire = HUtil32.StrToInt(fire, 0)
                            };
                            M2Share.StartPointList.Add(startPoint);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 读取解包物品配置UnbindList.txt
        /// </summary>
        /// <returns></returns>
        public int LoadUnbindList()
        {
            int result = 0;
            string sData = string.Empty;
            string sItemName = string.Empty;
            string sFileName = M2Share.GetEnvirFilePath("UnbindList.txt");
            if (File.Exists(sFileName))
            {
                using StringList loadList = new StringList();
                loadList.LoadFromFile(sFileName);
                for (int i = 0; i < loadList.Count; i++)
                {
                    string readLine = loadList[i];
                    if (!string.IsNullOrEmpty(readLine) && readLine[0] != ';')
                    {
                        readLine = HUtil32.GetValidStr3(readLine, ref sData, TextSplitConst);
                        readLine = HUtil32.GetValidStrCap(readLine, ref sItemName, TextSplitConst);
                        if (!string.IsNullOrEmpty(sItemName) && sItemName[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(sItemName, "\"", "\"", ref sItemName);
                        }
                        int n10 = HUtil32.StrToInt(sData, 0);
                        if (n10 > 0)
                        {
                            if (!M2Share.UnbindList.TryAdd(n10, sItemName))
                            {
                                LogService.Warn($"重复解包物品[{sItemName}]...");
                                continue;
                            }
                        }
                        else
                        {
                            result = -i;// 需要取负数
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 保存商品记录Envir\Market_Saved\*.sav
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="sFile"></param>
        /// <returns></returns>
        public static int SaveGoodRecord(Merchant npc, string sFile)
        {
            int result = -1;
            string sFileName = ".\\Envir\\Market_Saved\\" + sFile + ".sav";
            //if (File.Exists(sFileName))
            //{
            //    FileHandle = File.Open(sFileName, (FileMode) FileAccess.Write | FileShare.ReadWrite);
            //}
            //else
            //{
            //    FileHandle = File.Create(sFileName);
            //}
            //if (FileHandle > 0)
            //{
            //    
            //    FillChar(Header420, sizeof(TGoodFileHeader), '\0');
            //    for (I = 0; I < NPC.m_GoodsList.Count; I ++ )
            //    {
            //        List = ((NPC.m_GoodsList[I]) as ArrayList);
            //        Header420.nItemCount += List.Count;
            //    }
            //    
            //    FileWrite(FileHandle, Header420, sizeof(TGoodFileHeader));
            //    for (I = 0; I < NPC.m_GoodsList.Count; I ++ )
            //    {
            //        List = ((NPC.m_GoodsList[I]) as ArrayList);
            //        for (II = 0; II < List.Count; II ++ )
            //        {
            //            UserItem = List[II];
            //            
            //            FileWrite(FileHandle, UserItem, sizeof(TUserItem));
            //        }
            //    }
            //    result = 1;
            //}
            return result;
        }

        /// <summary>
        /// 保存商品价格记录Envir\Market_Prices\*.prc
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="sFile"></param>
        /// <returns></returns>
        public static int SaveGoodPriceRecord(Merchant npc, string sFile)
        {
            int result = -1;
            string sFileName = ".\\Envir\\Market_Prices\\" + sFile + ".prc";
            //if (File.Exists(sFileName))
            //{
            //    FileHandle = File.Open(sFileName, (FileMode) FileAccess.Write | FileShare.ReadWrite);
            //}
            //else
            //{
            //    FileHandle = File.Create(sFileName);
            //}
            //if (FileHandle > 0)
            //{
            //    
            //    FillChar(Header420, sizeof(TGoodFileHeader), '\0');
            //    Header420.nItemCount = NPC.m_ItemPriceList.Count;
            //    
            //    FileWrite(FileHandle, Header420, sizeof(TGoodFileHeader));
            //    for (I = 0; I < NPC.m_ItemPriceList.Count; I ++ )
            //    {
            //        ItemPrice = NPC.m_ItemPriceList[I];
            //        
            //        FileWrite(FileHandle, ItemPrice, sizeof(TItemPrice));
            //    }
            //    result = 1;
            //}
            return result;
        }

        public static void ReLoadNpc()
        {

        }

        /// <summary>
        /// 重新加载Merchant.txt
        /// </summary>
        public void ReLoadMerchants()
        {
            string sScript = string.Empty;
            string sMapName = string.Empty;
            string sX = string.Empty;
            string sY = string.Empty;
            string sChrName = string.Empty;
            string sFlag = string.Empty;
            string sAppr = string.Empty;
            string sCastle = string.Empty;
            string sCanMove = string.Empty;
            string sMoveTime = string.Empty;
            IMerchant merchant;
            string sFileName = M2Share.GetEnvirFilePath("Merchant.txt");
            if (!File.Exists(sFileName))
            {
                return;
            }
            for (int i = 0; i < SystemShare.WorldEngine.MerchantList.Count; i++)
            {
                merchant = SystemShare.WorldEngine.MerchantList[i];
                if (merchant != SystemShare.FunctionNPC)
                {
                    merchant.NpcFlag = -1;
                }
            }
            using StringList loadList = new StringList();
            loadList.LoadFromFile(sFileName);
            for (int i = 0; i < loadList.Count; i++)
            {
                string sLineText = loadList[i].Trim();
                if (!string.IsNullOrEmpty(sLineText) && sLineText[0] != ';')
                {
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sScript, TextSplitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sMapName, TextSplitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sX, TextSplitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sY, TextSplitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sChrName, TextSplitConst);
                    if (!string.IsNullOrEmpty(sChrName) && sChrName[0] == '\"')
                    {
                        HUtil32.ArrestStringEx(sChrName, "\"", "\"", ref sChrName);
                    }
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sFlag, TextSplitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sAppr, TextSplitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sCastle, TextSplitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sCanMove, TextSplitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sMoveTime, TextSplitConst);
                    short nX = (short)HUtil32.StrToInt(sX, 0);
                    short nY = (short)HUtil32.StrToInt(sY, 0);
                    bool boNewNpc = true;
                    for (int j = 0; j < SystemShare.WorldEngine.MerchantList.Count; j++)
                    {
                        merchant = SystemShare.WorldEngine.MerchantList[j];
                        if (merchant.MapName == sMapName && merchant.CurrX == nX && merchant.CurrY == nY)
                        {
                            boNewNpc = false;
                            merchant.ScriptName = sScript;
                            merchant.ChrName = sChrName;
                            merchant.NpcFlag = HUtil32.StrToInt16(sFlag, 0);
                            merchant.Appr = (ushort)HUtil32.StrToInt(sAppr, 0);
                            merchant.MoveTime = HUtil32.StrToInt(sMoveTime, 0);
                            if (HUtil32.StrToInt(sCastle, 0) != 1)
                            {
                                merchant.CastleMerchant = true;
                            }
                            else
                            {
                                merchant.CastleMerchant = false;
                            }
                            if (HUtil32.StrToInt(sCanMove, 0) != 0 && merchant.MoveTime > 0)
                            {
                                merchant.BoCanMove = true;
                            }
                            break;
                        }
                    }
                    if (boNewNpc)
                    {
                        merchant = new Merchant
                        {
                            MapName = sMapName
                        };
                        merchant.Envir = SystemShare.MapMgr.FindMap(merchant.MapName);
                        if (merchant.Envir != null)
                        {
                            merchant.ScriptName = sScript;
                            merchant.CurrX = nX;
                            merchant.CurrY = nY;
                            merchant.ChrName = sChrName;
                            merchant.NpcFlag = HUtil32.StrToInt16(sFlag, 0);
                            merchant.Appr = (ushort)HUtil32.StrToInt(sAppr, 0);
                            merchant.MoveTime = HUtil32.StrToInt(sMoveTime, 0);
                            if (HUtil32.StrToInt(sCastle, 0) != 1)
                            {
                                merchant.CastleMerchant = true;
                            }
                            else
                            {
                                merchant.CastleMerchant = false;
                            }
                            if (HUtil32.StrToInt(sCanMove, 0) != 0 && merchant.MoveTime > 0)
                            {
                                merchant.BoCanMove = true;
                            }
                            SystemShare.WorldEngine.MerchantList.Add(merchant);
                            merchant.Initialize();
                        }
                    }
                }
            }
            for (int i = SystemShare.WorldEngine.MerchantList.Count - 1; i >= 0; i--)
            {
                merchant = SystemShare.WorldEngine.MerchantList[i];
                if (merchant.NpcFlag == -1)
                {
                    merchant.Ghost = true;
                    merchant.GhostTick = HUtil32.GetTickCount();
                    SystemShare.WorldEngine.MerchantList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 加载商品记录Envir\Market_Saved\*.sav
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="sFile"></param>
        /// <returns></returns>
        public static int LoadGoodRecord(Merchant npc, string sFile)
        {
            int result = -1;
            string sFileName = ".\\Envir\\Market_Saved\\" + sFile + ".sav";
            //if (File.Exists(sFileName))
            //{
            //    FileHandle = File.Open(sFileName, (FileMode) FileAccess.Read | FileShare.ReadWrite);
            //    List = null;
            //    if (FileHandle > 0)
            //    {
            //        
            //        if (FileRead(FileHandle, Header420, sizeof(TGoodFileHeader)) == sizeof(TGoodFileHeader))
            //        {
            //            for (I = 0; I < Header420.nItemCount; I ++ )
            //            {
            //                UserItem = new TUserItem();
            //                
            //                if (FileRead(FileHandle, UserItem, sizeof(TUserItem)) == sizeof(TUserItem))
            //                {
            //                    if (List == null)
            //                    {
            //                        List = new ArrayList();
            //                        List.Add(UserItem);
            //                    }
            //                    else
            //                    {
            //                        if (((TUserItem)(List[0])).wIndex == UserItem.Index)
            //                        {
            //                            List.Add(UserItem);
            //                        }
            //                        else
            //                        {
            //                            NPC.m_GoodsList.Add(List);
            //                            List = new ArrayList();
            //                            List.Add(UserItem);
            //                        }
            //                    }
            //                }
            //            }
            //            if (List != null)
            //            {
            //                NPC.m_GoodsList.Add(List);
            //            }
            //            FileHandle.Close();
            //            result = 1;
            //        }
            //    }
            //}
            return result;
        }

        /// <summary>
        /// 加载商品价格记录Envir\Market_Prices\*.prc
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="sFile"></param>
        /// <returns></returns>
        public static int LoadGoodPriceRecord(Merchant npc, string sFile)
        {
            int result = -1;
            string sFileName = ".\\Envir\\Market_Prices\\" + sFile + ".prc";
            //if (File.Exists(sFileName))
            //{
            //    FileHandle = File.Open(sFileName, (FileMode)FileAccess.Read | FileShare.ReadWrite);
            //    if (FileHandle > 0)
            //    {
            //        @ Unsupported function or procedure: 'FileRead'
            //        if (FileRead(FileHandle, Header420, sizeof(TGoodFileHeader)) == sizeof(TGoodFileHeader))
            //        {
            //            for (I = 0; I < Header420.nItemCount; I++)
            //            {
            //                ItemPrice = new TItemPrice();
            //                @ Unsupported function or procedure: 'FileRead'
            //                if (FileRead(FileHandle, ItemPrice, sizeof(TItemPrice)) == sizeof(TItemPrice))
            //                {
            //                    NPC.m_ItemPriceList.Add(ItemPrice);
            //                }
            //                else
            //                {
            //                    @ Unsupported function or procedure: 'Dispose'
            //                    Dispose(ItemPrice);
            //                    break;
            //                }
            //            }
            //        }
            //        FileHandle.Close();
            //        result = 1;
            //    }
            //}
            return result;
        }
    }
}