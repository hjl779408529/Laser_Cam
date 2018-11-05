using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using netDxf;
using netDxf.Header;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Objects;
using netDxf.Tables;
using netDxf.Units;
using System.IO;
using System.Timers;
using CCWin;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;
using Laser_Version2._0;

namespace Laser_Build_1._0
{
    public partial class Dxf : Form
    {
        public Dxf()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;//关闭CheckForIllegalCrossThreadCalls
        }

        Display_Dxf display_dxf;//dxf文件显示
        //定义文件名 
        string Dxf_filename = "sample.dxf";
        //建立定时器
        System.Timers.Timer Refresh_Timer = new System.Timers.Timer(200);
        //建立整合数据组Para_List.Parameter.
        List<List<Interpolation_Data>> Arc_Line_List_Data = new List<List<Interpolation_Data>>();
        List<List<Interpolation_Data>> Circle_List_Data = new List<List<Interpolation_Data>>();
        List<List<Interpolation_Data>> LwPolyline_List_Data = new List<List<Interpolation_Data>>();
        List<List<Interpolation_Data>> Concat_List_Data = new List<List<Interpolation_Data>>();
        List<Entity_Data> Mark_Circle_Entity_Data = new List<Entity_Data>();//mark 点数据收集
        //rtc和gts解析数据
        List<List<Interpolation_Data>> Rtc_List_Data = new List<List<Interpolation_Data>>();
        //定义补偿函数 
        Data_Resolve Data_Cal = new Data_Resolve();
        private void Dxf_Load(object sender, EventArgs e)
        {
            
            //初始数据刷新
            //工件坐标系偏移
            textBox4.Text = Para_List.Parameter.Work.X.ToString(6);
            textBox3.Text = Para_List.Parameter.Work.Y.ToString(6);
            //直线插补
            textBox2.Text = Para_List.Parameter.Line_synVel.ToString(6);
            textBox1.Text = Para_List.Parameter.Line_synAcc.ToString(6);
            //圆弧插补
            textBox6.Text = Para_List.Parameter.Circle_synVel.ToString(6);
            textBox5.Text = Para_List.Parameter.Circle_synAcc.ToString(6);

            //坐标运动平滑系数
            textBox12.Text = Para_List.Parameter.Syn_EvenTime.ToString(6);

            //插补终止速度
            textBox14.Text = Para_List.Parameter.Line_endVel.ToString(6);
            textBox13.Text = Para_List.Parameter.Circle_endVel.ToString(6);

            //坐标系定位坐标
            textBox23.Text = Convert.ToString(200);
            textBox22.Text = Convert.ToString(330);

            //刀具半径
            Cutter_Radius.Text = Para_List.Parameter.Cutter_Radius.ToString(6);
            //刀具补偿类型
            Cutter_Comp.SelectedIndex = Para_List.Parameter.Cutter_Type;
            //选择起始加工位置
            Start_Pos_Sel.SelectedIndex = Para_List.Parameter.Calibration_Type;

            //加工重复次数
            Rtc_Repeat_Num.Text = Para_List.Parameter.Rtc_Repeat.ToString();
            Gts_Repeat_Num.Text = Para_List.Parameter.Gts_Repeat.ToString();

            //启用定时器
            Refresh_Timer.Elapsed += Refresh_Timer_Elapsed_Thread;
            Refresh_Timer.AutoReset = true;
            Refresh_Timer.Enabled = true;
            Refresh_Timer.Start();

        }
        //线程函数
        private void Refresh_Timer_Elapsed_Thread(object sender, ElapsedEventArgs e)
        {
            Refresh_Timer_Elapsed();
        }
        //定时器线程函数
        private void Refresh_Timer_Elapsed()
        {
            /*
            this.Invoke((EventHandler)delegate {
               
                //工件坐标系偏移
                textBox4.Text = Convert.ToString(Para_List.Parameter.Work_X);
                textBox3.Text = Convert.ToString(Para_List.Parameter.Work_Y);
                //直线插补
                textBox2.Text = Convert.ToString(Para_List.Parameter.Line_synVel);
                textBox1.Text = Convert.ToString(Para_List.Parameter.Line_synAcc);
                //圆弧插补
                textBox6.Text = Convert.ToString(Para_List.Parameter.Circle_synVel);
                textBox5.Text = Convert.ToString(Para_List.Parameter.Circle_synAcc);
                //角度补偿值
                textBox7.Text = Convert.ToString(Para_List.Parameter.Arc_Compensation_A);

                //刀具绝对坐标
                textBox9.Text = Convert.ToString(Para_List.Parameter.Laser_X);
                textBox8.Text = Convert.ToString(Para_List.Parameter.Laser_Y);

                
            });
            */

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        public void appendInfo(string info)
        {
            Debug_Info_Display.AppendText(Cal_Elapse_Time.Get_Current_Time(1) + "\r\n");
            Debug_Info_Display.AppendText(info + "\r\n");
        }
        //获取文件名
        private void button6_Click(object sender, EventArgs e)
        {
            //获取文件名
            OpenFileDialog openfile = new OpenFileDialog
            {
                Filter = "dxf 文件(*.dxf)|*.dxf"
            };
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                Dxf_filename = openfile.FileName;
                appendInfo(Dxf_filename);
                if (display_dxf == null)
                {
                    display_dxf = new Display_Dxf();
                    display_dxf.Show();
                }
                else
                {
                    if (display_dxf.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                    {
                        display_dxf = new Display_Dxf();
                        display_dxf.Show();
                    }
                    else
                    {
                        display_dxf.Activate();//使子窗体获得焦点
                    }
                }
                display_dxf.axMxDrawX1.OpenDwgFile(openfile.FileName);
            }
        }
        //DXF文件数据处理        
        private void button8_Click(object sender, EventArgs e)
        {
            Thread Dxf_Open_thread = new Thread(Dxf_Open);
            Dxf_Open_thread.Start();
        }
        private void Dxf_Open()
        {
            //读取Dxf文件
            DxfDocument dxf = Data_Cal.Read_File(Dxf_filename);
            if (dxf == null)
            {
                return;
            }
            appendInfo("Dxf数据提取中！！！！");
            //建立临时数据存储组 和数据矫正
            List<Entity_Data> Arc_Line_Entity_Data = new List<Entity_Data>(Data_Cal.Calibration_Entity(Data_Cal.Resolve_Arc_Line(dxf), Para_List.Parameter.Trans_Affinity));//提取圆弧直线数据
            ///测试 圆弧角度
            //for (int i = 0; i < Arc_Line_Entity_Data.Count; i++)
            //{
            //    if (Arc_Line_Entity_Data[i].Type == 2)
            //    {
            //        appendInfo(string.Format("起始角度：{0}，终点角度：{1}", Arc_Line_Entity_Data[i].Cir_Start_Angle, Arc_Line_Entity_Data[i].Cir_End_Angle));
            //    }
            //}

            List<Entity_Data> Circle_Entity_Data = new List<Entity_Data>(Data_Cal.Calibration_Entity(Data_Cal.Resolve_Circle(dxf), Para_List.Parameter.Trans_Affinity));//提取圆数据
            List<List<Entity_Data>> LwPolylines_Entity_Data = new List<List<Entity_Data>>(Data_Cal.Calibration_List_Entity(Data_Cal.Resolve_LWPolyline(dxf), Para_List.Parameter.Trans_Affinity)); //提取多边形数据   
            appendInfo("Dxf数据提取完成！！！！");
            appendInfo("直线和圆弧 数据计数：" + Arc_Line_Entity_Data.Count);
            appendInfo("多边形 数据计数：" + LwPolylines_Entity_Data.Count);
            appendInfo("圆形 数据计数：" + Circle_Entity_Data.Count);
            appendInfo("轨迹数据生成中！！！！");
            //直线圆弧数据转换为  轨迹加工数据
            Arc_Line_List_Data = new List<List<Interpolation_Data>>(Data_Cal.Integrate_Arc_Line(Arc_Line_Entity_Data));
            ///测试 圆弧角度
            //for (int i = 0; i < Arc_Line_List_Data.Count; i++)
            //{
            //    for (int j = 0;j< Arc_Line_List_Data[i].Count; j++)
            //    {
            //        if (Arc_Line_List_Data[i][j].Type == 2)
            //        {
            //            appendInfo(string.Format("角度：{0}", Arc_Line_List_Data[i][j].Angle));
            //        }
            //    }
               
            //}
            //多边形转换为  轨迹加工数据
            LwPolyline_List_Data = new List<List<Interpolation_Data>>(Data_Cal.Integrate_LWPolyline(LwPolylines_Entity_Data));
            //整圆数据转换为  轨迹加工数据
            Circle_List_Data = new List<List<Interpolation_Data>>(Data_Cal.Integrate_Circle(Circle_Entity_Data));
            //圆形、直线和圆弧、多边形Rtc+Gts 
            Concat_List_Data.Clear();//清空数据
            Concat_List_Data.AddRange(Arc_Line_List_Data);
            Concat_List_Data.AddRange(LwPolyline_List_Data);
            Concat_List_Data.AddRange(Circle_List_Data);
            //for (int i = 0; i < Concat_List_Data.Count; i++)
            //{
            //    appendInfo("序号：" + i);
            //    for (int j = 0; j < Concat_List_Data[i].Count; j++)
            //    {

            //        if (Concat_List_Data[i][j].Type == 2)
            //        {
            //            appendInfo(string.Format("    类型：{4},起点({0},{1}),终点({2},{3}),角度{5}", Concat_List_Data[i][j].Start_x, Concat_List_Data[i][j].Start_y, Concat_List_Data[i][j].End_x, Concat_List_Data[i][j].End_y, Concat_List_Data[i][j].Type, Concat_List_Data[i][j].Angle));
            //        }
            //        else if (Concat_List_Data[i][j].Type == 1)
            //        {
            //            appendInfo(string.Format("    类型：{4},起点({0},{1}),终点({2},{3})", Concat_List_Data[i][j].Start_x, Concat_List_Data[i][j].Start_y, Concat_List_Data[i][j].End_x, Concat_List_Data[i][j].End_y, Concat_List_Data[i][j].Type));
            //        }
            //    }
            //}
            appendInfo("轨迹数据生成完成！！！！");
            appendInfo("刀具补偿数据生成中！！！！");
            //刀具补偿
            Concat_List_Data = new List<List<Interpolation_Data>>(Data_Cal.Cutter_Compensation(Concat_List_Data));
            //调试信息
            appendInfo("刀具补偿数据生成完成！！！！");
            appendInfo("直线和圆弧轨迹 数据计数：" + Arc_Line_List_Data.Count);
            appendInfo("多边形轨迹 数据计数：" + LwPolyline_List_Data.Count);
            appendInfo("圆形轨迹 数据计数：" + Circle_List_Data.Count);
            appendInfo("融合数据 数据计数：" + Concat_List_Data.Count);

        }
        ///get mark point list
        private void Get_Mark_Data_Click(object sender, EventArgs e)
        {
            //读取Dxf文件
            DxfDocument dxf = Data_Cal.Read_File(Dxf_filename);
            if (dxf == null)
            {
                return;
            }
            appendInfo("Mark 数据提取中！！！");
            Mark_Circle_Entity_Data = new List<Entity_Data>(Data_Cal.Resolve_Mark_Point(dxf));//提取Mark点数据
            if (Mark_Circle_Entity_Data.Count >= 4) Data_Cal.Mark_Calculate(Mark_Circle_Entity_Data);
            appendInfo("Mark 数据提取完成！！！");
            appendInfo("Mark 数据计数：！！！" + Mark_Circle_Entity_Data.Count);
        }
        //两轴回零
        private void button1_Click(object sender, EventArgs e)
        {
            Thread Axis01_home_thread = new Thread(this.Axis01_Home);
            Thread Axis02_home_thread = new Thread(this.Axis02_Home);
            Axis01_home_thread.Start();
            Axis02_home_thread.Start();
        }
        private void Axis01_Home()
        {
            if (Prompt.Refresh.Axis01_EN)
            {
                //GTS_Fun.Axis_Home.Home_Upper_Monitor(1);
                if (GTS_Fun.Axis_Home.Axis01_Home_Down_Motor() == 0)
                {
                    appendInfo("X轴归零完成！！！");
                }
                else
                {
                    appendInfo("X轴归零失败！！！");
                }
                    
            }
            else
            {
                appendInfo("X轴使能关闭，归零失败！！！");
            }
        }
        private void Axis02_Home()
        {
            if (Prompt.Refresh.Axis02_EN)
            {
                //GTS_Fun.Axis_Home.Home_Upper_Monitor(2);
                if (GTS_Fun.Axis_Home.Axis02_Home_Down_Motor() == 0)
                {
                    appendInfo("Y轴归零完成！！！");
                }
                else
                {
                    appendInfo("Y轴归零失败！！！");
                }
            }
            else
            {
                appendInfo("Y轴使能关闭，归零失败！！！");
            }
        }

        //建立直角坐标系
        private void button2_Click(object sender, EventArgs e)
        {
            GTS_Fun.Interpolation.Coordination(Para_List.Parameter.Work.X, Para_List.Parameter.Work.Y);
            appendInfo("直角坐标系建立完成！！！");
        }
        //DXF 窗口关闭
        private void Dxf_FormClosed(object sender, FormClosedEventArgs e)
        {
            Refresh_Timer.Close();
        }
        //工件坐标系偏移X
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox4.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Work = new Vector(tmp, Para_List.Parameter.Work.Y);
            });
        }
        //工件坐标系偏移Y
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox3.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Work = new Vector(Para_List.Parameter.Work.X, tmp);
            });
        }
        //直线插补速度
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox2.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Line_synVel = tmp;
            });
        }
        //直线插补加速度
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox1.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Line_synAcc = tmp;
            });
        }
        //圆弧插补速度
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox6.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Circle_synVel = tmp;
            });
        }
        //圆弧插补加速度
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox5.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Circle_synAcc = tmp;
            });
        }
        //坐标运动平滑系数
        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox12.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Syn_EvenTime = tmp;
            });
        }
        //直线终止速度
        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox14.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Line_endVel = tmp;
            });
        }
        //圆弧终止速度
        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox13.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Circle_endVel = tmp;
            });
        }
        //参数保存
        private void button15_Click(object sender, EventArgs e)
        {
            Para_List.Serialize_Parameter.Serialize("Para.xml");
            appendInfo("参数保存成功！！！");
        }
        //参数读取
        private void button14_Click(object sender, EventArgs e)
        {
            Para_List.Serialize_Parameter.Reserialize("Para.xml");
            appendInfo("参数读取成功！！！");
        }
        //calibrate calibration Standard 
        private void button16_Click(object sender, EventArgs e)
        {
            Thread Correct_Data_thread = new Thread(Correct_Data);
            Correct_Data_thread.Start();
        }
        private void Correct_Data()
        {
            Calibration.Exit_Flag = false;
            Calibration.Get_Datas();

        }
        //calibrate calibration Standard Exit
        private void button17_Click(object sender, EventArgs e)
        {
            Calibration.Exit_Flag = true;
        }
        //without Compensation process work start
        private void button20_Click(object sender, EventArgs e)
        {
            Thread Integrate_thread = new Thread(Integrated_Start);
            Integrate_thread.Start();

        }
        private void Integrated_Start()
        {
            Integrated.Rts_Gts(Rtc_List_Data);
        }
        //整合加工终止 
        private void button19_Click(object sender, EventArgs e)
        {
            Integrated.Exit_Flag = true;
        }
        //数据拆分
        private void button25_Click(object sender, EventArgs e)
        {
            Thread Resolve_thread = new Thread(Resolve_Start);
            Resolve_thread.Start();
            Resolve_thread.Join();
        }
        private void Resolve_Start()
        {
            Rtc_List_Data.Clear();
            Data_Resolve Test = new Data_Resolve();
            if (Concat_List_Data.Count>=1)
            {
                Test.Separate_Rtc_Gts_Limit(Concat_List_Data).ForEach(m => Rtc_List_Data.Add(m));
                appendInfo("RTC和GTS数据拆分完成！！！！");
                appendInfo("RTC和GTS数据拆分后数据数量：" + Rtc_List_Data.Count);
                //for (int i = 0; i < Rtc_List_Data.Count; i++)
                //{
                //    appendInfo(string.Format("计数:{0}", Rtc_List_Data[i].Count));
                //}
                //for (int i = 0; i < Rtc_List_Data.Count; i++)
                //{
                //    for (int j = 0; j < Rtc_List_Data[i].Count; j++)
                //    {
                //        if (Rtc_List_Data[i][j].Type == 11)
                //        {
                //            appendInfo(string.Format("X：{0}，Y：{1}", Rtc_List_Data[i][0].Rtc_x, Rtc_List_Data[i][0].Rtc_y));
                //            appendInfo(string.Format("角度：{0}", Rtc_List_Data[i][j].Angle));
                //        }
                //    }
                //}
                //验证坐标
                //for (int i = 0; i < Rtc_List_Data.Count; i++)
                //{
                //    for (int j = 0; j < Rtc_List_Data[i].Count; j++)
                //    {
                //        if (Rtc_List_Data[i][j].Type == 11)
                //        {
                //            appendInfo(string.Format("起点({0},{1}),终点({2},{3})", Rtc_List_Data[i][0].Rtc_x + Rtc_List_Data[i][0].Gts_x, Rtc_List_Data[i][0].Rtc_y + Rtc_List_Data[i][0].Gts_y, Rtc_List_Data[i][0].End_x + Rtc_List_Data[i][0].Gts_x, Rtc_List_Data[i][0].End_y + Rtc_List_Data[i][0].Gts_y));
                //            //appendInfo(string.Format("角度：{0}", Rtc_List_Data[i][j].Angle));
                //        }
                //    }
                //}
                //for (int i = 0; i < Rtc_List_Data.Count; i++)
                //{
                //    appendInfo("序号：" + i + "加工类型:" + Rtc_List_Data[i][0].Work);
                //    for (int j = 0; j < Rtc_List_Data[i].Count; j++)
                //    {
                //        if (Rtc_List_Data[i][j].Work == 10)
                //        {
                //            if (Rtc_List_Data[i][j].Type == 2)
                //            {
                //                appendInfo(string.Format("    类型：{4},起点({0},{1}),终点({2},{3}),角度{5}", Rtc_List_Data[i][j].Start_x, Rtc_List_Data[i][j].Start_y, Rtc_List_Data[i][j].End_x, Rtc_List_Data[i][j].End_y, Rtc_List_Data[i][j].Type, Rtc_List_Data[i][j].Angle));
                //            }
                //            else if (Rtc_List_Data[i][j].Type == 3)
                //            {
                //                appendInfo(string.Format("    类型：{4},起点({0},{1}),终点({2},{3}),角度{5}", Rtc_List_Data[i][j].Start_x, Rtc_List_Data[i][j].Start_y, Rtc_List_Data[i][j].End_x, Rtc_List_Data[i][j].End_y, Rtc_List_Data[i][j].Type, Rtc_List_Data[i][j].Angle));
                //            }
                //            else if (Rtc_List_Data[i][j].Type == 1)
                //            {
                //                appendInfo(string.Format("    类型：{4},起点({0},{1}),终点({2},{3})", Rtc_List_Data[i][j].Start_x, Rtc_List_Data[i][j].Start_y, Rtc_List_Data[i][j].End_x, Rtc_List_Data[i][j].End_y, Rtc_List_Data[i][j].Type));
                //            }
                //        }
                //        else if (Rtc_List_Data[i][j].Work == 20)
                //        {
                //            if (j == 0) appendInfo(string.Format("    本段起点({0},{1})", Rtc_List_Data[i][j].Rtc_x, Rtc_List_Data[i][j].Rtc_y));
                //            if (Rtc_List_Data[i][j].Type == 11)
                //            {
                //                appendInfo(string.Format("    类型：{4},起点({0},{1}),终点({2},{3}),角度{5},圆心({6},{7})", Rtc_List_Data[i][j].Start_x, Rtc_List_Data[i][j].Start_y, Rtc_List_Data[i][j].End_x, Rtc_List_Data[i][j].End_y, Rtc_List_Data[i][j].Type, Rtc_List_Data[i][j].Angle, Rtc_List_Data[i][j].Center_x, Rtc_List_Data[i][j].Center_y));
                //            }
                //            else if (Rtc_List_Data[i][j].Type == 15)
                //            {
                //                appendInfo(string.Format("    类型：{4},起点({0},{1}),终点({2},{3})", Rtc_List_Data[i][j].Start_x, Rtc_List_Data[i][j].Start_y, Rtc_List_Data[i][j].End_x, Rtc_List_Data[i][j].End_y, Rtc_List_Data[i][j].Type));
                //            }
                //            else if (Rtc_List_Data[i][j].Type == 13)
                //            {
                //                appendInfo(string.Format("    类型：{4},起点({0},{1}),终点({2},{3})", Rtc_List_Data[i][j].Start_x, Rtc_List_Data[i][j].Start_y, Rtc_List_Data[i][j].End_x, Rtc_List_Data[i][j].End_y, Rtc_List_Data[i][j].Type));
                //            }
                //        }
                //    }
                //}
                //for (int i = 0; i < Rtc_List_Data.Count; i++)
                //{
                //    appendInfo("序号：" + i + "加工类型:" + Rtc_List_Data[i][0].Work);
                //    if (Rtc_List_Data[i].Count >= 1)
                //    {
                //        if (Rtc_List_Data[i][0].Work == 20)
                //        {
                //            appendInfo(string.Format("    Gts基准点({0},{1})", Rtc_List_Data[i][0].Gts_x, Rtc_List_Data[i][0].Gts_y));
                //            appendInfo(string.Format("    Rtc起点({0},{1})", Rtc_List_Data[i][0].Rtc_x, Rtc_List_Data[i][0].Rtc_y));
                //        }
                //    }
                //}
            }
            else
            {
                appendInfo("无可用数据拆分！！！！");
            }            
        }
        //定位坐标原点
        private void button10_Click(object sender, EventArgs e)
        {
            GTS_Fun.Interpolation.Gts_Ready(0, 0);
        }
        //计算标定板仿射参数
        private void button12_Click(object sender, EventArgs e)
        {
            Thread Get_Cal_Angle_thread = new Thread(Get_Cal_Angle_Matricx);
            Get_Cal_Angle_thread.Start();

        }
        private void Get_Cal_Angle_Matricx()
        {
            ///先矫正坐标原点
            if (Calibration.Calibrate_Org())
            {
                appendInfo("坐标原点矫正成功！！！");
            }
            else
            {
                appendInfo("坐标原点矫正失败！！！");
                return;
            }
            appendInfo("矫正后数据 X：" + Para_List.Parameter.Cal_Org.X + ", Y：" + Para_List.Parameter.Cal_Org.Y);
            ///建立直角坐标系
            GTS_Fun.Interpolation.Coordination(Para_List.Parameter.Work.X, Para_List.Parameter.Work.Y);

            //矫正标定板旋转参数
            if (Calibration.Cal_Calibration_Angle_Matrix())
            {
                appendInfo("标定板旋转参数矫正成功！！！");
            }
            else
            {
                appendInfo("标定板旋转参数矫正失败！！！");
                return;
            }
            appendInfo("标定板旋转参数Stretch_X：" + Para_List.Parameter.Cal_Trans_Angle.Stretch_X);
            appendInfo("标定板旋转参数Distortion_X：" + Para_List.Parameter.Cal_Trans_Angle.Distortion_X);
            appendInfo("标定板旋转参数DeltaX：" + Para_List.Parameter.Cal_Trans_Angle.Delta_X);
            appendInfo("标定板旋转参数Stretch_Y：" + Para_List.Parameter.Cal_Trans_Angle.Stretch_Y);
            appendInfo("标定板旋转参数Distortion_Y：" + Para_List.Parameter.Cal_Trans_Angle.Distortion_Y);
            appendInfo("标定板旋转参数DeltaY：" + Para_List.Parameter.Cal_Trans_Angle.Delta_Y);
        }

        //提取坐标
        private void button11_Click(object sender, EventArgs e)
        {
            Vector Tem = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
            MessageBox.Show("X坐标：" + Convert.ToString(Tem.X) + "  Y坐标：" + Convert.ToString(Tem.Y));
        }
        decimal Cor_x, Cor_y;
        //X坐标
        private void textBox23_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox23.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Cor_x = tmp;
            });
        }

        //校准相机坐标原点
        private void Cal_Org_Point_Click(object sender, EventArgs e)
        {
            if (Calibration.Calibrate_Org())
            {
                appendInfo("坐标原点矫正成功！！！");
            }
            else
            {
                appendInfo("坐标原点矫正失败！！！");
                return;
            }
            appendInfo("矫正后数据 X：" + Para_List.Parameter.Cal_Org.X + ", Y：" + Para_List.Parameter.Cal_Org.Y);
        }
        //生成GTS校准数据
        private void Csv_Test_Click(object sender, EventArgs e)
        {
            string File_Name = "";
            if (Para_List.Parameter.Gts_Affinity_Type == 1)
            {
                File_Name = "Gts_Affinity_Matrix_All.xml";
            }
            else
            {
                File_Name = "Gts_Affinity_Matrix_Three.xml";
            }
            //生成全新GTS校准文件
            Gts_Cal_Data_Resolve.Resolve(Serialize_Data.Reserialize_Correct_Data("Gts_Correct_Data.xml"));

            //将Affinity数据转换为CSV并保存
            //CSV_RW.SaveCSV(CSV_RW.Affinity_Matrix_DataTable(Serialize_Data.Reserialize_Affinity_Matrix(File_Name)), File_Name);

            //将Gts Correctdata数据转换为CSV并保存
            //CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Serialize_Data.Reserialize_Correct_Data("Gts_Correct_Data.xml")), "Gts_Correct_Data");
        }
        //生成GTS校准数据
        private void Generate_Cal_Aquisition_Click(object sender, EventArgs e)
        {
            //Calibration.Get_Datas_Test();

            //计算偏差
            Vector Cam = new Vector();
            Vector Cal_Actual_Point = new Vector();
            Vector Result = new Vector();
            //定位矫正坐标
            GTS_Fun.Interpolation.Gts_Ready_Test(Cor_x, Cor_y);            
            //相机反馈的当前坐标
            Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Coordinate_Correct(1));//触发拍照 
            if (Cam.Length == 0)
            {
                MessageBox.Show("相机坐标提取失败，请检查！！！");
                return;
            }
            //当前平台坐标 对应的 标定板坐标
            Cal_Actual_Point = Calibration.Get_Cal_Angle_Point(new Vector(Cor_x, Cor_y));
            //数据保存
            Result = new Vector(Cal_Actual_Point + Cam);//实际坐标
            Result = new Vector(Result.X - Cor_x,Result.Y - Cor_y);
            //信息输出
            MessageBox.Show(string.Format("计算差值XY:({0},{1})", Result.X, Result.Y));

            //Laser_Watt_Cal.Resolve(CSV_RW.OpenCSV(@"./\Config/Laser_Data.csv"));


        }
        //坐标补偿加工
        private void Correct_Rtc_Click(object sender, EventArgs e)
        {
            Thread Integrated_Correct_thread = new Thread(Integrated_Correct_Start);
            Integrated_Correct_thread.Start();
        }
        private void Integrated_Correct_Start()
        {
            Integrated.Rts_Gts_Correct(Rtc_List_Data);
        }
        //生成RTC校准数据
        private void Rtc_Affinity_Click(object sender, EventArgs e)
        {           
            //生成RTC仿射变换参数
            Rtc_Cal_Data_Resolve.Resolve(Serialize_Data.Reserialize_Correct_Data("Rtc_Correct_Data.xml"));
        }
        //barreal_distortion 桶形畸变加工
        private void Barrel_Distortion_Click(object sender, EventArgs e)
        {
            Thread Barrel_Distortion_Correct_thread = new Thread(Barrel_Distortion_Correct);
            Barrel_Distortion_Correct_thread.Start();
        }
        private void Barrel_Distortion_Correct()
        {
            if (((Int16)(Para_List.Parameter.Rtc_Distortion_Data_Limit / Para_List.Parameter.Rtc_Distortion_Data_Interval)%2) != 0)
            {
                appendInfo(string.Format("振镜矫正范围除以打标间距：{0}不为偶数，禁止加工！！！", Para_List.Parameter.Rtc_Distortion_Data_Limit / Para_List.Parameter.Rtc_Distortion_Data_Interval));
                return;
            }
            else
            {
                Rtc_List_Data.Clear();
                Rtc_List_Data = new List<List<Interpolation_Data>>(Calibration.Generate_Rtc_Correct_Data(Para_List.Parameter.Rtc_Distortion_Data_Type, Para_List.Parameter.Rtc_Distortion_Data_Radius, Para_List.Parameter.Rtc_Distortion_Data_Interval, Para_List.Parameter.Rtc_Distortion_Data_Limit));
                Integrated.Rts_Gts_Cal_Rtc(Rtc_List_Data);
            }
            
        }
        //标定板二次校准
        private void Calibration_Target_RE_Click(object sender, EventArgs e)
        {
            Thread Re_Correct_Data_thread = new Thread(Re_Correct_Data);
            Re_Correct_Data_thread.Start();
        }
        private void Re_Correct_Data()
        {
            Calibration.Exit_Flag = false;
            Calibration.Get_Datas_Correct();
        }

        //Y坐标
        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox22.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Cor_y = tmp;
            });
        }
        //刀具半径
        private void Cutter_Radius_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(Cutter_Radius.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Cutter_Radius = tmp;
            });
        }
        //刀具补偿类型
        private void Cutter_Comp_SelectedIndexChanged(object sender, EventArgs e)
        {
            Para_List.Parameter.Cutter_Type = (short)Cutter_Comp.SelectedIndex;
        }
        //定位矫正坐标点
        private void Go_Cal_Point_Click(object sender, EventArgs e)
        {
            GTS_Fun.Interpolation.Gts_Ready_Test(Cor_x, Cor_y);
            //计算偏差
            Vector Cam = new Vector();
            Vector Cam_Delta = new Vector();
            Vector Cal_Actual_Point = new Vector();
            Vector Result = new Vector(); 
            //调用相机，获取对比的坐标信息
            Thread.Sleep(50);
            //相机反馈的当前坐标
            Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Pixel_Correct(1));//触发拍照 
            if (Cam.Length == 0)
            {
                MessageBox.Show("相机坐标提取失败，请检查！！！");
                return;
            }
            //相机测算的实际偏差值:(相机反馈的当前坐标) - (相机中心坐标)
            Cam_Delta = new Vector(Cam.X - 243 * Para_List.Parameter.Cam_Reference, Cam.Y - 324 * Para_List.Parameter.Cam_Reference);
            //当前平台坐标 对应的 标定板坐标
            Cal_Actual_Point =Calibration.Get_Cal_Angle_Point(new Vector(Cor_x, Cor_y));
            //数据保存
            Result = new Vector(Cal_Actual_Point.X - Cam_Delta.X - Cor_x, Cal_Actual_Point.Y - Cam_Delta.Y - Cor_y);//实际坐标
            //信息输出
            MessageBox.Show(string.Format("计算差值XY:({0},{1})", Result.X,Result.Y));
        }
        //加工起始位置选择
        private void Start_Pos_Sel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Para_List.Parameter.Calibration_Type = (UInt16)Start_Pos_Sel.SelectedIndex;
        }
        //RTC加工重复次数
        private void Rtc_Repeat_Num_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!ushort.TryParse(Rtc_Repeat_Num.Text, out ushort tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Rtc_Repeat = tmp;
            });
        }
        //GTS加工重复次数
        private void Gts_Repeat_Num_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!ushort.TryParse(Gts_Repeat_Num.Text, out ushort tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Gts_Repeat = tmp;
            });
        }
        /// <summary>
        /// 重新加载Gts 校准文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Gts_AM_Click(object sender, EventArgs e)
        {
            GTS_Fun.Interpolation.Load_Affinity_Matrix();
        }
        /// <summary>
        /// clear info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Debug_Info_Display_DoubleClick(object sender, EventArgs e)
        {
            Debug_Info_Display.Text = "";
        }
        //参数验证
        private void button3_Click(object sender, EventArgs e)
        {
            //验证补偿前后坐标互转
            //Vector Tmp1 = new Vector(Cor_x, Cor_y);
            //Vector Tmp2 = Gts_Cal_Data_Resolve.Get_Affinity_Point(0, Tmp1.X, Tmp1.Y, GTS_Fun.Interpolation.affinity_Matrices);
            //Vector Tmp3 = Gts_Cal_Data_Resolve.Get_Affinity_Point(1, Tmp2.X, Tmp2.Y, GTS_Fun.Interpolation.affinity_Matrices);
            //MessageBox.Show(string.Format("原始({0},{1}),补偿后({2},{3}),补偿后反转({4},{5})", Tmp1.X, Tmp1.Y, Tmp2.X, Tmp2.Y, Tmp3.X, Tmp3.Y));
            //List<List<Interpolation_Data>> Test = new List<List<Interpolation_Data>>(Calibration.Generate_Rtc_Correct_Data(1,1,2.5m,60));
            //appendInfo("直线数量:" + (Test.Count - 1));
            //for (int i = 0; i < Test.Count; i++)
            //{
            //    for (int j = 0; j < Test[i].Count; j++)
            //    {
            //        if (Test[i][j].Type == 15)
            //        {
            //            appendInfo(string.Format("起点({0},{1}),终点({2},{3})", Test[i][0].Rtc_x, Test[i][0].Rtc_y, Test[i][0].End_x, Test[i][0].End_y));                        
            //        }
            //    }
            //}
            //List<List<Interpolation_Data>> Test01 = new List<List<Interpolation_Data>>(Calibration.Generate_Rtc_Correct_Data(0, 1, 2.5m, 60));
            //appendInfo("整圆数量:"+ (Test01.Count - 1));
            //for (int i = 0; i < Test01.Count; i++)
            //{
            //    for (int j = 0; j < Test01[i].Count; j++)
            //    {
            //        if (Test01[i][j].Type == 11)
            //        {
            //            appendInfo(string.Format("圆心({0},{1}),起点({2},{3})", Test01[i][0].Center_x, Test01[i][0].Center_y, Test01[i][0].Rtc_x, Test01[i][0].Rtc_y));
            //        }
            //    }
            //}
            //采集生成校准文件的数据
            //List<Vector> Test02 = new List<Vector>(Calibration.Rtc_Correct_Coordinate());
            //for (int i = 0; i < Test02.Count; i++)
            //{
            //    appendInfo(string.Format("坐标({0},{1})", Test02[i].X, Test02[i].Y));
            //}
            //采集生成Rtc坐标系矫正的数据
            List<Vector> Test02 = new List<Vector>(Calibration.Rtc_Correct_AFF_Coordinate());
            for (int i = 0; i < Test02.Count; i++)
            {
                appendInfo(string.Format("坐标({0},{1})", Test02[i].X, Test02[i].Y));
            }
        }
        //测试
        private void Test_01_Click(object sender, EventArgs e)
        {
            Thread Test_Correct_thread = new Thread(Test_Correct);
            Test_Correct_thread.Start();
        }
        private void Test_Correct()
        {
            Integrated.Rts_Gts_Cal_Rtc_Affinity(Rtc_List_Data);
            //if (Calibration.Cal_Cam_Affinity_Test())
            //{
            //    MessageBox.Show("相机坐标系矫正完成！！！");
            //}
            //else
            //{
            //    MessageBox.Show("相机坐标系矫正失败！！！");
            //}
        }
        /// <summary>
        /// Rtc坐标系校准数据采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rtc_Cor_Data_Acqu_Click(object sender, EventArgs e)
        {
            Thread Rtc_AFF_Correct_thread = new Thread(Rtc_AFF_Correct);
            Rtc_AFF_Correct_thread.Start();
        }
        private void Rtc_AFF_Correct()
        {
            if ((((Int16)(Para_List.Parameter.Rtc_Calibration_X_Len / Para_List.Parameter.Rtc_Calibration_Cell) % 2) != 0) || (((Int16)(Para_List.Parameter.Rtc_Calibration_Y_Len / Para_List.Parameter.Rtc_Calibration_Cell) % 2) != 0))
            {
                MessageBox.Show(string.Format("振镜矫正范围除以打标间距：X-{0}或Y-{1} 不为偶数，数据采集终止！！！", Para_List.Parameter.Rtc_Calibration_X_Len / Para_List.Parameter.Rtc_Calibration_Cell, Para_List.Parameter.Rtc_Calibration_Y_Len / Para_List.Parameter.Rtc_Calibration_Cell));
                return;
            }
            else
            {
                Rtc_List_Data.Clear();
                Rtc_List_Data = new List<List<Interpolation_Data>>(Calibration.Generate_Rtc_AFF_Correct_Data(Para_List.Parameter.Rtc_Distortion_Data_Type, 0.8m, Para_List.Parameter.Rtc_Calibration_Cell, Para_List.Parameter.Rtc_Calibration_Y_Len));
                Integrated.Rts_No_Gts_Yes_Correct(Rtc_List_Data);
            }

        }

        private void Rtc_Cor_Circle_Acqui_Click(object sender, EventArgs e)
        {

            Thread Rtc_Cor_Circle_thread = new Thread(Rtc_Cor_Circle);
            Rtc_Cor_Circle_thread.Start();
        }
        private void Rtc_Cor_Circle()
        {
            Calibration.Rtc_Correct_AFF_Coordinate();
        }
       //参数验证
        private void button4_Click(object sender, EventArgs e)
        {
            Calibration.Get_Datas_Angle();
            //Calibration.Get_Datas_Test();
        }
        /// <summary>
        /// 振镜不补偿，平台补偿
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rtc_No_Gts_Yes_Click(object sender, EventArgs e)
        {
            Thread Rtc_No_Gts_Yes_thread = new Thread(Rtc_No_Gts_Yes_Test);
            Rtc_No_Gts_Yes_thread.Start();
        }
        private void Rtc_No_Gts_Yes_Test()
        {
            Integrated.Rts_No_Gts_Yes_Correct(Rtc_List_Data);
        }
        //RTC矩阵补偿
        private void button5_Click(object sender, EventArgs e)
        {

            Thread Rtc_Mat_Gts_Yes_thread = new Thread(Rtc_Mat_Gts_Yes);
            Rtc_Mat_Gts_Yes_thread.Start();
        }
        private void Rtc_Mat_Gts_Yes()
        {
            Integrated.Rtc_Mat_Gts_Yes_Correct(Rtc_List_Data);
        }

        ///// <summary>
        /// 定位坐标点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button21_Click(object sender, EventArgs e)
        {
            GTS_Fun.Interpolation.Gts_Ready(Cor_x, Cor_y);
        }

    }
}
