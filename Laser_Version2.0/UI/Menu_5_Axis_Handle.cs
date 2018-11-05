using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using GTS;
using System.Threading;
using Prompt;

namespace Laser_Build_1._0
{
    public partial class Menu_5_Axis_Handle : Form
    {
        public Menu_5_Axis_Handle()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;//关闭CheckForIllegalCrossThreadCalls
        }
        //定义Menu_5_Axis_Handle窗口刷新定时器
        System.Timers.Timer Menu_5_Axis_Handle_Timer = new System.Timers.Timer(10);

        //定义当前窗口所需变量
        decimal acc=500m, dcc=500m, autoVel=200m, mannualVel=100m;
        decimal step=10m;//默认值10mm 

        //定义点动/连动切换变量
        short Point_Con;

        //定义轴号选择
        short Axis_No = 1;

        //指令返回值
        short Gts_return;
        private void Menu_5_Axis_Handle_Load(object sender, EventArgs e)
        {
            //启用定时器
            Menu_5_Axis_Handle_Timer.Elapsed += Menu_5_Axis_Handle_Timer_Elapsed;
            Menu_5_Axis_Handle_Timer.AutoReset = true;
            Menu_5_Axis_Handle_Timer.Enabled = true;
            Menu_5_Axis_Handle_Timer.Start();

            //初始化显示
            numericUpDown1.Value = Axis_No;
            //加速度
            textBox1.Text = Convert.ToString(acc);
            //减速度
            textBox2.Text = Convert.ToString(dcc);
            //步长
            textBox3.Text = Convert.ToString(step);//当前1pluse-10um,转化为mm需除以100
            //自动速度
            textBox4.Text = Convert.ToString(autoVel);//当前pluse/ms,转化为mm/s需除以10
            //手动速度
            textBox5.Text = Convert.ToString(mannualVel);

            //原点回归参数
            //回零速度
            textBox7.Text = Convert.ToString(Para_List.Parameter.Home_High_Speed);
            //回零加速度
            textBox8.Text = Convert.ToString(Para_List.Parameter.Home_acc);
            //回零减速度
            textBox9.Text = Convert.ToString(Para_List.Parameter.Home_dcc);
            //回零平滑时间
            textBox10.Text = Convert.ToString(Para_List.Parameter.Home_smoothTime);
            //原点偏移
            textBox6.Text = Convert.ToString(Para_List.Parameter.Home_OffSet);


        }
        
        //定时器刷新程序
        private void Menu_5_Axis_Handle_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (Axis_No == 1)
                {
                    if (Prompt.Refresh.Axis01_Limit_Up) { button1.BackColor = Color.Red; } else { button1.BackColor = SystemColors.Control; }//Axis01轴正限位
                    if (Prompt.Refresh.Axis01_Limit_Down) { button3.BackColor = Color.Red; } else { button3.BackColor = SystemColors.Control; }//Axis01轴负限位
                    if (Prompt.Refresh.Axis01_Home) { button2.BackColor = Color.Green; } else { button2.BackColor = SystemColors.Control; }// Axis01轴原点
                    if (Prompt.Refresh.Axis01_Alarm) { button4.BackColor = Color.Green; } else { button4.BackColor = SystemColors.Control; }//Axis01轴报警
                    if (Prompt.Refresh.Axis01_Alarm_Cl) { button5.BackColor = Color.Green; } else { button5.BackColor = SystemColors.Control; }//Axis01轴报警清除
                    if (Prompt.Refresh.Axis01_MC_Err) { button15.BackColor = Color.Green; } else { button15.BackColor = SystemColors.Control; }//Axis01轴板卡报警(跟随误差越限)
                    if (Prompt.Refresh.Axis01_EN) { button8.BackColor = Color.Green; button8.Text = "使能打开"; } else { button8.BackColor = SystemColors.Control; button8.Text = "使能关闭"; }//Axis01轴使能
                    if (Prompt.Refresh.Axis01_Busy) { button14.BackColor = Color.Green; } else { button14.BackColor = SystemColors.Control; }//Axis01轴输出中
                    //if (Prompt.Refresh.Axis01_IO_Stop  ) { buttonx.BackColor = Color.Green;  } else { buttonx.BackColor = SystemColors.Control; }//Axis01轴IO停止
                    //if (Prompt.Refresh.Axis01_IO_EMG) { buttonx.BackColor = Color.Green;  } else { buttonx.BackColor = SystemColors.Control; }//Axis01轴IO急停
                    if (Prompt.Refresh.Axis01_Motor_Posed) { Axis_Motor_Posed.BackColor = Color.Green; } else { Axis_Motor_Posed.BackColor = SystemColors.Control; }//Axis01轴 Motor到位
                    if (Prompt.Refresh.Axis01_Upper_Posed) { Axis_Upper_Posed.BackColor = Color.Green; } else { Axis_Upper_Posed.BackColor = SystemColors.Control; }//Axis01轴 Upper到位
                    //规划位置指示
                    label12.Text = Convert.ToString(Convert.ToDecimal(Prompt.Refresh.Axis01_prfPos) / Para_List.Parameter.Gts_Pos_reference);
                    //实际位置指示
                    label23.Text = Convert.ToString(Convert.ToDecimal(Prompt.Refresh.Axis01_encPos) / Para_List.Parameter.Gts_Pos_reference);
                    //加速度指示
                    label13.Text = Convert.ToString(Convert.ToDecimal(Prompt.Refresh.Axis01_acc) * Para_List.Parameter.Gts_Acc_reference);
                    //减速度指示
                    label14.Text = Convert.ToString(Convert.ToDecimal(Prompt.Refresh.Axis01_dcc) * Para_List.Parameter.Gts_Acc_reference);
                    //速度指示
                    label15.Text = Convert.ToString(Convert.ToDecimal(Prompt.Refresh.Axis01_vel) * Para_List.Parameter.Gts_Vel_reference);
                    //模式指示
                    switch (Prompt.Refresh.Axis01_mode)
                    {
                        case 0:
                            label16.Text = "点位运动";
                            break;
                        case 1:
                            label16.Text = "Jog模式";
                            break;
                        case 2:
                            label16.Text = "PT模式";
                            break;
                        case 3:
                            label16.Text = "电子齿轮模式";
                            break;
                        case 4:
                            label16.Text = "Follow模式";
                            break;
                        case 5:
                            label16.Text = "插补模式";
                            break;
                        case 6:
                            label16.Text = "Pvt模式";
                            break;
                        default:
                            label16.Text = "  ";
                            break;
                    }
                }
                else if (Axis_No == 2)
                {
                    if (Prompt.Refresh.Axis02_Limit_Up) { button1.BackColor = Color.Red; } else { button1.BackColor = SystemColors.Control; }//Axis02轴正限位
                    if (Prompt.Refresh.Axis02_Limit_Down) { button3.BackColor = Color.Red; } else { button3.BackColor = SystemColors.Control; }//Axis02轴负限位
                    if (Prompt.Refresh.Axis02_Home) { button2.BackColor = Color.Green; } else { button2.BackColor = SystemColors.Control; }// Axis02轴原点
                    if (Prompt.Refresh.Axis02_Alarm) { button4.BackColor = Color.Green; } else { button4.BackColor = SystemColors.Control; }//Axis02轴报警
                    if (Prompt.Refresh.Axis02_Alarm_Cl) { button5.BackColor = Color.Green; } else { button5.BackColor = SystemColors.Control; }//Axis02轴报警清除
                    if (Prompt.Refresh.Axis02_MC_Err) { button15.BackColor = Color.Green; } else { button15.BackColor = SystemColors.Control; }//Axis02轴板卡报警(跟随误差越限)
                    if (Prompt.Refresh.Axis02_EN) { button8.BackColor = Color.Green; button8.Text = "使能打开"; } else { button8.BackColor = SystemColors.Control; button8.Text = "使能关闭"; }//Axis02轴使能
                    if (Prompt.Refresh.Axis02_Busy) { button14.BackColor = Color.Green; } else { button14.BackColor = SystemColors.Control; }//Axis02轴输出中
                    //if (Prompt.Refresh.Axis02_IO_Stop  ) { buttonx.BackColor = Color.Green;  } else { buttonx.BackColor = SystemColors.Control; }//Axis02轴IO停止
                    //if (Prompt.Refresh.Axis02_IO_EMG) { buttonx.BackColor = Color.Green;  } else { buttonx.BackColor = SystemColors.Control; }//Axis02轴IO急停
                    if (Prompt.Refresh.Axis02_Motor_Posed) { Axis_Motor_Posed.BackColor = Color.Green; } else { Axis_Motor_Posed.BackColor = SystemColors.Control; }//Axis02轴 Motor到位
                    if (Prompt.Refresh.Axis02_Upper_Posed) { Axis_Upper_Posed.BackColor = Color.Green; } else { Axis_Upper_Posed.BackColor = SystemColors.Control; }//Axis02轴 Upper到位

                    //规划位置指示
                    label12.Text = Convert.ToString(Convert.ToDecimal(Prompt.Refresh.Axis02_prfPos) / Para_List.Parameter.Gts_Pos_reference);
                    //实际位置指示
                    label23.Text = Convert.ToString(Convert.ToDecimal(Prompt.Refresh.Axis02_encPos) / Para_List.Parameter.Gts_Pos_reference);
                    //加速度指示
                    label13.Text = Convert.ToString(Convert.ToDecimal(Prompt.Refresh.Axis02_acc) * Para_List.Parameter.Gts_Acc_reference);
                    //减速度指示
                    label14.Text = Convert.ToString(Convert.ToDecimal(Prompt.Refresh.Axis02_dcc) * Para_List.Parameter.Gts_Acc_reference);
                    //速度指示
                    label15.Text = Convert.ToString(Convert.ToDecimal(Prompt.Refresh.Axis02_vel) * Para_List.Parameter.Gts_Vel_reference);
                    //模式指示
                    switch (Prompt.Refresh.Axis02_mode)
                    {
                        case 0:
                            label16.Text = "点位运动";
                            break;
                        case 1:
                            label16.Text = "Jog模式";
                            break;
                        case 2:
                            label16.Text = "PT模式";
                            break;
                        case 3:
                            label16.Text = "电子齿轮模式";
                            break;
                        case 4:
                            label16.Text = "Follow模式";
                            break;
                        case 5:
                            label16.Text = "插补模式";
                            break;
                        case 6:
                            label16.Text = "Pvt模式";
                            break;
                        default:
                            label16.Text = "  ";
                            break;
                    }
                }
                //点动/连动模式按钮指示
                if (Point_Con == 0) { button13.Text = "连动模式"; } else { button13.Text = "点动模式"; };
                //标题操作指示
                if (Axis_No == 1) { this.Text = "Axis01_手动操作"; } else if (Axis_No == 2) { this.Text = "Axis02_手动操作"; } else { this.Text = "Axis_手动操作"; };
            });
        }

        private void Menu_5_Axis_Handle_FormClosed(object sender, FormClosedEventArgs e)
        {
            Menu_5_Axis_Handle_Timer.Close();
        }

        //报警清除按钮Button5 点击
        private void button5_Click(object sender, EventArgs e)
        {

        }
        //报警清除按钮Button5 鼠标按下
        private void button5_MouseDown(object sender, MouseEventArgs e)
        {

            if (Axis_No == 1)
            {
                //输出Alarm清除 ON
                Gts_return = MC.GT_SetDoBit(11, Axis_No, 0);
                Log.Commandhandler("Menu_5_Axis_Handle_Gts_return_GT_SetDoBit", Gts_return);
            }
            else if (Axis_No == 2)
            {
                //输出Alarm清除 ON
                Gts_return = MC.GT_SetDoBit(11, Axis_No, 0);
                Log.Commandhandler("Menu_5_Axis_Handle_Gts_return_GT_SetDoBit", Gts_return);
            }
            
        }
        //报警清除按钮Button5 鼠标抬起
        private void button5_MouseUp(object sender, MouseEventArgs e)
        {
            if (Axis_No == 1)
            {
                //输出Alarm清除 OFF
                Gts_return = MC.GT_SetDoBit(11, 1, 1);
                Log.Commandhandler("Menu_5_Axis_Handle_Gts_return_GT_SetDoBit", Gts_return);
            }else if (Axis_No == 2)
            {
                //输出Alarm清除 OFF
                Gts_return = MC.GT_SetDoBit(11, 2, 1);
                Log.Commandhandler("Menu_5_Axis_Handle_Gts_return_GT_SetDoBit", Gts_return);
            }
        }
        
        
        
        //点动、连动切换
        private void button13_Click(object sender, EventArgs e)
        {  
            if (Point_Con == 0)
            {
                Point_Con = 1;//切换至点动
            }
            else
            {
                Point_Con = 0;//切换至连动
            }
        }
        //平滑停止
        private void button9_Click(object sender, EventArgs e)
        {
            GTS_Fun.Motion.Smooth_Stop(Axis_No); //平滑停止轴         
        }
        //紧急停止
        private void button10_Click(object sender, EventArgs e)
        {
            GTS_Fun.Motion.Emg_Stop(Axis_No);//紧急停止轴
        }
        //jog+
        private void button12_MouseDown(object sender, MouseEventArgs e)
        {
            if (Axis_No == 1)
            {
                if (Prompt.Refresh.Axis01_EN)
                {
                    if (Point_Con == 0)//连动模式
                    {
                        GTS_Fun.Motion.Jog(Axis_No, 0,mannualVel, acc, dcc);
                    }
                    else//点动模式
                    {
                        GTS_Fun.Motion.Inc(Axis_No, acc, dcc, 20,step, mannualVel);
                    }
                }
            }
            else if (Axis_No == 2)
            {
                if (Prompt.Refresh.Axis02_EN)
                {
                    if (Point_Con == 0)//连动模式
                    {
                        GTS_Fun.Motion.Jog(Axis_No, 0, mannualVel, acc, dcc);
                    }
                    else//点动模式
                    {
                        GTS_Fun.Motion.Inc(Axis_No, acc, dcc, 20, step, mannualVel);
                    }
                }
            }            
        }
        //jog-
        private void button11_MouseDown(object sender, MouseEventArgs e)
        {           

            if (Axis_No == 1)
            {
                if (Prompt.Refresh.Axis01_EN)
                {
                    if (Point_Con == 0)//连动模式
                    {
                        GTS_Fun.Motion.Jog(Axis_No, 1, mannualVel, acc, dcc);
                    }
                    else//点动模式
                    {
                        GTS_Fun.Motion.Inc(Axis_No, acc, dcc, 20, -step, mannualVel);
                    }
                }
            }
            else if (Axis_No == 2)
            {
                if (Prompt.Refresh.Axis02_EN)
                {
                    if (Point_Con == 0)//连动模式
                    {
                        GTS_Fun.Motion.Jog(Axis_No, 1, mannualVel, acc, dcc);
                    }
                    else//点动模式
                    {
                        GTS_Fun.Motion.Inc(Axis_No, acc, dcc, 20, -step, mannualVel);
                    }
                }
            }
        }
        //jog+停止运行
        private void button12_MouseUp(object sender, MouseEventArgs e)
        {
            if (Point_Con == 0)//连动模式
            {
                GTS_Fun.Motion.Smooth_Stop(Axis_No); ;//停止轴
            }            
        }
        //jog-停止运行
        private void button11_MouseUp(object sender, MouseEventArgs e)
        {
            if (Point_Con == 0)//连动模式
            {
                GTS_Fun.Motion.Smooth_Stop(Axis_No); ;//停止轴
            }
        }
        //轴编号切换
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Axis_No =Convert.ToInt16(numericUpDown1.Value);
        }
        //机械回零
        private void button16_Click(object sender, EventArgs e)
        {

            Thread home_thread = new Thread(this.Home_Thread);
            home_thread.Start();
        }
        //Home_Thread
        private void Home_Thread()
        {
            if (Axis_No == 1)
            {
                if (Prompt.Refresh.Axis01_EN)
                {
                    //GTS_Fun.Axis_Home.Home_Upper_Monitor(1);
                    if (GTS_Fun.Axis_Home.Axis01_Home_Down_Motor() == 0)
                    {
                        MessageBox.Show("X轴归零完成！！！");
                    }
                    else
                    {
                        MessageBox.Show("X轴归零失败！！！");
                    }
                }
            }
            else if (Axis_No == 2)
            {
                if (Prompt.Refresh.Axis02_EN)
                {
                    //GTS_Fun.Axis_Home.Home_Upper_Monitor(2);
                    if (GTS_Fun.Axis_Home.Axis02_Home_Down_Motor() == 0)
                    {
                        MessageBox.Show("Y轴归零完成！！！");
                    }
                    else
                    {
                        MessageBox.Show("Y轴归零失败！！！");
                    }
                }
            }
        }
        
       
        //加速度值写入变量
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {                
                if (!decimal.TryParse(textBox1.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                acc = tmp ;
            });
        }
        //减速度写入变量
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox2.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                dcc = tmp;
            });
        }
        //步长写入变量
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox3.Text, out decimal tmp))
                {                    
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                step = tmp;
            });
        }
        //自动速度写入变量
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox4.Text, out decimal tmp))
                {                    
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                autoVel = (tmp);
            });
        }
        //手动速度写入变量
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox5.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                mannualVel = (tmp);
            });
        }
        //回零速度
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox7.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Home_High_Speed = tmp;
            });
        }
        //回零加速度
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox8.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Home_acc = tmp;
            });
        }
        //回零减速度
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox9.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Home_dcc = tmp;
            });
        }
        //回零平滑时间
        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!Int16.TryParse(textBox10.Text, out Int16 tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Home_smoothTime = tmp;
            });
        }
        //原点偏移
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox6.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Home_OffSet = tmp;
            });
        }
        //复位
        private void button17_Click(object sender, EventArgs e)
        {
            GTS_Fun.Factory.Reset();
        }

        //伺服使能切换
        private void button8_Click(object sender, EventArgs e)
        {         

            if (Axis_No == 1)
            {
                if (Prompt.Refresh.Axis01_EN)
                {
                    //伺服使能 OFF
                    Gts_return = MC.GT_AxisOff(Axis_No);
                    Log.Commandhandler("Menu_5_Axis_Handle_Gts_return_GT_AxisOff", Gts_return);
                }
                else
                {
                    //伺服使能 ON
                    Gts_return = MC.GT_AxisOn(Axis_No);
                    Log.Commandhandler("Menu_5_Axis_Handle_Gts_return_GT_AxisOn", Gts_return);
                }
            }
            else if (Axis_No == 2)
            {
                if (Prompt.Refresh.Axis02_EN)
                {
                    //伺服使能 OFF
                    Gts_return = MC.GT_AxisOff(Axis_No);
                    Log.Commandhandler("Menu_5_Axis_Handle_Gts_return_GT_AxisOff", Gts_return);
                }
                else
                {
                    //伺服使能 ON
                    Gts_return = MC.GT_AxisOn(Axis_No);
                    Log.Commandhandler("Menu_5_Axis_Handle_Gts_return_GT_AxisOn", Gts_return);
                }
            }
        }
        //Axis01状态清除
        private void button6_Click(object sender, EventArgs e)
        {
            //清除轴报警状态：驱动器报警、跟随误差越限报警、限位触发报警
            Gts_return = MC.GT_ClrSts(Axis_No, 1);
            Log.Commandhandler("Menu_5_Axis_Handle_Gts_return_GT_ClrSts", Gts_return);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //清除轴规划位置、实际位置
            Gts_return = MC.GT_ZeroPos(Axis_No, 1);
            Log.Commandhandler("Menu_5_Axis_Handle_Gts_return_GT_ZeroPos", Gts_return);
        }
    }
}
