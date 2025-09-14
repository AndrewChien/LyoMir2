using Microsoft.Extensions.Hosting;
using OpenMir2;
using SelGate.Conf;
using SelGate.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SelGate
{
    /// <summary>
    /// 服务管理器
    /// </summary>
    public class AppService : IHostedLifecycleService
    {
        private readonly ConfigManager _configManager;
        private readonly ServerService _serverService;
        private readonly ClientManager _clientManager;
        private readonly SessionManager _sessionManager;

        /// <summary>
        /// lyo
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static AppService BuildAppService(IServiceProvider ip)
        {
            var cnfm = new ConfigManager(Path.Combine(AppContext.BaseDirectory, "config.conf"));
            var ssm = new SessionManager();
            var cm = new ClientManager(ssm, cnfm);
            var ss = new ServerService(ssm, cm, cnfm);
            return new AppService(cnfm, ss, cm, ssm);
        }

        /// <summary>
        /// 服务管理器
        /// </summary>
        /// <param name="configManager"></param>
        /// <param name="serverService"></param>
        /// <param name="clientManager"></param>
        /// <param name="sessionManager"></param>
        public AppService(ConfigManager configManager, ServerService serverService, ClientManager clientManager, SessionManager sessionManager)
        {
            _configManager = configManager;
            _serverService = serverService;
            _clientManager = clientManager;
            _sessionManager = sessionManager;
        }

        public Task StartingAsync(CancellationToken cancellationToken)
        {
            GateShare.Initialization();
            LogService.Info("正在启动服务...");
            _configManager.LoadConfig();
            _clientManager.Initialization();
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _serverService.ProcessReviceMessage(cancellationToken);
            _sessionManager.ProcessSendMessage(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StartedAsync(CancellationToken cancellationToken)
        {
            _serverService.Start();
            _clientManager.Start();
            LogService.Info("服务已启动成功...");
            LogService.Info("欢迎使用LYO引擎...");
            LogService.Info("网站:http://www.chengxihot.top");
            LogService.Info("论坛:http://bbs.chengxihot.top");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            LogService.Info("SelGate is stopping.");
            LogService.Info("正在停止服务...");
            _serverService.Stop();
            _clientManager.Stop();
            LogService.Info("服务停止成功...");
            return Task.CompletedTask;
        }

        public Task StoppingAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StoppedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}