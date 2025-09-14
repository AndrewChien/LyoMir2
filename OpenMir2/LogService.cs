//using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using TouchSocket.Core;

namespace OpenMir2
{
    public enum CosoleOutputMode
    {
        OriginMode = 0,//按照代码所写原样输出
        ControlMode = 1,//控制器包装模式（禁用Console.Write）
    }

    public static class LogService
    {
        //private static Logger logService = LogManager.Setup()
        //        .SetupExtensions(ext => ext.RegisterConfigSettings(new ConfigurationBuilder().Build()))
        //        .GetCurrentClassLogger();


        //private static Logger logService = LogManager.GetCurrentClassLogger();


        public static void Info(string message)
        {
            //logService.Info(message);
            Console.WriteLine(message);
        }

        public static void Info(string message, params object[] args)
        {
            //logService.Info(message, args);
            Console.WriteLine(message);
        }

        public static void Error(string message)
        {
            //logService.Error(message);
            Console.WriteLine(message);
        }

        public static void Error(string message, Exception ex)
        {
            //logService.Error(ex, message);
            Console.WriteLine(message);
        }

        public static void Error(Exception ex)
        {
            //logService.Error(ex.Message, ex);
            Console.WriteLine(ex.Message);
        }

        public static void Debug(string message)
        {
            //logService.Debug(message);
            Console.WriteLine(message);
        }

        public static void Debug(string message, params object[] args)
        {
            //logService.Debug(message, args);
            Console.WriteLine(message);
        }

        public static void Warn(string message)
        {
            //logService.Warn(message);
            Console.WriteLine(message);
        }

        public static void Fatal(string message)
        {
            //logService.Fatal(message);
            Console.WriteLine(message);
        }
    }
}