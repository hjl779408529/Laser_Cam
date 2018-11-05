using GTS;
using Laser_Build_1._0;
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

namespace Laser_Version2._0
{
    public partial class ParameterSet : Form
    {
        public ParameterSet()
        {
            InitializeComponent();
        }
        //输入转换变量
        Vector Tmp_Mark = new Vector();
        //触发数据
        short Intrigue = 1;
        // 像素 毫米 比
        private void Set_txt_valueK_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(Set_txt_valueK.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Cam_Reference = tmp;//1像素=0.008806mm ()
            });
        }
        /// <summary>
        /// 矫正Mark点坐标实际位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Set_Btn_Mark1_Click(object sender, EventArgs e)
        {
            if (Calibration.Calibrate_Mark(0))
            {
                MessageBox.Show("Mark点矫正完成！！！");
            }
            else
            {
                MessageBox.Show("Mark点矫正失败！！！");
            }
            //更新显示
            Set_txt_markX1.Text = Para_List.Parameter.Mark1.X.ToString();
            Set_txt_markY1.Text = Para_List.Parameter.Mark1.Y.ToString();
            Set_txt_markX2.Text = Para_List.Parameter.Mark2.X.ToString();
            Set_txt_markY2.Text = Para_List.Parameter.Mark2.Y.ToString();
            Set_txt_markX3.Text = Para_List.Parameter.Mark3.X.ToString();
            Set_txt_markY3.Text = Para_List.Parameter.Mark3.Y.ToString();
            Set_txt_markX4.Text = Para_List.Parameter.Mark4.X.ToString();
            Set_txt_markY4.Text = Para_List.Parameter.Mark4.Y.ToString();
        }
        ///Re_Calibration Mark
        private void Re_Cali_Mark_Click(object sender, EventArgs e)
        {
            Calibration.Calibrate_Mark(0);
        }
        //Mark1 X坐标
        private void Set_txt_markX1_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markX1.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = tmp;
                Tmp_Mark.Y = Para_List.Parameter.Mark1.Y;
                Para_List.Parameter.Mark1 =new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
            
        }
        //Mark1 Y坐标
        private void Set_txt_markY1_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markY1.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp>=0) && (tmp <= 350))
            {
                Tmp_Mark.X = Para_List.Parameter.Mark1.X;
                Tmp_Mark.Y = tmp;
                Para_List.Parameter.Mark1 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
            
        }
        //Mark2 X坐标
        private void Set_txt_markX2_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markX2.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = tmp;
                Tmp_Mark.Y = Para_List.Parameter.Mark2.Y;
                Para_List.Parameter.Mark2 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
            
        }
        //Mark2 Y坐标
        private void Set_txt_markY2_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markY2.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = Para_List.Parameter.Mark2.X;
                Tmp_Mark.Y = tmp;
                Para_List.Parameter.Mark2 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
            
        }
        //Mark3 X坐标
        private void Set_txt_markX3_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markX3.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = tmp;
                Tmp_Mark.Y = Para_List.Parameter.Mark3.Y;
                Para_List.Parameter.Mark3 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
            
        }
        //Mark3 Y坐标
        private void Set_txt_markY3_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markY3.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = Para_List.Parameter.Mark3.X;
                Tmp_Mark.Y = tmp;
                Para_List.Parameter.Mark3 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }            
        }
        //Mark4 X坐标
        private void Set_txt_markX4_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markX4.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = tmp;
                Tmp_Mark.Y = Para_List.Parameter.Mark4.Y;
                Para_List.Parameter.Mark4 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
        }
        //Mark4 Y坐标
        private void Set_txt_markY4_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markY4.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = Para_List.Parameter.Mark4.X;
                Tmp_Mark.Y = tmp;
                Para_List.Parameter.Mark4 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
        }
        //form load
        private void ParameterSet_Load(object sender, EventArgs e)
        {
            Set_txt_valueK.Text = Para_List.Parameter.Cam_Reference.ToString();
            Set_txt_markX1.Text = Para_List.Parameter.Mark1.X.ToString();
            Set_txt_markY1.Text = Para_List.Parameter.Mark1.Y.ToString();
            Set_txt_markX2.Text = Para_List.Parameter.Mark2.X.ToString();
            Set_txt_markY2.Text = Para_List.Parameter.Mark2.Y.ToString();
            Set_txt_markX3.Text = Para_List.Parameter.Mark3.X.ToString();
            Set_txt_markY3.Text = Para_List.Parameter.Mark3.Y.ToString();
            Set_txt_markX4.Text = Para_List.Parameter.Mark4.X.ToString();
            Set_txt_markY4.Text = Para_List.Parameter.Mark4.Y.ToString();
            textBox19.Text = Para_List.Parameter.Rtc_Org.X.ToString(6);
            textBox18.Text = Para_List.Parameter.Rtc_Org.Y.ToString(6);
            numericUpDown1.Value = (short)Intrigue;
            Mark_Type_List.SelectedIndex = (Para_List.Parameter.Camera_Mark_Type - 1);
        }       
        //定位mark点1
        private void button1_Click(object sender, EventArgs e)
        {
            Calibration.Mark_Correct(Para_List.Parameter.Mark1);
        }
        //定位mark点2
        private void button2_Click(object sender, EventArgs e)
        {
            Calibration.Mark_Correct(Para_List.Parameter.Mark2);
        }
        //定位mark点3
        private void button3_Click(object sender, EventArgs e)
        {
            Calibration.Mark_Correct(Para_List.Parameter.Mark3);
        }
        //定位mark点4
        private void button6_Click(object sender, EventArgs e)
        {
            Calibration.Mark_Correct(Para_List.Parameter.Mark4);
        }
        //振镜与ORG 中心差值X/mm
        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox19.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Rtc_Org=new Vector(tmp, Para_List.Parameter.Rtc_Org.Y);
        }
        //振镜与ORG 中心差值Y/mm
        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox18.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Rtc_Org=new Vector(Para_List.Parameter.Rtc_Org.X,tmp);
        }
        /// <summary>
        /// 振镜与ORG的距离矫正
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (Calibration.Calibrate_RTC_ORG())
            {
                MessageBox.Show("振镜与ORG的距离矫正完成！！");
            }
            else
            {
                MessageBox.Show("振镜与ORG的距离矫正失败！！");
            }
            textBox19.Text = Para_List.Parameter.Rtc_Org.X.ToString(6);
            textBox18.Text = Para_List.Parameter.Rtc_Org.Y.ToString(6);
        }
        //触发拍照
        private void button5_Click(object sender, EventArgs e)
        {
            Vector Tmp = Initialization.Initial.T_Client.Get_Cam_Actual_Pixel(Intrigue);//触发拍照 
            Vector Cor_Data = Initialization.Initial.T_Client.Get_Coordinate_Corrrect_Point(Tmp.X, Tmp.Y);
            MessageBox.Show(string.Format("相机坐标：({0},{1})，实际坐标：({2},{3})", Tmp.X, Tmp.Y, Cor_Data.X, Cor_Data.Y));
        }
        //Cam_Intrigue_Num
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Intrigue = (short)numericUpDown1.Value;
        }
        //Re_Connect_Tcp
        private void Re_Connect_Click(object sender, EventArgs e)
        {
            Initialization.Initial.T_Client.TCP_Start("127.0.0.1", Para_List.Parameter.Server_Port);
        }
        //Disconnect_Tcp
        private void Disconnect_Tcp_Click(object sender, EventArgs e)
        {
            Initialization.Initial.T_Client.Stop_Connect();
        }
        /// <summary>
        /// 相机坐标系标定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Correct_Cam_Cor_Click(object sender, EventArgs e)
        {
            if (Calibration.Cal_Cam_Affinity())
            {
                MessageBox.Show("相机坐标系标定完成！！！");
            }
            else
            {
                MessageBox.Show("相机坐标系标定失败！！！");
            }
        }
        /// <summary>
        /// 矫正振镜坐标系偏转角
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Correct_Rtc_Cor_Click(object sender, EventArgs e)
        {
            if (Calibration.Get_Rtc_Coordinate_Affinity())
            {
                MessageBox.Show("振镜坐标系偏转角矫正完成！！！");
            }
            else
            {
                MessageBox.Show("振镜坐标系偏转角矫正失败！！！");
            }
        }
        /// <summary>
        /// 采集振镜矫正数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Acquisation_Rtc_Correct_Click(object sender, EventArgs e)
        {
            Thread Get_Rtc_Data_thread = new Thread(Get_Rtc_Data);
            Get_Rtc_Data_thread.Start();
        }
        private void Get_Rtc_Data()
        {
            Calibration.Rtc_Correct_Coordinate();
        }
        /// <summary>
        /// 修改mark类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mark_Type_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            Para_List.Parameter.Camera_Mark_Type = (UInt16)(Mark_Type_List.SelectedIndex + 1);
        }
    }
}
