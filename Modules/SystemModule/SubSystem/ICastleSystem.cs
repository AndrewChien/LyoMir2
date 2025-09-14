using SystemModule.Actors;
using SystemModule.Castles;
using SystemModule.Maps;

namespace SystemModule.SubSystem
{
    public interface ICastleSystem
    {
        IUserCastle Find(string sCastleName);
        IUserCastle GetCastle(int nIndex);
        void GetCastleGoldInfo(IList<string> List);
        void GetCastleNameList(IList<string> List);
        /// <summary>
        /// 是否沙巴克攻城战役区域
        /// </summary>
        IUserCastle InCastleWarArea(IEnvirnoment Envir, int nX, int nY);
        /// <summary>
        /// 是否沙巴克攻城战役区域
        /// </summary>
        IUserCastle InCastleWarArea(IActor BaseObject);
        void IncRateGold(int nGold);
        /// <summary>
        /// 初始化城堡
        /// </summary>
        void Initialize();
        IUserCastle IsCastleEnvir(IEnvirnoment envir);
        IUserCastle IsCastleMember(IPlayerActor playObject);
        IUserCastle IsCastlePalaceEnvir(IEnvirnoment Envir);
        /// <summary>
        /// 加载城堡列表
        /// </summary>
        void LoadCastleList();
        void Run();
        void Save();
    }
}