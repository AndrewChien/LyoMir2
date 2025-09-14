﻿using LoginSrv.Services;

namespace LoginSrv
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class TimedService : BackgroundService
    {

        private readonly LoginServer _loginService;
        private readonly SessionServer _sessionService;
        private int _processMonSocTick;
        private int _processServerStatusTick;

        /// <summary>
        /// 定时任务
        /// </summary>
        /// <param name="loginService"></param>
        /// <param name="sessionService"></param>
        public TimedService(LoginServer loginService, SessionServer sessionService)
        {
            _loginService = loginService;
            _sessionService = sessionService;
        }

        /// <summary>
        /// 定时任务执行主体
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processMonSocTick = HUtil32.GetTickCount();
            _processServerStatusTick = HUtil32.GetTickCount();
            while (!stoppingToken.IsCancellationRequested)
            {
                _loginService.SessionClearKick();
                _sessionService.SessionClearNoPayMent();
                SystemStatus();
                CheckServerStatus();
                await Task.Delay(TimeSpan.FromMilliseconds(10), stoppingToken);
            }
        }

        /// <summary>
        /// 定时统计
        /// </summary>
        private void SystemStatus()
        {
            if (HUtil32.GetTickCount() - _processMonSocTick > 20000)
            {
                _processMonSocTick = HUtil32.GetTickCount();
                StringBuilder builder = new StringBuilder();
                int serverListCount = _sessionService.ServerList.Count;
                for (int i = 0; i < serverListCount; i++)
                {
                    ServerSessionInfo msgServer = _sessionService.ServerList[i];
                    if (!string.IsNullOrEmpty(msgServer.ServerName))
                    {
                        builder.Append(msgServer.ServerName + "/" + msgServer.ServerIndex + "/" + msgServer.OnlineCount + "/");
                        if (msgServer.ServerIndex == 99)
                        {
                            builder.Append("DBServer/");
                        }
                        else
                        {
                            builder.Append("GameServer/");
                            switch (msgServer.PayMentMode)
                            {
                                case 0:
                                    builder.Append("免费/");
                                    break;
                                case 1:
                                    builder.Append("试玩/");
                                    break;
                                case 2:
                                    builder.Append("测试/");
                                    break;
                                case 3:
                                    builder.Append("付费/");
                                    break;
                            }
                        }
                        builder.Append($"Online:{msgServer.OnlineCount}/");
                        if ((HUtil32.GetTickCount() - msgServer.KeepAliveTick) < 30000)
                        {
                            builder.Append("正常");
                        }
                        else
                        {
                            builder.Append("超时");
                        }
                        builder.Append("；");
                    }
                    else
                    {
                        builder.Append("-/-/-/-;");
                    }
                }
                if (builder.Length > 0)
                {
                    LogService.Info($"LoginSrv(5600)：状态检查：" + builder.ToString());
                }
            }
        }

        /// <summary>
        /// 定时检查DBSvr和GameSvr服务连接状态
        /// </summary>
        private void CheckServerStatus()
        {
            if (HUtil32.GetTickCount() - _processServerStatusTick > 10000)
            {
                _processServerStatusTick = HUtil32.GetTickCount();
                System.Collections.Generic.IList<ServerSessionInfo> serverList = _sessionService.ServerList;
                if (!serverList.Any())
                {
                    return;
                }
                for (int i = 0; i < serverList.Count; i++)
                {
                    ServerSessionInfo msgServer = serverList[i];
                    string sServerName = msgServer.ServerName;
                    if (!string.IsNullOrEmpty(sServerName))
                    {
                        int tickTime = HUtil32.GetTickCount() - msgServer.KeepAliveTick;
                        if (tickTime <= 20000)
                        {
                            continue;
                        }

                        msgServer.Socket.Close();
                        if (msgServer.ServerIndex == 99)
                        {
                            if (string.IsNullOrEmpty(sServerName))
                            {
                                LogService.Warn($"数据库服务器[{msgServer.IPaddr}]响应超时,关闭链接.");
                            }
                            else
                            {
                                LogService.Warn($"[{sServerName}]数据库服务器响应超时,关闭链接.");
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(sServerName))
                            {
                                LogService.Warn($"游戏服务器[{msgServer.IPaddr}]响应超时,关闭链接.");
                            }
                            else
                            {
                                LogService.Warn($"[{sServerName}]游戏服务器响应超时,关闭链接.");
                            }
                        }
                    }
                }
            }
        }
    }
}