using OpenMir2.Packets.ServerPackets;
using SystemModule.Actors;

namespace M2Server.Net
{
    /// <summary>
    /// M2核心通讯服务（GameSvr服务端，开启5000端口）
    /// </summary>
    public interface INetChannel
    {
        void AddGameGateQueue(int gateIdx, ServerMessage packet, byte[] data);

        void AddGateBuffer(int gateIdx, byte[] senData);

        void CloseAllGate();

        void CloseUser(int gateIdx, int nSocket);
        /// <summary>
        /// 初始化游戏网关（5000端口）
        /// </summary>
        void Initialize();
        /// <summary>
        /// 踢账号
        /// </summary>
        void KickUser(string account, int sessionId, int payMode);
        /// <summary>
        /// 运行游戏网关
        /// </summary>
        void Run();

        void SendOutConnectMsg(int gateIdx, int nSocket, ushort nGsIdx);

        void SendServerStopMsg();

        void SetGateUserList(int gateIdx, int nSocket, IPlayerActor playObject);
        /// <summary>
        /// 启动游戏网关（GameSvr服务端，开启5000端口）
        /// </summary>
        Task Start(CancellationToken cancellationToken = default);

        void Send(string connectId, byte[] buff);
        /// <summary>
        /// 关闭游戏网关
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="endPoint"></param>
        void CloseGate(string connectionId, string endPoint);

        Task StopAsync(CancellationToken cancellationToken = default);
    }
}