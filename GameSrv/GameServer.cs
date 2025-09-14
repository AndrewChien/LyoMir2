using GameSrv.Maps;

namespace GameSrv
{
    /// <summary>
    /// M2核心引擎基类（开启GameSvr:5000，并开启客户端连接LoginSvr:5600、DBSvr:6000）
    /// </summary>
    public class ServerBase
    {
        private readonly IServiceProvider serviceProvider;
        /// <summary>
        /// 核心引擎基类
        /// </summary>
        protected ServerBase(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 启动核心引擎（开启GameSvr:5000，并开启客户端连接LoginSvr:5600、DBSvr:6000）
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public Task StartUp(CancellationToken stoppingToken)
        {
            _ = GameShare.GeneratorProcessor.StartAsync(stoppingToken);
            _ = GameShare.SystemProcess.StartAsync(stoppingToken);
            _ = GameShare.UserProcessor.StartAsync(stoppingToken);
            _ = GameShare.MerchantProcessor.StartAsync(stoppingToken);
            _ = GameShare.EventProcessor.StartAsync(stoppingToken);
            _ = GameShare.CharacterDataProcessor.StartAsync(stoppingToken);
            _ = GameShare.TimedRobotProcessor.StartAsync(stoppingToken);
            _ = GameShare.ActorBuffProcessor.StartAsync(stoppingToken);
            Map.StartMakeStoneThread();

            IEnumerable<IModuleInitializer> modules = serviceProvider.GetServices<IModuleInitializer>();
            foreach (IModuleInitializer module in modules)
            {
                module.Startup(stoppingToken); //启动模块
            }

            _ = GameShare.DataServer.Start();//（GameSvr客户端 --> DBSvr:6000）
            GameShare.PlanesService.Start();
            M2Share.Authentication.Start();//启动账号会话认证服务（GameSvr客户端 --> LoginSvr:5600）
            _ = M2Share.NetChannel.Start(stoppingToken);//启动游戏网关（GameSvr服务端，开启5000端口）

            return Task.CompletedTask;
        }

        public async Task Stopping(CancellationToken cancellationToken)
        {
            IEnumerable<IModuleInitializer> modules = serviceProvider.GetServices<IModuleInitializer>();
            foreach (IModuleInitializer module in modules)
            {
                module.Stopping(cancellationToken);
            }

            await GameShare.GeneratorProcessor.StopAsync(cancellationToken);
            await GameShare.SystemProcess.StopAsync(cancellationToken);
            await GameShare.UserProcessor.StopAsync(cancellationToken);
            await GameShare.MerchantProcessor.StopAsync(cancellationToken);
            await GameShare.EventProcessor.StopAsync(cancellationToken);
            await GameShare.CharacterDataProcessor.StopAsync(cancellationToken);
            await GameShare.TimedRobotProcessor.StopAsync(cancellationToken);
            await GameShare.ActorBuffProcessor.StopAsync(cancellationToken);
            await M2Share.NetChannel.StopAsync(cancellationToken);
            GameShare.DataServer.Stop();

            LogService.Info("游戏世界服务线程停止...");
        }
    }
}