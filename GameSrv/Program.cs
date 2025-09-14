using System.Runtime;

namespace GameSrv
{
    /// <summary>
    /// GameSvr (游戏数据服务,处理玩家数据 走路 攻击 施法等操作).
    /// </summary>
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            //args = new string[] { "-z" };
            foreach (var item in args)
            {
                if (item.StartsWith('-') && item.TrimStart('-').ToUpper().Contains("Z")) //-z 代表控制器启动模式，不显示加载单个物品过程
                {
                    GameSrvPub.OutputMode = CosoleOutputMode.ControlMode;
                }
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            GCSettings.LatencyMode = GCSettings.IsServerGC ? GCLatencyMode.Batch : GCLatencyMode.Interactive;
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;//强制压缩大对象堆 LOH
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

    public static class GameSrvPub
    {
        public static CosoleOutputMode OutputMode { get; set; } = CosoleOutputMode.OriginMode;
    }
}