using System.Diagnostics;
using System.Reflection;
using MethodInvoker = System.Windows.Forms.MethodInvoker;

namespace LyoMir2_ST
{
    public partial class FrmController : Form
    {
        public FrmController()
        {
            InitializeComponent();
            cc_init();
        }

        private void FrmController_Load(object sender, EventArgs e)
        {
            var ver = Assembly.GetEntryAssembly()?.GetName().Version;
            this.Text = $"LYO引擎控制器 v{ver}";
            //清理残留进程
            Msgout("正在清理环境...");
            CloseProcess("DBSrv");
            CloseProcess("ChatSrv");
            CloseProcess("MapSrv");
            CloseProcess("LoginSrv");
            CloseProcess("LoginGate");
            CloseProcess("GameSrv");
            CloseProcess("GameGate");
            CloseProcess("SelGate");
            Msgout($"欢迎使用LYO引擎！版本号：{ver}");
            Msgout("网站:http://www.chengxihot.top");
            Msgout("论坛:http://bbs.chengxihot.top");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "开启引擎")
            {
                StartProc();
            }
            else if (btnStart.Text == "关闭引擎")
            {
                StopProc();
            }
        }

        private void StartProc()
        {
            Task.Run(() =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnStart.Text = "关闭引擎";
                    btnStart.Enabled = false;
                });

                if (ckbLoginSrv.Checked)//3
                {
                    Msgout("正在启动登陆服务...");
                    consoleControl3.StartProcess("LoginSrv/LoginSrv.exe", null);
                    //StartProc(proLoginSvr, "LoginSrv/LoginSrv.exe", rtbLoginSvr, fLogin);
                    Msgout("登陆服务启动完成！");
                }

                Thread.Sleep(1000);
                if (ckbDB.Checked)//2
                {
                    Msgout("正在启动DB服务...");
                    consoleControl2.StartProcess("DBServer/DBSrv.exe", null);
                    //StartProc(proDB, "DBServer/DBSrv.exe", rtbDB, fDB);
                    Msgout("DB服务启动完成！");
                }

                Thread.Sleep(1000);
                if (ckbLoginGate.Checked)//4
                {
                    Msgout("正在启动登陆网关...");
                    consoleControl4.StartProcess("LoginGate/LoginGate.exe", null);
                    //StartProc(proLoginGate, "LoginGate/LoginGate.exe", rtbLoginGate, fLoginGate);
                    Msgout("登陆网关启动完成！");
                }

                Thread.Sleep(1000);
                if (ckbSelGate.Checked)//7
                {
                    Msgout("正在启动角色网关...");
                    consoleControl7.StartProcess("SelGate/SelGate.exe", null);
                    //StartProc(proSelGate, "SelGate/SelGate.exe", rtbSelGate, fSelGate);
                    Msgout("角色网关启动完成！");
                }

                Thread.Sleep(1000);
                if (ckbM2.Checked)//5
                {
                    Msgout("正在启动M2主引擎...");
                    consoleControl5.StartProcess("Mir200/GameSrv.exe", "-z");
                    //StartProc(proM2, "Mir200/GameSrv.exe", rtbM2, fM2, "-z");//用-z参数消除
                    Msgout("M2主引擎启动完成！");
                }

                Thread.Sleep(1000);
                if (ckbGameGate.Checked)//6
                {
                    Msgout("正在启动游戏网关...");
                    consoleControl6.StartProcess("RunGate/GameGate.exe", null);
                    //StartProc(proGameGate, "RunGate/GameGate.exe", rtbGameGate, fGameGate);
                    Msgout("游戏网关启动完成！");
                }

                Thread.Sleep(1000);
                if (ckbLog.Checked)//8
                {
                    Msgout("正在启动日志服务...");
                    consoleControl8.StartProcess("MapServer/MapSrv.exe", null);
                    //StartProc(proLog, "MapServer/MapSrv.exe", rtbLog, fLog);
                    Msgout("日志服务启动完成！");
                }

                Thread.Sleep(1000);
                if (ckbChat.Checked)//9
                {
                    Msgout("正在启动聊天服务...");
                    consoleControl9.StartProcess("ChatSrv/ChatSrv.exe", null);
                    //StartProc(proChat, "ChatSrv/ChatSrv.exe", rtbChat, fChat);
                    Msgout("聊天服务启动完成！");
                }

                Msgout("\r\n所有服务已全部启动！");

                this.Invoke((MethodInvoker)delegate
                {
                    btnStart.Enabled = true;
                });
            });
        }

        private void StopProc()
        {
            btnStart.Text = "开启引擎";
            btnStart.Enabled = false;

            //清空文本并关闭进程
            cc_clearclose();
            //清理进程
            CloseProcess("DBSrv");
            CloseProcess("ChatSrv");
            CloseProcess("MapSrv");
            CloseProcess("LoginSrv");
            CloseProcess("LoginGate");
            CloseProcess("GameSrv");
            CloseProcess("GameGate");
            CloseProcess("SelGate");

            btnStart.Enabled = true;
            Msgout("\r\n引擎已关闭！");
        }

        private void Msgout(string msg, RichTextBox? rtb = null)
        {
            if (rtb == null)
            {
                rtb = consoleControl1.InternalRichTextBox;
            }
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    rtb.AppendText(msg);
                    rtb.AppendText("\r\n");
                    rtb.ScrollToCaret();
                });
            }
            else
            {
                rtb.AppendText(msg);
                rtb.AppendText("\r\n");
                rtb.ScrollToCaret();
            }
        }

        private void btnSetPath_Click(object sender, EventArgs e)
        {

        }

        #region ConsoleControl控制

        private void cc_init()
        {
            consoleControl1.AutoScroll = true;
            consoleControl1.ShowDiagnostics = false;
            consoleControl1.IsInputEnabled = false;//是否接受输入
            consoleControl1.SendKeyboardCommandsToProcess = false;//是否能使用快捷键（Ctrl+C等）
            consoleControl1.InternalRichTextBox.ReadOnly = true;
            consoleControl1.InternalRichTextBox.BackColor = Color.Black;
            consoleControl1.InternalRichTextBox.ForeColor = Color.Lime;

            consoleControl2.AutoScroll = true;
            consoleControl2.ShowDiagnostics = false;
            consoleControl2.IsInputEnabled = false;//是否接受输入
            consoleControl2.SendKeyboardCommandsToProcess = false;//是否能使用快捷键（Ctrl+C等）
            consoleControl2.InternalRichTextBox.ReadOnly = true;

            consoleControl3.AutoScroll = true;
            consoleControl3.ShowDiagnostics = false;
            consoleControl3.IsInputEnabled = false;//是否接受输入
            consoleControl3.SendKeyboardCommandsToProcess = false;//是否能使用快捷键（Ctrl+C等）
            consoleControl3.InternalRichTextBox.ReadOnly = true;

            consoleControl4.AutoScroll = true;
            consoleControl4.ShowDiagnostics = false;
            consoleControl4.IsInputEnabled = false;//是否接受输入
            consoleControl4.SendKeyboardCommandsToProcess = false;//是否能使用快捷键（Ctrl+C等）
            consoleControl4.InternalRichTextBox.ReadOnly = true;

            consoleControl5.AutoScroll = true;
            consoleControl5.ShowDiagnostics = false;
            consoleControl5.IsInputEnabled = false;//是否接受输入
            consoleControl5.SendKeyboardCommandsToProcess = false;//是否能使用快捷键（Ctrl+C等）
            consoleControl5.InternalRichTextBox.ReadOnly = true;

            consoleControl6.AutoScroll = true;
            consoleControl6.ShowDiagnostics = false;
            consoleControl6.IsInputEnabled = false;//是否接受输入
            consoleControl6.SendKeyboardCommandsToProcess = false;//是否能使用快捷键（Ctrl+C等）
            consoleControl6.InternalRichTextBox.ReadOnly = true;

            consoleControl7.AutoScroll = true;
            consoleControl7.ShowDiagnostics = false;
            consoleControl7.IsInputEnabled = false;//是否接受输入
            consoleControl7.SendKeyboardCommandsToProcess = false;//是否能使用快捷键（Ctrl+C等）
            consoleControl7.InternalRichTextBox.ReadOnly = true;

            consoleControl8.AutoScroll = true;
            consoleControl8.ShowDiagnostics = false;
            consoleControl8.IsInputEnabled = false;//是否接受输入
            consoleControl8.SendKeyboardCommandsToProcess = false;//是否能使用快捷键（Ctrl+C等）
            consoleControl8.InternalRichTextBox.ReadOnly = true;

            consoleControl9.AutoScroll = true;
            consoleControl9.ShowDiagnostics = false;
            consoleControl9.IsInputEnabled = false;//是否接受输入
            consoleControl9.SendKeyboardCommandsToProcess = false;//是否能使用快捷键（Ctrl+C等）
            consoleControl9.InternalRichTextBox.ReadOnly = true;
        }

        private void cc_clearclose()
        {
            consoleControl2.ClearOutput();
            consoleControl2.StopProcess();

            consoleControl3.ClearOutput();
            consoleControl3.StopProcess();

            consoleControl4.ClearOutput();
            consoleControl4.StopProcess();

            consoleControl5.ClearOutput();
            consoleControl5.StopProcess();

            consoleControl6.ClearOutput();
            consoleControl6.StopProcess();

            consoleControl7.ClearOutput();
            consoleControl7.StopProcess();

            consoleControl8.ClearOutput();
            consoleControl8.StopProcess();

            consoleControl9.ClearOutput();
            consoleControl9.StopProcess();
        }

        private void test()
        {
            var a1 = consoleControl1.IsProcessRunning;
            var a2 = consoleControl1.ProcessInterface.ProcessFileName;
            var a3 = consoleControl1.ShowDiagnostics;
            var a4 = consoleControl1.IsInputEnabled;//是否接受输入
            var a5 = consoleControl1.SendKeyboardCommandsToProcess;//是否能使用快捷键（Ctrl+C等）

            consoleControl1.StartProcess("cmd", null);

            consoleControl1.ClearOutput();

            consoleControl1.StopProcess();
        }

        #endregion

        public bool CloseProcess(string name1)
        {
            try
            {
                Process[] ps = Process.GetProcessesByName(name1);
                foreach (Process p in ps)
                {
                    p.Kill();
                    p.WaitForExit();//关键，等待外部程序退出后才能往下执行
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabMain.SelectedIndex)
            {
                case 0:
                    consoleControl1.InternalRichTextBox.ScrollToCaret();
                    break;
                case 1:
                    consoleControl2.InternalRichTextBox.ScrollToCaret();
                    break;
                case 2:
                    consoleControl3.InternalRichTextBox.ScrollToCaret();
                    break;
                case 3:
                    consoleControl4.InternalRichTextBox.ScrollToCaret();
                    break;
                case 4:
                    consoleControl5.InternalRichTextBox.ScrollToCaret();
                    break;
                case 5:
                    consoleControl6.InternalRichTextBox.ScrollToCaret();
                    break;
                case 6:
                    consoleControl7.InternalRichTextBox.ScrollToCaret();
                    break;
                case 7:
                    consoleControl8.InternalRichTextBox.ScrollToCaret();
                    break;
                case 8:
                    consoleControl9.InternalRichTextBox.ScrollToCaret();
                    break;
                default:
                    break;
            }
        }
    }
}
