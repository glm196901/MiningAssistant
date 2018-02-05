using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using Microsoft.Win32;
using System.Security.Principal;

namespace requestJson
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitInfo();
            autoreader(@"C:\temp\bminer-v2.0.0-24861a7\saveUserInput.ini");
            Administrator();
        }

        //page1
        private BatStatus curBatSataus = BatStatus.NONE;

        private Process curProcess = new Process();

        private void InitInfo()
        {
            Console.WriteLine(this.mainEnterance.Text.GetType())
            ;
            //curProcess.OutputDataReceived -= new DataReceivedEventHandler(ProcessOutDataReceived);
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = "cmd.exe";
            //p.Arguments = @"cd C:\temp\";
            p.UseShellExecute = false;
            p.WindowStyle = ProcessWindowStyle.Hidden;
            p.CreateNoWindow = true;
            p.RedirectStandardError = true;
            p.RedirectStandardInput = true;
            p.RedirectStandardOutput = true;
            p.RedirectStandardError = true;

            curProcess.StartInfo = p;
            curProcess.Start();
            curProcess.BeginErrorReadLine();
            curProcess.BeginOutputReadLine();
            curProcess.ErrorDataReceived += new DataReceivedEventHandler(ProcessOutDataReceived);

        }

        
        //判断温度是否填写
        private void judgementTem()
        {
            int MaxTemp = Convert.ToInt32(this.maxTemp.Text);
            if (this.radioButton1.Checked == true)
            {

                if (string.IsNullOrEmpty(plAdd1.Text) || string.IsNullOrEmpty(plPort1.Text))
                {
                    MessageBox.Show("请检查 是否有未填写的内容");
                }
                else
                {
                    curProcess.StandardInput.WriteLine(this.mainEnterance.Text + wltAdd.Text + "." + minName.Text + "@" + plAdd1.Text + ":" + plPort1.Text + " -max-temperature " + MaxTemp);
                    locked();
                }
            }
            else if (this.radioButton2.Checked == true)
            {

                if (string.IsNullOrEmpty(plAdd.Text) || string.IsNullOrEmpty(plPort.Text))
                {
                    MessageBox.Show("请检查 是否有未填写的内容");
                }
                else
                {
                    curProcess.StandardInput.WriteLine(this.mainEnterance.Text + wltAdd.Text + "." + minName.Text + "@" + plAdd.Text + ":" + plPort.Text + " -max-temperature " + MaxTemp);
                    locked();
                }
            }
            else
            {
                MessageBox.Show("请先添加地址和端口号");
            }
        }

        //用户启动程序之后，在停止运行之前，禁止修改
        private void locked()
        {
            this.wltAdd.Enabled = false;
            this.maxTemp.Enabled = false;
            this.minName.Enabled = false;
            this.radioButton1.Enabled = false;
            this.radioButton2.Enabled = false;
            this.plAdd.Enabled = false;
            this.plPort.Enabled = false;
            this.plAdd1.Enabled = false;
            this.plPort1.Enabled = false;
            this.btnStart.Enabled = false;
            this.btnClear.Enabled = false;
            this.btnStop.Enabled = true;
            this.btnMonitor.Enabled = true;
            this.btnMonitor.BackColor = System.Drawing.Color.LimeGreen;
            this.btnClear.BackColor = System.Drawing.Color.Silver;
            this.btnStart.BackColor = System.Drawing.Color.Silver;
            this.btnStop.BackColor = System.Drawing.Color.Crimson;
            this.walletAddress.ForeColor = System.Drawing.Color.Silver;
            this.maxTemperature.ForeColor = System.Drawing.Color.Silver;
            this.minerName.ForeColor = System.Drawing.Color.Silver;
            this.wltAdd.BackColor = System.Drawing.Color.Silver;
            this.maxTemp.BackColor = System.Drawing.Color.Silver;
            this.minName.BackColor = System.Drawing.Color.Silver;
            this.plAdd1.BackColor = System.Drawing.Color.Silver;
            this.plPort.BackColor = System.Drawing.Color.Silver;
            this.plPort1.BackColor = System.Drawing.Color.Silver;
            this.label1.ForeColor = System.Drawing.Color.Silver;
        }

        //再点击stop之后对button以及变灰的空间初始化
        private void unlocked()
        {
            this.radioButton1.Enabled = true;
            this.radioButton2.Enabled = true;
            this.wltAdd.Enabled = true;
            this.maxTemp.Enabled = true;
            this.minName.Enabled = true;
            this.plAdd.Enabled = true;
            this.plPort.Enabled = true;
            this.plAdd1.Enabled = true;
            this.plPort1.Enabled = true;
            this.btnStart.Enabled = true;
            this.btnClear.Enabled = true;
            this.btnStop.Enabled = false;
            this.btnMonitor.Enabled = false;
            this.btnClear.BackColor = System.Drawing.Color.Turquoise;
            this.btnStart.BackColor = System.Drawing.Color.LimeGreen;
            this.btnStop.BackColor = System.Drawing.Color.Silver;
            this.btnMonitor.BackColor = System.Drawing.Color.Silver;
            this.walletAddress.ForeColor = System.Drawing.Color.Transparent;
            this.maxTemperature.ForeColor = System.Drawing.Color.Transparent;
            this.minerName.ForeColor = System.Drawing.Color.Transparent;
            this.wltAdd.BackColor = System.Drawing.Color.White;
            this.maxTemp.BackColor = System.Drawing.Color.White;
            this.minName.BackColor = System.Drawing.Color.White;
            this.plAdd1.BackColor = System.Drawing.Color.White;
            this.plPort.BackColor = System.Drawing.Color.White;
            this.plPort1.BackColor = System.Drawing.Color.White;
            this.label1.ForeColor = System.Drawing.Color.White;
        }

        //将用户上一次的输入保存在txt中当用户下次启动程序读取
        public void autoWriter(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            ArrayList autoSave = new ArrayList();
            WindowsIdentity wid = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(wid);
            string path1 = Application.ExecutablePath;
             RegistryKey rk = Registry.LocalMachine;
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                if ((this.radioButton1.Checked == true) && (this.autoStart.Checked == true))
                {
                    this.plAdd.Text = "";
                    this.plPort.Text = "";
                    autoSave.Add(this.wltAdd.Text.Trim() + ",");
                    autoSave.Add(this.minName.Text.Trim() + ",");
                    autoSave.Add(this.maxTemp.Text.Trim() + ",");
                    autoSave.Add(this.plAdd1.Text.Trim());
                    autoSave.Add(this.plPort1.Text.Trim());
                    autoSave.Add(this.autoStart.Checked);
                    sw.Write(autoSave[0]);
                    sw.Write(autoSave[1]);
                    sw.Write(autoSave[2]);
                    sw.Write(autoSave[3] = this.plAdd1.Text);
                    sw.Write(autoSave[4] = "," + this.plPort1.Text);
                    sw.Write(autoSave[5]= "," + this.autoStart.Checked);
                    //motify
                    MessageBox.Show("设置开机自启动设置成功，需要修改注册表，重启后执行", "提示");
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.SetValue("requestJson", path1);
                    rk2.Close();
                    rk.Close();
                }
                else if ((this.radioButton2.Checked == true) && (this.autoStart.Checked == true))
                {
                    this.plAdd1.Text = "";
                    this.plPort1.Text = "";
                    autoSave.Add(this.wltAdd.Text.Trim() + "|");
                    autoSave.Add(this.minName.Text.Trim() + "|");
                    autoSave.Add(this.maxTemp.Text.Trim() + "|");
                    autoSave.Add(this.plAdd.Text.Trim());
                    autoSave.Add(this.plPort.Text.Trim());
                    autoSave.Add(this.autoStart.Checked);
                    sw.Write(autoSave[0]);
                    sw.Write(autoSave[1]);
                    sw.Write(autoSave[2]);
                    sw.Write(autoSave[3] = this.plAdd.Text.Trim());
                    sw.Write(autoSave[4] = "|" + this.plPort.Text);
                    sw.Write(autoSave[5] = "|" + this.autoStart.Checked);
                    MessageBox.Show("设置开机自启动设置成功，需要修改注册表，重启后执行", "提示");
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.SetValue("requestJson", path1);
                    rk2.Close();
                    rk.Close();
                }
                else if ((this.radioButton1.Checked == true) && (this.autoStart.Checked == false))
                {
                    this.plAdd.Text = "";
                    this.plPort.Text = "";
                    autoSave.Add(this.wltAdd.Text.Trim() + ",");
                    autoSave.Add(this.minName.Text.Trim() + ",");
                    autoSave.Add(this.maxTemp.Text.Trim() + ",");
                    autoSave.Add(this.plAdd1.Text.Trim());
                    autoSave.Add(this.plPort1.Text.Trim());
                    autoSave.Add(this.autoStart.Checked);
                    sw.Write(autoSave[0]);
                    sw.Write(autoSave[1]);
                    sw.Write(autoSave[2]);
                    sw.Write(autoSave[3] = this.plAdd1.Text);
                    sw.Write(autoSave[4] = "," + this.plPort1.Text);
                    sw.Write(autoSave[5] = "," + this.autoStart.Checked);
                    MessageBox.Show("开机启动取消成功，需要修改注册表，重启后执行", "提示");
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.DeleteValue("requestJson", false);
                    rk2.Close();
                    rk.Close();
                }
                else if ((this.radioButton2.Checked == true) && (this.autoStart.Checked == false))
                {
                    this.plAdd1.Text = "";
                    this.plPort1.Text = "";
                    autoSave.Add(this.wltAdd.Text.Trim() + "|");
                    autoSave.Add(this.minName.Text.Trim() + "|");
                    autoSave.Add(this.maxTemp.Text.Trim() + "|");
                    autoSave.Add(this.plAdd.Text.Trim());
                    autoSave.Add(this.plPort.Text.Trim());
                    autoSave.Add(this.autoStart.Checked);
                    sw.Write(autoSave[0]);
                    sw.Write(autoSave[1]);
                    sw.Write(autoSave[2]);
                    sw.Write(autoSave[3] = this.plAdd.Text.Trim());
                    sw.Write(autoSave[4] = "|" + this.plPort.Text);
                    sw.Write(autoSave[5] = "|" + this.autoStart.Checked);
                    MessageBox.Show("开机启动取消成功，需要修改注册表，重启后执行", "提示");
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.DeleteValue("requestJson", false);
                    rk2.Close();
                    rk.Close();
                }
            }
            else {
                if ((this.radioButton1.Checked == true) && (this.autoStart.Checked == true))
                {
                    this.plAdd.Text = "";
                    this.plPort.Text = "";
                    autoSave.Add(this.wltAdd.Text.Trim() + ",");
                    autoSave.Add(this.minName.Text.Trim() + ",");
                    autoSave.Add(this.maxTemp.Text.Trim() + ",");
                    autoSave.Add(this.plAdd1.Text.Trim());
                    autoSave.Add(this.plPort.Text.Trim());
                    autoSave.Add(this.autoStart.Checked);
                    sw.Write(autoSave[0]);
                    sw.Write(autoSave[1]);
                    sw.Write(autoSave[2]);
                    sw.Write(autoSave[3] = this.plAdd1.Text.Trim());
                    sw.Write(autoSave[4] = "," + this.plPort1.Text);
                    sw.Write(autoSave[5] = "," + this.autoStart.Checked);
                }
                else if ((this.radioButton2.Checked == true) && (this.autoStart.Checked == true))
                {
                    this.plAdd1.Text = "";
                    this.plPort1.Text = "";
                    autoSave.Add(this.wltAdd.Text.Trim() + "|");
                    autoSave.Add(this.minName.Text.Trim() + "|");
                    autoSave.Add(this.maxTemp.Text.Trim() + "|");
                    autoSave.Add(this.plAdd.Text.Trim());
                    autoSave.Add(this.plPort.Text.Trim());
                    autoSave.Add(this.autoStart.Checked);
                    sw.Write(autoSave[0]);
                    sw.Write(autoSave[1]);
                    sw.Write(autoSave[2]);
                    sw.Write(autoSave[3] = this.plAdd.Text.Trim());
                    sw.Write(autoSave[4] = "|" + this.plPort.Text);
                    sw.Write(autoSave[5] = "|" + this.autoStart.Checked);


                }
                else if ((this.radioButton1.Checked == true) && (this.autoStart.Checked == false))
                {
                    this.plAdd.Text = "";
                    this.plPort.Text = "";
                    autoSave.Add(this.wltAdd.Text.Trim() + ",");
                    autoSave.Add(this.minName.Text.Trim() + ",");
                    autoSave.Add(this.maxTemp.Text.Trim() + ",");
                    autoSave.Add(this.plAdd1.Text.Trim());
                    autoSave.Add(this.plPort1.Text.Trim());
                    autoSave.Add(this.autoStart.Checked);
                    sw.Write(autoSave[0]);
                    sw.Write(autoSave[1]);
                    sw.Write(autoSave[2]);
                    sw.Write(autoSave[3] = this.plAdd1.Text.Trim());
                    sw.Write(autoSave[4] = "," + this.plPort1.Text);
                    sw.Write(autoSave[5] = "," + this.autoStart.Checked);

                }
                else if ((this.radioButton2.Checked == true) && (this.autoStart.Checked == false))
                {
                    this.plAdd1.Text = "";
                    this.plPort1.Text = "";
                    autoSave.Add(this.wltAdd.Text.Trim() + "|");
                    autoSave.Add(this.minName.Text.Trim() + "|");
                    autoSave.Add(this.maxTemp.Text.Trim() + "|");
                    autoSave.Add(this.plAdd.Text.Trim());
                    autoSave.Add(this.plPort.Text.Trim());
                    autoSave.Add(this.autoStart.Checked);
                    sw.Write(autoSave[0]);
                    sw.Write(autoSave[1]);
                    sw.Write(autoSave[2]);
                    sw.Write(autoSave[3] = this.plAdd.Text.Trim());
                    sw.Write(autoSave[4] = "|" + this.plPort.Text);
                    sw.Write(autoSave[5] = "|" + this.autoStart.Checked);

                }
                else
                {
                    MessageBox.Show("首次使用请设置好参数");

                }

            }

            //开始写入
            //清空缓冲区
            //sw.Flush();
            //关闭流
            sw.Flush();
            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }


        //读取存储好的参数 ,将用户勾选记录读出      
        private void autoreader(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                int indexStr = line.IndexOf("|");
                int indexStr1 = line.IndexOf(",");
                int lastIndexT = line.LastIndexOf("True");
                int lastIndexF = line.LastIndexOf("False");
                if ((indexStr > -1) && (lastIndexT > -1))
                {
                    string[] sArray = line.Split('|');
                    //Console.WriteLine(sArray[0]);
                    this.wltAdd.Text = sArray[0];
                    this.minName.Text = sArray[1];
                    this.maxTemp.Text = sArray[2];
                    this.radioButton2.Checked = true;
                    this.radioButton1.Checked = false;
                    this.plAdd.Visible = true;
                    this.plPort.Visible = true;
                    this.plAdd1.Visible = false;
                    this.plPort1.Visible = false;
                    this.plAdd.Text = sArray[3].Trim();
                    this.plPort.Text = sArray[4];
                    this.autoStart.Checked = Convert.ToBoolean(sArray[5]);
                    //this.btnStart.PerformClick();

                }
                else if ((indexStr1 > -1) && (lastIndexT > -1))
                {
                    string[] sArray = line.Split(',');
                    //Console.WriteLine(sArray[0]);
                    this.wltAdd.Text = sArray[0];
                    this.minName.Text = sArray[1];
                    this.maxTemp.Text = sArray[2];
                    this.radioButton1.Checked = true;
                    this.radioButton2.Checked = false;
                    this.plAdd1.Visible = true;
                    this.plPort1.Visible = true;
                    this.plAdd.Visible = false;
                    this.plPort.Visible = false;
                    this.plAdd1.Text = sArray[3].Trim();
                    this.plPort1.Text = sArray[4];
                    this.autoStart.Checked = Convert.ToBoolean(sArray[5]);
                    //this.btnStart.PerformClick();

                }
                else if ((indexStr > -1)&& (lastIndexF > -1))
                {

                    string[] sArray = line.Split('|');
                    //Console.WriteLine(sArray[0]);
                    this.wltAdd.Text = sArray[0];
                    this.minName.Text = sArray[1];
                    this.maxTemp.Text = sArray[2];
                    this.radioButton1.Checked = false;
                    this.radioButton2.Checked = true;
                    this.plAdd1.Visible = false;
                    this.plPort1.Visible = false;
                    this.plAdd.Visible = true;
                    this.plPort.Visible = true;
                    this.plAdd.Text = sArray[3].Trim();
                    this.plPort.Text = sArray[4];
                    this.autoStart.Checked = Convert.ToBoolean(sArray[5]);

                }
                else if ((indexStr1 > -1) && (lastIndexF > -1))
                {
                    string[] sArray = line.Split(',');
                    //Console.WriteLine(sArray[0]);
                    this.wltAdd.Text = sArray[0];
                    this.minName.Text = sArray[1];
                    this.maxTemp.Text = sArray[2];
                    this.radioButton1.Checked = true;
                    this.radioButton2.Checked = false;
                    this.plAdd1.Visible = true;
                    this.plPort1.Visible = true;
                    this.plAdd.Visible = false;
                    this.plPort.Visible = false;
                    this.plAdd1.Text = sArray[3].Trim();
                    this.plPort1.Text = sArray[4];
                    this.autoStart.Checked = Convert.ToBoolean(sArray[5]);
                    //Console.WriteLine(Convert.ToBoolean(sArray[5]));

                }
                //else
                //{
                //    //this.autoStart.Checked = true;
                //    //this.plAdd.Text = "eu1-zcash.flypool.org";
                //    //this.plPort.Text= "3333";
                //    //this.autoStart.Enabled = false;
                //}

            }
            //关闭流          
            sr.Close();
        }


        //关闭bminer
        private void stopApp() {
            Process[] myProgress;
            myProgress = Process.GetProcesses();          //获取当前启动的所有进程

            foreach (Process p in myProgress)            //关闭当前启动的bminer进程
            {
                Console.WriteLine(p.ProcessName);
                if (p.ProcessName == "bminer")          //通过进程名来寻找
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {   // 进程被保护而抛出异常(可以使用其它手段,如C\C++)
                        continue;
                    }
                }
            }

        }


        private void Administrator() {

            WindowsIdentity wid = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(wid);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                //MessageBox.Show("请以管理员身份运行");
                this.autoStart.Enabled = true;
            }
            else
            {
                this.autoStart.Enabled = false;
            }

        }


        //关闭程序 and save last input
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("点击确认后您将停止程序？", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                //用户选择确认的操作
                stopApp();
                this.autoWriter(@"C:\temp\bminer-v2.0.0-24861a7\saveUserInput.ini");
                //Environment.Exit(1);
                Dispose();
                Application.Exit();

            }
            else if (dr == DialogResult.Cancel)
            {
                //用户选择取消的操作
                e.Cancel = true;
            }
        }

        private void btnStart_Click_1(object sender, EventArgs e)
        {
            //MessageBox.Show("1231231");
            //int maxTemp;
            //int.TryParse(this.maxTemp.Text, out maxTemp);
            if (string.IsNullOrEmpty(this.mainEnterance.Text.Trim()))
            {
                MessageBox.Show("请输入命令");
            }

            if (string.IsNullOrEmpty(wltAdd.Text) || string.IsNullOrEmpty(minName.Text))
            {
                MessageBox.Show("请检查 是否有未填写的内容");
                return;
            }

            else if (this.maxTemp.Text == "85")
            {
                judgementTem();
                curBatSataus = BatStatus.ON;
            }
            else if (this.maxTemp.Text == "")
            {
                this.maxTemp.Text = "85";
                judgementTem();

            }
            else
            {
                int MaxTemp = Convert.ToInt32(this.maxTemp.Text);
                judgementTem();
            }

        }

        private void btnStop_Click_1(object sender, EventArgs e)
        {
            unlocked();
            stopApp();
        }

        private void button3_Click(object sender, EventArgs e)
        {            
            this.bminer.Text = "感谢您使用我们的产品";
        }

        public void ProcessOutDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (this.bminer.InvokeRequired)
            {
                this.bminer.Invoke(new Action(() =>
                {
                    this.bminer.AppendText(e.Data + "\r\n");
                }));
            }
            else
            {
                this.bminer.AppendText(e.Data + "\r\n");
            }
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.plAdd1.Visible = true;
            this.plPort1.Visible = true;
            this.plAdd.Visible = false;
            this.plPort.Visible = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.plAdd.Visible = true;
            this.plPort.Visible = true;
            this.plAdd1.Visible = false;
            this.plPort1.Visible = false;
            //this.plAdd1.Text = "";
            //this.plPort1.Text = "";
        }

        private void plAdd_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indexfly = this.plAdd.Text.IndexOf("flypool");
            int indexnano = this.plAdd.Text.IndexOf("nanopool");
            int indexeth = this.plAdd.Text.IndexOf("ethfans");

            if ((indexfly > -1) || (indexeth > -1))
            {
                this.plPort.Text = "3333";
            }
            else if (indexnano > -1)
            {
                this.plPort.Text = "6666";
                //this.plPort.Text = "6666";
            }
            else
            {
                this.plPort.Text = "3357";
            }

        }

        private void bminer_TextChanged(object sender, EventArgs e)
        {
            
            if (this.bminer.Text == "")
            {
                DialogResult dr = MessageBox.Show("请输入正确参数后再启动", "启动失败", MessageBoxButtons.OK, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    this.bminer.Text = "请输入正确参数后再启动";
                    unlocked();
                    stopApp();

                }
            }
        }

        public enum BatStatus
        {
            NONE = 0,
            ON = 1,
            OFF = 2
        }


        // page2
        public static string HttpGet(string url)
        {
            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";
            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                res = (HttpWebResponse)ex.Response;
                return "res";
            }

            //using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
            //   {
            //      return reader.ReadToEnd();
            //   }  

        }

        public void getApi()
        {
                try
                {
                    string ss = HttpGet("http://127.0.0.1:1880/api/status");
                    JObject Jobjs = JObject.Parse(ss);
                    // Console.WriteLine(o);
                    //Console.WriteLine(ss.ToString());         
                    foreach (JProperty Jobj in Jobjs.Properties())
                    {
                        if (Jobj.Name == "stratum")
                        {
                        this.textBox1.Text = Jobj.Value["accepted_shares"].ToString() + "\t";
                        this.textBox2.Text = Jobj.Value["rejected_shares"].ToString() + "\t";
                        this.textBox3.Text = Jobj.Value["accepted_share_rate"].ToString() + "\t";
                        this.textBox4.Text = Jobj.Value["rejected_share_rate"].ToString() + "\t";
                        ListViewItem lvi1 = new ListViewItem();
                        lvi1.SubItems.Add(Jobj.Value["accepted_shares"].ToString());
                    }

                        if (Jobj.Name == "miners")
                        {
                            //List<string> listArr = new List<string>();
                            List<List<string>> array = new List<List<string>>();
                            List<string> item1 = new List<string>();
                            List<string> item2 = new List<string>();
                            List<string> item3 = new List<string>();
                            List<string> item4 = new List<string>();
                            List<string> item5 = new List<string>();
                            //List<string> item6 = new List<string>();
                            //List<string> item7 = new List<string>();
                            //List<string> item8 = new List<string>();
                            //List<string> item9 = new List<string>();
                            //List<string> item10 = new List<string>();
                            List<string> item11 = new List<string>();
                            List<string> item12 = new List<string>();

                            foreach (JProperty num in Jobj.Value)
                            {
                                foreach (JProperty oJobj in num.Value)
                                {

                                    // Console.WriteLine(listArr);

                                    //Console.WriteLine(oJobj);
                                    if (oJobj.Name == "solver")
                                    {
                                        //Console.WriteLine(oJobj.Name + oJobj.Value);

                                        string sltRate = oJobj.Value["solution_rate"].ToString();
                                        string noRate = oJobj.Value["nonce_rate"].ToString();
                                        item11.Add(sltRate);
                                        item12.Add(noRate);
                                        array.Add(item11);
                                        array.Add(item12);

                                    }
                                    else
                                    {
                                        //Console.WriteLine(oJobj.Name);

                                        string devtemp = oJobj.Value["temperature"].ToString();
                                        string devpow = oJobj.Value["power"].ToString();
                                        string glbMemUsed = oJobj.Value["global_memory_used"].ToString();
                                        string utiGpu = oJobj.Value["utilization"]["gpu"].ToString();
                                        string utiMem = oJobj.Value["utilization"]["memory"].ToString();
                                        //string clksCore = oJobj.Value["clocks"]["core"].ToString();
                                        //string clksMem = oJobj.Value["clocks"]["memory"].ToString();
                                        //string pciBar = oJobj.Value["pci"]["bar1_used"].ToString();
                                        //string pciRx = oJobj.Value["pci"]["rx_throughput"].ToString();
                                        //string pciTx = oJobj.Value["pci"]["tx_throughput"].ToString();

                                        //inside
                                        item1.Add(devtemp);
                                        item2.Add(devpow);
                                        item3.Add(glbMemUsed);
                                        item4.Add(utiGpu);
                                        item5.Add(utiMem);
                                        //item6.Add(clksCore);
                                        //item7.Add(clksMem);
                                        //item8.Add(pciBar);
                                        //item9.Add(pciRx);
                                        //item10.Add(pciTx);
                                        //outside
                                        array.Add(item1);
                                        array.Add(item2);
                                        array.Add(item3);
                                        array.Add(item4);
                                        array.Add(item5);
                                        //array.Add(item6);
                                        //array.Add(item7);
                                        //array.Add(item8);
                                        //array.Add(item9);
                                        //array.Add(item10);

                                    }
                                }
                            }

                            this.listView1.Items.Clear();
                            for (int i = 0; i < 6; i++)
                            {
                                ListViewItem lvi = new ListViewItem();

                                lvi.Text = i + " 号矿机";
                                lvi.SubItems.Add(array[0][i]);
                                lvi.SubItems.Add(array[1][i]);
                                lvi.SubItems.Add(array[2][i]);
                                lvi.SubItems.Add(array[3][i]);
                                lvi.SubItems.Add(array[4][i]);
                                lvi.SubItems.Add(array[5][i]);
                                //lvi.SubItems.Add(array[6][i]);
                                //lvi.SubItems.Add(array[7][i]);
                                //lvi.SubItems.Add(array[8][i]);
                                //lvi.SubItems.Add(array[9][i]);
                                lvi.SubItems.Add(array[12][i]);
                                lvi.SubItems.Add(array[11][i]);
                                //int sumSol;
                                //int.TryParse(array[10][i], out );
                                this.listView1.Items.Add(lvi);

                            }
                            //this.textBox5.Text = 

                            Console.WriteLine(array[0][0]);
                            //Console.WriteLine((array[10][0]).GetType());
                            float sol0, sol1, sol2, sol3, sol4, sol5;
                            float.TryParse(array[0][0], out sol0);
                            float.TryParse(array[0][1], out sol1);
                            float.TryParse(array[0][2], out sol2);
                            float.TryParse(array[0][3], out sol3);
                            float.TryParse(array[0][4], out sol4);
                            float.TryParse(array[0][5], out sol5);
                            float sum = sol0 + sol1 + sol2 + sol3 + sol4 + sol5;

                            this.allSol.Text = sum.ToString();
                            Console.WriteLine(sol0 + sol1 + sol2 + sol3 + sol4 + sol5);
                            Console.WriteLine(sol1.GetType());
                        }
                    }
               }
                catch
                {
                    MessageBox.Show("请等到程序完全启动之后在开始监测");
                    this.timer1.Stop();
                }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.textBox1.Clear();
            getApi();

        }

        private void stopMonitor(object sender, EventArgs e)
        {
            this.timer1.Stop();
        }

        private void startMonitor(object sender, EventArgs e)
        {

            this.timer1.Start();
            //this.textBox1.Text = "initialization";
            this.timer1.Interval = 5000;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //autoreader(@"C:\temp\bminer-v2.0.0-24861a7\saveUserInput.ini");
            if (this.autoStart.Checked == true) {
                this.btnStart.PerformClick();
            }
        }
    }
}
