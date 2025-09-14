using OpenMir2.Generation.Entities;

namespace GameSrv.Word.Threads
{
    /// <summary>
    /// 生成器处理器
    /// </summary>
    public class GeneratorProcessor : TimerScheduledService
    {

        private readonly StandardRandomizer _standardRandomizer = new StandardRandomizer();
        private readonly Stopwatch sw = new Stopwatch();
        /// <summary>
        /// 生成器处理器
        /// </summary>
        public GeneratorProcessor() : base(TimeSpan.FromSeconds(10), "GeneratorProcessor")
        {

        }
        /// <summary>
        /// 初始化精灵生成器队列
        /// </summary>
        /// <param name="cancellationToken"></param>
        public override void Initialize(CancellationToken cancellationToken)
        {
            GenerateIdThread();
        }

        protected override Task ExecuteInternal(CancellationToken stoppingToken)
        {
            GenerateIdThread();
            return Task.CompletedTask;
        }

        protected override void Startup(CancellationToken stoppingToken)
        {
            LogService.Info("Id生成器启动...");
        }

        protected override void Stopping(CancellationToken stoppingToken)
        {
            LogService.Info("Id生成器停止...");
        }
        /// <summary>
        /// 生成精灵
        /// </summary>
        private void GenerateIdThread()
        {
            if (SystemShare.ActorMgr.GenerateQueueCount < 20000)
            {
                sw.Reset();
                sw.Start();
                for (int i = 0; i < 100000; i++)
                {
                    int sequence = _standardRandomizer.NextInteger();
                    if (SystemShare.ActorMgr.ContainsKey(sequence))
                    {
                        while (true)
                        {
                            sequence = _standardRandomizer.NextInteger();
                            if (!SystemShare.ActorMgr.ContainsKey(sequence))
                            {
                                break;
                            }
                        }
                    }
                    while (sequence < 0)
                    {
                        sequence = Environment.TickCount + HUtil32.Sequence();
                        if (sequence > 0)
                        {
                            break;
                        }
                    }
                    SystemShare.ActorMgr.AddToQueue(sequence);
                }
                sw.Stop();
                LogService.Info($"Id生成完毕 耗时:{sw.Elapsed} 可用数:[{SystemShare.ActorMgr.GenerateQueueCount}]");
            }
        }
    }
}