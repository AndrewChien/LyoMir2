using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using DBSrv.Conf;
using DBSrv.Services.Impl;
using DBSrv.Storage;
using DBSrv.Storage.Impl;
using DBSrv.Storage.MySQL;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using SystemModule;
using Color = Spectre.Console.Color;

namespace DBSrv
{
    /// <summary>
    /// 注册各客户端及服务端（5600，5100、5700、6000）
    /// </summary>
    public class AppServer
    {
        private readonly IHostBuilder builder;
        private readonly IHost host;
        private static PeriodicTimer _timer;
        //private readonly ServerHost _serverHost;
        private readonly AppConfigManager configManager;
        private static Logger LogService;
        private static readonly CancellationTokenSource CancellationToken = new CancellationTokenSource();

        /// <summary>
        /// 注册各客户端及服务端（5600，5100、5700、6000）
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
            configManager = new AppConfigManager();
            configManager.LoadConfig();

            builder = new HostBuilder()
                .ConfigureServices((hostContext, service) =>
                {
                    service.AddSingleton(configManager.Settings);
                    service.AddSingleton<ClientSession>();//登陆会话同步服务（开启客户端连接LoginSrv：5600）
                    service.AddSingleton<UserService>();//角色数据服务（开启DBSrv：5100）
                    service.AddSingleton<DataService>();//玩家数据服务（开启DBSrv：6000）
                    service.AddSingleton<MarketService>();//拍卖行数据存储服务（开启DBSrv：5700）
                    service.AddSingleton<ICacheStorage, CacheStorageService>();
                    service.AddHostedService<TimedService>();//数据同步
                    service.AddHostedService<AppService>();//服务管理器
                    LoadAssembly(service, "MySQL");//反射启动MYSQL
                }).ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    logging.AddNLog(config);
                });
            host = builder.Build();

            //_serverHost = new ServerHost();
            ////1、在公共容器注册service
            //_serverHost.ConfigureServices(service =>
            //{
            //    service.AddSingleton(configManager.Settings);
            //    service.AddSingleton<ClientSession>();//登陆会话同步服务（开启客户端连接LoginSrv：5600）
            //    service.AddSingleton<UserService>();//角色数据服务（开启DBSrv：5100）
            //    service.AddSingleton<DataService>();//玩家数据服务（开启DBSrv：6000）
            //    service.AddSingleton<MarketService>();//拍卖行数据存储服务（开启DBSrv：5700）
            //    service.AddSingleton<ICacheStorage, CacheStorageService>();
            //    service.AddHostedService<TimedService>();//数据同步
            //    service.AddHostedService<AppService>();//服务管理器
            //    LoadAssembly(service, "MySQL");//反射启动MYSQL
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
            FigletText header2 = new FigletText("DBSrv")
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            //_serverHost.BuildHost(AppService.BuildAppService, typeof(AppService));
            //await _serverHost.StartAsync(cancellationToken);

            DBShare.ServiceProvider = host.Services;
            await host.StartAsync(cancellationToken);

            await ProcessLoopAsync();
            Stop();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await host.StopAsync(cancellationToken);
        }

        private void LoadMySqlDirect()
        {

        }

        /// <summary>
        /// lyo：手动显示注册替代反射
        /// </summary>
        //public class PlayDataStorage
        //{
        //}
        //public class PlayRecordStorage
        //{
        //}
        //public class MarketStoageService
        //{
        //}

        /// <summary>
        /// 反射启动MYSQL
        /// </summary>
        /// <param name="services"></param>
        /// <param name="storageName"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private void LoadAssembly(IServiceCollection services, string storageName)
        {
            //// 显式触发类型注册（AOT编译时会被识别）
            //RuntimeHelpers.RunClassConstructor(typeof(PlayDataStorage).TypeHandle);
            //// 动态创建泛型实例
            //var playDataStorageType = typeof(PlayDataStorage);
            //var playRecordStorageType = typeof(PlayRecordStorage);
            //var marketStorageType = typeof(MarketStoageService);

            //var mylist = typeof(List<>).MakeGenericType(type);
            //var instance = Activator.CreateInstance(mylist);
            //type.GetMethod("MethodName").Invoke(instance, new object[] { });
            //FieldInfo[] info = type.GetFields();
            //foreach (FieldInfo i in info)
            //{
            //    object obj = i.GetValue("RDMySQL");
            //    string name = i.Name;
            //}



            //string storageFileName = $"DBSrv.Storage.{storageName}.dll";
            //string storagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, storageFileName);
            //if (!File.Exists(storagePath))
            //{
            //    throw new Exception($"{storageFileName} 存储策略文件不存在,服务启动失败.");
            //}
            //AssemblyLoadContext context = new AssemblyLoadContext(storagePath);
            //context.Resolving += ContextResolving;
            //Assembly assembly = context.LoadFromAssemblyPath(storagePath);
            //if (assembly == null)
            //{
            //    throw new Exception($"获取{storageName}数据存储实例失败，请确认文件是否正确.");
            //}
            //Type playDataStorageType = assembly.GetType($"DBSrv.Storage.{storageName}.PlayDataStorage", true);
            //Type playRecordStorageType = assembly.GetType($"DBSrv.Storage.{storageName}.PlayRecordStorage", true);
            //Type marketStorageType = assembly.GetType($"DBSrv.Storage.{storageName}.MarketStoageService", true);
            //if (playDataStorageType == null)
            //{
            //    throw new ArgumentNullException(nameof(storageName), "获取数据存储实例失败，请确认文件是否正确或程序版本是否正确.");
            //}
            //if (playRecordStorageType == null)
            //{
            //    throw new ArgumentNullException(nameof(storageName), "获取数据索引存储实例失败，请确认文件是否正确或程序版本是否正确.");
            //}
            //if (marketStorageType == null)
            //{
            //    throw new ArgumentNullException(nameof(storageName), "获取拍卖行存储实例失败，请确认文件是否正确或程序版本是否正确.");
            //}

            //StorageOption storageOption = new StorageOption();
            //storageOption.ConnectionString = configManager.Settings.ConnctionString;
            //var playDataStorage = new PlayDataStorage(storageOption);
            //var playRecordStorage = new PlayRecordStorage(storageOption);
            //var marketStorage = new MarketStoageService(storageOption);

            //IPlayDataStorage playDataStorage = (IPlayDataStorage)Activator.CreateInstance(playDataStorageType, storageOption);
            //IPlayRecordStorage playRecordStorage = (IPlayRecordStorage)Activator.CreateInstance(playRecordStorageType, storageOption);
            //IMarketStorage marketStorage = (IMarketStorage)Activator.CreateInstance(marketStorageType, storageOption);
            //if (playDataStorage == null)
            //{
            //    throw new ArgumentNullException(nameof(storageName), "创建数据存储实例失败，请确认文件是否正确或程序版本是否正确.");
            //}
            //if (playRecordStorage == null)
            //{
            //    throw new ArgumentNullException(nameof(storageName), "创建数据索引存储实例失败，请确认文件是否正确或程序版本是否正确.");
            //}
            //if (marketStorage == null)
            //{
            //    throw new ArgumentNullException(nameof(storageName), "创建拍卖行数据存储实力失败，请确认文件是否正确或程序版本是否正确.");
            //}

            services.AddSingleton<StorageOption>();
            StorageOption storageOption = new StorageOption();
            storageOption.ConnectionString = configManager.Settings.ConnctionString;
            services.AddSingleton<IPlayDataStorage>(new PlayDataStorage(storageOption));
            services.AddSingleton<IPlayRecordStorage>(new PlayRecordStorage(storageOption));
            services.AddSingleton<IMarketStorage>(new MarketStoageService(storageOption));

            //services.AddSingleton(playDataStorage);
            //services.AddSingleton(playRecordStorage);
            //services.AddSingleton(marketStorage);
            LogService.Info($"[{storageName}]数据存储初始化成功.");
        }

        /// <summary>
        /// 加载依赖项
        /// </summary>
        /// <returns></returns>
        private Assembly ContextResolving(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            string expectedPath = Path.Combine(AppContext.BaseDirectory, assemblyName.Name + ".dll");
            if (File.Exists(expectedPath))
            {
                try
                {
                    using FileStream stream = File.OpenRead(expectedPath);
                    return context.LoadFromStream(stream);
                }
                catch (Exception ex)
                {
                    LogService.Error($"加载依赖项{expectedPath}发生异常：{ex.Message},{ex.StackTrace}");
                }
            }
            else
            {
                LogService.Error($"依赖项不存在：{expectedPath}");
            }
            return null;
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
            UserService userService = host.Services.GetService<UserService>();
            if (userService == null)
            {
                return;
            }
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
            SelGateInfo[] serverList = userService.GetGates.ToArray();
            Table table = new Table().Expand().BorderColor(Color.Grey);
            table.AddColumn("[yellow]ServerName[/]");
            table.AddColumn("[yellow]EndPoint[/]");
            table.AddColumn("[yellow]Status[/]");
            table.AddColumn("[yellow]Sessions[/]");
            table.AddColumn("[yellow]Send[/]");
            table.AddColumn("[yellow]Revice[/]");
            table.AddColumn("[yellow]Queue[/]");

            await AnsiConsole.Live(table)
                 .AutoClear(true)
                 .Overflow(VerticalOverflow.Crop)
                 .Cropping(VerticalOverflowCropping.Bottom)
                 .StartAsync(async ctx =>
                 {
                     foreach (int _ in Enumerable.Range(0, serverList.Length))
                     {
                         table.AddRow(new[] { new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-") });
                     }

                     while (await _timer.WaitForNextTickAsync())
                     {
                         for (int i = 0; i < serverList.Length; i++)
                         {
                             (string serverIp, string status, string sessionCount, string reviceTotal, string sendTotal, string queueCount) = serverList[i].GetStatus();

                             table.UpdateCell(i, 0, "[bold][blue]SelGate[/][/]");
                             table.UpdateCell(i, 1, ($"[bold]{serverIp}[/]"));
                             table.UpdateCell(i, 2, ($"[bold]{status}[/]"));
                             table.UpdateCell(i, 3, ($"[bold]{sessionCount}[/]"));
                             table.UpdateCell(i, 4, ($"[bold]{sendTotal}[/]"));
                             table.UpdateCell(i, 5, ($"[bold]{reviceTotal}[/]"));
                             table.UpdateCell(i, 6, ($"[bold]{queueCount}[/]"));
                         }
                         ctx.Refresh();
                     }
                 });
        }
    }
}