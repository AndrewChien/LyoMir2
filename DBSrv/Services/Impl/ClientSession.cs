using DBSrv.Conf;
using OpenMir2.Enums;
using TouchSocket.Sockets;

namespace DBSrv.Services.Impl
{
    /// <summary>
    /// 登陆会话同步服务（开启客户端连接LoginSrv：5600）
    /// </summary>
    public class ClientSession
    {
        /// <summary>
        /// to(server)：账号服务器LoginSvr(5600)
        /// self(client)：登陆会话同步
        /// </summary>
        private readonly TcpClient _clientScoket;
        private readonly IList<GlobaSessionInfo> _globaSessionList = null;
        private readonly SettingsModel _setting;
        private string _sockMsg = string.Empty;

        /// <summary>
        /// 登陆会话同步服务（开启客户端连接LoginSrv：5600）
        /// </summary>
        /// <param name="conf"></param>
        public ClientSession(SettingsModel conf)
        {
            _setting = conf;
            _clientScoket = new TcpClient();
            TouchSocketConfig config = new TouchSocketConfig()
                .SetRemoteIPHost(new IPHost(IPAddress.Parse(_setting.LoginServerAddr), _setting.LoginServerPort))
                .ConfigureContainer(x => { x.AddConsoleLogger(); })
                .ConfigurePlugins(x => { x.UseReconnection(); });
            _clientScoket.Setup(config);
            _clientScoket.Received += LoginSocketRead;
            _clientScoket.Connected += LoginSocketConnected;
            _clientScoket.Disconnected += LoginSocketDisconnected;
            _globaSessionList = new List<GlobaSessionInfo>();
        }

        public async Task Start()
        {
            try
            {
                if (_clientScoket.Online)
                {
                    return;
                }
                await _clientScoket.ConnectAsync();
            }
            catch (TimeoutException)
            {
                LogService.Error($"账号服务器[{_setting.LoginServerAddr}:{_setting.LoginServerPort}]链接超时...");
            }
            catch (Exception)
            {
                LogService.Error($"账号服务器[{_setting.LoginServerAddr}:{_setting.LoginServerPort}]链接失败...");
            }
        }

        public void Stop()
        {
            for (int i = 0; i < _globaSessionList.Count; i++)
            {
                _globaSessionList[i] = null;
            }
        }

        private Task LoginSocketConnected(ITcpClientBase client, ConnectedEventArgs e)
        {
            LogService.Info($"账号服务器[{client.GetIPPort()}]链接成功.");
            return Task.CompletedTask;
        }

        private Task LoginSocketDisconnected(ITcpClientBase client, DisconnectEventArgs e)
        {
            LogService.Error($"账号服务器[{client.GetIPPort()}]断开链接.");
            return Task.CompletedTask;
        }

        private Task LoginSocketRead(IClient client, ReceivedDataEventArgs e)
        {
            var msg = HUtil32.GetString(e.ByteBlock.Buffer, 0, e.ByteBlock.Len);
            _sockMsg += msg;
            if (_sockMsg.IndexOf(")", StringComparison.OrdinalIgnoreCase) > 0)
            {
                ProcessSocketMsg();
            }
            return Task.CompletedTask;
        }

        private void ProcessSocketMsg()
        {
            string sData = string.Empty;
            string sCode = string.Empty;
            string sScoketText = _sockMsg;
            while (sScoketText.IndexOf(")", StringComparison.OrdinalIgnoreCase) > 0)
            {
                sScoketText = HUtil32.ArrestStringEx(sScoketText, "(", ")", ref sData);
                if (string.IsNullOrEmpty(sData))
                {
                    break;
                }
                string sBody = HUtil32.GetValidStr3(sData, ref sCode, HUtil32.Backslash);
                int nIdent = HUtil32.StrToInt(sCode, 0);

                switch (nIdent)
                {
                    case Messages.SS_OPENSESSION:
                        LogService.Info($"DBSrv：收到[Messages.SS_OPENSESSION]类型消息");
                        ProcessAddSession(sBody);
                        break;
                    case Messages.SS_CLOSESESSION:
                        LogService.Info($"DBSrv：收到[Messages.SS_CLOSESESSION]类型消息");
                        ProcessDelSession(sBody);
                        break;
                    case Messages.SS_KEEPALIVE:
                        //LogService.Info($"DBSrv：收到[Messages.SS_KEEPALIVE]类型消息");
                        ProcessGetOnlineCount(sBody);
                        break;
                }
            }
            _sockMsg = sScoketText;
        }

        public void SendSocketMsg(short wIdent, string sMsg)
        {
            const string sFormatMsg = "({0}/{1})";
            string sSendText = string.Format(sFormatMsg, wIdent, sMsg);
            _clientScoket.Send(sSendText);//发给LoginSvr(5600)
        }

        public bool CheckSession(string account, string sIPaddr, int sessionId)
        {
            bool result = false;
            for (int i = 0; i < _globaSessionList.Count; i++)
            {
                GlobaSessionInfo globaSessionInfo = _globaSessionList[i];
                if (globaSessionInfo != null)
                {
                    if ((globaSessionInfo.Account == account) && (globaSessionInfo.SessionID == sessionId))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public int CheckSessionLoadRcd(string sAccount, string sIPaddr, int nSessionId, ref bool boFoundSession)
        {
            int result = -1;
            boFoundSession = false;
            for (int i = 0; i < _globaSessionList.Count; i++)
            {
                GlobaSessionInfo globaSessionInfo = _globaSessionList[i];
                if (globaSessionInfo != null)
                {
                    if ((globaSessionInfo.Account == sAccount) && (globaSessionInfo.SessionID == nSessionId))
                    {
                        boFoundSession = true;
                        if (!globaSessionInfo.LoadRcd)
                        {
                            globaSessionInfo.LoadRcd = true;
                            result = 1;
                        }
                        break;
                    }
                }
            }
            return result;
        }

        public bool SetSessionSaveRcd(string sAccount)
        {
            bool result = false;
            for (int i = 0; i < _globaSessionList.Count; i++)
            {
                GlobaSessionInfo globaSessionInfo = _globaSessionList[i];
                if (globaSessionInfo != null)
                {
                    if ((globaSessionInfo.Account == sAccount))
                    {
                        globaSessionInfo.LoadRcd = false;
                        result = true;
                    }
                }
            }
            return result;
        }

        public void SetGlobaSessionNoPlay(int nSessionId)
        {
            for (int i = 0; i < _globaSessionList.Count; i++)
            {
                GlobaSessionInfo globaSessionInfo = _globaSessionList[i];
                if (globaSessionInfo != null)
                {
                    if ((globaSessionInfo.SessionID == nSessionId))
                    {
                        globaSessionInfo.StartPlay = false;
                        break;
                    }
                }
            }
        }

        public void SetGlobaSessionPlay(int nSessionId)
        {
            for (int i = 0; i < _globaSessionList.Count; i++)
            {
                GlobaSessionInfo globaSessionInfo = _globaSessionList[i];
                if (globaSessionInfo != null)
                {
                    if ((globaSessionInfo.SessionID == nSessionId))
                    {
                        globaSessionInfo.StartPlay = true;
                        break;
                    }
                }
            }
        }

        public bool GetGlobaSessionStatus(int nSessionId)
        {
            bool result = false;
            for (int i = 0; i < _globaSessionList.Count; i++)
            {
                GlobaSessionInfo globaSessionInfo = _globaSessionList[i];
                if (globaSessionInfo != null)
                {
                    if ((globaSessionInfo.SessionID == nSessionId))
                    {
                        result = globaSessionInfo.StartPlay;
                        break;
                    }
                }
            }
            return result;
        }

        public void CloseSession(string sAccount, int nSessionId)
        {
            for (int i = 0; i < _globaSessionList.Count; i++)
            {
                GlobaSessionInfo globaSessionInfo = _globaSessionList[i];
                if (globaSessionInfo != null)
                {
                    if ((globaSessionInfo.SessionID == nSessionId))
                    {
                        if (globaSessionInfo.Account == sAccount)
                        {
                            globaSessionInfo = null;
                            _globaSessionList.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        private void ProcessAddSession(string sData)
        {
            string sAccount = string.Empty;
            string s10 = string.Empty;
            string s14 = string.Empty;
            string s18 = string.Empty;
            string sIPaddr = string.Empty;
            sData = HUtil32.GetValidStr3(sData, ref sAccount, HUtil32.Backslash);
            sData = HUtil32.GetValidStr3(sData, ref s10, HUtil32.Backslash);
            sData = HUtil32.GetValidStr3(sData, ref s14, HUtil32.Backslash);
            sData = HUtil32.GetValidStr3(sData, ref s18, HUtil32.Backslash);
            sData = HUtil32.GetValidStr3(sData, ref sIPaddr, HUtil32.Backslash);
            GlobaSessionInfo globaSessionInfo = new GlobaSessionInfo();
            globaSessionInfo.Account = sAccount;
            globaSessionInfo.IPaddr = sIPaddr;
            globaSessionInfo.SessionID = HUtil32.StrToInt(s10, 0);
            //GlobaSessionInfo.n24 = HUtil32.StrToInt(s14, 0);
            globaSessionInfo.StartPlay = false;
            globaSessionInfo.LoadRcd = false;
            globaSessionInfo.AddTick = HUtil32.GetTickCount();
            globaSessionInfo.AddDate = DateTime.Now;
            _globaSessionList.Add(globaSessionInfo);
            LogService.Info($"同步账号服务[{sAccount}]同步会话消息...");
        }

        private void ProcessDelSession(string sData)
        {
            string sAccount = string.Empty;
            sData = HUtil32.GetValidStr3(sData, ref sAccount, HUtil32.Backslash);
            int nSessionId = HUtil32.StrToInt(sData, 0);
            for (int i = 0; i < _globaSessionList.Count; i++)
            {
                GlobaSessionInfo globaSessionInfo = _globaSessionList[i];
                if (globaSessionInfo != null)
                {
                    if ((globaSessionInfo.SessionID == nSessionId) && (globaSessionInfo.Account == sAccount))
                    {
                        globaSessionInfo = null;
                        _globaSessionList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public bool GetSession(string sAccount, string sIPaddr)
        {
            bool result = false;
            for (int i = 0; i < _globaSessionList.Count; i++)
            {
                GlobaSessionInfo globaSessionInfo = _globaSessionList[i];
                if (globaSessionInfo != null)
                {
                    if ((globaSessionInfo.Account == sAccount) && (globaSessionInfo.IPaddr == sIPaddr))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private static void ProcessGetOnlineCount(string sData)
        {

        }

        public void SendKeepAlivePacket(int userCount)
        {
            if (_clientScoket.Online)
            {
                //发给LoginSvr(5600)
                _clientScoket.Send(HUtil32.GetBytes("(" + Messages.SS_SERVERINFO + "/" + _setting.ServerName + "/" + "99" + "/" + userCount + ")"));
            }
        }
    }
}