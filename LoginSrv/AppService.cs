using LoginSrv.Conf;
using LoginSrv.Services;
using LoginSrv.Storage;

namespace LoginSrv
{
    /// <summary>
    /// 服务管理器
    /// </summary>
    public class AppService : IHostedLifecycleService
    {
        private readonly ConfigManager _configManager;
        private readonly SessionServer _masSocService;
        private readonly LoginServer _loginService;
        private readonly AccountStorage _accountStorage;

        /// <summary>
        /// lyo
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static AppService BuildAppService(IServiceProvider ip)
        {
            var cnfm = new ConfigManager(Path.Combine(AppContext.BaseDirectory, "logsrv.conf"));
            var clnm = new ClientManager();
            var ssnm = new SessionManager();
            var actm = new AccountStorage(cnfm);
            var ssns = new SessionServer(cnfm, actm);
            var clns = new ClientSession(actm, cnfm, ssns, clnm, ssnm);
            var lgnm = new LoginServer(cnfm, clns, clnm, ssnm);
            return new AppService(ssns,lgnm,actm,cnfm);
        }

        /// <summary>
        /// 服务管理器
        /// </summary>
        /// <param name="masSocService"></param>
        /// <param name="loginService"></param>
        /// <param name="accountStorage"></param>
        /// <param name="configManager"></param>
        public AppService(SessionServer masSocService, LoginServer loginService, AccountStorage accountStorage, ConfigManager configManager)
        {
            _masSocService = masSocService;
            _loginService = loginService;
            _accountStorage = accountStorage;
            _configManager = configManager;
        }

        public Task StartingAsync(CancellationToken cancellationToken)
        {
            LsShare.Initialization();
            _configManager.LoadConfig();
            _configManager.LoadAddrTable();
            return Task.CompletedTask;
        }

        public Task StartedAsync(CancellationToken cancellationToken)
        {
            _loginService.Start(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _loginService.StartServer();
            _masSocService.StartServer();
            _accountStorage.Initialization();
            LogService.Info("服务已启动成功...");
            LogService.Info("欢迎使用LYO引擎...");
            LogService.Info("网站:http://www.chengxihot.top");
            LogService.Info("论坛:http://bbs.chengxihot.top");
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _loginService.StopServer();
            _masSocService.StopServer();
            return Task.CompletedTask;
        }
    }
}