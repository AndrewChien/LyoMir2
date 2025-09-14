using TouchSocket.Sockets;


/// <summary>
/// 客户端登录服务（开启LoginGate：7000）
/// </summary>
public class ServerService
{
    /// <summary>
    /// Mir2-LoginGate
    /// </summary>
    private readonly TcpService _serverSocket;
    private readonly SessionManager _sessionManager;
    private readonly ClientManager _clientManager;
    private readonly ServerManager _serverManager;

    /// <summary>
    /// 客户端登录服务（开启LoginGate：7000）
    /// </summary>
    /// <param name="serverManager"></param>
    /// <param name="clientManager"></param>
    /// <param name="sessionManager"></param>
    public ServerService(ServerManager serverManager, ClientManager clientManager, SessionManager sessionManager)
    {
        _serverManager = serverManager;
        _clientManager = clientManager;
        _sessionManager = sessionManager;
        _serverSocket = new TcpService();
        _serverSocket.Connected += ServerSocketClientConnect;
        _serverSocket.Disconnected += ServerSocketClientDisconnect;
        _serverSocket.Received += ServerSocketClientRead;
    }

    public void Start(GameGateInfo gateInfo)
    {
        var config = new TouchSocketConfig().SetListenIPHosts(new IPHost(IPAddress.Parse(gateInfo.GateAddress), gateInfo.GatePort));
        _serverSocket.Setup(config);
        _serverSocket.Start();
        LogService.Info($"登录网关[{gateInfo.GateAddress}:{gateInfo.GatePort}]已启动...");
    }

    public void Stop()
    {
        _serverSocket.Stop();
        LogService.Info($"登录网关[{_serverSocket.ServerName}]停止服务...");
    }

    public string GetEndPoint()
    {
        return _serverSocket.ServerName;
    }

    public void SendMessage(string connectionId, byte[] data)
    {
        _serverSocket.Send(connectionId, data);
    }

    public void SendMessage(string connectionId, byte[] data, int len)
    {
        _serverSocket.Send(connectionId, data, 0, len);
    }

    public void CloseClient(string connectionId)
    {
        if (_serverSocket.TryGetSocketClient(connectionId, out SocketClient client))
        {
            client.Close();
        }
    }

    /// <summary>
    /// Mir2链接
    /// </summary>
    private Task ServerSocketClientConnect(ITcpClientBase client, ConnectedEventArgs e)
    {
        string sRemoteAddress = client.GetIPPort();
        ClientThread clientThread = _clientManager.GetClientThread();
        if (clientThread == null)
        {
            LogService.Info("获取登陆服务失败。");
            return Task.CompletedTask;
        }

        if (!clientThread.ConnectState)
        {
            LogService.Info("未就绪: " + sRemoteAddress);
            LogService.Info($"游戏引擎链接失败 Server:[{clientThread.EndPoint}] Ip:[{client.IP}]");
            return Task.CompletedTask;
        }

        LogService.Info($"用户[{sRemoteAddress}]分配到数据库服务器[{clientThread.ClientId}] Server:{clientThread.EndPoint}");
        TSessionInfo sessionInfo = null;

        for (int nIdx = 0; nIdx < GateShare.MaxSession; nIdx++)
        {
            sessionInfo = clientThread.SessionArray[nIdx];
            if (sessionInfo == null)
            {
                sessionInfo = new TSessionInfo();
                sessionInfo.ConnectionId = ((SocketClient)client).Id;
                sessionInfo.ReceiveTick = HUtil32.GetTickCount();
                sessionInfo.ClientIP = client.IP;
                clientThread.SessionArray[nIdx] = sessionInfo;
                break;
            }
        }

        if (sessionInfo != null)
        {
            LogService.Info("开始连接: " + sRemoteAddress);
            _sessionManager.AddSession(sessionInfo, clientThread);
        }
        else
        {
            client.Close();
            LogService.Info("禁止连接: " + sRemoteAddress);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 游戏客户端断开链接
    /// </summary>
    private Task ServerSocketClientDisconnect(ITcpClientBase client, DisconnectEventArgs e)
    {
        SocketClient socClient = client as SocketClient;
        ClientSession userSession = _sessionManager.GetSession(socClient.Id);
        if (userSession != null)
        {
            userSession.UserLeave();
            userSession.CloseSession();
            LogService.Info("断开连接: " + client.IP);
            LogService.Info($"用户[{client.IP}] 会话ID:[{client.MainSocket.Handle.ToInt32()}] 断开链接.");
        }

        _sessionManager.CloseSession(socClient.Id);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 读取游戏客户端数据
    /// </summary>
    private Task ServerSocketClientRead(SocketClient client, ReceivedDataEventArgs e)
    {
        LogService.Info($"LoginGate：收到来自客户端{client?.IP}:{client?.Port}的消息");
        byte[] data = new byte[e.ByteBlock.Len];
        Buffer.BlockCopy(e.ByteBlock.Buffer, 0, data, 0, data.Length);
        MessageData message = new MessageData();
        message.ClientIP = client.IP;
        message.Body = data;
        message.ConnectionId = client.Id;
        _serverManager.SendQueue(message);
        return Task.CompletedTask;
    }
}