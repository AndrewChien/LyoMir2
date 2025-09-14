using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using GameSrv.Module;
using MediatR;

namespace GameSrv
{
    /// <summary>
    /// 注册各客户端及服务端（5600、6000，5000）
    /// </summary>
    public class AppServer
    {
        private readonly IHostBuilder builder;
        private readonly IHost host;
        //private readonly ServerHost _serverHost;
        private static Logger LogService;
        private static readonly CancellationTokenSource CancellationToken = new CancellationTokenSource();

        /// <summary>
        /// 注册各客户端及服务端（5600、6000，5000）
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

            builder = new HostBuilder()
                .ConfigureServices((hostContext, service) =>
                {
                    //1、加载插件进GameShare.Modules中
                    service.AddModules();
                    //2、先注册GameApp，初始化M2Share实例和SystemShare实例
                    service.AddSingleton<GameApp>();
                    //3、再注册AppService，AddHostedService：该服务被添加到应用程序中，并且主机在启动时会自动开启该服务
                    service.AddHostedService<AppService>();
                    //4、注册TimedService
                    service.AddHostedService<TimedService>();
                    //5、为MediatR插件（一个进程内消息传递的发布订阅中介）注册中介者服务，用在交易NPC发插件消息（Merchant.UserSelect）
                    service.AddScoped<IMediator, Mediator>();
                    //6、配置GameShare.Modules中已加载的插件为单例模式
                    foreach (ModuleInfo module in GameShare.Modules)
                    {
                        Type moduleInitializerType = module.Assembly.GetTypes().FirstOrDefault(t => typeof(IModuleInitializer).IsAssignableFrom(t));
                        if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer)))
                        {
                            IModuleInitializer moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
                            service.AddSingleton(typeof(IModuleInitializer), moduleInitializer);
                            moduleInitializer.ConfigureServices(service);
                        }
                    }
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
            //    //1、加载插件进GameShare.Modules中
            //    service.AddModules();
            //    //2、先注册GameApp，初始化M2Share实例和SystemShare实例
            //    service.AddSingleton<GameApp>();
            //    //3、再注册AppService，AddHostedService：该服务被添加到应用程序中，并且主机在启动时会自动开启该服务
            //    service.AddHostedService<AppService>();
            //    //4、注册TimedService
            //    service.AddHostedService<TimedService>();
            //    //5、为MediatR插件（一个进程内消息传递的发布订阅中介）注册中介者服务，用在交易NPC发插件消息（Merchant.UserSelect）
            //    service.AddScoped<IMediator, Mediator>();
            //    //6、配置GameShare.Modules中已加载的插件为单例模式
            //    foreach (ModuleInfo module in GameShare.Modules)
            //    {
            //        Type moduleInitializerType = module.Assembly.GetTypes().FirstOrDefault(t => typeof(IModuleInitializer).IsAssignableFrom(t));
            //        if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer)))
            //        {
            //            IModuleInitializer moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
            //            service.AddSingleton(typeof(IModuleInitializer), moduleInitializer);
            //            moduleInitializer.ConfigureServices(service);
            //        }
            //    }
            //});
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //_serverHost.BuildHost(AppService.BuildAppService, typeof(AppService));
            ////_serverHost.BuildHost();
            //await _serverHost.StartAsync(cancellationToken);

            await host.StartAsync(cancellationToken);

            await ProcessLoopAsync();
            Stop();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await host.StopAsync(cancellationToken);
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
                    //"/r" => ReLoadConfig(),
                    "/q" => Exit(),
                    _ => null
                } is Task task)
                {
                    await task;
                    continue;
                }

            } while (input is not "/exit");
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

        private static void Stop()
        {
            AnsiConsole.Status().Start("Disconnecting...", ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
            });
        }

        private async Task ShowServerStatus()
        {
            //LsShare.ShowLog = false;
            //PeriodicTimer periodicTimer = _timer ?? new PeriodicTimer(TimeSpan.FromSeconds(5));
            //SessionServer masSocService = (SessionServer)host.Services.GetService(typeof(SessionServer));
            //System.Collections.Generic.IList<ServerSessionInfo> serverList = masSocService?.ServerList;
            //Table table = new Table().Expand().BorderColor(Color.Grey);
            //table.AddColumn("[yellow]Server[/]");
            //table.AddColumn("[yellow]EndPoint[/]");
            //table.AddColumn("[yellow]Status[/]");
            //table.AddColumn("[yellow]Online[/]");

            //await AnsiConsole.Live(table)
            //     .AutoClear(false)
            //     .Overflow(VerticalOverflow.Ellipsis)
            //     .Cropping(VerticalOverflowCropping.Top)
            //     .StartAsync(async ctx =>
            //     {
            //         foreach (int _ in Enumerable.Range(0, 10))
            //         {
            //             table.AddRow(new[] { new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-") });
            //         }

            //         while (await periodicTimer.WaitForNextTickAsync())
            //         {
            //             for (int i = 0; i < serverList.Count; i++)
            //             {
            //                 ServerSessionInfo msgServer = serverList[i];
            //                 if (!string.IsNullOrEmpty(msgServer.ServerName))
            //                 {
            //                     string serverType = msgServer.ServerIndex == 99 ? " (DB)" : " (GameSrv)";
            //                     table.UpdateCell(i, 0, $"[bold]{msgServer.ServerName}{serverType}[/]");
            //                     table.UpdateCell(i, 1, ($"[bold]{msgServer.EndPoint}[/]"));
            //                     if (!msgServer.Socket.Connected)
            //                     {
            //                         table.UpdateCell(i, 2, ($"[red]Not Connected[/]"));
            //                     }
            //                     else if ((HUtil32.GetTickCount() - msgServer.KeepAliveTick) < 30000)
            //                     {
            //                         table.UpdateCell(i, 2, ($"[green]Connected[/]"));
            //                     }
            //                     else
            //                     {
            //                         table.UpdateCell(i, 2, ($"[red]Timeout[/]"));
            //                     }
            //                 }
            //                 table.UpdateCell(i, 3, ($"[bold]{msgServer.OnlineCount}[/]"));
            //             }
            //             ctx.Refresh();
            //         }
            //     });
        }
    }
}