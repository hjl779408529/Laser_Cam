using Initialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Laser_Build_1._0;

namespace Laser_Version2._0.UI
{
    public partial class Laser_Watt : Form
    {
        public Laser_Watt()
        {
            InitializeComponent();
        }
        System.Timers.Timer Refresh_Timer_1s = new System.Timers.Timer(200);//1s刷新一次
        DataTable Laser_Watt_Percent_Data = new DataTable();
        private void Laser_Watt_Load(object sender, EventArgs e)
        {
            //初始化通讯端口列表
            Com_List.Items.AddRange(Initial.Laser_Watt_Com.PortName.ToArray());
            Laser_Percent.Text = Para_List.Parameter.PEC.ToString();
            //初始化默认的Com端口
            if (Initial.Laser_Watt_Com.PortName.Count >= 1) Com_List.SelectedIndex = Para_List.Parameter.Laser_Watt_Com_No;
            if (Initial.Laser_Watt_Com.ComDevice.IsOpen == false)
            {
                Re_connect.Text = "打开串口";
                Com_Status.BackgroundImage = Properties.Resources.red;                
            }
            else
            {               
                Re_connect.Text = "关闭串口";
                Com_Status.BackgroundImage = Properties.Resources.green;                
            }
            Laser_Watt_Percent_Data.Columns.Add("激光输出百分比(%)", typeof(decimal));
            Laser_Watt_Percent_Data.Columns.Add("激光实测功率(mw)", typeof(decimal));
            //启用定时器
            Refresh_Timer_1s.Elapsed += Display_Watt;
            Refresh_Timer_1s.AutoReset = true;
            Refresh_Timer_1s.Enabled = true;
            Refresh_Timer_1s.Start();
        }
        //更改串口端口号
        private void Com_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            Para_List.Parameter.Laser_Watt_Com_No = Com_List.SelectedIndex;
        }
        //重连串口
        private void Re_connect_Click(object sender, EventArgs e)
        {
            if (Para_List.Parameter.Laser_Watt_Com_No < Initial.Laser_Watt_Com.PortName.Count)
            {

                if (Initial.Laser_Watt_Com.Open_Com(Para_List.Parameter.Laser_Watt_Com_No,3))
                {
                    //状态刷新
                    Re_connect.Text = "关闭串口";
                    Com_Status.BackgroundImage = Properties.Resources.green;
                }
                else
                {
                    //状态刷新
                    Re_connect.Text = "打开串口";
                    Com_Status.BackgroundImage = Properties.Resources.red;
                }
            }
            else
            {
                MessageBox.Show("激光功率计通讯串口端口编号异常，请在激光功率计面板选择正确的串口编号！！！");
                return;
            }
        }
        ///
        public void Display_Watt(object sender, EventArgs e)
        {
            Current_Laser_Watt.Text =Laser_Watt_Cal.Watt_To_Watt(Initial.Laser_Watt_00.Current_Watt / 1000m).ToString(6);
            Laser_Percent.Text = Para_List.Parameter.PEC.ToString(6);
        }
        /// <summary>
        /// 校准数据采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cal_Data_Acquisition_Click(object sender, EventArgs e)
        {
            
        }
        /// <summary>
        /// 记录一次采集数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Acquisition_Once_Click(object sender, EventArgs e)
        {
            Laser_Watt_Percent_Data.Rows.Add(new object[] { Para_List.Parameter.PEC, Initial.Laser_Watt_00.Current_Watt });
        }
        /// <summary>
        /// Laser_Watt窗口关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Laser_Watt_FormClosed(object sender, FormClosedEventArgs e)
        {
            Refresh_Timer_1s.Dispose();
        }
        /// <summary>
        /// 保存采集数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Data_Click(object sender, EventArgs e)
        {
            CSV_RW.SaveCSV(Laser_Watt_Percent_Data,"Laser_PEC_Watt_Data.csv");
        }
        /// <summary>
        /// 更新串口列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_List_Click(object sender, EventArgs e)
        {            
            //刷新列表
            Initial.Laser_Watt_Com.Refresh_Com_List();
            //初始化通讯端口列表
            Com_List.Items.Clear();
            Com_List.Items.AddRange(Initial.Laser_Watt_Com.PortName.ToArray());
        }
    }
}
