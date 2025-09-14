﻿namespace GameSrv.Maps
{
    public class Map
    {
        private static Thread _makeStoneMinesThread;
        /// <summary>
        /// 启动初始化挖矿地图矿物数据
        /// </summary>
        public static void StartMakeStoneThread()
        {
            _makeStoneMinesThread = new Thread(MakeStoneMines)
            {
                IsBackground = true
            };
            _makeStoneMinesThread.Start();
        }
        /// <summary>
        /// 加载大地图MapInfo.txt
        /// </summary>
        /// <returns></returns>
        public static int LoadMapInfo(CosoleOutputMode outputMode)
        {
            LogService.Info("正在加载地图数据...");
            
            string sFlag = string.Empty;
            string sCommand = string.Empty;
            string sLine = string.Empty;
            string sReConnectMap = string.Empty;
            int result = -1;
            string sFileName = M2Share.GetEnvirFilePath("MapInfo.txt");
            if (File.Exists(sFileName))
            {
                using StringList loadList = new StringList();
                loadList.LoadFromFile(sFileName);//按行加载：D:\MirServer\Mir200\Envir\MapInfo.txt
                if (loadList.Count < 0)
                {
                    return result;
                }
                int count = 0;
                while (true)
                {
                    if (count >= loadList.Count)
                    {
                        break;
                    }
                    if (HUtil32.CompareLStr("ConnectMapInfo", loadList[count]))
                    {
                        string sMapInfoFile = HUtil32.GetValidStr3(loadList[count], ref sFlag, new[] { ' ', '\t' });
                        loadList.RemoveAt(count);
                        if (!string.IsNullOrEmpty(sMapInfoFile))
                        {
                            LoadSubMapInfo(loadList, sMapInfoFile);
                        }
                    }
                    count++;
                }
                result = 1;
                // 加载地图设置
                if (outputMode == CosoleOutputMode.ControlMode)
                {
                    Console.WriteLine("\r正在读取地图数据...");
                }
                string sMapName;
                for (int i = 0; i < loadList.Count; i++)
                {
                    sFlag = loadList[i];
                    if (!string.IsNullOrEmpty(sFlag) && sFlag[0] == '[')
                    {
                        sMapName = string.Empty;
                        MapInfoFlag MapFlag = new MapInfoFlag
                        {
                            SafeArea = false
                        };
                        sFlag = HUtil32.ArrestStringEx(sFlag, "[", "]", ref sMapName);
                        string sMapDesc = HUtil32.GetValidStrCap(sMapName, ref sMapName, HUtil32.Separator);
                        if (!string.IsNullOrEmpty(sMapDesc) && sMapDesc[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(sMapDesc, "\"", "\"", ref sMapDesc);
                        }
                        string s4C = HUtil32.GetValidStr3(sMapDesc, ref sMapDesc, HUtil32.Separator).Trim();
                        byte nServerIndex = (byte)HUtil32.StrToInt(s4C, 0);
                        if (string.IsNullOrEmpty(sMapName))
                        {
                            continue;
                        }
                        MapFlag.RequestLevel = 1;
                        MapFlag.SafeArea = false;
                        MapFlag.NeedSetonFlag = -1;
                        MapFlag.NeedOnOff = -1;
                        MapFlag.MusicId = -1;
                        Merchant QuestNPC = null;
                        while (true)
                        {
                            if (string.IsNullOrEmpty(sFlag))
                            {
                                break;
                            }
                            sFlag = HUtil32.GetValidStr3(sFlag, ref sCommand, HUtil32.Separator);
                            if (string.IsNullOrEmpty(sCommand))
                            {
                                break;
                            }
                            if (sCommand.Equals("SAFE", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.SafeArea = true;
                                continue;
                            }
                            if (string.Compare(sCommand, "DARK", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                MapFlag.boDarkness = true;
                                continue;
                            }
                            if (string.Compare(sCommand, "FIGHT", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                MapFlag.FightZone = true;
                                continue;
                            }
                            if (string.Compare(sCommand, "FIGHT3", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                MapFlag.Fight3Zone = true;
                                continue;
                            }
                            if (string.Compare(sCommand, "DAY", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                MapFlag.DayLight = true;
                                continue;
                            }
                            if (string.Compare(sCommand, "QUIZ", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                MapFlag.boQUIZ = true;
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "NORECONNECT"))
                            {
                                MapFlag.boNORECONNECT = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sReConnectMap);
                                MapFlag.sNoReConnectMap = sReConnectMap;
                                if (string.IsNullOrEmpty(MapFlag.sNoReConnectMap))
                                {
                                }
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "CHECKQUEST"))
                            {
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                QuestNPC = LoadMapQuest(sLine);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "NEEDSET_ON"))
                            {
                                MapFlag.NeedOnOff = 1;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.NeedSetonFlag = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "NEEDSET_OFF"))
                            {
                                MapFlag.NeedOnOff = 0;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.NeedSetonFlag = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "MUSIC"))
                            {
                                MapFlag.Music = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.MusicId = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "EXPRATE"))
                            {
                                MapFlag.boEXPRATE = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.ExpRate = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "PKWINLEVEL"))
                            {
                                MapFlag.boPKWINLEVEL = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.nPKWINLEVEL = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "PKWINEXP"))
                            {
                                MapFlag.boPKWINEXP = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.nPKWINEXP = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "PKLOSTLEVEL"))
                            {
                                MapFlag.boPKLOSTLEVEL = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.nPKLOSTLEVEL = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "PKLOSTEXP"))
                            {
                                MapFlag.boPKLOSTEXP = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.nPKLOSTEXP = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "DECHP"))
                            {
                                MapFlag.boDECHP = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.nDECHPPOINT = HUtil32.StrToInt(HUtil32.GetValidStr3(sLine, ref sLine, HUtil32.Backslash), -1);
                                MapFlag.nDECHPTIME = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "INCHP"))
                            {
                                MapFlag.boINCHP = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.nINCHPPOINT = HUtil32.StrToInt(HUtil32.GetValidStr3(sLine, ref sLine, HUtil32.Backslash), -1);
                                MapFlag.nINCHPTIME = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "DECGAMEGOLD"))
                            {
                                MapFlag.boDECGAMEGOLD = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.nDECGAMEGOLD = HUtil32.StrToInt(HUtil32.GetValidStr3(sLine, ref sLine, HUtil32.Backslash), -1);
                                MapFlag.nDECGAMEGOLDTIME = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "DECGAMEPOINT"))
                            {
                                MapFlag.boDECGAMEPOINT = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.nDECGAMEPOINT = HUtil32.StrToInt(HUtil32.GetValidStr3(sLine, ref sLine, HUtil32.Backslash), -1);
                                MapFlag.nDECGAMEPOINTTIME = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "INCGAMEGOLD"))
                            {
                                MapFlag.boINCGAMEGOLD = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.nINCGAMEGOLD = HUtil32.StrToInt(HUtil32.GetValidStr3(sLine, ref sLine, HUtil32.Backslash), -1);
                                MapFlag.nINCGAMEGOLDTIME = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "INCGAMEPOINT"))
                            {
                                MapFlag.boINCGAMEPOINT = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.nINCGAMEPOINT = HUtil32.StrToInt(HUtil32.GetValidStr3(sLine, ref sLine, HUtil32.Backslash), -1);
                                MapFlag.nINCGAMEPOINTTIME = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (sCommand.Equals("RUNHUMAN", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.RunHuman = true;
                                continue;
                            }
                            if (sCommand.Equals("RUNMON", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.RunMon = true;
                                continue;
                            }
                            if (sCommand.Equals("NEEDHOLE", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.boNEEDHOLE = true;
                                continue;
                            }
                            if (sCommand.Equals("NORECALL", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.NoReCall = true;
                                continue;
                            }
                            if (sCommand.Equals("NOGUILDRECALL", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.NoGuildReCall = true;
                                continue;
                            }
                            if (sCommand.Equals("NODEARRECALL", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.boNODEARRECALL = true;
                                continue;
                            }
                            if (sCommand.Equals("NOMASTERRECALL", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.MasterReCall = true;
                                continue;
                            }
                            if (sCommand.Equals("NORANDOMMOVE", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.boNORANDOMMOVE = true;
                                continue;
                            }
                            if (sCommand.Equals("NODRUG", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.boNODRUG = true;
                                continue;
                            }
                            if (sCommand.Equals("MINE", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.Mine = true;
                                continue;
                            }
                            if (sCommand.Equals("MINE2", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.boMINE2 = true;
                                continue;
                            }
                            if (sCommand.Equals("NOTHROWITEM", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.NoThrowItem = true;
                                continue;
                            }
                            if (sCommand.Equals("NODROPITEM", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.NoDropItem = true;
                                continue;
                            }
                            if (sCommand.Equals("NOPOSITIONMOVE", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.boNOPOSITIONMOVE = true;
                                continue;
                            }
                            if (sCommand.Equals("NOHORSE", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.boNOPOSITIONMOVE = true;
                                continue;
                            }
                            if (sCommand.Equals("NOCHAT", StringComparison.OrdinalIgnoreCase))
                            {
                                MapFlag.boNOCHAT = true;
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "KILLFUNC"))
                            {
                                MapFlag.boKILLFUNC = true;
                                HUtil32.ArrestStringEx(sCommand, "(", ")", ref sLine);
                                MapFlag.nKILLFUNCNO = HUtil32.StrToInt(sLine, -1);
                                continue;
                            }
                            if (HUtil32.CompareLStr(sCommand, "NOHUMNOMON"))
                            {
                                // 有人才开始刷怪
                                MapFlag.boNOHUMNOMON = true;
                                continue;
                            }
                            if (sCommand[0] == 'L')
                            {
                                MapFlag.RequestLevel = HUtil32.StrToInt(sCommand[1..], 1);
                            }
                        }
                        SystemShare.MapMgr.AddMapInfo(sMapName, sMapDesc, nServerIndex, MapFlag, QuestNPC);
                        result = 1;
                    }
                    
                    if(outputMode != CosoleOutputMode.ControlMode)
                    {
                        Console.Write("\r正在读取地图数据...[{0}/{1}]", i, loadList.Count);
                    }
                }
                if (outputMode == CosoleOutputMode.ControlMode)
                {
                    Console.WriteLine("地图加载成功[{0}]...", loadList.Count);
                }
                else
                {
                    Console.WriteLine();
                }
                //Console.Write("\n"); // 完成后换行
                //Console.SetCursorPosition(0, Console.CursorTop - 1); // 将光标移动到上一行
                //Console.Write(new string(' ', Console.WindowWidth)); // 使用空格覆盖这一行
                //Console.SetCursorPosition(0, Console.CursorTop - 1); // 再次将光标移动到上一行

                // 加载地图连接点
                for (int i = 0; i < loadList.Count; i++)
                {
                    sFlag = loadList[i];
                    if (!string.IsNullOrEmpty(sFlag) && sFlag[0] != '[' && sFlag[0] != ';')
                    {
                        sFlag = HUtil32.GetValidStr3(sFlag, ref sCommand, HUtil32.Separator);
                        sMapName = sCommand;
                        sFlag = HUtil32.GetValidStr3(sFlag, ref sCommand, HUtil32.Separator);
                        int nX = HUtil32.StrToInt(sCommand, 0);
                        sFlag = HUtil32.GetValidStr3(sFlag, ref sCommand, HUtil32.Separator);
                        int n18 = HUtil32.StrToInt(sCommand, 0);
                        sFlag = HUtil32.GetValidStr3(sFlag, ref sCommand, new[] { ' ', ',', '-', '>', '\t' });
                        string s44 = sCommand;
                        sFlag = HUtil32.GetValidStr3(sFlag, ref sCommand, HUtil32.Separator);
                        int n1C = HUtil32.StrToInt(sCommand, 0);
                        sFlag = HUtil32.GetValidStr3(sFlag, ref sCommand, new[] { ' ', ',', ';', '\t' });
                        int n20 = HUtil32.StrToInt(sCommand, 0);
                        SystemShare.MapMgr.AddMapRoute(sMapName, nX, n18, s44, n1C, n20);
                    }
                }
            }
            LogService.Info($"地图数据加载成功...[{SystemShare.MapMgr.Maps.Count}]");
            return result;
        }
        /// <summary>
        /// 加载小地图MiniMap.txt
        /// </summary>
        /// <returns></returns>
        public static int LoadMinMap()
        {
            LogService.Info("正在加小地图数据文件...");
            string sMapNo = string.Empty;
            string sMapIdx = string.Empty;
            int result = 0;
            string sFileName = M2Share.GetEnvirFilePath("MiniMap.txt");
            if (File.Exists(sFileName))
            {
                M2Share.MiniMapList.Clear();
                StringList tMapList = new StringList();
                tMapList.LoadFromFile(sFileName);
                for (int i = 0; i < tMapList.Count; i++)
                {
                    string tStr = tMapList[i];
                    if (!string.IsNullOrEmpty(tStr) && tStr[0] != ';')
                    {
                        tStr = HUtil32.GetValidStr3(tStr, ref sMapNo, HUtil32.Separator);
                        tStr = HUtil32.GetValidStr3(tStr, ref sMapIdx, HUtil32.Separator);
                        short nIdx = (short)HUtil32.StrToInt(sMapIdx, 0);
                        if (nIdx > 0)
                        {
                            if (M2Share.MiniMapList.ContainsKey(sMapNo))
                            {
                                LogService.Error($"重复小地图配置信息[{sMapNo}]");
                                continue;
                            }
                            M2Share.MiniMapList.TryAdd(sMapNo, nIdx);
                        }
                    }
                }
            }
            LogService.Info("小地图数据加载成功...");
            return result;
        }

        /// <summary>
        /// 初始化挖矿地图矿物数据
        /// </summary>
        private static void MakeStoneMines()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            IList<SystemModule.Maps.IEnvirnoment> mineMapList = SystemShare.MapMgr.GetMineMaps();
            LogService.Info($"初始化地图矿物数据...[{mineMapList.Count}]");
            for (int i = 0; i < mineMapList.Count; i++)
            {
                SystemModule.Maps.IEnvirnoment mineEnvir = mineMapList[i];
                for (short nW = 0; nW < mineEnvir.Width; nW++)
                {
                    for (short nH = 0; nH < mineEnvir.Height; nH++)
                    {
                        StoneMineEvent mine = new StoneMineEvent(mineEnvir, nW, nH, Grobal2.ET_MINE);
                        if (!mine.AddToMap)
                        {
                            M2Share.CellObjectMgr.Remove(mine.Id);
                            mine.Dispose();
                        }
                    }
                }
            }
            sw.Stop();
            LogService.Info($"地图矿物数据初始化完成. 耗时:{sw.Elapsed}");
        }
        /// <summary>
        /// 加载MapQuest_def文件夹下NPC数据
        /// （TODO：已改为：在地图0坐标(0,0)处加载一个新的NPC）
        /// 这个有问题！
        /// </summary>
        /// <param name="sName"></param>
        /// <returns></returns>
        private static Merchant LoadMapQuest(string sName)
        {
            Merchant questNPC = new Merchant
            {
                MapName = "0",
                CurrX = 0,
                CurrY = 0,
                ChrName = sName,
                NpcFlag = 0,
                Appr = 0,
                FilePath = "MapQuest_def",
                IsHide = true,
                IsQuest = false
            };
            SystemShare.WorldEngine.AddQuestNpc(questNPC);
            return questNPC;
        }
        /// <summary>
        /// 加载MapInfo下所有数据
        /// </summary>
        /// <param name="loadList"></param>
        /// <param name="sFileName"></param>
        private static void LoadSubMapInfo(StringList loadList, string sFileName)
        {
            string fileDir = M2Share.GetEnvirFilePath("MapInfo");
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            string sFilePatchName = fileDir + sFileName;
            if (File.Exists(sFilePatchName))
            {
                StringList loadMapList = new StringList();
                loadMapList.LoadFromFile(sFilePatchName);
                for (int i = 0; i < loadMapList.Count; i++)
                {
                    loadList.Add(loadMapList[i]);
                }
            }
        }
    }
}