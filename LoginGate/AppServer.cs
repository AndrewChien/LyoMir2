using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using SystemModule;
using TouchSocket.Sockets;
using Color = Spectre.Console.Color;

/// <summary>
/// 注册各客户端及服务端（5500，7000）
/// </summary>
public class AppServer
{
    private readonly IHostBuilder builder;
    private readonly IHost host;
    //private readonly ServerHost _serverHost;
    private PeriodicTimer _timer;
    private static Logger LogService;
    private static readonly CancellationTokenSource CancellationToken = new CancellationTokenSource();

    /// <summary>
    /// 注册各客户端及服务端（5500，7000）
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

        builder = new HostBuilder()
                .ConfigureServices((hostContext, service) =>
                {
                    service.AddSingleton(new ConfigManager(Path.Combine(AppContext.BaseDirectory, "config.conf")));//读取config.conf
                    service.AddSingleton<ServerService>();//登录服务（开启LoginGate：7000）
                    service.AddSingleton<ClientManager>();//客户端消息转发LoginSvr
                    service.AddSingleton<ClientThread>();//登录数据交互（开启客户端连接LoginSrv：5500）
                    service.AddSingleton<ServerManager>();//登录网关管理器
                    service.AddSingleton<SessionManager>();//客户端会话管理器
                    service.AddHostedService<AppService>();//服务管理器(ConfigManager、ServerManager、ClientManager)
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
        //    service.AddSingleton<ServerService>();//登录服务（开启LoginGate：7000）
        //    service.AddSingleton<ClientManager>();//客户端消息转发LoginSvr
        //    service.AddSingleton<ClientThread>();//登录数据交互（开启客户端连接LoginSrv：5500）
        //    service.AddSingleton<ServerManager>();//登录网关管理器
        //    service.AddSingleton<SessionManager>();//客户端会话管理器
        //    service.AddHostedService<AppService>();//服务管理器(ConfigManager、ServerManager、ClientManager)
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
        FigletText header2 = new FigletText("LoginGate")
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

        //Console.WriteLine(@"                       ");
        //Console.WriteLine(@"    __                 ");
        //Console.WriteLine(@"   / /    __  __  ____ ");
        //Console.WriteLine(@"  / /    / / / / / __ \");
        //Console.WriteLine(@" / /___ / /_/ / / /_/ /");
        //Console.WriteLine(@"/_____/ \__, /  \____/ ");
        //Console.WriteLine(@"       /____/          ");
        //Console.WriteLine(@"    __                     _            ______           __       ");
        //Console.WriteLine(@"   / /   ____    ____ _   (_)   ____   / ____/  ____ _  / /_  ___ ");
        //Console.WriteLine(@"  / /   / __ \  / __ `/  / /   / __ \ / / __   / __ `/ / __/ / _ \");
        //Console.WriteLine(@" / /___/ /_/ / / /_/ /  / /   / / / // /_/ /  / /_/ / / /_  /  __/");
        //Console.WriteLine(@"/_____/\____/  \__, /  /_/   /_/ /_/ \____/   \__,_/  \__/  \___/ ");
        //Console.WriteLine(@"              /____/                                              ");
        //Console.WriteLine(@"                                                                  ");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        LogService.Info("正在启动服务...");

        //_serverHost.BuildHost(AppService.BuildAppService, typeof(AppService));
        ////_serverHost.BuildHost();
        //await _serverHost.StartAsync(cancellationToken);

        GateShare.ServiceProvider = host.Services;
        await host.StartAsync(cancellationToken);
        await ProcessLoopAsync();
        Stop();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return host.StopAsync(cancellationToken);
    }

    private void Stop()
    {
        AnsiConsole.Status().Start("Disconnecting...", ctx => { ctx.Spinner(Spinner.Known.Dots); });
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

            if (input.Length < 2)
            {
                return;
            }

            string firstTwoCharacters = input[..2];
            if (firstTwoCharacters switch
            {
                "/s" => ShowServerStatus(),
                "/c" => ClearConsole(),
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

    private async Task ShowServerStatus()
    {
        //GateShare.ShowLog = false;
        var periodicTimer = _timer ?? new PeriodicTimer(TimeSpan.FromSeconds(5));
        ClientManager clientManager = (ClientManager)GateShare.ServiceProvider.GetService(typeof(ClientManager));
        if (clientManager == null)
        {
            return;
        }

        ServerManager serverManager = (ServerManager)GateShare.ServiceProvider.GetService(typeof(ServerManager));
        if (serverManager == null)
        {
            return;
        }

        IList<ClientThread> clientList = clientManager.Clients;
        IList<ServerService> serverList = serverManager.GetServerList();
        Table table = new Table().Expand().BorderColor(Color.Grey);
        table.AddColumn("[yellow]LocalEndPoint[/]");
        table.AddColumn("[yellow]RemoteEndPoint[/]");
        table.AddColumn("[yellow]Status[/]");
        table.AddColumn("[yellow]Online[/]");
        table.AddColumn("[yellow]Send[/]");
        table.AddColumn("[yellow]Revice[/]");
        table.AddColumn("[yellow]Queue[/]");
        table.AddColumn("[yellow]Thread[/]");

        await AnsiConsole.Live(table)
            .AutoClear(true)
            .Overflow(VerticalOverflow.Crop)
            .Cropping(VerticalOverflowCropping.Bottom)
            .StartAsync(async ctx =>
            {
                foreach (int _ in Enumerable.Range(0, 10))
                {
                    table.AddRow(new[]
                    {
                        new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-"),
                        new Markup("-"), new Markup("-")
                    });
                }

                while (await periodicTimer.WaitForNextTickAsync())
                {
                    AnsiConsole.Clear();
                    for (int i = 0; i < clientList.Count; i++)
                    {
                        (string remoteendpoint, string status, string playCount, string reviceTotal, string sendTotal, string threadCount) =
                            clientList[i].GetStatus();

                        table.UpdateCell(i, 0, $"[bold]{serverList[i].GetEndPoint()}[/]");
                        table.UpdateCell(i, 1, $"[bold]{remoteendpoint}[/]");
                        table.UpdateCell(i, 2, $"[bold]{status}[/]");
                        table.UpdateCell(i, 3, $"[bold]{playCount}[/]");
                        table.UpdateCell(i, 4, $"[bold]{sendTotal}[/]");
                        table.UpdateCell(i, 5, $"[bold]{reviceTotal}[/]");
                        table.UpdateCell(i, 6, $"[bold]{clientManager.GetQueueCount()}[/]");
                        table.UpdateCell(i, 7, $"[bold]{threadCount}[/]");
                    }

                    ctx.Refresh();
                }
            });
    }
}