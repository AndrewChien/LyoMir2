using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LyoMir2
{
    internal static class Program
    {
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(System.IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(System.IntPtr hWnd);

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // winform aot
            ComWrappers.RegisterForMarshalling(WinFormsComInterop.WinFormsComWrappers.Instance);

            ApplicationConfiguration.Initialize();
            Mutex instance = new Mutex(true, "LyoMir220250718", out bool createdNew);
            if (createdNew)
            {
                Application.Run(new ServerMain());
                instance.ReleaseMutex();
            }
            else
            {
                Application.Exit();
                HandleRunningInstance(RunningInstance());
            }
        }

        private static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    return process;
                }
            }
            return null;
        }

        private static void HandleRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, 1);
            SetForegroundWindow(instance.MainWindowHandle);
        }
    }
}