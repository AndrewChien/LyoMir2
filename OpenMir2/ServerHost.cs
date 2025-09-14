using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using OpenMir2;
//using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Sockets;

namespace SystemModule
{
    public class ServerHost
    {
        //private readonly IHostBuilder _hostBuilder;
        private readonly IConfiguration _configuration;

        public ServerHost()
        {
            _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

            //_hostBuilder = CreateHost();//AOT不可用

            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(_configuration)
            //    .CreateLogger();
            //LogService.Logger = Log.Logger;
        }

        //private IHostBuilder CreateHost()
        //{
        //    //注意，此处CreateDefaultBuilder为反射方法，aot不可用
        //    return Host.CreateDefaultBuilder()
        //        .UseConsoleLifetime()
        //        .ConfigureLogging((context, logging) =>
        //        {
        //            logging.ClearProviders();
        //            logging.AddConfiguration(context.Configuration.GetSection("Logging"));
        //            //logging.AddSerilog();
        //            logging.AddConsole();
        //        });
        //}

        private readonly List<Action<ServiceCollection>> _configActions = new();
        /// <summary>
        /// lyo改造
        /// </summary>
        /// <param name="configureServices"></param>
        public ServerHost ConfigureServices(Action<IServiceCollection> configureServices)
        {
            //lyo:改造IHostBuilder.Build()反射方法
            _configActions.Add(configureServices);
            return this;

            //_hostBuilder.ConfigureServices(configureServices);//取消反射式配置服务
        }

        //AOT参考
        //public IHost Build()
        //{
        //    var services = new ServiceCollection();
        //    foreach (var config in _configActions)
        //    {
        //        config(services);
        //    }
        //    return new AotHost(services.BuildServiceProvider());
        //}

        /// <summary>
        /// lyo改造
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private IHost Build<T>(Func<IServiceProvider, T> func, Type type)
        {
            var services = new ServiceCollection();
            foreach (var configureServices in _configActions)
            {
                configureServices(services);
            }
            return (IHost)func(services.BuildServiceProvider());

            //switch(type.Namespace)
            //{
            //    case "DBSrv":
            //        return (IHost)func(services.BuildServiceProvider());
            //    case "GameSrv":
            //        return (IHost)func(services.BuildServiceProvider());
            //    case "LoginGate":
            //        return (IHost)func(services.BuildServiceProvider());
            //    case "LoginSrv":
            //        return (IHost)func(services.BuildServiceProvider());
            //    case "SelGate":
            //        return (IHost)func(services.BuildServiceProvider());
            //    default: break;
            //}
            //return null;
            //return new AotHost(services.BuildServiceProvider());//原
        }
        /// <summary>
        /// lyo改造
        /// </summary>
        /// <param name="func"></param>
        public void BuildHost<T>(Func<IServiceProvider, T> func, Type type)
        {
            AppHost = Build(func, type);
            //AppHost = _hostBuilder.Build();
            if (AppHost != null)
            {
                ServiceProvider = AppHost.Services;
            }
        }

        private IHost AppHost { get; set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get { return _configuration; } }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            PrintUsage();
            await AppHost.StartAsync(cancellationToken);
            await AppHost.WaitForShutdownAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await AppHost.StopAsync(cancellationToken);
        }

        private static void PrintUsage()
        {
            //AnsiConsole.WriteLine();

            //Table table = new Table()
            //{
            //    Border = TableBorder.None,
            //    Expand = true,
            //}.HideHeaders();
            //table.AddColumn(new TableColumn("One"));

            //FigletText header = new FigletText("OpenMir2")
            //{
            //    Color = Color.Fuchsia
            //};
            //FigletText header2 = new FigletText("Game Server")
            //{
            //    Color = Color.Aqua
            //};

            //table.AddColumn(new TableColumn("Two"));

            //Table rightTable = new Table()
            //    .HideHeaders()
            //    .Border(TableBorder.None)
            //    .AddColumn(new TableColumn("Content"));

            //rightTable.AddRow(header)
            //    .AddRow(header2)
            //    .AddEmptyRow()
            //    .AddEmptyRow();
            //table.AddRow(rightTable);

            //AnsiConsole.Write(table);

            //AnsiConsole.Write(new Rule($"[green3] Free open source, OpenMir2 creates unlimited possibilities.[/]").RuleStyle("grey").LeftJustified());
            //AnsiConsole.Write(new Rule($"[green3] Version:{MessageSettings.Version} UpdateTime:{MessageSettings.UpDateTime}[/]").RuleStyle("grey").LeftJustified());
        }

    }
}