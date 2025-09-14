using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using OpenMir2;
using SelGate.Conf;
using SelGate.Services;
using Spectre.Console;
using System;
using System.IO;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemModule;

namespace SelGate
{
    /// <summary>
    /// 注册各客户端及服务端（5100，7100）
    /// </summary>
    public class AppServer
    {
        private readonly IHostBuilder builder;
        private readonly IHost host;
        //private readonly ServerHost _serverHost;
        private static readonly PeriodicTimer _timer;
        private static Logger LogService;

        /// <summary>
        /// 注册各客户端及服务端（5100，7100）
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
            //    if (_timer != null)
            //    {
            //        _timer.Dispose();
            //    }
            //    AnsiConsole.Reset();
            //};

            builder = new HostBuilder()
                .ConfigureServices((hostContext, service) =>
                {
                    service.AddSingleton(new ConfigManager(Path.Combine(AppContext.BaseDirectory, "config.conf")));//读取config.conf
                    service.AddSingleton<ServerService>();//客户端选角色服务（开启SelGate：7100）
                    service.AddSingleton<SessionManager>();//会话管理器
                    service.AddSingleton<ClientManager>();//客户端选角消息转发至GameSrv
                    service.AddHostedService<AppService>();//服务管理器
                    service.AddHostedService<TimedService>();//定时任务
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
            //    service.AddSingleton(new ConfigManager(Path.Combine(AppContext.BaseDirectory, "config.conf")));//读取config.conf
            //    service.AddSingleton<ServerService>();//客户端选角色服务（开启SelGate：7100）
            //    service.AddSingleton<SessionManager>();//会话管理器
            //    service.AddSingleton<ClientManager>();//客户端选角消息转发至GameSrv
            //    service.AddHostedService<AppService>();//服务管理器
            //    service.AddHostedService<TimedService>();//定时任务
            //});
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
            FigletText header2 = new FigletText("SelGate")
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

            //Console.WriteLine(@"    __                 ");
            //Console.WriteLine(@"   / /    __  __  ____ ");
            //Console.WriteLine(@"  / /    / / / / / __ \");
            //Console.WriteLine(@" / /___ / /_/ / / /_/ /");
            //Console.WriteLine(@"/_____/ \__, /  \____/ ");
            //Console.WriteLine(@"       /____/          ");
            //Console.WriteLine(@"   _____          __   ______           __       ");
            //Console.WriteLine(@"  / ___/  ___    / /  / ____/  ____ _  / /_  ___ ");
            //Console.WriteLine(@"  \__ \  / _ \  / /  / / __   / __ `/ / __/ / _ \");
            //Console.WriteLine(@" ___/ / /  __/ / /  / /_/ /  / /_/ / / /_  /  __/");
            //Console.WriteLine(@"/____/  \___/ /_/   \____/   \__,_/  \__/  \___/ ");
            //Console.WriteLine(@"                                                 ");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //_serverHost.BuildHost(AppService.BuildAppService, typeof(AppService));
            ////_serverHost.BuildHost();
            //await _serverHost.StartAsync(cancellationToken);
            //await builder.StartAsync(cancellationToken);

            GateShare.ServiceProvider = host.Services;
            await host.StartAsync(cancellationToken);
            await ProcessLoopAsync();
            Stop();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return host.StopAsync(cancellationToken);
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

        private Task ReLoadConfig()
        {
            ConfigManager config = new ConfigManager(Path.Combine(AppContext.BaseDirectory, "config.conf"));
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

        private static Task ShowServerStatus()
        {
            //GateShare.ShowLog = false;
            //_timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
            //var serverList = ServerManager.Instance.GetServerList();
            //var table = new Table().Expand().BorderColor(Color.Grey);
            //table.AddColumn("[yellow]Address[/]");
            //table.AddColumn("[yellow]Port[/]");
            //table.AddColumn("[yellow]Status[/]");
            //table.AddColumn("[yellow]Online[/]");
            //table.AddColumn("[yellow]Send[/]");
            //table.AddColumn("[yellow]Revice[/]");
            //table.AddColumn("[yellow]Queue[/]");

            //await AnsiConsole.Live(table)
            //     .AutoClear(true)
            //     .Overflow(VerticalOverflow.Crop)
            //     .Cropping(VerticalOverflowCropping.Bottom)
            //     .StartAsync(async ctx =>
            //     {
            //         foreach (var _ in Enumerable.Range(0, 10))
            //         {
            //             table.AddRow(new[] { new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-") });
            //         }

            //         while (await _timer.WaitForNextTickAsync(cts.Token))
            //         {
            //             for (int i = 0; i < serverList.Count; i++)
            //             {
            //                 var (serverIp, serverPort, Status, playCount, reviceTotal, sendTotal, queueCount) = serverList[i].GetStatus();

            //                 table.UpdateCell(i, 0, $"[bold]{serverIp}[/]");
            //                 table.UpdateCell(i, 1, ($"[bold]{serverPort}[/]"));
            //                 table.UpdateCell(i, 2, ($"[bold]{Status}[/]"));
            //                 table.UpdateCell(i, 3, ($"[bold]{playCount}[/]"));
            //                 table.UpdateCell(i, 4, ($"[bold]{sendTotal}[/]"));
            //                 table.UpdateCell(i, 5, ($"[bold]{reviceTotal}[/]"));
            //                 table.UpdateCell(i, 6, ($"[bold]{queueCount}[/]"));
            //             }
            //             ctx.Refresh();
            //         }
            //     });
            return Task.CompletedTask;
        }
    }
}