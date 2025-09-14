using OpenMir2.Data;
using SystemModule.Actors;
using SystemModule.Maps;

namespace SystemModule.SubSystem
{
    public interface IMapSystem
    {
        IList<IEnvirnoment> Maps { get; }
        void AddMapInfo(string sMapName, string sMapDesc, byte nServerNumber, MapInfoFlag mapFlag, IMerchant questNpc);
        bool AddMapRoute(string sSMapNo, int nSMapX, int nSMapY, string sDMapNo, int nDMapX, int nDMapY);
        IEnvirnoment FindMap(string sMapName);
        IList<IEnvirnoment> GetDoorMapList();
        IEnvirnoment GetMapInfo(int nServerIdx, string sMapName);
        int GetMapOfServerIndex(string sMapName);
        IList<IEnvirnoment> GetMineMaps();
        /// <summary>
        /// 加载地图出入口
        /// </summary>
        void LoadMapDoor();
        /// <summary>
        /// 地图安全区
        /// </summary>
        void MakeSafePkZone();
    }
}
