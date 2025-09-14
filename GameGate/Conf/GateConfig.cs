namespace GameGate.Conf
{
    /// <summary>
    /// 网关配置类
    /// </summary>
    public class GateConfig
    {
        /// <summary>
        /// 是否启用OTP动态加密
        /// </summary>
        public bool EnableOtp;
        public bool m_fAddLog;
        /// <summary>
        /// 日志等级
        /// </summary>
        public byte LogLevel;
        /// <summary>
        /// 是否显示Debug日志
        /// </summary>
        public bool Debug;
        /// <summary>
        /// 是否使用云网关
        /// </summary>
        public bool UseCloudGate;
        /// <summary>
        /// 云网关服务地址
        /// </summary>
        public string CloudAddr;
        /// <summary>
        /// 云网关服务地址端口
        /// </summary>
        public int CloudPort;
        /// <summary>
        /// 云网关授权码
        /// </summary>
        public string LicenseCode;
        /// <summary>
        /// 消息工作线程数
        /// 最多255个工作线程
        /// </summary>
        public byte MessageWorkThread;
        /// <summary>
        /// 网关数
        /// 最多50个ServerService线程
        /// </summary>
        public byte ServerWorkThread;
        /// <summary>
        /// 检查空会话
        /// </summary>
        public bool CheckNullSession;
        /// <summary>
        /// 加速方式
        /// </summary>
        public bool IsOverSpeedSendBack;
        /// <summary>
        /// 开启防御CC攻击
        /// </summary>
        public bool IsDefenceCCPacket;
        public bool IsKickOverSpeed;
        public bool IsDenyPresend;
        public bool IsItemSpeedCompensate;
        public bool IsDoMotaeboSpeedCheck;
        public bool IsKickOverPacketSize;
        public int MaxConnectOfIP;
        public int MaxClientCount;
        public int ClientTimeOutTime;
        /// <summary>
        /// 客户端封包大小
        /// </summary>
        public int NomClientPacketSize;
        public int MaxClientPacketSize;
        public int MaxClientPacketCount;
        public string SpaceMoveCommand;
        public string ClientOverCntMsg;
        public string HWIDBlockedMsg;
        public string ChatFilterReplace;
        public string OverSpeedSendBack;
        public string PacketDecryptFailed;
        public string BlockHWIDFileName;
        /// <summary>
        /// 聊天过滤
        /// </summary>
        public bool IsChatFilter;
        /// <summary>
        /// 是否开启发言间隔控制
        /// </summary>
        public bool IsChatInterval;
        /// <summary>
        /// 是否开启发言命令过滤
        /// </summary>
        public bool IsChatCmdFilter;
        /// <summary>
        /// 是否开启转向间隔
        /// </summary>
        public bool IsTurnInterval;
        /// <summary>
        /// 是否开启移动间隔
        /// </summary>
        public bool IsMoveInterval;
        /// <summary>
        /// 是否开启施法间隔
        /// </summary>
        public bool IsSpellInterval;
        /// <summary>
        /// 是否开启攻击间隔
        /// </summary>
        public bool IsAttackInterval;
        /// <summary>
        /// 是否开启挖取间隔
        /// </summary>
        public bool IsButchInterval;
        /// <summary>
        /// 是否开启蹲下间隔
        /// </summary>
        public bool IsSitDownInterval;
        /// <summary>
        /// 使用传送命令之后,多久才能拾取物品  
        /// </summary>
        public bool IsSpaceMoveNextPickupInterval;
        /// <summary>
        /// 是否开启拾取间隔
        /// </summary>
        public bool IsPickupInterval;
        /// <summary>
        /// 是否开启吃东西间隔
        /// </summary>
        public bool IsEatInterval;
        /// <summary>
        /// 是否开启客户端机器码控制
        /// </summary>
        public bool IsProcClientHardwareID;
        /// <summary>
        /// 客户端机器码加密Key
        /// 开启机器码控制后客户端也需修改同步Key,否则将无法进入游戏
        /// </summary>
        public string ProClientHardwareKey;
        /// <summary>
        /// 聊天间隔时间
        /// </summary>
        public int ChatInterval;
        /// <summary>
        /// 转身间隔时间
        /// </summary>
        public int TurnInterval;
        /// <summary>
        /// 移动间隔时间
        /// </summary>
        public int MoveInterval;
        /// <summary>
        /// 施法间隔时间
        /// </summary>
        public int SpellNextInterval;
        /// <summary>
        /// 攻击间隔时间
        /// </summary>
        public int AttackInterval;
        /// <summary>
        /// 挖取间隔时间
        /// </summary>
        public int ButchInterval;
        /// <summary>
        /// 蹲下间隔时间
        /// </summary>
        public int SitDownInterval;
        /// <summary>
        /// 拾取间隔时间
        /// </summary>
        public int PickupInterval;
        /// <summary>
        /// 使用物品间隔时间
        /// </summary>
        public int EatInterval;
        /// <summary>
        /// 移动后对攻击动作的补偿时间，数值越小，封加速越严厉，默认280。
        /// </summary>
        public int MoveNextSpellCompensate;
        /// <summary>
        /// 移动后对魔法动作的补偿时间，数值越小，封加速越严厉，默认80。
        /// </summary>
        public int MoveNextAttackCompensate;
        /// <summary>
        /// 攻击后对移动，魔法等其他动作的补偿时间，数值越小，封加速越严厉，默认200。
        /// </summary>
        public int AttackNextMoveCompensate;
        /// <summary>
        /// 攻击后对移动，魔法等其他动作的补偿时间，数值越小，封加速越严厉，默认200。
        /// </summary>
        public int AttackNextSpellCompensate;
        /// <summary>
        /// 魔法后对移动，攻击等其他动作的补偿时间，数值越小，封加速越严厉，默认200。
        /// </summary>
        public int SpellNextMoveCompensate;
        /// <summary>
        /// 魔法后对移动，攻击等其他动作的补偿时间，数值越小，封加速越严厉，默认200。
        /// </summary>
        public int SpellNextAttackCompensate;
        /// <summary>
        /// 使用Move命令之后拾取物品间隔
        /// </summary>
        public int SpaceMoveNextPickupInterval;
        /// <summary>
        /// 对加速惩罚的基数，数据越大，封加速越严厉，默认0，建议调节到20~120。
        /// </summary>
        public int PunishBaseInterval;
        /// <summary>
        /// 对超速惩罚的倍数，数值越大，封加速越严厉，默认1.00。
        /// 超速越多，游戏越卡
        /// </summary>
        public double PunishIntervalRate;
        public int PunishMoveInterval;
        public int PunishSpellInterval;
        public int PunishAttackInterval;
        public int MaxItemSpeed;
        public int MaxItemSpeedRate;
        /// <summary>
        /// 客户端显示物品方式
        /// </summary>
        public bool ClientShowHintNewType;
        public bool OpenClientSpeedRate;
        public bool SyncClientSpeed;
        public int ClientMoveSpeedRate;
        public int ClientSpellSpeedRate;
        public int ClientAttackSpeedRate;
        public PunishMethod OverSpeedPunishMethod;
        public BlockIPMethod BlockIPMethod;
        public ChatFilterMethod ChatFilterMethod;
        public OverSpeedMsgMethod SpeedHackWarnMethod;

        public GateConfig()
        {
            EnableOtp = false;
            CheckNullSession = true;
            UseCloudGate = false;
            IsOverSpeedSendBack = false;
            IsDefenceCCPacket = false;
            IsKickOverSpeed = false;
            IsDenyPresend = false;
            IsItemSpeedCompensate = false;
            IsDoMotaeboSpeedCheck = true;
            IsKickOverPacketSize = true;
            BlockIPMethod = BlockIPMethod.Disconnect;
            NomClientPacketSize = 400;
            MaxClientPacketSize = 10240;
            MaxConnectOfIP = 50;
            MaxClientCount = 50;
            ClientTimeOutTime = 15 * 1000;
            MaxClientPacketCount = 15;
            OverSpeedSendBack = "[提示]：请爱护游戏环境，关闭加速外挂重新登陆！";
            SpaceMoveCommand = "Move";
            PacketDecryptFailed = "[警告]：游戏连接被断开，请重新登陆！原因：使用非法外挂，客户端不配套，开启的客户端数量过多。";
            ClientOverCntMsg = "开启游戏过多，链接被断开！";
            HWIDBlockedMsg = "机器码已被封，链接被断开！";
            ChatFilterReplace = "说话内容被屏蔽";
            BlockHWIDFileName = Path.Combine(AppContext.BaseDirectory, "BlockHWID.txt");
            IsChatCmdFilter = false;
            IsChatFilter = true;
            IsChatInterval = true;
            IsTurnInterval = true;
            IsMoveInterval = true;
            IsSpellInterval = true;
            IsAttackInterval = true;
            IsButchInterval = true;
            IsSitDownInterval = true;
            IsSpaceMoveNextPickupInterval = true;
            IsPickupInterval = true;
            IsEatInterval = true;
            IsProcClientHardwareID = false;
            ProClientHardwareKey = "openmir2";
            ChatInterval = 800;
            TurnInterval = 350;
            MoveInterval = 570;
            AttackInterval = 900;
            ButchInterval = 450;
            SitDownInterval = 450;
            PickupInterval = 330;
            EatInterval = 330;
            MoveNextSpellCompensate = 100;
            MoveNextAttackCompensate = 250;
            AttackNextMoveCompensate = 200;
            AttackNextSpellCompensate = 200;
            SpellNextMoveCompensate = 200;
            SpellNextAttackCompensate = 200;
            SpaceMoveNextPickupInterval = 600;
            PunishBaseInterval = 20;
            PunishIntervalRate = 1.00;
            OverSpeedPunishMethod = PunishMethod.DelaySend;
            PunishMoveInterval = 150;
            PunishSpellInterval = 150;
            PunishAttackInterval = 150;
            ChatFilterMethod = ChatFilterMethod.ReplaceAll;
            SpeedHackWarnMethod = OverSpeedMsgMethod.ptSysmsg;
            MaxItemSpeed = 6;
            MaxItemSpeedRate = 60;
            ClientShowHintNewType = true;
            OpenClientSpeedRate = false;
            SyncClientSpeed = false;
            ClientMoveSpeedRate = 0;
            ClientSpellSpeedRate = 0;
            ClientAttackSpeedRate = 0;
            Debug = false;
            MessageWorkThread = 1;
        }
    }
}