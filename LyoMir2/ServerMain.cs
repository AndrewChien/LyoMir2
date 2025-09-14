using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LyoMir2
{
    public partial class ServerMain : Form
    {
        Process proDB = new Process();
        Process proChat = new Process();
        Process proLog = new Process();
        Process proLoginSvr = new Process();
        Process proLoginGate = new Process();
        Process proM2 = new Process();
        Process proGameGate = new Process();
        Process proSelGate = new Process();
        static bool fDB, fChat, fLog, fLogin, fLoginGate, fM2, fGameGate, fSelGate;
        //StreamWriter swDB, swChat, swLog, swLoginSvr, swLoginGate, swM2, swGameGate, swSelGate;
        //StreamReader srDB, srChat, srLog, srLoginSvr, srLoginGate, srM2, srGameGate, srSelGate;

        public ServerMain()
        {
            InitializeComponent();
        }

        private void ServerMain_Load(object sender, EventArgs e)
        {
            //清理残留进程
            CloseProcess("DBSrv");
            CloseProcess("ChatSrv");
            CloseProcess("MapSrv");
            CloseProcess("LoginSrv");
            CloseProcess("LoginGate");
            CloseProcess("GameSrv");
            CloseProcess("GameGate");
            CloseProcess("SelGate");
            Msgout("清理残留进程完成！", rtbController);
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
            rdbMysql_CheckedChanged(null, null);
#pragma warning restore CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
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

        private void btnSet_Click(object sender, EventArgs e)
        {

        }

        private void SendCommand(string cmd, StreamWriter sw)
        {
            sw.WriteLine(cmd);
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

                if (ckbLoginSrv.Checked)
                {
                    Msgout("正在启动登陆服务...", rtbController);
                    StartProc(proLoginSvr, "LoginSrv/LoginSrv.exe", rtbLoginSvr, fLogin);
                    Msgout("登陆服务启动完成！", rtbController);
                }

                Thread.Sleep(1000);
                if (ckbDB.Checked)
                {
                    Msgout("正在启动DB服务...", rtbController);
                    StartProc(proDB, "DBServer/DBSrv.exe", rtbDB, fDB);
                    Msgout("DB服务启动完成！", rtbController);
                }

                Thread.Sleep(1000);
                if (ckbLoginGate.Checked)
                {
                    Msgout("正在启动登陆网关...", rtbController);
                    StartProc(proLoginGate, "LoginGate/LoginGate.exe", rtbLoginGate, fLoginGate);
                    Msgout("登陆网关启动完成！", rtbController);
                }

                Thread.Sleep(1000);
                if (ckbSelGate.Checked)
                {
                    Msgout("正在启动角色网关...", rtbController);
                    StartProc(proSelGate, "SelGate/SelGate.exe", rtbSelGate, fSelGate);
                    Msgout("角色网关启动完成！", rtbController);
                }

                Thread.Sleep(1000);
                if (ckbM2.Checked)
                {
                    Msgout("正在启动M2主引擎...", rtbController);
                    StartProc(proM2, "Mir200/GameSrv.exe", rtbM2, fM2, "-z");//用-z参数消除
                    Msgout("M2主引擎启动完成！", rtbController);
                }

                Thread.Sleep(1000);
                if (ckbGameGate.Checked)
                {
                    Msgout("正在启动游戏网关...", rtbController);
                    StartProc(proGameGate, "RunGate/GameGate.exe", rtbGameGate, fGameGate);
                    Msgout("游戏网关启动完成！", rtbController);
                }

                Thread.Sleep(1000);
                if (ckbLog.Checked)
                {
                    Msgout("正在启动日志服务...", rtbController);
                    StartProc(proLog, "MapServer/MapSrv.exe", rtbLog, fLog);
                    Msgout("日志服务启动完成！", rtbController);
                }

                Thread.Sleep(1000);
                if (ckbChat.Checked)
                {
                    Msgout("正在启动聊天服务...", rtbController);
                    StartProc(proChat, "ChatSrv/ChatSrv.exe", rtbChat, fChat);
                    Msgout("聊天服务启动完成！", rtbController);
                }

                Msgout("\r\n所有服务已全部启动！", rtbController);

                this.Invoke((MethodInvoker)delegate
                {
                    btnStart.Enabled = true;
                });
            });
        }

        private void StartProc(Process pro, string file, RichTextBox rtb, bool flag, string args = "")
        {
            Task.Factory.StartNew(o =>
            {
                flag = true;
                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = file,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    Arguments = args,
                };
                using (pro = Process.Start(psi))
                {
                    pro.StandardInput.AutoFlush = true;
                    //string cmd = $"ping www.baidu.com -t";
                    //proDB.StandardInput.WriteLine(cmd);
                    var line = "";
                    while (flag)
                    {
                        line = pro.StandardOutput.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            Msgout(line, rtb);
                        }
                        //Thread.Sleep(1);
                    }
                }
            }, this, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void Msgout(string msg, RichTextBox rtb)
        {
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

        private void StopProc()
        {
            btnStart.Text = "开启引擎";
            btnStart.Enabled = false;

            //发送Ctrl+C
            SendMsgToProc("^{C}", proDB);
            SendMsgToProc("^{C}", proChat);
            SendMsgToProc("^{C}", proLog);
            SendMsgToProc("^{C}", proLoginSvr);
            SendMsgToProc("^{C}", proLoginGate);
            SendMsgToProc("^{C}", proM2);
            SendMsgToProc("^{C}", proGameGate);
            SendMsgToProc("^{C}", proSelGate);
            //清理进程
            CloseProcess("DBSrv");
            CloseProcess("ChatSrv");
            CloseProcess("MapSrv");
            CloseProcess("LoginSrv");
            CloseProcess("LoginGate");
            CloseProcess("GameSrv");
            CloseProcess("GameGate");
            CloseProcess("SelGate");
            //清理日志
            rtbDB.Clear();
            rtbChat.Clear();
            rtbLog.Clear();
            rtbLoginSvr.Clear();
            rtbLoginGate.Clear();
            rtbM2.Clear();
            rtbGameGate.Clear();
            rtbSelGate.Clear();

            btnStart.Enabled = true;
            Msgout("\r\n引擎已关闭！", rtbController);
        }

        private void ServerMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            fDB = false;
            fChat = false;
            fLog = false;
            fLogin = false;
            fLoginGate = false;
            fM2 = false;
            fGameGate = false;
            fSelGate = false;
            Thread.Sleep(100);
            try
            {
                proDB.Close();
                proChat.Close();
                proLog.Close();
                proLoginSvr.Close();
                proLoginGate.Close();
                proM2.Close();
                proGameGate.Close();
                proSelGate.Close();
            }
            catch { }
            Environment.Exit(0);
        }

        private void SendMsgToProc(string cmd, Process proc)
        {
            try
            {
                proc.StandardInput.WriteLine(cmd);
            }
            catch { }
        }


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

        #region 选择数据库

        private void btnSqllite_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Sqllite文件(*.db;*.db3;*.sqllite)|*.db;*.db3;*.sqllite";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtSqllite.Text = openFileDialog.FileName;
                }
            }
        }

        private void btnMysql_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "exe文件(*.exe)|*.exe";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtMysql.Text = openFileDialog.FileName;
                }
            }
        }

        private void btnMongoDB_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "MongoDB文件(*.*)|*.*";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtMongoDB.Text = openFileDialog.FileName;
                }
            }
        }

        private void btnLocalFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "数据文件(*.*)|*.*";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtLocalFile.Text = openFileDialog.FileName;
                }
            }
        }

        private void rdbSqllite_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbSqllite.Checked)
            {
                txtSqllite.Enabled = btnSqllite.Enabled = true;
                rdbMysql.Checked = txtMysql.Enabled = btnMysql.Enabled = false;
                rdbMongoDB.Checked = txtMongoDB.Enabled = btnMongoDB.Enabled = false;
                rdbLocalFile.Checked = txtLocalFile.Enabled = btnLocalFile.Enabled = false;
            }
        }

        private void rdbMysql_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbMysql.Checked)
            {
                txtMysql.Enabled = btnMysql.Enabled = true;
                rdbSqllite.Checked = txtSqllite.Enabled = btnSqllite.Enabled = false;
                rdbMongoDB.Checked = txtMongoDB.Enabled = btnMongoDB.Enabled = false;
                rdbLocalFile.Checked = txtLocalFile.Enabled = btnLocalFile.Enabled = false;
            }
        }

        private void rdbMongoDB_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbMongoDB.Checked)
            {
                txtMongoDB.Enabled = btnMongoDB.Enabled = true;
                rdbSqllite.Checked = txtSqllite.Enabled = btnSqllite.Enabled = false;
                rdbMysql.Checked = txtMysql.Enabled = btnMysql.Enabled = false;
                rdbLocalFile.Checked = txtLocalFile.Enabled = btnLocalFile.Enabled = false;
            }
        }

        private void rdbLocalFile_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbLocalFile.Checked)
            {
                txtLocalFile.Enabled = btnLocalFile.Enabled = true;
                rdbSqllite.Checked = txtSqllite.Enabled = btnSqllite.Enabled = false;
                rdbMysql.Checked = txtMysql.Enabled = btnMysql.Enabled = false;
                rdbMongoDB.Checked = txtMongoDB.Enabled = btnMongoDB.Enabled = false;
            }
        }

        #endregion
    }
}
