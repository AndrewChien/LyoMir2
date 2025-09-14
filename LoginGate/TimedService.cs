﻿/// <summary>
/// 定时任务
/// </summary>
public class TimedService : BackgroundService
{
    private int _processDelayTick = 0;
    private int _heartInterval = 0;
    private readonly SessionManager _sessionManager;
    private readonly ClientManager _clientManager;

    public TimedService(ClientManager clientManager, SessionManager sessionManager)
    {
        _clientManager = clientManager;
        _sessionManager = sessionManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processDelayTick = HUtil32.GetTickCount();
        _heartInterval = HUtil32.GetTickCount();
        while (!stoppingToken.IsCancellationRequested)
        {
            ProcessDelayMsg();
            ProcessHeartbeat();
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private void ProcessHeartbeat()
    {
        if (HUtil32.GetTickCount() - _heartInterval > 10000)
        {
            _heartInterval = HUtil32.GetTickCount();
            IList<ClientThread> clientList = _clientManager.ServerGateList();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i] == null)
                {
                    continue;
                }
                _clientManager.ProcessClientHeart(clientList[i]);
            }
        }
    }

    private void ProcessDelayMsg()
    {
        if (HUtil32.GetTickCount() - _processDelayTick > 1000)
        {
            _processDelayTick = HUtil32.GetTickCount();
            IList<ClientThread> clientList = _clientManager.ServerGateList();
            for (int i = 0; i < clientList.Count; i++)
            {
                if (clientList[i] == null)
                {
                    continue;
                }

                if (clientList[i].SessionArray == null)
                {
                    continue;
                }

                for (int j = 0; j < clientList[i].SessionArray.Length; j++)
                {
                    TSessionInfo session = clientList[i].SessionArray[j];
                    if (session == null)
                    {
                        continue;
                    }

                    ClientSession userSession = _sessionManager.GetSession(session.ConnectionId);
                    if (userSession == null)
                    {
                        continue;
                    }

                    bool success = false;
                    userSession.HandleDelayMsg(ref success);
                    if (success)
                    {
                        _sessionManager.CloseSession(session.ConnectionId);
                        clientList[i].SessionArray[j] = null;
                    }
                }
            }
        }
    }
}