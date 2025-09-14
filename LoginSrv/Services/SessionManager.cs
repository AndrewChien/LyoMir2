namespace LoginSrv.Services
{
    /// <summary>
    /// 登录session管理器
    /// </summary>
    public class SessionManager
    {
        private readonly Dictionary<int, SessionConnInfo> sessionMap = new Dictionary<int, SessionConnInfo>();
        private readonly Dictionary<string, SessionConnInfo> sessionAccountMap = new Dictionary<string, SessionConnInfo>();

        public void AddSession(int sessionId, SessionConnInfo sessionConnInfo)
        {
            sessionMap.Add(sessionId, sessionConnInfo);
            sessionAccountMap.Add(sessionConnInfo.Account, sessionConnInfo);
        }

        public SessionConnInfo GetSession(string account)
        {
            return sessionAccountMap[account];
        }

        public void UpdateSession(int sessionId, string sServerName, bool isPayMent)
        {
            if (sessionMap.ContainsKey(sessionId))
            {
                sessionMap[sessionId].ServerName = sServerName;
                sessionMap[sessionId].IsPayMent = isPayMent;
            }
        }

        public bool IsLogin(int sessionId)
        {
            if (sessionMap.ContainsKey(sessionId))
            {
                return true;
            }
            return false;
        }

        public bool IsLogin(string sessionId)
        {
            if (sessionAccountMap.ContainsKey(sessionId))
            {
                return true;
            }
            return false;
        }

        public void Delete(string account, int sessionId)
        {
            if (sessionMap.Remove(sessionId))
            {
                sessionAccountMap.Remove(account);
            }
        }

        public SessionConnInfo[] GetSessions()
        {
            return sessionMap.Count > 0 ? sessionMap.Values.ToArray() : null;
        }
    }
}