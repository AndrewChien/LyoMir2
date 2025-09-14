namespace DBSrv.Conf
{
    public class AppConfigManager : ConfigFile
    {
        private readonly SettingsModel _config = new SettingsModel();
        private static readonly string ConfitFile = Path.Combine(AppContext.BaseDirectory, "dbsvr.conf");

        public AppConfigManager() : base(ConfitFile)
        {
            Load();
        }

        public SettingsModel Settings => _config;

        public void LoadConfig()
        {
            _config.ConnctionString = ReadWriteString("DataBase", "ConnctionString", _config.ConnctionString);
            _config.StoreageType = ReadWriteString("DataBase", "Storeage", _config.StoreageType);
            _config.ShowDebug = ReadWriteBool("Setup", "ShowDebug", _config.ShowDebug);
            _config.ServerPort = ReadWriteInteger("Setup", "ServerPort", _config.ServerPort);
            _config.ServerAddr = ReadWriteString("Setup", "ServerAddr", _config.ServerAddr);
            _config.GatePort = ReadWriteInteger("Setup", "GatePort", _config.GatePort);
            _config.GateAddr = ReadWriteString("Setup", "GateAddr", _config.GateAddr);
            _config.LoginServerAddr = ReadWriteString("Server", "IDSAddr", _config.LoginServerAddr);
            _config.LoginServerPort = ReadWriteInteger("Server", "IDSPort", _config.LoginServerPort);
            _config.MarketServerAddr = ReadWriteString("Server", "MarketAddr", _config.MarketServerAddr);
            _config.MarketServerPort = ReadWriteInteger("Server", "MarketPort", _config.MarketServerPort);
            _config.PushMarketInterval = ReadWriteInteger("Server", "PushMarketInterval", _config.PushMarketInterval);
            _config.ServerName = ReadWriteString("Setup", "ServerName", _config.ServerName);
            _config.boDenyChrName = ReadWriteBool("Setup", "DenyChrName", _config.boDenyChrName);
            _config.DeleteMinLevel = ReadWriteInteger("Setup", "DELMaxLevel", _config.DeleteMinLevel);
            _config.Interval = ReadWriteInteger("DBClear", "Interval", _config.Interval);
            int dynamicIpMode = ReadWriteInteger("Setup", "DynamicIPMode", -1);
            if (dynamicIpMode < 0)
            {
                WriteBool("Setup", "DynamicIPMode", _config.DynamicIpMode);
            }
            else
            {
                _config.DynamicIpMode = dynamicIpMode == 1;
            }
            _config.EnglishNames = ReadWriteBool("Setup", "EnglishNameOnly", _config.EnglishNames);
            _config.MapFile = ReadWriteString("Setup", "MapFile", _config.MapFile);
        }
    }
}