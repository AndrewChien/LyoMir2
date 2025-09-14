using GameGate.Conf;
using GameGate.Services;
using Microsoft.Extensions.Configuration;
//using Serilog;
using System.Runtime;
using Color = Spectre.Console.Color;

namespace GameGate
{
    /// <summary>
    /// GameGate (游戏网关,玩家的操作数据并转发到GameSvr处理).
    /// </summary>
    internal class Program
    {
        private static PeriodicTimer _timer;
        private static readonly CancellationTokenSource CancellationToken = new CancellationTokenSource();

        private static async Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            GCSettings.LatencyMode = GCLatencyMode.Batch;
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;

            ThreadPool.SetMaxThreads(200, 200);
            ThreadPool.GetMinThreads(out int workThreads, out int completionPortThreads);

            PrintUsage();
            Console.CancelKeyPress += delegate
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                }
            };

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(configuration)
            //    .CreateLogger();
            //LogService.Logger = Log.Logger;

            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<TimedService>();//定时任务
            builder.Services.AddHostedService<AppService>();//服务管理器
            //开启日志
            //builder.Logging.AddConfiguration(configuration.GetSection("Logging"));
            //builder.Logging.AddSerilog(dispose: true);

            IHost host = builder.Build();
            await host.StartAsync(CancellationToken.Token);
            LogService.Info($"ThreadPool.ThreadCount: {ThreadPool.ThreadCount} Minimum work threads: {workThreads} Minimum completion port threads: {completionPortThreads}");
            //启动后台服务
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

        private static async Task ProcessLoopAsync()
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

        private static Task Exit()
        {
            CancellationToken.CancelAfter(3000);
            Environment.Exit(Environment.ExitCode);
            return Task.CompletedTask;
        }

        private static Task ClearConsole()
        {
            Console.Clear();
            AnsiConsole.Clear();
            return Task.CompletedTask;
        }

        private static Task ReLoadConfig()
        {
            ConfigManager.Instance.ReLoadConfig();
            ServerManager.Instance.StartClientMessageWork(CancellationToken.Token);
            Console.WriteLine("重新读取配置文件完成...");
            return Task.CompletedTask;
        }

        private static async Task ShowServerStatus()
        {
            //GateShare.ShowLog = false;
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
            ServerService[] serverList = ServerManager.Instance.GetServerList();
            Table table = new Table().Expand().BorderColor(Color.Grey);
            table.AddColumn("[yellow]Id[/]");
            table.AddColumn("[yellow]EndPoint[/]");
            table.AddColumn("[yellow]Status[/]");
            table.AddColumn("[yellow]Online[/]");
            table.AddColumn("[yellow]Send[/]");
            table.AddColumn("[yellow]Revice[/]");
            table.AddColumn("[yellow]Total Send[/]");
            table.AddColumn("[yellow]Total Revice[/]");
            table.AddColumn("[yellow]Queue[/]");
            table.AddColumn("[yellow]WorkThread[/]");

            await AnsiConsole.Live(table)
                 .AutoClear(true)
                 .Overflow(VerticalOverflow.Crop)
                 .Cropping(VerticalOverflowCropping.Bottom)
                 .StartAsync(async ctx =>
                 {
                     foreach (int _ in Enumerable.Range(0, serverList.Length))
                     {
                         table.AddRow(new[] { new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-") });
                     }

                     while (await _timer.WaitForNextTickAsync(CancellationToken.Token))
                     {
                         for (int i = 0; i < serverList.Length; i++)
                         {
                             (string endPoint, string status, string playCount, string reviceTotal, string sendTotal, string totalrevice, string totalSend, string queueCount, int threads) = serverList[i].GetStatus();

                             table.UpdateCell(i, 0, $"[bold]{endPoint}[/]");
                             table.UpdateCell(i, 1, $"[bold]{status}[/]");
                             table.UpdateCell(i, 2, $"[bold]{playCount}[/]");
                             table.UpdateCell(i, 3, $"[bold]{sendTotal}[/]");
                             table.UpdateCell(i, 4, $"[bold]{reviceTotal}[/]");
                             table.UpdateCell(i, 5, $"[bold]{totalSend}[/]");
                             table.UpdateCell(i, 6, $"[bold]{totalrevice}[/]");
                             table.UpdateCell(i, 7, $"[bold]{queueCount}[/]");
                             table.UpdateCell(i, 8, $"[bold]{threads}[/]");
                         }
                         ctx.Refresh();
                     }
                 });
        }

        private static void PrintUsage()
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
            FigletText header2 = new FigletText("GameGate")
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
        }
    }
}