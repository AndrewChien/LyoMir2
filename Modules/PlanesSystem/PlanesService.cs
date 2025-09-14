using OpenMir2;
using SystemModule;

namespace PlanesSystem
{
    /// <summary>
    /// 位面服务
    /// </summary>
    public class PlanesService : IPlanesService
    {
        /// <summary>
        /// 启动位面服务
        /// </summary>
        public void Start()
        {
            if (SystemShare.ServerIndex == 0)
            {
                PlanesServer.Instance.StartPlanesServer();
            }
            else
            {
                PlanesClient.Instance.Initialize();
                PlanesClient.Instance.Start();
                LogService.Info($"节点运行模式...主机端口:[{SystemShare.Config.MasterSrvAddr}:{SystemShare.Config.MasterSrvPort}]");
            }
        }
    }
}