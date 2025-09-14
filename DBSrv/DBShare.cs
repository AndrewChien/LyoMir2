using System.Net.Sockets;

namespace DBSrv
{
    public static class DBShare
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static StringList DenyChrNameList = null;
        public static readonly StringList ClearMakeIndex = new StringList();
        public static readonly GateRouteInfo[] RouteInfo = new GateRouteInfo[20];
        public static Dictionary<string, int> MapList;
        private static HashSet<string> ServerIpList = null;
        private static Dictionary<string, short> _gateIdList = null;
        public static readonly string GateConfFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "!serverinfo.txt");
        private static readonly string ServerIpConfFileNmae = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "!addrtable.txt");
        private static readonly string GateIdConfFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SelectID.txt");

        public static int ServerIpCount => ServerIpList.Count;

        public static int GetMapIndex(string sMap)
        {
            if (string.IsNullOrEmpty(sMap))
            {
                return 0;
            }
            return MapList.TryGetValue(sMap, out int value) ? value : 0;
        }

        /// <summary>
        /// 检查是否禁止名称
        /// </summary>
        /// <returns></returns>
        public static bool CheckDenyChrName(string sChrName)
        {
            bool result = true;
            for (int i = 0; i < DenyChrNameList.Count; i++)
            {
                if (string.Compare(sChrName, DenyChrNameList[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        private static void LoadGateID()
        {
            string sID = string.Empty;
            string sIPaddr = string.Empty;
            _gateIdList.Clear();
            if (File.Exists(GateIdConfFileName))
            {
                StringList LoadList = new StringList();
                LoadList.LoadFromFile(GateIdConfFileName);
                for (int i = 0; i < LoadList.Count; i++)
                {
                    string sLineText = LoadList[i];
                    if ((string.IsNullOrEmpty(sLineText)) || (sLineText[0] == ';'))
                    {
                        continue;
                    }
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sID, new[] { " ", "\t" });
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sIPaddr, new[] { " ", "\t" });
                    int nID = HUtil32.StrToInt(sID, -1);
                    if (nID < 0)
                    {
                        continue;
                    }
                    _gateIdList.Add(sIPaddr, (short)nID);
                }
                LoadList = null;
            }
        }

        public static short GetGateID(string sIPaddr)
        {
            short result = 0;
            if (_gateIdList.ContainsKey(sIPaddr))
            {
                return _gateIdList[sIPaddr];
            }
            return result;
        }

        private static void LoadIPTable()
        {
            ServerIpList.Clear();
            try
            {
                StringList stringList = new StringList();
                stringList.LoadFromFile(ServerIpConfFileNmae);
                for (int i = 0; i < stringList.Count; i++)
                {
                    if (ServerIpList.Contains(stringList[i]))
                    {
                        continue;
                    }
                    ServerIpList.Add(stringList[i]);
                }
                stringList = null;
            }
            catch
            {
                LogService.Error("加载IP列表文件 " + ServerIpConfFileNmae + " 出错!!!");
            }
        }

        public static void LoadConfig()
        {
            LoadIPTable();
            LoadGateID();
        }

        public static bool CheckChrName(string sChrName)
        {
            try
            {
                char Chr;
                bool result = true;
                bool boIsTwoByte = false;
                char FirstChr = '\0';
                int bytesCount = 0;
                for (int i = 0; i < sChrName.Length; i++)
                {
                    int bytes = HUtil32.GetByteCount(sChrName[i]);
                    switch (bytes)
                    {
                        case 1:
                            bytesCount++;
                            break;
                        case 2:
                            bytesCount += 2;
                            if (i == sChrName.Length)
                            {
                                result = false;
                            }
                            break;
                    }
                }

                if (bytesCount != sChrName.Length)
                {
                    return false;
                }

                for (int i = 0; i < sChrName.Length; i++)
                {
                    Chr = sChrName[i];
                    if (boIsTwoByte)
                    {
                        if (!((FirstChr <= 0xF7) && (Chr >= 0x40) && (Chr <= 0xFE)))
                        {
                            if (!((FirstChr > 0xF7) && (Chr >= 0x40) && (Chr <= 0xA0)))
                            {
                                result = false;
                            }
                        }
                        boIsTwoByte = false;
                    }
                    else
                    {
                        if ((Chr >= 0x81) && (Chr <= 0xFE))
                        {
                            boIsTwoByte = true;
                            FirstChr = Chr;
                        }
                        else
                        {
                            if (!((Chr >= '0') && (Chr <= '9')) && !((Chr >= 'a') && (Chr <= 'z')) && !((Chr >= 'A') && (Chr <= 'Z')))
                            {
                                result = false;
                            }
                        }
                    }
                    if (!result)
                    {
                        break;
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                LogService.Error($"CheckChrName {sChrName} 异常信息:" + e.Message);
                return false;
            }
        }

        public static bool InClearMakeIndexList(int nIndex)
        {
            bool result = false;
            for (int i = 0; i < ClearMakeIndex.Count; i++)
            {
                //if (nIndex == ((int)g_ClearMakeIndex.Values[i]))
                //{
                //    result = true;
                //    break;
                //}
            }
            return result;
        }

        public static bool CheckServerIP(string sIP)
        {
            bool result = false;
            if (ServerIpList.Contains(sIP))
            {
                return true;
            }
            return result;
        }

        public static void Initialization()
        {
            DenyChrNameList = new StringList();
            ServerIpList = new HashSet<string>();
            _gateIdList = new Dictionary<string, short>();
            MapList = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }
    }

    public class THumSession
    {
        public string sChrName;
        public int nIndex;
        public string ConnectionId;
        public bool bo24;
        public bool bo2C;
        public long lastSessionTick;
    }

    public class GlobaSessionInfo
    {
        public string Account;
        public string IPaddr;
        public int SessionID;
        public long AddTick;
        public DateTime AddDate;
        public bool LoadRcd;
        public bool StartPlay;
    }

    public class SelGateInfo
    {
        public string ConnectionId;
        public Socket Socket;
        public EndPoint RemoteEndPoint;
        public IList<SessionUserInfo> UserList;
        /// <summary>
        /// 网关ID
        /// </summary>
        public short nGateID;

        public SelGateInfo()
        {

        }

        public (string serverIp, string Status, string playCount, string reviceTotal, string sendTotal, string queueCount) GetStatus()
        {
            return (RemoteEndPoint.ToString(), GetConnected(), UserList.Count.ToString(), "", "", "");
        }

        private string GetConnected()
        {
            return Socket.Connected ? "[green]Connected[/]" : "[red]Not Connected[/]";
        }
    }

    public class SessionUserInfo
    {
        public string sAccount;
        public string sUserIPaddr;
        public string sGateIPaddr;
        public string SessionId;
        public int nSessionID;
        public string ConnectionId;
        public string sText;
        public bool boChrSelected;
        public bool boChrQueryed;
        public long dwTick34;
        public long dwChrTick;
        public short nSelGateID;
    }

    public class GateRouteInfo
    {
        public int GateCount;
        /// <summary>
        /// 角色网关IP
        /// </summary>
        public string SelGateIP;
        /// <summary>
        /// 游戏网关
        /// </summary>
        public string[] GameGateIP;
        /// <summary>
        /// 游戏网关端口
        /// </summary>
        public int[] GameGatePort;

        public GateRouteInfo()
        {
            GameGateIP = new string[20];
            GameGatePort = new int[20];
        }
    }
}