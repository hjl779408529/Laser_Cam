using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Initialization;
using Laser_Build_1._0;
namespace Laser_Version2._0
{
    public partial class Laser_Control_Panel : Form
    {
        public Laser_Control_Panel()
        {
            InitializeComponent();
            
        }
       
        private void Laser_Control_Panel_Load(object sender, EventArgs e)
        {
            //状态
            richTextBox1.AppendText("Running"+"\r\n");
            richTextBox1.AppendText("PowerOn" + "\r\n");
            richTextBox1.AppendText("Shutter Enabled" + "\r\n");
            richTextBox1.AppendText("Key Switch On" + "\r\n");
            richTextBox1.AppendText("LDD On" + "\r\n");
            richTextBox1.AppendText("QSW On" + "\r\n");
            richTextBox1.AppendText("Shutter Interlock" + "\r\n");
            richTextBox1.AppendText("LDD Interlock" + "\r\n");
            //电流
            Seed1_Current.Text = "0.00";
            Amp1_Current.Text = "0.00";
            Amp2_Current.Text = "0.00";
            //设定电流值
            Seed_Set_Current.Text = Para_List.Parameter.Seed_Current.ToString();
            Amp1_Set_Current.Text = Para_List.Parameter.Amp1_Current.ToString();
            Amp2_Set_Current.Text = Para_List.Parameter.Amp2_Current.ToString();
            //Laser_Watt_Set_Value.Text = Laser_Watt_Cal.Percent_To_Watt(Para_List.Parameter.PEC).ToString();
            Laser_Watt_Set_Value.Text = Para_List.Parameter.PEC.ToString();
            Laser_Frequence_Set_Value.Text = Para_List.Parameter.PRF.ToString();//单位KHz
            //初始化通讯端口列表
            Com_List.Items.AddRange(Initialization.Initial.Laser_Control_Com.PortName.ToArray());
            //初始化默认的Com端口
            if (Initialization.Initial.Laser_Control_Com.PortName.Count>=1) Com_List.SelectedIndex = Para_List.Parameter.Laser_Control_Com_No;
            //状态刷新
            if (Initial.Laser_Control_Com.ComDevice.IsOpen == false)
            {
                Re_connect.Text = "打开串口";
                Com_Status.BackgroundImage = Properties.Resources.red;
                //按钮禁用
                Power_OFF.Enabled = false;
                Power_On.Enabled = false;
                Refresh_Status.Enabled = false;
                Reset_Laser.Enabled = false;
                Watt_Confirm.Enabled = false;
                Frequence_Confirm.Enabled = false;
            }
            else
            {
                //按钮启用
                Power_OFF.Enabled = true;
                Power_On.Enabled = true;
                Refresh_Status.Enabled = true;
                Reset_Laser.Enabled = true;
                Watt_Confirm.Enabled = true;
                Frequence_Confirm.Enabled = true;
                Re_connect.Text = "关闭串口";
                Com_Status.BackgroundImage = Properties.Resources.green;
                //刷新状态
                Refresh_All_Status();
            }
            
        }

        public void ChangeKeyColor(string key, Color color)
        {
            Regex regex = new Regex(key);
            //找出内容中所有的要替换的关键字
            MatchCollection collection = regex.Matches(richTextBox1.Text);
            //对所有的要替换颜色的关键字逐个替换颜色    
            foreach (Match match in collection)
            {
                //开始位置、长度、颜色缺一不可
                richTextBox1.SelectionStart = match.Index;
                richTextBox1.SelectionLength = key.Length;
                richTextBox1.SelectionColor = color;
            }
        }
        //更改串口端口号
        private void Com_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            Para_List.Parameter.Laser_Control_Com_No = Com_List.SelectedIndex;
        }       

        //重连串口
        private void Re_connect_Click(object sender, EventArgs e)
        {
            if (Para_List.Parameter.Laser_Control_Com_No < Initial.Laser_Control_Com.PortName.Count)
            {
              
                if (Initial.Laser_Control_Com.Open_Com(Para_List.Parameter.Laser_Control_Com_No))
                {                    
                    //按钮启用
                    Power_OFF.Enabled = true;
                    Power_On.Enabled = true;
                    Refresh_Status.Enabled = true;
                    Reset_Laser.Enabled = true;
                    //状态刷新
                    Re_connect.Text = "关闭串口";
                    Com_Status.BackgroundImage = Properties.Resources.green;
                }
                else
                {
                    //按钮禁用
                    Power_OFF.Enabled = false;
                    Power_On.Enabled = false;
                    Refresh_Status.Enabled = false;
                    Reset_Laser.Enabled = false;
                    //状态刷新
                    Re_connect.Text = "打开串口";
                    Com_Status.BackgroundImage = Properties.Resources.red;
                }   
            }
            else
            {
                MessageBox.Show("激光控制器通讯串口端口编号异常，请在激光控制面板选择正确的串口编号！！！");
                return;
            }
        }
        private void Refresh_Status_Click(object sender, EventArgs e)
        {
            Refresh_All_Status();
        }
        //状态刷新
        private void Refresh_All_Status() 
        {
            //读取Seed电流
            Initial.Laser_Operation_00.Read("00", "12");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Seed1_Current.Text = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m).ToString();
            //读取Amp1电流
            Initial.Laser_Operation_00.Read("01", "12");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Amp1_Current.Text = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m).ToString();
            //读取Amp2电流
            Initial.Laser_Operation_00.Read("02", "12");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Amp2_Current.Text = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m).ToString();
            //读取PEC功率值
            Initial.Laser_Operation_00.Read("00", "55");
            //if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Laser_Watt_Set_Value.Text = Laser_Watt_Cal.Percent_To_Watt(((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 10m)).ToString();
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Laser_Watt_Set_Value.Text = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 10m).ToString();
            //读取PRF频率值
            Initial.Laser_Operation_00.Read("00", "21");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 3)) Laser_Frequence_Set_Value.Text = ((decimal)BitConverter.ToUInt32(new byte[] { Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[0], Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[1], Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[2],0x00 }, 0) / 1000m).ToString();
            //读取状态
            Initial.Laser_Operation_00.Read("00", "0F");
            byte Status = 0x00;
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 5)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];

            //PowerOn
            if (Bit_Value.GetBitValue(Status, 0))
            {
                ChangeKeyColor("PowerOn", Color.Green);
            }
            else
            {
                ChangeKeyColor("PowerOn", Color.Red);
            }
            //Shuter Enabled
            if (Bit_Value.GetBitValue(Status, 1))
            {
                ChangeKeyColor("Shutter Enabled", Color.Green);
            }
            else
            {
                ChangeKeyColor("Shutter Enabled", Color.Red);
            }
            //Key Switch
            if (Bit_Value.GetBitValue(Status, 2))
            {
                ChangeKeyColor("Key Switch On", Color.Green);
            }
            else
            {
                ChangeKeyColor("Key Switch On", Color.Red);
            }
            //LDD On
            if (Bit_Value.GetBitValue(Status, 3))
            {
                ChangeKeyColor("LDD On", Color.Green);
            }
            else
            {
                ChangeKeyColor("LDD On", Color.Red);
            }
            //QSW On
            if (Bit_Value.GetBitValue(Status, 4))
            {
                ChangeKeyColor("QSW On", Color.Green);
            }
            else
            {
                ChangeKeyColor("QSW On", Color.Red);
            }
            //Shutter Interlock
            if (Bit_Value.GetBitValue(Status, 5))
            {
                ChangeKeyColor("Shutter Interlock", Color.Green);
            }
            else
            {
                ChangeKeyColor("Shutter Interlock", Color.Red);
            }
            //LDD Interlock
            if (Bit_Value.GetBitValue(Status, 6))
            {
                ChangeKeyColor("LDD Interlock", Color.Green);
            }
            else
            {
                ChangeKeyColor("LDD Interlock", Color.Red);
            }
        }
        private void Reset_Laser_Click(object sender, EventArgs e)
        {
            //复位
            Initial.Laser_Operation_00.Write("00", "07", "");
        }
        //一键开机
        private void Power_On_Click(object sender, EventArgs e)
        {
            Thread Power_On_Thread = new Thread(Power_On_Thread_Fun);
            Power_On_Thread.Start();
        }
        //一键开机功能
        private void Power_On_Thread_Fun()
        {
            if (One_Key_ON())
            {
                Power_On.Enabled = false;//禁用一键开机按钮
                Power_OFF.Enabled = true;//启用一键关机按钮
                Refresh_Status.Enabled = true;//启用状态更新按钮
                Reset_Laser.Enabled = true;//启用复位按钮
                Watt_Confirm.Enabled = true;//启用功率写入按钮
                Frequence_Confirm.Enabled = true;//启用频率写入按钮
                Re_connect.Enabled = true;//启用串口开关按钮

            }
            else
            {
                Power_On.Enabled = true;//启用一键开机按钮
                Power_OFF.Enabled = false;//禁用一键关机按钮
                Refresh_Status.Enabled = true;//启用状态更新按钮
                Reset_Laser.Enabled = true;//启用复位按钮
                Watt_Confirm.Enabled = true;//启用功率写入按钮
                Frequence_Confirm.Enabled = true;//启用频率写入按钮
                Re_connect.Enabled = true;//启用串口开关按钮
            }
        }
        //一键开机 程序
        private bool One_Key_ON()
        {
            //状态变量
            decimal Seed_Current = 0;//Seed电流
            decimal Amp1_Current = 0;//Amp1电流
            decimal Amp2_Current = 0;//Amp2电流 
            byte Status = 0x00;

            Power_On.Enabled = false;//禁用一键开机按钮
            Power_OFF.Enabled = false;//禁用一键关机按钮
            Refresh_Status.Enabled = false;//禁用状态更新按钮
            Reset_Laser.Enabled = false;//禁用复位按钮
            Watt_Confirm.Enabled = false;//禁用功率写入按钮
            Frequence_Confirm.Enabled = false;//禁用频率写入按钮
            Re_connect.Enabled = false;//禁用串口开关按钮
            /**************Seed Ldd***************/
            //读取Seed电流
            Initial.Laser_Operation_00.Read("00", "12");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Seed_Current = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m);
            Thread.Sleep(300);//等待300ms
            //打开Seed Ldd
            if (Seed_Current < 0.2m) Initial.Laser_Operation_00.Write("00", "10", "01");
            //状态判断
            Task.Factory.StartNew(() =>
            {
                do
                {
                    //读取Seed电流
                    Initial.Laser_Operation_00.Read("00", "12");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Seed_Current = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m);
                    Thread.Sleep(300);//等待300ms
                } while ((Para_List.Parameter.Seed_Current - Seed_Current) > 0.2m);
            }).Wait(120 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if ((Para_List.Parameter.Seed_Current - Seed_Current) > 0.2m) return false;
            //刷新状态
            Refresh_All_Status();

            /**************Seed Shutter Status***************/
            //读取Seed Shutter
            Initial.Laser_Operation_00.Read("00", "04");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
            Thread.Sleep(300);//等待300ms
            //打开Seed Shutter
            if (!Bit_Value.GetBitValue(Status, 0)) Initial.Laser_Operation_00.Write("00", "04", "01");
            Task.Factory.StartNew(() =>
            {
                do
                {
                    //读取Seed Shutter
                    Initial.Laser_Operation_00.Read("00", "04");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
                    Thread.Sleep(300);//等待300ms
                } while (!Bit_Value.GetBitValue(Status, 0));

            }).Wait(5 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if (!Bit_Value.GetBitValue(Status, 0)) return false;
            //刷新状态
            Refresh_All_Status();

            /**************PP Pulse Status***************/
            //读取PP Pulse
            Initial.Laser_Operation_00.Read("00", "65");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
            Thread.Sleep(300);//等待300ms
            //打开PP Pulse
            if (!Bit_Value.GetBitValue(Status, 0)) Initial.Laser_Operation_00.Write("00", "65", "01");
            Task.Factory.StartNew(() =>
            {
                do
                {
                    //读取PP Pulse
                    Initial.Laser_Operation_00.Read("00", "65");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
                    Thread.Sleep(300);//等待300ms
                } while (!Bit_Value.GetBitValue(Status, 0));

            }).Wait(5 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if (!Bit_Value.GetBitValue(Status, 0)) return false;
            //刷新状态
            Refresh_All_Status();

            /**************AOM Pulse Status***************/
            //读取AOM Pulse
            Initial.Laser_Operation_00.Read("00", "66");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
            Thread.Sleep(300);//等待300ms
            //打开AOM Pulse
            if (!Bit_Value.GetBitValue(Status, 0)) Initial.Laser_Operation_00.Write("00", "66", "01");
            Task.Factory.StartNew(() =>
            {
                do
                {
                    //读取AOM Pulse
                    Initial.Laser_Operation_00.Read("00", "66");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
                    Thread.Sleep(300);//等待300ms
                } while (!Bit_Value.GetBitValue(Status, 0));

            }).Wait(5 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if (!Bit_Value.GetBitValue(Status, 0)) return false;
            //刷新状态
            Refresh_All_Status();

            /**************AMP1 Ldd***************/
            //读取AMP1电流
            Initial.Laser_Operation_00.Read("01", "12");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Amp1_Current = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m);
            Thread.Sleep(300);//等待300ms
            //打开AMP1 Ldd
            if (Amp1_Current < 0.2m) Initial.Laser_Operation_00.Write("01", "10", "01");
            Task.Factory.StartNew(() =>
            {
                do
                {
                    //读取AMP1电流
                    Initial.Laser_Operation_00.Read("01", "12");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Amp1_Current = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m);
                    Thread.Sleep(300);//等待300ms
                } while ((Para_List.Parameter.Amp1_Current - Amp1_Current) > 0.2m);
            }).Wait(120 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if ((Para_List.Parameter.Amp1_Current - Amp1_Current) > 0.2m) return false;
            //刷新状态
            Refresh_All_Status();

            /**************AMP1 Shutter Status***************/
            //读取AMP1 Shutter
            Initial.Laser_Operation_00.Read("01", "04");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
            Thread.Sleep(300);//等待300ms
            //打开AMP1 Shutter
            if (!Bit_Value.GetBitValue(Status, 0)) Initial.Laser_Operation_00.Write("01", "04", "01");
            Task.Factory.StartNew(() =>
            {
                do
                {
                    //读取AMP1 Shutter
                    Initial.Laser_Operation_00.Read("01", "04");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
                    Thread.Sleep(300);//等待300ms
                } while (!Bit_Value.GetBitValue(Status, 0));

            }).Wait(5 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if (!Bit_Value.GetBitValue(Status, 0)) return false;
            //刷新状态
            Refresh_All_Status();

            /**************AMP2 Ldd***************/
            //读取AMP2电流
            Initial.Laser_Operation_00.Read("02", "12");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Amp2_Current = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m);
            Thread.Sleep(300);//等待300ms
            //打开AMP2 Ldd
            if (Amp2_Current < 0.2m) Initial.Laser_Operation_00.Write("02", "10", "01");
            Task.Factory.StartNew(() =>
            {

                do
                {
                    //读取AMP2电流
                    Initial.Laser_Operation_00.Read("02", "12");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Amp2_Current = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m);
                    Thread.Sleep(300);//等待300ms
                } while ((Para_List.Parameter.Amp2_Current - Amp2_Current) > 0.2m);
            }).Wait(120 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if ((Para_List.Parameter.Amp2_Current - Amp2_Current) > 0.2m) return false;
            //刷新状态
            Refresh_All_Status();
            //返回True
            return true;
        }
        //一键开机 程序
        private bool One_Key_OFF()
        {
            //状态变量
            decimal Seed_Current = 0;//Seed电流
            decimal Amp1_Current = 0;//Amp1电流
            decimal Amp2_Current = 0;//Amp2电流 
            byte Status = 0x00;

            Power_On.Enabled = false;//禁用一键开机按钮
            Power_OFF.Enabled = false;//禁用一键关机按钮
            Refresh_Status.Enabled = false;//禁用状态更新按钮
            Reset_Laser.Enabled = false;//禁用复位按钮
            Watt_Confirm.Enabled = false;//禁用功率写入按钮
            Frequence_Confirm.Enabled = false;//禁用频率写入按钮
            Re_connect.Enabled = false;//禁用串口开关按钮

            /**************AMP2 Ldd***************/
            //读取AMP2电流
            Initial.Laser_Operation_00.Read("02", "12");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Amp2_Current = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m);
            Thread.Sleep(300);//等待300ms
            //关闭AMP2 Ldd
            if (Amp2_Current > 0.2m) Initial.Laser_Operation_00.Write("02", "10", "00");
            Task.Factory.StartNew(() =>
            {
                do
                {
                    //读取AMP2电流
                    Initial.Laser_Operation_00.Read("02", "12");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Amp2_Current = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m);
                    Thread.Sleep(300);//等待300ms
                } while (Amp2_Current > 0.2m);
            }).Wait(120 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if (Amp2_Current > 0.2m) return false;
            //刷新状态
            Refresh_All_Status();

            /**************AMP1 Ldd***************/
            //读取AMP1电流
            Initial.Laser_Operation_00.Read("01", "12");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Amp1_Current = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m);
            Thread.Sleep(300);//等待300ms
            //关闭AMP1 Ldd
            if (Amp1_Current > 0.2m) Initial.Laser_Operation_00.Write("01", "10", "00");
            Task.Factory.StartNew(() =>
            {
                do
                {
                    //读取AMP1电流
                    Initial.Laser_Operation_00.Read("01", "12");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Amp1_Current = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m);
                    Thread.Sleep(300);//等待300ms
                } while (Amp1_Current > 0.2m);
            }).Wait(120 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if (Amp1_Current > 0.2m) return false;
            //刷新状态
            Refresh_All_Status();

            /**************AOM Pulse Status***************/
            //读取AOM Pulse
            Initial.Laser_Operation_00.Read("00", "66");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
            Thread.Sleep(300);//等待300ms
            //关闭AOM Pulse
            if (Bit_Value.GetBitValue(Status, 0)) Initial.Laser_Operation_00.Write("00", "66", "00");
            Task.Factory.StartNew(() =>
            {
                do
                {
                    //读取AOM Pulse
                    Initial.Laser_Operation_00.Read("00", "66");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
                    Thread.Sleep(300);//等待300ms
                } while (Bit_Value.GetBitValue(Status, 0));

            }).Wait(5 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if (Bit_Value.GetBitValue(Status, 0)) return false;
            //刷新状态
            Refresh_All_Status();

            /**************PP Pulse Status***************/
            //读取PP Pulse
            Initial.Laser_Operation_00.Read("00", "65");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
            Thread.Sleep(300);//等待300ms
            //关闭PP Pulse
            if (Bit_Value.GetBitValue(Status, 0)) Initial.Laser_Operation_00.Write("00", "65", "00");
            Task.Factory.StartNew(() =>
            {
                do
                {
                    //读取PP Pulse
                    Initial.Laser_Operation_00.Read("00", "65");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
                    Thread.Sleep(300);//等待300ms
                } while (Bit_Value.GetBitValue(Status, 0));

            }).Wait(5 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if (Bit_Value.GetBitValue(Status, 0)) return false;
            //刷新状态
            Refresh_All_Status();

            /**************Seed Shutter Status***************/
            //读取Seed Shutter
            Initial.Laser_Operation_00.Read("00", "04");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
            Thread.Sleep(300);//等待300ms
            //关闭Seed Shutter
            if (Bit_Value.GetBitValue(Status, 0)) Initial.Laser_Operation_00.Write("00", "04", "00");
            Task.Factory.StartNew(() =>
            {
                do
                {
                    //读取Seed Shutter
                    Initial.Laser_Operation_00.Read("00", "04");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 1)) Status = Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte[Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length - 1];
                    Thread.Sleep(300);//等待300ms
                } while (Bit_Value.GetBitValue(Status, 0));

            }).Wait(5 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if (Bit_Value.GetBitValue(Status, 0)) return false;
            //刷新状态
            Refresh_All_Status();

            /**************Seed Ldd***************/
            //读取Seed电流
            Initial.Laser_Operation_00.Read("00", "12");
            if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Seed_Current = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m);
            Thread.Sleep(300);//等待300ms
            //关闭Seed Ldd
            if (Seed_Current > 0.2m) Initial.Laser_Operation_00.Write("00", "10", "00");
            //状态判断
            Task.Factory.StartNew(() =>
            {
                do
                {
                    //读取Seed电流
                    Initial.Laser_Operation_00.Read("00", "12");
                    if (!(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte == null) && (Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte.Length == 2)) Seed_Current = ((decimal)BitConverter.ToUInt16(Initial.Laser_Operation_00.Resolve_Rec.Rec_Byte, 0) / 100m);
                    Thread.Sleep(300);//等待300ms
                } while (Seed_Current > 0.2m);
            }).Wait(120 * 1000);//2 * 1000,ms,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            if (Seed_Current > 0.2m) return false;
            //刷新状态
            Refresh_All_Status();
            //返回True
            return true;
        }
        //功率写入
        private void Watt_Confirm_Click(object sender, EventArgs e)
        {
            Initial.Laser_Operation_00.Change_Pec(Para_List.Parameter.PEC);
        }
        //频率写入
        private void Frequence_Confirm_Click(object sender, EventArgs e)
        {
            Initial.Laser_Operation_00.Write("00", "21", Initial.Laser_Operation_00.PRF_To_Str((UInt32)(Para_List.Parameter.PRF * 1000)));
        }
        //Seed 电流 设置值 
        private void Seed_Set_Current_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Seed_Set_Current.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Seed_Current = tmp;
        }
        //Amp1 电流 设置值 
        private void Amp1_Set_Current_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Amp1_Set_Current.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Amp1_Current = tmp;
        }
        //Amp2 电流 设置值 
        private void Amp2_Set_Current_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Amp2_Set_Current.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Amp2_Current = tmp;
        }
        //激光功率 显示值和设置值 TextChanged
        private void Laser_Watt_Set_Value_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Laser_Watt_Set_Value.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            //Para_List.Parameter.PEC =Laser_Watt_Cal.Watt_To_Percent(tmp);
            Para_List.Parameter.PEC = tmp;
        }
        //激光频率 显示值和设置值 TextChanged
        private void Laser_Frequence_Set_Value_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Laser_Frequence_Set_Value.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.PRF = tmp;
        }
        //激光频率 显示值和设置值 MouseEnter
        private void Laser_Frequence_Set_Value_MouseEnter(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Laser_Frequence_Set_Value.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp > 1000) || (tmp < 400))
            {
                MessageBox.Show("频率限制范围:400-1000Khz!!!");
                Laser_Frequence_Set_Value.Text = Para_List.Parameter.PRF.ToString();
                return;
            }
            Para_List.Parameter.PRF = tmp;
        }
        //激光功率 显示值和设置值 MouseEnter
        private void Laser_Watt_Set_Value_MouseEnter(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Amp2_Set_Current.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Amp2_Current = tmp;
        }
        //一键关机
        private void Power_OFF_Click(object sender, EventArgs e)
        {
            Thread Power_Off_Thread = new Thread(Power_Off_Thread_Fun);
            Power_Off_Thread.Start(); 
        }
        //一键关机功能
        private void Power_Off_Thread_Fun()
        {
            if (One_Key_OFF())
            { 
                Power_On.Enabled = true;//启用一键开机按钮
                Power_OFF.Enabled = false;//禁用一键关机按钮
                Refresh_Status.Enabled = true;//启用状态更新按钮
                Reset_Laser.Enabled = true;//启用复位按钮
                Watt_Confirm.Enabled = true;//启用功率写入按钮
                Frequence_Confirm.Enabled = true;//启用频率写入按钮
                Re_connect.Enabled = true;//启用串口开关按钮
            }
            else
            {
                Power_On.Enabled = false;//禁用一键开机按钮
                Power_OFF.Enabled = true;//启用一键关机按钮
                Refresh_Status.Enabled = true;//启用状态更新按钮
                Reset_Laser.Enabled = true;//启用复位按钮
                Watt_Confirm.Enabled = true;//启用功率写入按钮
                Frequence_Confirm.Enabled = true;//启用频率写入按钮
                Re_connect.Enabled = true;//启用串口开关按钮
            }
        }
        /// <summary>
        /// 更新串口列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_List_Click(object sender, EventArgs e)
        {
            //刷新列表
            Initialization.Initial.Laser_Control_Com.Refresh_Com_List();
            //初始化通讯端口列表
            Com_List.Items.Clear();
            Com_List.Items.AddRange(Initialization.Initial.Laser_Control_Com.PortName.ToArray());
            
        }
    }
}
