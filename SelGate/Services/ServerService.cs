using OpenMir2;
using SelGate.Conf;
using SelGate.Datas;
using System;
using System.Net;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace SelGate.Services
{
    /// <summary>
    /// 角色服务（开启SelGate：7100）
    /// </summary>
    public class ServerService
    {
        /// <summary>
        /// 角色服务（开启SelGate：7100）
        /// Mir2-SelGate
        /// </summary>
        private readonly TcpService _serverSocket;
        private readonly SessionManager _sessionManager;
        /// <summary>
        /// 接收封包（客户端-》网关）
        /// </summary>
        private readonly Channel<MessageData> _sendQueue;
        private readonly ClientManager _clientManager;
        private readonly ConfigManager _configManager;

        /// <summary>
        /// 角色服务（开启SelGate：7100）
        /// </summary>
        /// <param name="sessionManager"></param>
        /// <param name="clientManager"></param>
        /// <param name="configManager"></param>
        public ServerService(SessionManager sessionManager, ClientManager clientManager, ConfigManager configManager)
        {
            _sessionManager = sessionManager;
            _clientManager = clientManager;
            _configManager = configManager;
            _sendQueue = Channel.CreateUnbounded<MessageData>();
            _serverSocket = new TcpService();
            _serverSocket.Connected += ServerSocketClientConnect;
            _serverSocket.Disconnected += ServerSocketClientDisconnect;
            _serverSocket.Received += ServerSocketClientRead;
        }

        public void Start()
        {
            _serverSocket.Setup(new TouchSocketConfig().SetListenIPHosts(new IPHost(IPAddress.Any, GateShare.GatePort)));
            _serverSocket.Start();
            LogService.Info($"登陆网关[127.0.0.1:{GateShare.GatePort}]已启动.");
        }

        public void Stop()
        {
            _serverSocket.Stop();
        }

        /// <summary>
        /// 处理客户端发过来的消息
        /// </summary>
        public void ProcessReviceMessage(CancellationToken stoppingToken)
        {
            Task.Factory.StartNew(async () =>
            {
                while (await _sendQueue.Reader.WaitToReadAsync(stoppingToken))
                {
                    while (_sendQueue.Reader.TryRead(out MessageData message))
                    {
                        ClientSession clientSession = _sessionManager.GetSession(message.SessionId);
                        clientSession?.HandleUserPacket(message);
                    }
                }
            }, stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
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

        private Task ServerSocketClientConnect(ITcpClientBase client, ConnectedEventArgs e)
        {
            ClientThread clientThread = _clientManager.GetClientThread();
            if (clientThread == null)
            {
                LogService.Info("获取服务器实例失败。");
                return Task.CompletedTask;
            }
            string sRemoteAddress = client.MainSocket.RemoteEndPoint.GetIP();
            LogService.Info($"用户[{sRemoteAddress}]分配到数据库服务器[{clientThread.ClientId}] Server:{clientThread.GetEndPoint()}");
            SessionInfo sessionInfo = null;
            for (int nIdx = 0; nIdx < ClientThread.MaxSession; nIdx++)
            {
                sessionInfo = clientThread.SessionArray[nIdx];
                if (sessionInfo == null)
                {
                    sessionInfo = new SessionInfo();
                    sessionInfo.SocketId = ((SocketClient)client).Id;
                    sessionInfo.dwReceiveTick = HUtil32.GetTickCount();
                    sessionInfo.ClientIP = sRemoteAddress;
                    break;
                }
            }
            if (sessionInfo != null)
            {
                LogService.Info("开始连接: " + sRemoteAddress);
                _clientManager.AddClientThread(sessionInfo.SocketId, clientThread);//链接成功后建立对应关系
                ClientSession userSession = new ClientSession(_configManager, sessionInfo, clientThread);
                userSession.UserEnter();
                _sessionManager.AddSession(sessionInfo.SocketId, userSession);
            }
            else
            {
                LogService.Info("禁止连接: " + sRemoteAddress);
            }
            return Task.CompletedTask;
        }

        private Task ServerSocketClientDisconnect(IClient client, DisconnectEventArgs e)
        {
            SocketClient clientSoc = ((SocketClient)client);
            string nSockIndex = clientSoc.Id;
            string sRemoteAddr = clientSoc.IP;
            ClientThread clientThread = _clientManager.GetClientThread(nSockIndex);
            if (clientThread != null && clientThread.boGateReady)
            {
                ClientSession userSession = _sessionManager.GetSession(nSockIndex);
                if (userSession != null)
                {
                    userSession.UserLeave();
                    userSession.CloseSession();
                    LogService.Info("断开连接: " + sRemoteAddr);
                }
                _sessionManager.CloseSession(nSockIndex);
            }
            else
            {
                LogService.Info("断开链接: " + sRemoteAddr);
                LogService.Info($"获取用户对应网关失败 RemoteAddr:[{sRemoteAddr}] ConnectionId:[{clientSoc.Id}]");
            }
            _clientManager.DeleteClientThread(nSockIndex);
            return Task.CompletedTask;
        }

        private Task ServerSocketClientRead(IClient client, ReceivedDataEventArgs e)
        {
            LogService.Info($"SelGate（7100）：收到来自于客户端{(client as SocketClient)?.IP}:{(client as SocketClient)?.Port}的消息");
            SocketClient clientSoc = client as SocketClient;
            string sRemoteAddress = clientSoc.IP;


            ClientThread userClient = _clientManager.GetClientThread(clientSoc.Id);
            if (userClient == null)
            {
                LogService.Info("非法攻击: " + sRemoteAddress);
                LogService.Info($"获取用户对应网关失败 RemoteAddr:[{sRemoteAddress}] ConnectionId:[{clientSoc.Id}]");
                return Task.CompletedTask;
            }
            if (!userClient.boGateReady)
            {
                LogService.Info("未就绪: " + sRemoteAddress);
                LogService.Info($"游戏引擎链接失败 Server:[{userClient.GetEndPoint()}] ConnectionId:[{clientSoc.Id}]");
                return Task.CompletedTask;
            }
            byte[] data = new byte[e.ByteBlock.Len];
            Array.Copy(e.ByteBlock.Buffer, 0, data, 0, data.Length);
            MessageData userData = new MessageData();
            userData.Body = data;
            userData.SessionId = clientSoc.Id;
            _sendQueue.Writer.TryWrite(userData);
            return Task.CompletedTask;
        }
    }
}