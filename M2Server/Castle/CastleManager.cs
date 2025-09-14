﻿using OpenMir2;
using OpenMir2.Common;
using SystemModule;
using SystemModule.Actors;
using SystemModule.Castles;
using SystemModule.Maps;
using SystemModule.SubSystem;

namespace M2Server.Castle
{
    public class CastleManager : ICastleSystem
    {

        public readonly IList<IUserCastle> CastleList;

        public CastleManager()
        {
            CastleList = new List<IUserCastle>();
        }

        public IUserCastle Find(string sCastleName)
        {
            for (int i = 0; i < CastleList.Count; i++)
            {
                if (string.Compare(CastleList[i].sName, sCastleName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return CastleList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 是否沙巴克攻城战役区域
        /// </summary>
        /// <param name="BaseObject"></param>
        /// <returns></returns>
        public IUserCastle InCastleWarArea(IActor BaseObject)
        {
            for (int i = 0; i < CastleList.Count; i++)
            {
                if (CastleList[i].InCastleWarArea(BaseObject.Envir, BaseObject.CurrX, BaseObject.CurrY))
                {
                    return CastleList[i];
                }
            }
            return null;
        }

        public IUserCastle InCastleWarArea(IEnvirnoment Envir, int nX, int nY)
        {
            for (int i = 0; i < CastleList.Count; i++)
            {
                if (CastleList[i].InCastleWarArea(Envir, nX, nY))
                {
                    return CastleList[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 初始化城堡
        /// </summary>
        public void Initialize()
        {
            LogService.Info("初始化城堡服务...");
            if (CastleList.Count <= 0)
            {
                IUserCastle castle = new UserCastle(SystemShare.Config.CastleDir);
                castle.Initialize();
                castle.ConfigDir = "0";
                castle.EnvirList.Add("0151");
                castle.EnvirList.Add("0152");
                castle.EnvirList.Add("0153");
                castle.EnvirList.Add("0154");
                castle.EnvirList.Add("0155");
                castle.EnvirList.Add("0156");
                CastleList.Add(castle);
                Save();
            }
            else
            {
                for (int i = 0; i < CastleList.Count; i++)
                {
                    CastleList[i].Initialize();
                }
            }
            LogService.Info("城堡城初始完成...");
        }

        // 城堡皇宫所在地图
        public IUserCastle IsCastlePalaceEnvir(IEnvirnoment Envir)
        {
            for (int i = 0; i < CastleList.Count; i++)
            {
                if (CastleList[i].PalaceEnvir == Envir)
                {
                    return CastleList[i];
                }
            }
            return null;
        }

        // 城堡所在地图
        public IUserCastle IsCastleEnvir(IEnvirnoment envir)
        {
            for (int i = 0; i < CastleList.Count; i++)
            {
                if (CastleList[i].CastleEnvir == envir)
                {
                    return CastleList[i];
                }
            }
            return null;
        }

        public IUserCastle IsCastleMember(IPlayerActor playObject)
        {
            for (int i = 0; i < CastleList.Count; i++)
            {
                if (CastleList[i].IsMember(playObject))
                {
                    return CastleList[i];
                }
            }
            return null;
        }

        public void Run()
        {
            for (int i = 0; i < CastleList.Count; i++)
            {
                CastleList[i].Run();
            }
        }

        public void GetCastleGoldInfo(IList<string> List)
        {
            for (int i = 0; i < CastleList.Count; i++)
            {
                IUserCastle castle = CastleList[i];
                //List.Add(string.Format(CommandHelp.GameCommandSbkGoldShowMsg, castle.sName, castle.TotalGold, castle.TodayIncome));
            }
        }

        public void Save()
        {
            SaveCastleList();
            for (int i = 0; i < CastleList.Count; i++)
            {
                IUserCastle castle = CastleList[i];
                castle.Save();
            }
        }
        /// <summary>
        /// 加载城堡列表
        /// </summary>
        public void LoadCastleList()
        {
            string castleFile = Path.Combine(M2Share.BasePath, SystemShare.Config.CastleFile);
            if (File.Exists(castleFile))
            {
                using StringList loadList = new StringList();
                loadList.LoadFromFile(castleFile);
                for (int i = 0; i < loadList.Count; i++)
                {
                    string sCastleDir = loadList[i].Trim();
                    if (!string.IsNullOrEmpty(sCastleDir))
                    {
                        UserCastle castle = new UserCastle(sCastleDir);
                        CastleList.Add(castle);
                    }
                }
                LogService.Info($"已读取 [{CastleList.Count}] 个城堡信息...");
            }
            else
            {
                LogService.Error("城堡列表文件未找到!!!");
            }
        }

        private void SaveCastleList()
        {
            string castleDirPath = Path.Combine(M2Share.BasePath, SystemShare.Config.CastleDir);
            if (!Directory.Exists(castleDirPath))
            {
                Directory.CreateDirectory(castleDirPath);
            }
            using StringList loadList = new StringList(CastleList.Count);
            for (int i = 0; i < CastleList.Count; i++)
            {
                loadList.Add(i.ToString());
            }
            string savePath = Path.Combine(M2Share.BasePath, SystemShare.Config.CastleFile);
            loadList.SaveToFile(savePath);
        }

        public IUserCastle GetCastle(int nIndex)
        {
            if (nIndex >= 0 && nIndex < CastleList.Count)
            {
                return CastleList[nIndex];
            }
            return null;
        }

        public void GetCastleNameList(IList<string> List)
        {
            for (int i = 0; i < CastleList.Count; i++)
            {
                List.Add(CastleList[i].sName);
            }
        }

        public void IncRateGold(int nGold)
        {
            for (int i = 0; i < CastleList.Count; i++)
            {
                CastleList[i].IncRateGold(nGold);
            }
        }
    }
}