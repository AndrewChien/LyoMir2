﻿using OpenMir2.Base;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace GameSrv
{
    /// <summary>
    /// 统计系统运行状态 
    /// </summary>
    public class WordStatistics
    {
        private StringBuilder _builder = new StringBuilder();
        private readonly string processName;
        private readonly PerformanceCounter MemoryCounter;
        private readonly PerformanceCounter CpuCounter;
        private readonly string AppVersion;
        private const string TITLE_FORMAT_S = @"[{0}] - Legend of Mir 2 Game Server [{8}] - {1} - Players: {3} (max:{4}) - {2} - Threads[S:{5:0000},U:{6:0000},A:{7:0000}]ms";

        public WordStatistics()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                processName = Process.GetCurrentProcess().ProcessName;
                MemoryCounter = new PerformanceCounter();
                CpuCounter = new PerformanceCounter();
            }
            AppVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Error";
        }

        /// <summary>
        /// 统计系统使用数据，回收GC和内存
        /// </summary>
        public void ShowServerStatus()
        {
            _builder.AppendLine();
            _builder.AppendLine($"{"=".PadLeft(64, '=')}");
            _builder.AppendLine(string.Format(TITLE_FORMAT_S, SystemShare.Config.ServerName, DateTimeOffset.Now.ToString("G"),
            GameShare.NetworkMonitor.UpdateStatsAsync(1000), SystemShare.WorldEngine.OnlinePlayObject, SystemShare.WorldEngine.PlayObjectCount,
            GameShare.SystemProcess.ElapsedMilliseconds, GameShare.UserProcessor.ElapsedMilliseconds,
            0, AppVersion));

            _builder.AppendLine(SystemShare.ActorMgr.Analytics());

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))//todo 待实现MACOS下的状态显示
            {
                ServerEnvironment.GetCPULoad();
                ServerEnvironment.MemoryInfo memoryInfo = ServerEnvironment.GetMemoryStatus();
                _builder.AppendLine($"CPU使用率:[{ServerEnvironment.CpuLoad.ToString("F")}%]");
                _builder.AppendLine($"物理内存:[{HUtil32.FormatBytesValue(memoryInfo.ullTotalPhys)}] 内存使用率:[{memoryInfo.dwMemoryLoad}%] 空闲内存:[{HUtil32.FormatBytesValue(memoryInfo.ullAvailPhys)}]");
                _builder.AppendLine($"虚拟内存:[{HUtil32.FormatBytesValue(memoryInfo.ullTotalPageFile)}] 虚拟内存使用率:[{ServerEnvironment.VirtualMemoryLoad}%] 空闲虚拟内存:[{HUtil32.FormatBytesValue(memoryInfo.ullAvailPageFile)}]");
                _builder.AppendLine($"已用内存:[{HUtil32.FormatBytesValue(ServerEnvironment.UsedPhysicalMemory)}] 程序内存:[{HUtil32.FormatBytesValue(ServerEnvironment.PrivateWorkingSet)}] 剩余内存:[{HUtil32.FormatBytesValue(memoryInfo.ullTotalPhys - ServerEnvironment.UsedPhysicalMemory)}]");
            }
            ShowGCStatus();
            TimeSpan ts = DateTimeOffset.Now - DateTimeOffset.FromUnixTimeMilliseconds(GameShare.StartTime);
            _builder.AppendLine($"Server Start Time: {DateTimeOffset.FromUnixTimeMilliseconds(GameShare.StartTime):G}");
            _builder.AppendLine($"Total Online Time: {(int)ts.TotalDays} days, {ts.Hours} hours, {ts.Minutes} minutes, {ts.Seconds} seconds");
            _builder.AppendLine($"Online Players[{SystemShare.WorldEngine.OnlinePlayObject}], Max Online Players[{SystemShare.WorldEngine.PlayObjectCount}], Offline Players[{SystemShare.WorldEngine.OfflinePlayCount}], Role Count[{SystemShare.WorldEngine.PlayObjectCount}]");
            _builder.AppendLine($"Total Bytes Sent: {HUtil32.FormatBytesValue(GameShare.NetworkMonitor.TotalBytesSent)}, Total Packets Sent: {HUtil32.FormatBytesValue(GameShare.NetworkMonitor.TotalPacketsSent)}");
            _builder.AppendLine($"Total Bytes Recv: {HUtil32.FormatBytesValue(GameShare.NetworkMonitor.TotalBytesRecv)}, Total Packets Recv: {HUtil32.FormatBytesValue(GameShare.NetworkMonitor.TotalPacketsRecv)}");
            _builder.AppendLine($"System Thread: {GameShare.SystemProcess.ElapsedMilliseconds:N0}ms");
            //_builder.AppendLine("{0} - {1}", $"User Thread: [{GameShare.UserProcessor.ElapsedMilliseconds:N0}ms]", $"RobotUser Thread: [{GameShare.RobotProcessor.ElapsedMilliseconds:N0}ms] Online/Queue:({SystemShare.WorldEngine.RobotPlayerCount}/{SystemShare.WorldEngine.RobotLogonQueueCount})");
            _builder.AppendLine($"Event Thread: [{GameShare.EventProcessor.ElapsedMilliseconds:N0}ms] - Storage Thread: [{GameShare.CharacterDataProcessor.ElapsedMilliseconds:N0}ms]");
            _builder.AppendLine($"Merchant Thread: [{GameShare.MerchantProcessor.ElapsedMilliseconds:N0}ms] - TimedBot Thread: [{GameShare.TimedRobotProcessor.ElapsedMilliseconds:N0}ms]");
            _builder.AppendLine($"Generator Thread: [{GameShare.GeneratorProcessor.ElapsedMilliseconds}ms] Identities Remaining: ");
            //LogService.Info("{0}", $"\tMonster: {IdentityGenerator.Monster.IdentitiesCount()}");
            //LogService.Info("{0}", $"\tFurniture: {IdentityGenerator.Furniture.IdentitiesCount()}");
            //LogService.Info("{0}", $"\tMapItem: {IdentityGenerator.MapItem.IdentitiesCount()}");
            //LogService.Info("{0}", $"\tTraps: {IdentityGenerator.Traps.IdentitiesCount()}");
            _builder.AppendLine("=".PadLeft(64, '='));
            LogService.Info(_builder.ToString());
            _builder.Clear();
            FreeMemory();
            //GetRunTime();
        }

        /// <summary>
        /// 清理内存
        /// </summary>
        private void FreeMemory()
        {
            GCMemoryInfo gcMemoryInfo = GC.GetGCMemoryInfo();
            if (gcMemoryInfo.TotalCommittedBytes > gcMemoryInfo.TotalAvailableMemoryBytes / 2)
            {
                LogService.Info("清理内存...");
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);
            }
        }

        private void GetRunTime()
        {
            TimeSpan ts = DateTimeOffset.Now - DateTimeOffset.FromUnixTimeMilliseconds(GameShare.StartTime);
            LogService.Info($"服务器运行:[{ts.Days}天{ts.Hours}小时{ts.Minutes}分{ts.Seconds}秒]");
        }

        /// <summary>
        /// 统计GC回收数据，并强制回收GC
        /// </summary>
        private void ShowGCStatus()
        {
            int gen0 = GC.CollectionCount(0);
            int gen1 = GC.CollectionCount(1);
            int gen2 = GC.CollectionCount(2);
            int total = gen0 + gen1 + gen2;
            _builder.AppendLine($"GC回收:[{total}]次 GC内存:[{HUtil32.FormatBytesValue(GC.GetTotalMemory(false))}] ");
            GC.Collect(0, GCCollectionMode.Forced, false);
        }

        /// <summary>
        /// 获取CPU使用率
        /// </summary>
        /// <returns></returns>
        private string GetProcessorData()
        {
            float d = GetCounterValue(CpuCounter, "Processor", "% Processor Time", processName);
            return d.ToString("F") + "%";
        }

        private static string GetCpuUsageForProcess()
        {
            DateTime startTime = DateTime.UtcNow;
            TimeSpan startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            Thread.Sleep(500);
            DateTime endTime = DateTime.UtcNow;
            TimeSpan endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            double cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            double totalMsPassed = (endTime - startTime).TotalMilliseconds;
            double cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return (cpuUsageTotal * 100).ToString("F") + "%";
        }

        /// <summary>
        /// 获取当前程序线程数
        /// </summary>
        /// <returns></returns>
        private float GetThreadCount()
        {
            return GetCounterValue(CpuCounter, "Process", "Thread Count", processName);
        }

        /// <summary>
        /// 获取工作集内存大小
        /// </summary>
        /// <returns></returns>
        private float GetWorkingSet()
        {
            return GetCounterValue(MemoryCounter, "Memory", "Working Set", processName);
        }

        /// <summary>
        /// 获取虚拟内存使用率详情
        /// </summary>
        /// <returns></returns>
        private string GetMemoryVData()
        {
            float d = GetCounterValue(MemoryCounter, "Memory", "% Committed Bytes In Use", null);
            string str = d.ToString("F") + "% (";
            d = GetCounterValue(MemoryCounter, "Memory", "Committed Bytes", null);
            str += HUtil32.FormatBytesValue(d) + " / ";
            d = GetCounterValue(MemoryCounter, "Memory", "Commit Limit", null);
            return str + HUtil32.FormatBytesValue(d) + ") ";
        }

        /// <summary>
        /// 获取虚拟内存使用率
        /// </summary>
        /// <returns></returns>
        private float GetUsageVirtualMemory()
        {
            return GetCounterValue(MemoryCounter, "Memory", "% Committed Bytes In Use", null);
        }

        /// <summary>
        /// 获取虚拟内存已用大小
        /// </summary>
        /// <returns></returns>
        private float GetUsedVirtualMemory()
        {
            return GetCounterValue(MemoryCounter, "Memory", "Committed Bytes", null);
        }

        /// <summary>
        /// 获取虚拟内存总大小
        /// </summary>
        /// <returns></returns>
        private float GetTotalVirtualMemory()
        {
            return GetCounterValue(MemoryCounter, "Memory", "Commit Limit", null);
        }

        /// <summary>
        /// 获取空闲的物理内存数，单位B
        /// </summary>
        /// <returns></returns>
        private float GetFreePhysicalMemory()
        {
            return GetCounterValue(MemoryCounter, "Memory", "Available Bytes", null);
        }

        private static float GetCounterValue(PerformanceCounter pc, string categoryName, string counterName, string instanceName)
        {
            if (OperatingSystem.IsWindows())
            {
                pc.CategoryName = categoryName;
                pc.CounterName = counterName;
                pc.InstanceName = instanceName;
                return pc.NextValue();
            }
            else //非windows平台未实现
            {
                return 0;
            }
        }
    }
}