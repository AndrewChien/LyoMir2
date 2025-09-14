﻿using M2Server.Event.Events;
using M2Server.Maps;
using SystemModule.Actors;
using SystemModule.Maps;

namespace GameSrv.Maps
{
    public class MapManager : IMapSystem
    {
        private readonly Dictionary<string, IEnvirnoment> _mapList = new Dictionary<string, IEnvirnoment>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 地图上门列表
        /// </summary>
        private readonly List<IEnvirnoment> _mapDoorList = new List<IEnvirnoment>();
        /// <summary>
        /// 矿物地图列表
        /// </summary>
        private readonly List<IEnvirnoment> _mapMineList = new List<IEnvirnoment>();

        public IList<IEnvirnoment> Maps => _mapList.Values.ToList();

        /// <summary>
        /// 地图安全区
        /// </summary>
        public void MakeSafePkZone()
        {
            for (int i = 0; i < M2Share.StartPointList.Count; i++)
            {
                StartPoint startPoint = M2Share.StartPointList[i];
                if (string.IsNullOrEmpty(startPoint.MapName) && startPoint.Type > 0)
                {
                    IEnvirnoment envir = FindMap(startPoint.MapName);
                    if (envir != null)
                    {
                        short nMinX = (short)(startPoint.CurrX - startPoint.Range);
                        short nMaxX = (short)(startPoint.CurrX + startPoint.Range);
                        short nMinY = (short)(startPoint.CurrY - startPoint.Range);
                        short nMaxY = (short)(startPoint.CurrY + startPoint.Range);
                        for (short nX = nMinX; nX <= nMaxX; nX++)
                        {
                            for (short nY = nMinY; nY <= nMaxY; nY++)
                            {
                                if (nX < nMaxX && nY == nMinY || nY < nMaxY && nX == nMinX || nX == nMaxX || nY == nMaxY)
                                {
                                    SystemShare.EventMgr.AddEvent(new SafeEvent(envir, nX, nY, (byte)startPoint.Type));
                                }
                            }
                        }
                    }
                }
            }
        }

        public IList<IEnvirnoment> GetMineMaps()
        {
            return _mapMineList;
        }

        public IList<IEnvirnoment> GetDoorMapList()
        {
            return _mapDoorList;
        }

        public void AddMapInfo(string sMapName, string sMapDesc, byte nServerNumber, MapInfoFlag mapFlag, IMerchant questNpc)
        {
            string sMapFileName = string.Empty;
            string sTempName = sMapName;
            if (sTempName.IndexOf('|') > -1)
            {
                sMapFileName = HUtil32.GetValidStr3(sTempName, ref sMapName, '|');
            }
            else
            {
                sTempName = HUtil32.ArrestStringEx(sTempName, "<", ">", ref sMapFileName);
                if (string.IsNullOrEmpty(sMapFileName))
                {
                    sMapFileName = sMapName;
                }
                else
                {
                    sMapName = sTempName;
                }
            }
            IEnvirnoment envirnoment = new Envirnoment
            {
                MapName = sMapName,
                MapFileName = sMapFileName,
                MapDesc = sMapDesc,
                ServerIndex = nServerNumber,
                Flag = mapFlag,
            };
            if (M2Share.MiniMapList.TryGetValue(envirnoment.MapName, out short minMap))
            {
                envirnoment.MinMap = minMap;
            }
            if (envirnoment.LoadMapData(Path.Combine(M2Share.BasePath, SystemShare.Config.MapDir, sMapFileName + ".map")))
            {
                if (!_mapList.ContainsKey(sMapName))
                {
                    _mapList.Add(sMapName, envirnoment);
                }
                else
                {
                    LogService.Error("地图名称重复 [" + sMapName + "]，请确认配置文件是否正确.");
                }
                if (envirnoment.DoorList.Count > 0)
                {
                    _mapDoorList.Add(envirnoment);
                }
                if (envirnoment.Flag.Mine || envirnoment.Flag.boMINE2)
                {
                    _mapMineList.Add(envirnoment);
                }
            }
            else
            {
                LogService.Error("地图文件:" + sMapName + ".map" + "未找到,或者加载出错!!!");
            }
        }

        /// <summary>
        /// 添加地图链接点
        /// </summary>
        /// <returns></returns>
        public bool AddMapRoute(string sSMapNo, int nSMapX, int nSMapY, string sDMapNo, int nDMapX, int nDMapY)
        {
            bool result = false;
            IEnvirnoment sEnvir = FindMap(sSMapNo);
            IEnvirnoment dEnvir = FindMap(sDMapNo);
            if (sEnvir != null && dEnvir != null)
            {
                MapRouteItem mapRoute = new MapRouteItem
                {
                    RouteId = SystemShare.ActorMgr.GetNextIdentity(),
                    Flag = false,
                    Envir = dEnvir,
                    X = (short)nDMapX,
                    Y = (short)nDMapY
                };
                sEnvir.AddMapRoute(nSMapX, nSMapY, mapRoute);
                result = true;
            }
            return result;
        }

        public IEnvirnoment FindMap(string sMapName)
        {
            if(string.IsNullOrEmpty(sMapName))
            {
                return null;
            }
            if(_mapList.TryGetValue(sMapName, out IEnvirnoment map))
            {
                return map;
            }
            else
            {
                Console.WriteLine($"[{sMapName} 防错增补]");
                return null;
            }
            //return _mapList.TryGetValue(sMapName, out IEnvirnoment map) ? map : null;
        }

        public IEnvirnoment GetMapInfo(int nServerIdx, string sMapName)
        {
            IEnvirnoment result = null;
            if (_mapList.TryGetValue(sMapName, out IEnvirnoment envirnoment))
            {
                if (envirnoment.ServerIndex == nServerIdx)
                {
                    result = envirnoment;
                }
            }
            if(result == null)
            {
                Console.WriteLine($"[{sMapName} 防错增补]");
            }
            return result;
        }

        /// <summary>
        /// 取地图编号服务器
        /// </summary>
        /// <param name="sMapName"></param>
        /// <returns></returns>
        public int GetMapOfServerIndex(string sMapName)
        {
            if (string.IsNullOrEmpty(sMapName))
            {
                return 0;
            }
            if (_mapList.TryGetValue(sMapName, out IEnvirnoment envirnoment))
            {
                return envirnoment.ServerIndex;
            }
            return 0;
        }
        /// <summary>
        /// 加载地图出入口
        /// </summary>
        public void LoadMapDoor()
        {
            for (int i = 0; i < Maps.Count; i++)
            {
                this.Maps[i].AddDoorToMap();
            }
            LogService.Info("地图环境加载成功...");
        }

        public static void ProcessMapDoor()
        {

        }

        public static void ReSetMinMap()
        {
            // for (var I = 0; I < this.Count; I ++ )
            // {
            //     var Envirnoment = ((this.Items[I]) as TEnvirnoment);
            //     for (var II = 0; II < M2Share.MiniMapList.Count; II ++ )
            //     {
            //         if ((M2Share.MiniMapList[II]).CompareTo((Envirnoment.sMapName)) == 0)
            //         {
            //             Envirnoment.nMinMap = ((int)M2Share.MiniMapList.Values[II]);
            //             break;
            //         }
            //     }
            // }
        }

        public static void Run()
        {

        }
    }
}