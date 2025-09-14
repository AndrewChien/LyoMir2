﻿using M2Server.Player;
using OpenMir2.Enums;
using SystemModule.Actors;

namespace GameSrv.Robots
{
    public class RobotObject : PlayObject, IRobotObject
    {
        private readonly char[] LoadSriptSpitConst = new[] { ' ', '/', '\t' };
        private readonly IList<AutoRunInfo> _autoRunList;
        public string ScriptFileName = string.Empty;

        public RobotObject()
        {
            _autoRunList = new List<AutoRunInfo>(20);
            this.SuperMan = true;
            this.Race = ActorRace.RoBot;
        }

        ~RobotObject()
        {
            ClearScript();
        }

        public override void Initialize()
        {
            if (SystemShare.ManageNPC != null)
            {
                SystemShare.ManageNPC.GotoLable(this, "@Startup", false);
            }
        }

        private void AutoRun(AutoRunInfo autoRunInfo)
        {
            long currentTime = HUtil32.GetTimestamp();
            if (currentTime >= autoRunInfo.RunTimeTick)
            {
                switch (autoRunInfo.Moethod)
                {
                    case RobotConst.nRODAY:
                        autoRunInfo.RunTimeTick = DateTimeOffset.Now.AddDays(1).ToUnixTimeMilliseconds();
                        autoRunInfo.RunTick = HUtil32.GetTimestamp();
                        SystemShare.RobotNPC.GotoLable(this, autoRunInfo.sParam2);
                        break;
                    case RobotConst.nROHOUR:
                        autoRunInfo.RunTimeTick = DateTimeOffset.Now.AddHours(1).ToUnixTimeMilliseconds();
                        autoRunInfo.RunTick = HUtil32.GetTimestamp();
                        SystemShare.RobotNPC.GotoLable(this, autoRunInfo.sParam2);
                        break;
                    case RobotConst.nROMIN:
                        autoRunInfo.RunTimeTick = DateTimeOffset.Now.AddMinutes(1).ToUnixTimeMilliseconds();
                        autoRunInfo.RunTick = HUtil32.GetTimestamp();
                        SystemShare.RobotNPC.GotoLable(this, autoRunInfo.sParam2);
                        break;
                    case RobotConst.nROSEC:
                        autoRunInfo.RunTimeTick = DateTimeOffset.Now.AddSeconds(1).ToUnixTimeMilliseconds();
                        autoRunInfo.RunTick = HUtil32.GetTimestamp();
                        SystemShare.RobotNPC.GotoLable(this, autoRunInfo.sParam2);
                        break;
                    case RobotConst.nRUNONWEEK:
                        GetWeekTime(autoRunInfo.sParam1, ref autoRunInfo.RunTimeTick);
                        autoRunInfo.RunTick = HUtil32.GetTimestamp();
                        AutoRunOfOnWeek(autoRunInfo);
                        break;
                    case RobotConst.nRUNONDAY:
                        GetDayTime(autoRunInfo.sParam1, ref autoRunInfo.RunTimeTick);
                        autoRunInfo.RunTick = HUtil32.GetTimestamp();
                        AutoRunOfOnDay(autoRunInfo);
                        break;
                    case RobotConst.nRUNONHOUR:
                        GetHourTime(autoRunInfo.sParam1, ref autoRunInfo.RunTimeTick);
                        autoRunInfo.RunTick = HUtil32.GetTimestamp();
                        AutoRunOfOnHour(autoRunInfo);
                        break;
                    case RobotConst.nRUNONMIN:
                        GetMinuteTime(autoRunInfo.sParam1, ref autoRunInfo.RunTimeTick);
                        autoRunInfo.RunTick = HUtil32.GetTimestamp();
                        AutoRunOfOnMin(autoRunInfo);
                        break;
                    case RobotConst.nRUNONSEC:
                        GetSecondTime(autoRunInfo.sParam1, ref autoRunInfo.RunTimeTick);
                        autoRunInfo.RunTick = HUtil32.GetTimestamp();
                        AutoRunOfOnSec(autoRunInfo);
                        break;
                }
            }
        }

        private void AutoRunOfOnDay(AutoRunInfo autoRunInfo)
        {
            string sMin = string.Empty;
            string sHour = string.Empty;
            string sLineText = autoRunInfo.sParam1;
            sLineText = HUtil32.GetValidStr3(sLineText, ref sHour, ':');
            sLineText = HUtil32.GetValidStr3(sLineText, ref sMin, ':');
            int nHour = HUtil32.StrToInt(sHour, -1);
            int nMin = HUtil32.StrToInt(sMin, -1);
            int wHour = DateTime.Now.Hour;
            int wMin = DateTime.Now.Minute;
            if (nHour >= 0 && nHour <= 24 && nMin >= 0 && nMin <= 60)
            {
                if (wHour == nHour)
                {
                    if (wMin == nMin)
                    {
                        if (autoRunInfo.Status)
                        {
                            return;
                        }

                        SystemShare.RobotNPC.GotoLable(this, autoRunInfo.sParam2);
                        autoRunInfo.Status = true;
                    }
                    else
                    {
                        autoRunInfo.Status = false;
                    }
                }
            }
        }

        private static void AutoRunOfOnHour(AutoRunInfo autoRunInfo)
        {
        }

        private void AutoRunOfOnMin(AutoRunInfo autoRunInfo)
        {
            string sMin = string.Empty;
            string sLineText = autoRunInfo.sParam1;
            sLineText = HUtil32.GetValidStr3(sLineText, ref sMin, ':');
            int nMin = HUtil32.StrToInt(sMin, -1);
            if (nMin >= 0 && nMin <= 60)
            {
                int wMin = DateTime.Now.Minute;
                if (wMin == nMin)
                {
                    if (autoRunInfo.Status)
                    {
                        return;
                    }

                    SystemShare.RobotNPC.GotoLable(this, autoRunInfo.sParam2);
                    autoRunInfo.Status = true;
                }
                else
                {
                    autoRunInfo.Status = false;
                }
            }
        }

        private static void AutoRunOfOnSec(AutoRunInfo autoRunInfo)
        {
        }

        private void AutoRunOfOnWeek(AutoRunInfo autoRunInfo)
        {
            string sMin = string.Empty;
            string sHour = string.Empty;
            string sWeek = string.Empty;
            string sLineText = autoRunInfo.sParam1;
            sLineText = HUtil32.GetValidStr3(sLineText, ref sWeek, ':');
            sLineText = HUtil32.GetValidStr3(sLineText, ref sHour, ':');
            sLineText = HUtil32.GetValidStr3(sLineText, ref sMin, ':');
            int nWeek = HUtil32.StrToInt(sWeek, -1);
            int nHour = HUtil32.StrToInt(sHour, -1);
            int nMin = HUtil32.StrToInt(sMin, -1);
            if (nWeek >= 1 && nWeek <= 7 && nHour >= 0 && nHour <= 24 && nMin >= 0 && nMin <= 60)
            {
                int wHour = DateTime.Now.Hour;
                int wMin = DateTime.Now.Minute;
                DayOfWeek wWeek = DateTime.Now.DayOfWeek;
                if ((int)wWeek == nWeek && wHour == nHour)
                {
                    if (wMin == nMin)
                    {
                        if (autoRunInfo.Status)
                        {
                            return;
                        }

                        SystemShare.RobotNPC.GotoLable(this, autoRunInfo.sParam2);
                        autoRunInfo.Status = true;
                    }
                    else
                    {
                        autoRunInfo.Status = false;
                    }
                }
            }
        }

        private void ClearScript()
        {
            for (int i = 0; i < _autoRunList.Count; i++)
            {
                _autoRunList[i] = null;
            }
            _autoRunList.Clear();
        }

        public void LoadScript()
        {
            string sActionType = string.Empty;
            string sRunCmd = string.Empty;
            string sMoethod = string.Empty;
            string sParam1 = string.Empty;
            string sParam2 = string.Empty;
            string sParam3 = string.Empty;
            string sParam4 = string.Empty;
            string sFileName = M2Share.GetEnvirFilePath("Robot_def", $"{ScriptFileName}.txt");
            if (File.Exists(sFileName))
            {
                using StringList loadList = new StringList();
                loadList.LoadFromFile(sFileName);
                for (int i = 0; i < loadList.Count; i++)
                {
                    string sLineText = loadList[i];
                    if (!string.IsNullOrEmpty(sLineText) && sLineText[0] != ';')
                    {
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sActionType, LoadSriptSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sRunCmd, LoadSriptSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sMoethod, LoadSriptSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sParam1, LoadSriptSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sParam2, LoadSriptSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sParam3, LoadSriptSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sParam4, LoadSriptSpitConst);
                        if (string.Compare(sActionType, RobotConst.sROAUTORUN, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (string.Compare(sRunCmd, RobotConst.sRONPCLABLEJMP, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                AutoRunInfo autoRunInfo = new AutoRunInfo();
                                autoRunInfo.RunTick = HUtil32.GetTimestamp();
                                autoRunInfo.RunTimeTick = 0;
                                autoRunInfo.Status = false;
                                if (string.Compare(sMoethod, RobotConst.sRODAY, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    autoRunInfo.Moethod = RobotConst.nRODAY;
                                }
                                if (string.Compare(sMoethod, RobotConst.sROHOUR, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    autoRunInfo.Moethod = RobotConst.nROHOUR;
                                }
                                if (string.Compare(sMoethod, RobotConst.sROMIN, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    autoRunInfo.Moethod = RobotConst.nROMIN;
                                }
                                if (string.Compare(sMoethod, RobotConst.sROSEC, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    autoRunInfo.Moethod = RobotConst.nROSEC;
                                }
                                if (string.Compare(sMoethod, RobotConst.sRUNONWEEK, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    autoRunInfo.Moethod = RobotConst.nRUNONWEEK;
                                    if (!GetWeekTime(sParam1, ref autoRunInfo.RunTimeTick))
                                    {
                                        OutErrorMessage(sActionType, sRunCmd, sParam1, sParam2, sParam3, sParam4);
                                    }
                                }
                                if (string.Compare(sMoethod, RobotConst.sRUNONDAY, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    autoRunInfo.Moethod = RobotConst.nRUNONDAY;
                                    if (!GetDayTime(sParam1, ref autoRunInfo.RunTimeTick))
                                    {
                                        OutErrorMessage(sActionType, sRunCmd, sParam1, sParam2, sParam3, sParam4);
                                    }
                                }
                                if (string.Compare(sMoethod, RobotConst.sRUNONHOUR, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    autoRunInfo.Moethod = RobotConst.nRUNONHOUR;
                                    if (!GetHourTime(sParam1, ref autoRunInfo.RunTimeTick))
                                    {
                                        OutErrorMessage(sActionType, sRunCmd, sParam1, sParam2, sParam3, sParam4);
                                    }
                                }
                                if (string.Compare(sMoethod, RobotConst.sRUNONMIN, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    autoRunInfo.Moethod = RobotConst.nRUNONMIN;
                                    if (!GetMinuteTime(sParam1, ref autoRunInfo.RunTimeTick))
                                    {
                                        OutErrorMessage(sActionType, sRunCmd, sParam1, sParam2, sParam3, sParam4);
                                    }
                                }
                                if (string.Compare(sMoethod, RobotConst.sRUNONSEC, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    autoRunInfo.Moethod = RobotConst.nRUNONSEC;
                                    if (!GetSecondTime(sParam1, ref autoRunInfo.RunTimeTick))
                                    {
                                        OutErrorMessage(sActionType, sRunCmd, sParam1, sParam2, sParam3, sParam4);
                                    }
                                }
                                autoRunInfo.sParam1 = sParam1;
                                autoRunInfo.sParam2 = sParam2;
                                autoRunInfo.sParam3 = sParam3;
                                autoRunInfo.sParam4 = sParam4;
                                autoRunInfo.nParam1 = HUtil32.StrToInt(sParam1, 1);
                                _autoRunList.Add(autoRunInfo);
                            }
                        }
                    }
                }
            }
        }

        private bool GetWeekTime(string param, ref long runTime)
        {
            if (!DateTimeOffset.TryParse(param, out DateTimeOffset runWeekTime))
            {
                return false;
            }

            runTime = GetSundayDate(runWeekTime).ToUnixTimeMilliseconds();
            return true;
        }

        private static bool GetDayTime(string param, ref long runTime)
        {
            if (!DateTimeOffset.TryParse(param, out DateTimeOffset runDayTime))
            {
                return false;
            }

            runTime = runDayTime.ToUnixTimeMilliseconds();
            return true;
        }

        private static bool GetHourTime(string param, ref long runTime)
        {
            if (!int.TryParse(param, out int runHour))
            {
                return false;
            }

            runTime = DateTimeOffset.Now.AddHours(runHour).ToUnixTimeMilliseconds();
            return true;
        }

        private static bool GetMinuteTime(string param, ref long runTime)
        {
            if (!int.TryParse(param, out int runHour))
            {
                return false;
            }

            runTime = DateTimeOffset.Now.AddMinutes(runHour).ToUnixTimeMilliseconds();
            return true;
        }

        private static bool GetSecondTime(string param, ref long runTime)
        {
            if (!int.TryParse(param, out int runHour))
            {
                return false;
            }

            runTime = DateTimeOffset.Now.AddSeconds(runHour).ToUnixTimeMilliseconds();
            return true;
        }

        private void OutErrorMessage(string sActionType, string sRunCmd, string sParam1, string sParam2, string sParam3, string sParam4)
        {
            LogService.Error($"机器人脚本错误 ActionType:{sActionType} RunCmd:{sRunCmd} Params1:{sParam1} Params2:{sParam2} Params3:{sParam3} Params4:{sParam4}");
        }

        /// <summary>
        /// 计算某日结束日期（礼拜日的日期）
        /// </summary>
        /// <param name="someDate">该周中任意一天</param>
        /// <returns>返回礼拜日日期，后面的具体时、分、秒和传入值相等</returns>
        private static DateTimeOffset GetSundayDate(DateTimeOffset someDate)
        {
            int i = (7 - (int)someDate.DayOfWeek);
            return someDate.Add(new TimeSpan(i, 0, 0, 0));
        }

        public void ReloadScript()
        {
            ClearScript();
            LoadScript();
        }

        public override void Run()
        {
            if (SystemShare.RobotNPC == null)
            {
                return;
            }
            for (int i = _autoRunList.Count - 1; i >= 0; i--)
            {
                AutoRun(_autoRunList[i]);
            }
        }
    }
}