using OpenMir2;
using System.Net;
using System.Net.Sockets;
using SystemModule;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace PlanesSystem
{
    /// <summary>
    /// 位面服务器
    /// </summary>
    public class PlanesServer
    {
        private readonly TServerMsgInfo[] srvArray;
        /// <summary>
        /// 位面服务器，4900
        /// </summary>
        private readonly TcpService _serverSocket;
        private readonly PlanesMessage _groupMessageHandle;
        private static PlanesServer instance;
        public static PlanesServer Instance => instance ??= new PlanesServer();

        private PlanesServer()
        {
            srvArray = new TServerMsgInfo[10];
            _serverSocket = new TcpService();
            _serverSocket.Connected += Connecting;
            _serverSocket.Disconnected += Disconnected;
            _serverSocket.Received += Received;
            _groupMessageHandle = new PlanesMessage();
        }

        public void StartPlanesServer()
        {
            TouchSocketConfig touchSocketConfig = new TouchSocketConfig();
            touchSocketConfig.SetListenIPHosts(new IPHost[1]
            {
                new IPHost(IPAddress.Parse(SystemShare.Config.MasterSrvAddr), SystemShare.Config.MasterSrvPort)
            });
            _serverSocket.Setup(touchSocketConfig);
            //LogService.Info($"节点数据服务[{ModuleShare.Config.MasterSrvAddr}:{ModuleShare.Config.MasterSrvPort}]已启动.");
        }

        private void DecodeSocStr_SendOtherServer(TServerMsgInfo ps, string msgstr)
        {
            for (int i = 0; i < srvArray.Length; i++)
            {
                TServerMsgInfo serverMsgInfo = srvArray[i];
                if (serverMsgInfo == null)
                {
                    continue;
                }
                if (serverMsgInfo.Socket == null)
                {
                    continue;
                }

                if (serverMsgInfo.SocketId != ps.SocketId)
                {
                    SendSocket(serverMsgInfo.Socket, msgstr);
                }
            }
        }

        private void DecodeSocStr(TServerMsgInfo ps)
        {
            string Str = string.Empty;
            string sNumStr = string.Empty;
            string Head = string.Empty;
            int Ident;
            int sNum;
            if (string.IsNullOrEmpty(ps.SocData))
            {
                return;
            }
            if (ps.SocData.IndexOf(')') <= 0)
            {
                return;
            }
            try
            {
                string BufStr = ps.SocData;
                ps.SocData = "";
                while (BufStr.IndexOf(')') > 0)
                {
                    BufStr = HUtil32.ArrestStringEx(BufStr, "(", ")", ref Str);
                    if (!string.IsNullOrEmpty(Str))
                    {
                        DecodeSocStr_SendOtherServer(ps, Str);
                        string Body = HUtil32.GetValidStr3(Str, ref Head, HUtil32.Backslash);
                        Body = HUtil32.GetValidStr3(Body, ref sNumStr, HUtil32.Backslash);
                        Ident = HUtil32.StrToInt(Head, 0);
                        sNum = HUtil32.StrToInt(sNumStr, -1);
                        _groupMessageHandle.ProcessData(Ident, sNum, Body);
                    }
                    else
                    {
                        break;
                    }
                }
                ps.SocData = BufStr + ps.SocData;
            }
            catch (Exception)
            {
                //LogService.Error(ex.StackTrace);
            }
        }

        private static void SendSocket(Socket Socket, string sMsg)
        {
            if (Socket.Connected)
            {
                byte[] buffer = HUtil32.GetBytes("(" + sMsg + ")");
                Socket.Send(buffer);
            }
        }

        /// <summary>
        /// 发送消息给所有节点服务器
        /// </summary>
        /// <param name="msgstr"></param>
        public void SendServerSocket(string msgstr)
        {
            for (int i = 0; i < srvArray.Length; i++)
            {
                TServerMsgInfo serverMsgInfo = srvArray[i];
                if (serverMsgInfo == null)
                {
                    continue;
                }
                if (serverMsgInfo.Socket != null && serverMsgInfo.Socket.Connected)
                {
                    SendSocket(serverMsgInfo.Socket, msgstr);
                }
            }
        }

        private Task Received(SocketClient socketClient, ReceivedDataEventArgs e)
        {
            LogService.Info($"PlanSystem位面服务器（4900）：收到{socketClient?.IP}:{socketClient?.Port}消息");
            if (int.TryParse(socketClient.Id, out int clientId))
            {
                TServerMsgInfo serverInfo = srvArray[clientId - 1];
                serverInfo.SocData += HUtil32.GetString(e.ByteBlock.Buffer, 0, e.ByteBlock.Len);
            }
            return Task.CompletedTask;
        }

        private Task Connecting(object sender, TouchSocketEventArgs e)
        {
            SocketClient client = (SocketClient)sender;
            IPEndPoint endPoint = (IPEndPoint)client.MainSocket.RemoteEndPoint;
            for (int i = 0; i < srvArray.Length; i++)
            {
                TServerMsgInfo serverMsgInfo = srvArray[i];
                if (serverMsgInfo == null)
                {
                    serverMsgInfo = new TServerMsgInfo();
                    serverMsgInfo.Socket = client.MainSocket;
                    serverMsgInfo.SocData = string.Empty;
                    serverMsgInfo.SocketId = client.Id;
                    //LogService.Info($"节点服务器({endPoint})链接成功...");
                    srvArray[i] = serverMsgInfo;
                    break;
                }
            }
            return Task.CompletedTask;
        }

        private Task Disconnected(object sender, DisconnectEventArgs e)
        {
            SocketClient client = (SocketClient)sender;
            for (int i = 0; i < srvArray.Length; i++)
            {
                TServerMsgInfo serverMsgInfo = srvArray[i];
                if (serverMsgInfo == null)
                {
                    continue;
                }
                if (serverMsgInfo.SocketId == client.Id)
                {
                    serverMsgInfo.Socket = null;
                    serverMsgInfo.SocData = "";
                    //LogService.Error($"节点服务器({client.MainSocket.RemoteEndPoint})断开连接...");
                    srvArray[i] = null;
                    break;
                }
            }
            return Task.CompletedTask;
        }

        public void Run()
        {
            try
            {
                for (int i = 0; i < srvArray.Length; i++)
                {
                    TServerMsgInfo ps = srvArray[i];
                    if (ps == null)
                    {
                        continue;
                    }
                    if (ps.Socket != null)
                    {
                        DecodeSocStr(ps);
                    }
                }
            }
            catch
            {
                //LogService.Error(sExceptionMsg);
            }
        }
    }

    public class TServerMsgInfo
    {
        public Socket Socket;
        public string SocData;
        public string SocketId;
    }

    public struct ServerGruopInfo
    {
        public int nServerIdx;
        public string sChrName;
    }
}


