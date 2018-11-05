using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using RTC5Import;

namespace Laser_Version2._0
{
    public partial class Rtc_Handle : Form
    {
        public Rtc_Handle()
        {
            InitializeComponent();
        }
        //定义变量
        decimal Distence_X = 20;
        decimal Distence_Y = 20;
        decimal Abs_X = 20;
        decimal Abs_Y = 20;
        //建立定时器
        System.Timers.Timer Refresh_Timer = new System.Timers.Timer(200);
        //定义Home
        Vector Home_Vector = new Vector();
        //定义移动方式
        UInt16 Control_Type = 4;//4-jump,6-mark
        private void Rtc_Handle_Load(object sender, EventArgs e)
        {
            //启用定时器
            Refresh_Timer.Elapsed += Refresh_Timer_Elapsed_Thread;
            Refresh_Timer.AutoReset = true;
            Refresh_Timer.Enabled = true;
            Refresh_Timer.Start();

            textBox1.Text =Convert.ToString(Distence_X);
            textBox2.Text = Convert.ToString(Distence_Y);

            textBox4.Text = Convert.ToString(Para_List.Parameter.Rtc_Home.X);
            textBox3.Text = Convert.ToString(Para_List.Parameter.Rtc_Home.Y);

            textBox6.Text = Convert.ToString(Abs_X);
            textBox5.Text = Convert.ToString(Abs_Y);

            textBox7.Text = Convert.ToString(Para_List.Parameter.Rtc_Pos_Reference);
            Home_Vector = Para_List.Parameter.Rtc_Home;

            Mark_Speed.Text = (((decimal)Para_List.Parameter.Mark_Speed / Para_List.Parameter.Rtc_Pos_Reference) * 1000m).ToString();
            Jump_Speed.Text = (((decimal)Para_List.Parameter.Jump_Speed / Para_List.Parameter.Rtc_Pos_Reference) * 1000m).ToString();
            Laser_ON_Delay.Text = Para_List.Parameter.Laser_On_Delay.ToString();
            Laser_Off_Delay.Text = Para_List.Parameter.Laser_Off_Delay.ToString();
            Jump_Delay.Text = Para_List.Parameter.Jump_Delay.ToString();
            Mark_Delay.Text = Para_List.Parameter.Mark_Delay.ToString();
            Polygon_Delay.Text = Para_List.Parameter.Polygon_Delay.ToString();

        }
        //线程函数
        private void Refresh_Timer_Elapsed_Thread(object sender, ElapsedEventArgs e)
        {
            Refresh_Timer_Elapsed();
        }
        //定时器线程函数
        private void Refresh_Timer_Elapsed()
        {
            this.Invoke((EventHandler)delegate {
                if (Control_Type == 4)
                {
                    button7.Text = "Jump";
                }
                else if (Control_Type == 6)
                {
                    button7.Text = "Mark";
                }                
                
            });
        }
        //x方向定位步距
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox1.Text,out Distence_X))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
        }
        //y方向定位步距
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox2.Text, out Distence_Y))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
        }
        //Home
        private void button1_Click(object sender, EventArgs e)
        {
            RTC_Fun.Motion.Home();
        }
        //X+
        private void button2_Click(object sender, EventArgs e)
        {
            RTC_Fun.Motion.Inc_X(Distence_X, Control_Type, 1);
        }
        //X-
        private void button3_Click(object sender, EventArgs e)
        {
            RTC_Fun.Motion.Inc_X(-Distence_X, Control_Type, 1);
        }
        //Y+
        private void button5_Click(object sender, EventArgs e)
        {
            RTC_Fun.Motion.Inc_Y(Distence_Y, Control_Type, 1);
        }
        //Y-
        private void button4_Click(object sender, EventArgs e)
        {
            RTC_Fun.Motion.Inc_Y(-Distence_Y, Control_Type, 1);
        }
        //Para_List.Parameter.Rtc_Home.X
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox4.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Home_Vector.X = tmp;
            Home_Vector.Y = Para_List.Parameter.Rtc_Home.Y;
            Para_List.Parameter.Rtc_Home = Home_Vector;
        }
        //Para_List.Parameter.Rtc_Home.Y
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox3.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Home_Vector.Y = tmp;
            Home_Vector.X = Para_List.Parameter.Rtc_Home.X;
            Para_List.Parameter.Rtc_Home = Home_Vector;
        }
        //绝对坐标x
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox6.Text, out Abs_X))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
        }
        //绝对坐标y
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox5.Text, out Abs_Y))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
        }
        //绝对定位
        private void button6_Click(object sender, EventArgs e)
        {
            RTC_Fun.Motion.Abs_XY(Abs_X,Abs_Y, Control_Type, 1);
        }
        //移动方式切换
        private void button7_Click(object sender, EventArgs e)
        {
            if (Control_Type == 4)
            {
                Control_Type=6;
            }
            else if (Control_Type == 6)
            {
                Control_Type = 4;
            }
            else
            {
                Control_Type = 4;
            }
        }
        //激光开
        private void button8_Click(object sender, EventArgs e)
        {
            RTC_Fun.Motion.Open_Laser();
        }
        //激光关
        private void button9_Click(object sender, EventArgs e)
        {
            RTC_Fun.Motion.Close_Laser();
        }
        //Rtc位置基准
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox7.Text, out decimal tem)) 
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Rtc_Pos_Reference = tem;
        }
        /// <summary>
        /// Mark_Speed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mark_Speed_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Mark_Speed.Text, out decimal tem))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Mark_Speed =(double)(tem * Para_List.Parameter.Rtc_Pos_Reference / 1000m);
        }
        //参数生效
        private void Change_Para_List_Click(object sender, EventArgs e)
        {
            //Para_List.Serialize_Parameter.Reserialize("Para.xml");
            RTC_Fun.Factory.Change_Para();
        }
        /// <summary>
        /// Jump_Speed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Jump_Speed_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Jump_Speed.Text, out decimal tem))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Jump_Speed = (double)(tem * Para_List.Parameter.Rtc_Pos_Reference / 1000m);
        }
        /// <summary>
        /// Laser_On_Delay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Laser_ON_Delay_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(Laser_ON_Delay.Text, out int tem))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Laser_On_Delay = tem;
        }
        /// <summary>
        /// Laser_Off_Delay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Laser_Off_Delay_TextChanged(object sender, EventArgs e)
        {
            if (!uint.TryParse(Laser_Off_Delay.Text, out uint tem))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Laser_Off_Delay = tem;
        }
        /// <summary>
        /// Jump_Delay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Jump_Delay_TextChanged(object sender, EventArgs e)
        {
            if (!uint.TryParse(Jump_Delay.Text, out uint tem))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Jump_Delay = tem;
        }
        /// <summary>
        /// Mark_Delay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mark_Delay_TextChanged(object sender, EventArgs e)
        {
            if (!uint.TryParse(Mark_Delay.Text, out uint tem))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Mark_Delay = tem;
        }
        /// <summary>
        /// Polygon_Delay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Polygon_Delay_TextChanged(object sender, EventArgs e)
        {
            if (!uint.TryParse(Polygon_Delay.Text, out uint tem))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Polygon_Delay = tem;
        }
        /// <summary>
        /// 复位Rtc控制卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_Rtc_Click(object sender, EventArgs e)
        {
            RTC_Fun.Factory.Load_Correct_File();
        }
    }
}
