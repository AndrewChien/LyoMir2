﻿using DBSrv.Conf;
using DBSrv.Storage;
using OpenMir2.DataHandlingAdapters;

namespace DBSrv.Services.Impl
{
    /// <summary>
    /// 拍卖行数据存储服务（开启DBSrv：5700）
    /// </summary>
    public class MarketService : IService
    {
        private readonly ICacheStorage _cacheStorage;
        private readonly IMarketStorage _marketStorage;
        /// <summary>
        /// to(client)：游戏主引擎GameSrv
        /// self(server)：拍卖行数据存储服务（5700）
        /// GameSrv-> DBSrv
        /// </summary>
        private readonly TcpService _socketServer;
        private readonly SettingsModel _setting;

        /// <summary>
        /// 拍卖行数据存储服务（开启DBSrv：5700）
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="cacheStorage"></param>
        /// <param name="marketStorage"></param>
        public MarketService(SettingsModel setting, ICacheStorage cacheStorage, IMarketStorage marketStorage)
        {
            _setting = setting;
            _cacheStorage = cacheStorage;
            _marketStorage = marketStorage;
            _socketServer = new TcpService();
            _socketServer.Connected += Connecting;
            _socketServer.Disconnected += Disconnected;
            _socketServer.Received += Received;
        }

        public void Initialize()
        {
            TouchSocketConfig touchSocketConfig = new TouchSocketConfig();
            touchSocketConfig.SetListenIPHosts(new IPHost[1]
            {
                new IPHost(IPAddress.Parse(_setting.MarketServerAddr), _setting.MarketServerPort)
            }).SetTcpDataHandlingAdapter(() => new ServerPacketFixedHeaderDataHandlingAdapter());
            _socketServer.Setup(touchSocketConfig);
        }

        public void Start()
        {
            _socketServer.Start();
            LogService.Info($"拍卖行数据库服务[{_setting.MarketServerAddr}:{_setting.MarketServerPort}]已启动.等待链接...");
        }

        public void Stop()
        {
            _socketServer.Stop();
        }

        public void PushMarketData()
        {
            //todo 根据服务器分组推送到各个GameSrv或者推送到所有GameSrv
            byte groupId = 0;//GroupID为0时查询所有区服的拍卖行数据
            System.Collections.Generic.IEnumerable<OpenMir2.Data.MarketItem> marketItems = _marketStorage.QueryMarketItems(groupId);
            if (!marketItems.Any())
            {
                LogService.Info("拍卖行数据为空,跳过推送拍卖行数据.");
                return;
            }
            System.Collections.Generic.IEnumerable<SocketClient> socketList = _socketServer.GetClients();
            foreach (SocketClient client in socketList)
            {
                if (_socketServer.SocketClientExist(client.Id))
                {
                    _socketServer.Send(client.Id, Array.Empty<byte>());//推送拍卖行数据
                }
            }
            LogService.Info($"推送拍卖行数据成功.当前拍卖行物品数据:[{marketItems.Count()}],在线服务器:[{socketList.Count()}]");
        }

        private Task Received(IClient client, ReceivedDataEventArgs e)
        {
            if (e.RequestInfo is not ServerDataMessageFixedHeaderRequestInfo fixedHeader)
            {
                return Task.CompletedTask;
            }

            //LogService.Info($"DBSrv(5700)：收到来自{(client as SocketClient)?.IP}:{(client as SocketClient)?.Port}的消息");

            SocketClient clientSoc = (SocketClient)client;
            try
            {
                if (fixedHeader.Header.PacketCode != Grobal2.PacketCode)
                {
                    LogService.Error($"解析寄售行封包出现异常封包...");
                    return Task.CompletedTask;
                }
                ServerRequestData messageData = SerializerUtil.Deserialize<ServerRequestData>(fixedHeader.Message);
                ProcessMessagePacket(clientSoc.Id, messageData);
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
            }
            return Task.CompletedTask;
        }

        private Task Connecting(ITcpClientBase client, ConnectedEventArgs e)
        {
            string remoteIp = client.MainSocket.RemoteEndPoint.GetIP();
            if (!DBShare.CheckServerIP(remoteIp))
            {
                LogService.Warn("非法服务器连接: " + remoteIp);
                client.Close();
                return Task.CompletedTask;
            }
            LogService.Info("拍卖行客户端(GameSrv)连接 " + client.MainSocket.RemoteEndPoint);
            return Task.CompletedTask;
        }

        private Task Disconnected(object sender, DisconnectEventArgs e)
        {
            SocketClient client = (SocketClient)sender;
            LogService.Info("拍卖行客户端(GameSrv)断开连接 " + client.MainSocket.RemoteEndPoint);
            return Task.CompletedTask;
        }

        private void ProcessMessagePacket(string connectionId, ServerRequestData requestData)
        {
            int nQueryId = requestData.QueryId;
            ServerRequestMessage requestMessage = SerializerUtil.Deserialize<ServerRequestMessage>(EDCode.DecodeBuff(requestData.Message));
            int packetLen = requestData.Message.Length + requestData.Packet.Length + ServerDataPacket.FixedHeaderLen;
            if (packetLen >= Messages.DefBlockSize && nQueryId > 0 && requestData.Packet != null && requestData.Sign != null)
            {
                byte[] sData = EDCode.DecodeBuff(requestData.Packet);
                int queryId = HUtil32.MakeLong((ushort)(nQueryId ^ 170), (ushort)packetLen);
                if (queryId <= 0)
                {
                    SendFailMessage(nQueryId, connectionId, new ServerRequestMessage(Messages.DBR_FAIL, 0, 0, 0, 0));
                    return;
                }
                if (requestData.Sign.Length <= 0)
                {
                    SendFailMessage(nQueryId, connectionId, new ServerRequestMessage(Messages.DBR_FAIL, 0, 0, 0, 0));
                    return;
                }
                byte[] signatureBuff = BitConverter.GetBytes(queryId);
                short signatureId = BitConverter.ToInt16(signatureBuff);
                byte[] signBuff = EDCode.DecodeBuff(requestData.Sign);
                short signId = BitConverter.ToInt16(signBuff);
                if (signId == signatureId)
                {
                    ProcessMarketPacket(nQueryId, requestMessage, sData, connectionId);
                    return;
                }
                _socketServer.TryGetSocketClient(connectionId, out SocketClient client);
                client.Close();
                LogService.Error($"关闭错误的任务{nQueryId}查询请求.");
                return;
            }
            SendFailMessage(nQueryId, connectionId, new ServerRequestMessage(Messages.DBR_FAIL, 0, 0, 0, 0));
        }

        private void ProcessMarketPacket(int queryId, ServerRequestMessage packet, byte[] sData, string connectionId)
        {
            switch (packet.Ident)
            {
                case Messages.DB_LOADMARKET://GameSrv主动拉取拍卖行数据
                    LogService.Info($"DBSrv：收到[Messages.DB_LOADMARKET]类型消息");
                    LoadMarketList(queryId, sData, connectionId);
                    break;
                case Messages.DB_SAVEMARKET://GameSrv保存拍卖行数据
                    LogService.Info($"DBSrv：收到[Messages.DB_SAVEMARKET]类型消息");
                    SaveMarketItem(queryId, packet.Recog, sData, connectionId);
                    break;
                case Messages.DB_SEARCHMARKET://GameSrv搜索拍卖行数据
                    LogService.Info($"DBSrv：收到[Messages.DB_SEARCHMARKET]类型消息");
                    SearchMarketItem(queryId, packet.Recog, sData, connectionId);
                    break;
                case Messages.DB_LOADUSERMARKET://GameSrv拉取玩家拍卖行数据
                    LogService.Info($"DBSrv：收到[Messages.DB_LOADUSERMARKET]类型消息");
                    QueryMarketUserLoad(queryId, packet.Recog, sData, connectionId);
                    break;
            }
        }

        private void QueryMarketUserLoad(int nQueryId, int actorId, byte[] sData, string connectionId)
        {
            MarketSearchMessage userMarket = SerializerUtil.Deserialize<MarketSearchMessage>(sData);
            if (userMarket.GroupId == 0)
            {
                ServerRequestMessage messagePacket = new ServerRequestMessage(Messages.DB_SRARCHMARKETFAIL, 0, 0, 0, 0);
                SendFailMessage(nQueryId, connectionId, messagePacket);
                LogService.Info($"服务器组[{userMarket.GroupId}]拍卖行数据为空,搜索拍卖行数据失败.");
                return;
            }
            int userItemLoad = _marketStorage.QueryMarketItemsCount(userMarket.GroupId, userMarket.SearchWho);
            MarkerUserLoadMessage marketLoadMessgae = new MarkerUserLoadMessage();
            marketLoadMessgae.SellCount = userItemLoad;
            marketLoadMessgae.IsBusy = 0;
            marketLoadMessgae.MarketNPC = userMarket.MarketNPC;
            SendSuccessMessage(connectionId, actorId, Messages.DB_LOADUSERMARKETSUCCESS, marketLoadMessgae);
            LogService.Info($"获取服务器组[{userMarket.GroupId}] 用户[{userMarket.SearchWho}]个人拍卖行数据...");
        }

        private void SearchMarketItem(int nQueryId, int actorId, byte[] sData, string connectionId)
        {
            MarketSearchMessage marketSearch = SerializerUtil.Deserialize<MarketSearchMessage>(sData);
            if (marketSearch.GroupId == 0)
            {
                ServerRequestMessage messagePacket = new ServerRequestMessage(Messages.DB_SRARCHMARKETFAIL, 0, 0, 0, 0);
                SendFailMessage(nQueryId, connectionId, messagePacket);
                LogService.Info($"服务器组[{marketSearch.GroupId}]拍卖行数据为空,搜索拍卖行数据失败.");
                return;
            }
            System.Collections.Generic.IEnumerable<OpenMir2.Data.MarketItem> searchItems = _marketStorage.SearchMarketItems(marketSearch.GroupId, marketSearch.MarketName, marketSearch.SearchItem, marketSearch.SearchWho, marketSearch.ItemType, marketSearch.ItemSet);
            if (!searchItems.Any())
            {
                ServerRequestMessage messagePacket = new ServerRequestMessage(Messages.DB_SRARCHMARKETFAIL, 0, 1, 0, 0);
                SendFailMessage(nQueryId, connectionId, messagePacket);
                LogService.Info($"服务器组[{marketSearch.GroupId}]拍卖行数据为空,搜索拍卖行数据失败.");
                return;
            }
            MarketDataMessage marketItemMessgae = new MarketDataMessage();
            marketItemMessgae.List = searchItems.ToList();
            marketItemMessgae.TotalCount = searchItems.Count();
            SendSuccessMessage(connectionId, actorId, Messages.DB_SEARCHMARKETSUCCESS, marketItemMessgae);
            LogService.Info($"服务器组[{marketSearch.GroupId}]搜索拍卖行数据...");
        }

        private void LoadMarketList(int nQueryId, byte[] sData, string connectionId)
        {
            MarketRegisterMessage marketMessage = SerializerUtil.Deserialize<MarketRegisterMessage>(sData);
            if (string.IsNullOrEmpty(marketMessage.Token) || string.IsNullOrEmpty(marketMessage.ServerName))
            {
                LogService.Warn($"SocketId:{connectionId} QueryId:[{nQueryId}] 非法获取拍卖行数据...");
                return;
            }
            System.Collections.Generic.IEnumerable<OpenMir2.Data.MarketItem> marketItems = _marketStorage.QueryMarketItems(marketMessage.GroupId);
            if (!marketItems.Any())
            {
                ServerRequestMessage messagePacket = new ServerRequestMessage(Messages.DB_LOADMARKETFAIL, 0, 1, 0, 0);
                SendFailMessage(nQueryId, connectionId, messagePacket);
                LogService.Info($"当前服务器组[{marketMessage.GroupId}]拍卖行数据为空,读取拍卖行数据失败.");
                return;
            }
            MarketDataMessage marketItemMessgae = new MarketDataMessage();
            marketItemMessgae.List = marketItems.ToList();
            marketItemMessgae.TotalCount = marketItems.Count();
            SendSuccessMessage(connectionId, 0, Messages.DB_LOADMARKETSUCCESS, marketItemMessgae);
            LogService.Info($"服务器组[{marketMessage.GroupId}] [{marketMessage.ServerName}]读取拍卖行数据成功.当前拍卖行物品数据:[{marketItemMessgae.TotalCount}]");
        }

        private void SaveMarketItem(int nQueryId, int actorId, byte[] sData, string connectionId)
        {
            MarketSaveDataItem saveMessage = SerializerUtil.Deserialize<MarketSaveDataItem>(sData);
            if (saveMessage.GroupId == 0 || string.IsNullOrEmpty(saveMessage.ServerName))
            {
                LogService.Warn($"任务[{nQueryId}]非法获取拍卖行数据...");
                return;
            }
            bool marketItems = _marketStorage.SaveMarketItem(saveMessage.Item, saveMessage.GroupId, saveMessage.ServerIndex);
            if (!marketItems)
            {
                LogService.Info("当前服务器分组拍卖行数据为空,推送拍卖行数据失败.");
                return;
            }
            System.Collections.Generic.IEnumerable<OpenMir2.Data.MarketItem> marketItemsCount = _marketStorage.QueryMarketItems(saveMessage.GroupId);
            SendSuccessMessage(connectionId, actorId, Messages.DB_SAVEMARKETSUCCESS, saveMessage);
            LogService.Info($"服务器组[{saveMessage.GroupId}] [{saveMessage.ServerName}]保存拍卖行数据成功.当前拍卖行物品数据:[{marketItemsCount}]");
        }

        private void SendSuccessMessage<T>(string connectionId, int actorId, byte messageId, T data)
        {
            ServerRequestData responsePack = new ServerRequestData();
            ServerRequestMessage messagePacket = new ServerRequestMessage(messageId, actorId, 0, 0, 0);
            responsePack.Message = EDCode.EncodeBuffer(SerializerUtil.Serialize(messagePacket));
            responsePack.Packet = EDCode.EncodeBuffer(SerializerUtil.Serialize(data));
            SendRequest(connectionId, 1, responsePack);
        }

        private void SendFailMessage(int nQueryId, string connectionId, ServerRequestMessage messagePacket)
        {
            ServerRequestData responsePack = new ServerRequestData();
            responsePack.Message = EDCode.EncodeBuffer(SerializerUtil.Serialize(messagePacket));
            SendRequest(connectionId, nQueryId, responsePack);
        }

        private void SendRequest(string connectionId, int queryId, ServerRequestData requestPacket)
        {
            requestPacket.QueryId = queryId;
            int queryPart = 0;
            if (requestPacket.Packet != null)
            {
                queryPart = HUtil32.MakeLong((ushort)(queryId ^ 170), (ushort)(requestPacket.Message.Length + requestPacket.Packet.Length + ServerDataPacket.FixedHeaderLen));
            }
            else
            {
                requestPacket.Packet = Array.Empty<byte>();
                queryPart = HUtil32.MakeLong((ushort)(queryId ^ 170), (ushort)(requestPacket.Message.Length + ServerDataPacket.FixedHeaderLen));
            }
            byte[] nCheckCode = BitConverter.GetBytes(queryPart);
            requestPacket.Sign = EDCode.EncodeBuffer(nCheckCode);
            SendMessage(connectionId, SerializerUtil.Serialize(requestPacket));
        }

        private void SendMessage(string connectionId, byte[] sendBuffer)
        {
            ServerDataPacket serverMessage = new ServerDataPacket
            {
                PacketCode = Grobal2.PacketCode,
                PacketLen = (ushort)sendBuffer.Length
            };
            byte[] dataBuff = SerializerUtil.Serialize(serverMessage);
            byte[] data = new byte[ServerDataPacket.FixedHeaderLen + sendBuffer.Length];
            MemoryCopy.BlockCopy(dataBuff, 0, data, 0, data.Length);
            MemoryCopy.BlockCopy(sendBuffer, 0, data, dataBuff.Length, sendBuffer.Length);
            _socketServer.Send(connectionId, data);
        }
    }
}