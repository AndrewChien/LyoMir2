﻿using MQTTnet;
using MQTTnet.Client;
using OpenMir2;
using SystemModule;

namespace ChatSystem
{
    /// <summary>
    /// 公共聊天频道服务类
    /// 简单的设计，后续需要根据聊天频道架构进行修改
    /// </summary>
    public class ChatService : IChatService
    {

        private readonly MqttFactory mqttFactory;
        private readonly IMqttClient chatClient;

        public ChatService()
        {
            mqttFactory = new MqttFactory();
            chatClient = mqttFactory.CreateMqttClient();
            chatClient.DisconnectedAsync += ClientDisconnectedAsync;
            chatClient.ConnectedAsync += ClientConnectedAsync;
        }

        private Task ClientConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            LogService.Info("链接世界聊天频道服务器成功...");
            return Task.CompletedTask;
        }

        public bool IsEnableChatServer
        {
            get { return SystemShare.Config.EnableChatServer; }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (!IsEnableChatServer)
            {
                return;
            }
            LogService.Info("开始链接世界聊天频道...");
            MqttClientOptions chatClientOptions = new MqttClientOptionsBuilder().WithTcpServer(SystemShare.Config.ChatSrvAddr, SystemShare.Config.ChatSrvPort).Build();
            try
            {
                using CancellationTokenSource timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                MqttClientConnectResult response = await chatClient.ConnectAsync(chatClientOptions, cancellationToken);
                if (response.ResultCode == MqttClientConnectResultCode.Success)
                {
                    LogService.Info("链接世界聊天频道成功...");
                }
                else
                {
                    LogService.Info("链接世界聊天频道失败...");
                }
            }
            catch (OperationCanceledException)
            {
                LogService.Warn("链接世界聊天频道超时,请确认配置是否正确.");
                return;
            }
            LogService.Info("链接世界聊天频道初始化完成...");
        }

        private async Task ClientDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            if (arg.ClientWasConnected)
            {
                await chatClient.ConnectAsync(chatClient.Options);
            }
            LogService.Info("与世界聊天频道失去链接...");
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            LogService.Info("断开世界聊天频道...");
            await chatClient.DisconnectAsync(new MqttClientDisconnectOptionsBuilder().WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection).Build(), cancellationToken);
        }

        public async Task Ping()
        {
            if (!IsEnableChatServer)
            {
                return;
            }
            if (!await chatClient.TryPingAsync())
            {
                using (CancellationTokenSource timeout = new CancellationTokenSource(5000))
                {
                    await chatClient.ConnectAsync(chatClient.Options, timeout.Token);
                }
                LogService.Info("与世界聊天频道失去链接...");
            }
        }

        /// <summary>
        /// 发送公共频道消息（世界频道）
        /// 所有玩家可见
        /// </summary>
        /// <param name="sendMsg"></param>
        public void SendPubChannelMessage(string sendMsg)
        {
            //todo 需要对消息加密处理
            chatClient.PublishStringAsync("mir/chat", sendMsg);
        }
    }
}