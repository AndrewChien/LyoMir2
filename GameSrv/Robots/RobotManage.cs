namespace GameSrv.Robots
{
    public class RobotManage : IAutoBotSystem
    {

        private readonly IList<RobotObject> RobotHumanList;

        public RobotManage()
        {
            RobotHumanList = new List<RobotObject>();
            LoadRobot();
        }

        ~RobotManage()
        {
            UnLoadRobot();
        }

        public IList<RobotObject> Robots => RobotHumanList;

        /// <summary>
        /// 加载机器人配置Robot.txt
        /// </summary>
        private void LoadRobot()
        {
            string sRobotName = string.Empty;
            string sScriptFileName = string.Empty;
            string sFileName = M2Share.GetEnvirFilePath("Robot.txt");
            if (!File.Exists(sFileName))
            {
                return;
            }

            using StringList LoadList = new StringList();
            LoadList.LoadFromFile(sFileName);
            for (int i = 0; i < LoadList.Count; i++)
            {
                string sLineText = LoadList[i];
                if (string.IsNullOrEmpty(sLineText) || sLineText[0] == ';')
                {
                    continue;
                }

                sLineText = HUtil32.GetValidStr3(sLineText, ref sRobotName, new[] { ' ', '/', '\t' });
                sLineText = HUtil32.GetValidStr3(sLineText, ref sScriptFileName, new[] { ' ', '/', '\t' });
                if (string.IsNullOrEmpty(sRobotName) || string.IsNullOrEmpty(sScriptFileName))
                {
                    continue;
                }

                RobotObject robotHuman = new RobotObject();
                robotHuman.ChrName = sRobotName;
                robotHuman.ScriptFileName = sScriptFileName;
                robotHuman.LoadScript();
                RobotHumanList.Add(robotHuman);
            }
        }
        /// <summary>
        /// 重新加载机器人配置Robot.txt
        /// </summary>
        public void ReLoadRobot()
        {
            UnLoadRobot();
            LoadRobot();
        }
        /// <summary>
        /// 清空机器人列表
        /// </summary>
        private void UnLoadRobot()
        {
            for (int i = 0; i < RobotHumanList.Count; i++)
            {
                RobotHumanList[i] = null;
            }
            RobotHumanList.Clear();
        }
        /// <summary>
        /// 启动机器人管理器
        /// </summary>
        public void Run()
        {
            for (int i = 0; i < RobotHumanList.Count; i++)
            {
                RobotHumanList[i].Run();
            }
        }
        /// <summary>
        /// 初始化机器人配置
        /// </summary>
        public void Initialize()
        {
            for (int i = 0; i < RobotHumanList.Count; i++)
            {
                RobotHumanList[i].Initialize();
            }
        }
    }
}