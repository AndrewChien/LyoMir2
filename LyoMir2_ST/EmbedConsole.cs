using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyoMir2_ST
{
    /// <summary>
    /// 窗体内嵌Console
    /// </summary>
    public class EmbedConsole
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool AllocConsole();//开启

        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern void FreeConsole();//关闭

        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern bool AttachConsole(int dwProcessId);//附加

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindow")]
        extern static IntPtr FindWindow(string? lpClassName, string? lpWindowName);//找出运行的窗口   

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert); //取出窗口运行的菜单   

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        extern static IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags); //灰掉按钮

        [System.Runtime.InteropServices.DllImport("User32.dll ", EntryPoint = "SetParent")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);//设置父窗体

        [System.Runtime.InteropServices.DllImport("user32.dll ", EntryPoint = "ShowWindow")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);//显示窗口

        [System.Runtime.InteropServices.DllImport("user32.dll ", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);

        //控制台窗口句柄
        IntPtr windowHandle;


        public void AddConsole(IntPtr hwnd)
        {
            if (AllocConsole())
            {
                //通过程序名找窗口，两种方式，推荐第二种
                //windowHandle = FindWindow(null, Process.GetCurrentProcess().MainModule.FileName);
                windowHandle = FindWindow(null, Environment.ProcessPath);
                if (windowHandle == IntPtr.Zero)
                {
                    //通过控制台TITLE找窗口
                    Console.Title = "ConsoleTest";//定义窗口标题再通过标题找到控制台窗口
                    Thread.Sleep(100);
                    windowHandle = FindWindow(null, Console.Title);
                }

                //绑定窗口到panel容器
                if (windowHandle != IntPtr.Zero)
                {
                    SetParent(windowHandle, hwnd);
                    IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
                    uint SC_CLOSE = 0xF060;
                    RemoveMenu(closeMenu, SC_CLOSE, 0x0);//屏蔽关闭按钮 

                    Console.WindowWidth = 100;
                    Console.SetWindowPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Green;
                    ShowWindow(windowHandle, 3);
                }
            }
            else
            {
                MessageBox.Show("请注意控制台窗口未正常打开，请检查环境！", "环境缺失");
            }

        }
    }
}
