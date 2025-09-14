namespace SystemModule.SubSystem
{
    /// <summary>
    /// 机器人脚本系统(Robots)
    /// </summary>
    public interface IAutoBotSystem
    {
        /// <summary>
        /// 初始化机器人配置
        /// </summary>
        void Initialize();
        /// <summary>
        /// 重新加载机器人配置Robot.txt
        /// </summary>
        void ReLoadRobot();
        /// <summary>
        /// 启动机器人管理器
        /// </summary>
        void Run();
    }
}