using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace OpenMir2
{
    public abstract class TimerScheduledService : BackgroundService
    {
        //private readonly ILogger LogService = Log.ForContext<TimerScheduledService>();
        private readonly PeriodicTimer _timer;
        private readonly Stopwatch _stopwatch;

        protected TimerScheduledService(TimeSpan timeSpan, string name)
        {
            Name = name;
            _stopwatch = new Stopwatch();
            _timer = new PeriodicTimer(timeSpan);
        }

        public string Name { get; }

        public long ElapsedMilliseconds { get; private set; }

        public bool StopOnException { get; set; }

        public bool CloseRequest = false;

        /// <summary>
        /// ����ʱ����Ⱥ�̨����
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            LogService.Info($"Thread [{Name}] has started");
            Startup(cancellationToken);
            return base.StartAsync(cancellationToken);
        }
        /// <summary>
        /// �ر�ʱ����Ⱥ�̨����
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Stopping(cancellationToken);
            _timer.Dispose();
            LogService.Info($"Thread [{Name}] has finished");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(stoppingToken))
                {
                    _stopwatch.Start();
                    try
                    {
                        await ExecuteInternal(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        LogService.Error("Execute exception", ex);
                    }
                    finally
                    {
                        _stopwatch.Stop();
                        ElapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
                        _stopwatch.Reset();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                //LogService.Warn(operationCancelledException, "service stopped");
            }
        }

        public abstract void Initialize(CancellationToken cancellationToken);
        protected abstract Task ExecuteInternal(CancellationToken cancellationToken);
        protected abstract void Startup(CancellationToken cancellationToken);
        protected abstract void Stopping(CancellationToken cancellationToken);

        public override void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}