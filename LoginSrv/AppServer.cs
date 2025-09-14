using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using LoginSrv.Conf;
using LoginSrv.Services;
using LoginSrv.Storage;
using SystemModule;
using Color = Spectre.Console.Color;

namespace LoginSrv
{
    /// <summary>
    /// 注册各服务端（5500、5600）
    /// </summary>
    public class AppServer
    {
        private readonly IHostBuilder builder;
        private readonly IHost host;
        //private readonly ServerHost _serverHost;
        private readonly PeriodicTimer _timer;
        private static Logger LogService;
        private static readonly CancellationTokenSource CancellationToken = new CancellationTokenSource();

        /// <summary>
        /// 注册各服务端（5500、5600）
        /// </summary>
        public AppServer()
        {
            IConfigurationRoot config = new ConfigurationBuilder().Build();
            LogService = LogManager.Setup()
                .SetupExtensions(ext => ext.RegisterConfigSettings(config))
                .GetCurrentClassLogger();

            ThreadPool.SetMaxThreads(200, 200);
            ThreadPool.GetMinThreads(out int workThreads, out int completionPortThreads);
            LogService.Info(new StringBuilder()
                .Append($"ThreadPool.ThreadCount: {ThreadPool.ThreadCount}, ")
                .Append($"Minimum work threads: {workThreads}, ")
                .Append($"Minimum completion port threads: {completionPortThreads})").ToString());

            PrintUsage();

            //Console.CancelKeyPress += delegate
            //{
            //    LsShare.ShowLog = true;
            //    if (_timer != null)
            //    {
            //        _timer.Dispose();
            //    }
            //    AnsiConsole.Reset();
            //};

            builder = new HostBuilder()
                .ConfigureServices((hostContext, service) =>
                {
                    service.AddSingleton(new ConfigManager(Path.Combine(AppContext.BaseDirectory, "logsrv.conf")));
                    service.AddSingleton<SessionServer>();//面向DBSVr、GameSvr的数据服务（开启LoginSrv：5600）
                    service.AddSingleton<ClientSession>();//登录封包消息处理器
                    service.AddSingleton<SessionManager>();//登录session管理器
                    service.AddSingleton<LoginServer>();//面向LoginGate的登录服务（开启LoginSrv：5500）
                    service.AddSingleton<ClientManager>();//登录网关管理器
                    service.AddSingleton<AccountStorage>();//账号服务
                    service.AddHostedService<TimedService>();//定时任务
                    service.AddHostedService<AppService>();//服务管理器
                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    logging.AddNLog(config);
                });
            host = builder.Build();

            //_serverHost = new ServerHost();
            //_serverHost.ConfigureServices(service =>
            //{
            //    service.AddSingleton(new ConfigManager(Path.Combine(AppContext.BaseDirectory, "logsrv.conf")));
            //    service.AddSingleton<SessionServer>();//面向DBSVr、GameSvr的数据服务（开启LoginSrv：5600）
            //    service.AddSingleton<ClientSession>();//登录封包消息处理器
            //    service.AddSingleton<SessionManager>();//登录session管理器
            //    service.AddSingleton<LoginServer>();//面向LoginGate的登录服务（开启LoginSrv：5500）
            //    service.AddSingleton<ClientManager>();//登录网关管理器
            //    service.AddSingleton<AccountStorage>();//账号服务
            //    service.AddHostedService<TimedService>();//定时任务
            //    service.AddHostedService<AppService>();//服务管理器
            //});
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            LogService.Info("正在启动服务器...");
            await host.StartAsync(cancellationToken);

            //_serverHost.BuildHost(AppService.BuildAppService, typeof(AppService));
            ////_serverHost.BuildHost();
            //await _serverHost.StartAsync(cancellationToken);

            await ProcessLoopAsync();
            Stop();
        }

        private static void Stop()
        {
            AnsiConsole.Status().Start("Disconnecting...", ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
            });
        }

        private async Task ProcessLoopAsync()
        {
            string input = null;
            do
            {
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                if (input.StartsWith("/exit") && AnsiConsole.Confirm("Do you really want to exit?"))
                {
                    return;
                }

                string firstTwoCharacters = input[..2];

                if (firstTwoCharacters switch
                {
                    "/s" => ShowServerStatus(),
                    "/c" => ClearConsole(),
                    "/r" => ReLoadConfig(),
                    "/q" => Exit(),
                    _ => null
                } is Task task)
                {
                    await task;
                    continue;
                }

            } while (input is not "/exit");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return host.StopAsync(cancellationToken);
        }

        private Task ReLoadConfig()
        {
            ConfigManager config = new ConfigManager(Path.Combine(AppContext.BaseDirectory, "logsrv.conf"));
            //ConfigManager config = _serverHost.ServiceProvider.GetService<ConfigManager>();
            config?.ReLoadConfig();
            LogService.Info("重新读取配置文件完成...");
            return Task.CompletedTask;
        }

        private static Task Exit()
        {
            Environment.Exit(Environment.ExitCode);
            return Task.CompletedTask;
        }

        private static Task ClearConsole()
        {
            Console.Clear();
            AnsiConsole.Clear();
            return Task.CompletedTask;
        }

        private async Task ShowServerStatus()
        {
            LsShare.ShowLog = false;
            PeriodicTimer periodicTimer = _timer ?? new PeriodicTimer(TimeSpan.FromSeconds(5));
            SessionServer masSocService = (SessionServer)host.Services.GetService(typeof(SessionServer));
            System.Collections.Generic.IList<ServerSessionInfo> serverList = masSocService?.ServerList;
            Table table = new Table().Expand().BorderColor(Color.Grey);
            table.AddColumn("[yellow]Server[/]");
            table.AddColumn("[yellow]EndPoint[/]");
            table.AddColumn("[yellow]Status[/]");
            table.AddColumn("[yellow]Online[/]");

            await AnsiConsole.Live(table)
                 .AutoClear(false)
                 .Overflow(VerticalOverflow.Ellipsis)
                 .Cropping(VerticalOverflowCropping.Top)
                 .StartAsync(async ctx =>
                 {
                     foreach (int _ in Enumerable.Range(0, 10))
                     {
                         table.AddRow(new[] { new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-") });
                     }

                     while (await periodicTimer.WaitForNextTickAsync())
                     {
                         for (int i = 0; i < serverList.Count; i++)
                         {
                             ServerSessionInfo msgServer = serverList[i];
                             if (!string.IsNullOrEmpty(msgServer.ServerName))
                             {
                                 string serverType = msgServer.ServerIndex == 99 ? " (DB)" : " (GameSrv)";
                                 table.UpdateCell(i, 0, $"[bold]{msgServer.ServerName}{serverType}[/]");
                                 table.UpdateCell(i, 1, ($"[bold]{msgServer.EndPoint}[/]"));
                                 if (!msgServer.Socket.Connected)
                                 {
                                     table.UpdateCell(i, 2, ($"[red]Not Connected[/]"));
                                 }
                                 else if ((HUtil32.GetTickCount() - msgServer.KeepAliveTick) < 30000)
                                 {
                                     table.UpdateCell(i, 2, ($"[green]Connected[/]"));
                                 }
                                 else
                                 {
                                     table.UpdateCell(i, 2, ($"[red]Timeout[/]"));
                                 }
                             }
                             table.UpdateCell(i, 3, ($"[bold]{msgServer.OnlineCount}[/]"));
                         }
                         ctx.Refresh();
                     }
                 });
        }

        private void PrintUsage()
        {
            AnsiConsole.WriteLine();

            Table table = new Table()
            {
                Border = TableBorder.None,
                Expand = true,
            }.HideHeaders();
            table.AddColumn(new TableColumn("One"));

            FigletText header = new FigletText("Lyo")
            {
                Color = Color.Fuchsia
            };
            FigletText header2 = new FigletText("LoginSrv")
            {
                Color = Color.Aqua
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("[bold fuchsia]/r[/] [aqua]重读[/] 配置文件\n");
            sb.Append("[bold fuchsia]/c[/] [aqua]清空[/] 清除屏幕\n");
            sb.Append("[bold fuchsia]/q[/] [aqua]退出[/] 退出程序\n");
            Markup markup = new Markup(sb.ToString());

            table.AddColumn(new TableColumn("Two"));

            Table rightTable = new Table()
                .HideHeaders()
                .Border(TableBorder.None)
                .AddColumn(new TableColumn("Content"));

            rightTable.AddRow(header)
                .AddRow(header2)
                .AddEmptyRow()
                .AddEmptyRow()
                .AddRow(markup);
            table.AddRow(rightTable);

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

            //Console.WriteLine();
            //Console.WriteLine(@"    __                 ");
            //Console.WriteLine(@"   / /    __  __  ____ ");
            //Console.WriteLine(@"  / /    / / / / / __ \");
            //Console.WriteLine(@" / /___ / /_/ / / /_/ /");
            //Console.WriteLine(@"/_____/ \__, /  \____/ ");
            //Console.WriteLine(@"       /____/          ");
            //Console.WriteLine(@"    __                     _            _____               ");
            //Console.WriteLine(@"   / /   ____    ____ _   (_)   ____   / ___/   _____ _   __");
            //Console.WriteLine(@"  / /   / __ \  / __ `/  / /   / __ \  \__ \   / ___/| | / /");
            //Console.WriteLine(@" / /___/ /_/ / / /_/ /  / /   / / / / ___/ /  / /    | |/ / ");
            //Console.WriteLine(@"/_____/\____/  \__, /  /_/   /_/ /_/ /____/  /_/     |___/  ");
            //Console.WriteLine(@"              /____/                                        ");
            //Console.WriteLine();
        }
    }
}