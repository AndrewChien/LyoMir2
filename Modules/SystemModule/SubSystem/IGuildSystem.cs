using SystemModule.Castles;

namespace SystemModule.SubSystem
{
    public interface IGuildSystem
    {
        /// <summary>
        /// 添加行会
        /// </summary>
        /// <param name="sGuildName"></param>
        /// <param name="sChief"></param>
        /// <returns></returns>
        bool AddGuild(string sGuildName, string sChief);
        /// <summary>
        /// 清空行会列表
        /// </summary>
        void ClearGuildInf();
        /// <summary>
        /// 删除行会
        /// </summary>
        /// <param name="sGuildName"></param>
        /// <returns></returns>
        bool DelGuild(string sGuildName);
        /// <summary>
        /// 查找行会
        /// </summary>
        IGuild FindGuild(string sGuildName);
        /// <summary>
        /// 加载行会信息
        /// </summary>
        void LoadGuildInfo();

        IGuild MemberOfGuild(string sName);

        void Run();
    }
}
