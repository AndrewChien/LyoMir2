using MemoryPack;
using System.Runtime;

namespace LoginGate;

/// <summary>
/// LoginGate (登陆网关,客户端登陆操作 账号注册、找回密码等并转发到LoginSvr处理).
/// </summary>
internal class Program
{
    private static async Task Main(string[] args)
    {
        // AOT模式下手动注册所有需要序列化的类型
        MemoryPackFormatterProvider.Register<ServerDataMessage>();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        AppServer serviceRunner = new AppServer();
        CancellationTokenSource cts = new CancellationTokenSource();
        cts.Token.Register(() => _ = serviceRunner.StopAsync(cts.Token));
        // 监听 Ctrl+C 事件
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("Ctrl+C pressed");
            cts.Cancel();
            // 阻止其他处理程序处理此事件，以及默认的操作（终止程序）
            e.Cancel = true;
        };
        await serviceRunner.StartAsync(cts.Token);
    }
}