﻿namespace DBSrv.Conf
{
    public class SettingsModel
    {
        public bool ShowDebug = false;
        public int ShowLogLevel = 1;
        /// <summary>
        /// 数据库服务端口
        /// </summary>
        public int ServerPort = 6000;
        /// <summary>
        /// 数据库服务地址
        /// </summary>
        public string ServerAddr = "127.0.0.1";
        /// <summary>
        /// 数据库网关服务端口
        /// </summary>
        public int GatePort = 5100;
        /// <summary>
        /// 数据库网关服务地址
        /// </summary>
        public string GateAddr = "127.0.0.1";
        /// <summary>
        /// 账号服务器端口
        /// </summary>
        public int LoginServerPort = 5600;
        /// <summary>
        /// 账号服务器地址
        /// </summary>
        public string LoginServerAddr = "127.0.0.1";
        /// <summary>
        /// 拍卖行服务器地址
        /// </summary>
        public string MarketServerAddr = "127.0.0.1";
        /// <summary>
        /// 拍卖行服务器端口
        /// </summary>
        public int MarketServerPort = 5700;
        /// <summary>
        /// 同步拍卖行数据间隔(毫秒)
        /// </summary>
        public int PushMarketInterval = 30000;
        /// <summary>
        /// 是否禁止全英文名字
        /// </summary>
        public bool EnglishNames = false;
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName = "热血传奇";
        /// <summary>
        /// 是否禁止检测玩家名字
        /// </summary>
        public bool boDenyChrName = true;
        /// <summary>
        /// 角色删除最小等级，小于该值的角色无法删除
        /// </summary>
        public int DeleteMinLevel = 30;
        public int Interval = 3000;
        /// <summary>
        /// 动态IP模式
        /// </summary>
        public bool DynamicIpMode = false;
        public string MapFile = string.Empty;
        public string StoreageType = "MySQL";
        public string ConnctionString = "server=127.0.0.1;uid=root;pwd=123456;database=mir2_db;";
    }
}