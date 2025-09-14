using OpenMir2.Data;

namespace M2Server
{
    public interface IAuthentication
    {
        /// <summary>
        /// 初始化账号会话认证服务
        /// </summary>
        void Initialize();
        /// <summary>
        /// 启动账号会话认证服务（GameSvr客户端 --> LoginSvr:5600）
        /// </summary>
        /// <returns></returns>
        Task Start();
        /// <summary>
        /// 运行账号会话认证服务
        /// </summary>
        void Run();
        /// <summary>
        /// 关闭账号会话认证服务
        /// </summary>
        void Close();

        AccountSession GetAdmission(string account, string paddr, int sessionId, ref int payMode, ref int payMent, ref long playTime);

        int GetSessionCount();

        void GetSessionList(IList<AccountSession> sessions);

        void SendHumanLogOutMsg(string userId, int nId);

        void SendHumanLogOutMsgA(string userId, int nId);

        void SendLogonCostMsg(string account, int nTime);

        void SendOnlineHumCountMsg(int nCount);

        void SendSocket(string sendMsg);

        void SendUserPlayTime(string account, long playTime);
    }
}