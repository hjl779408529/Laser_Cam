using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTS;
using System.Timers;
using System.Threading;
using CCWin;
using Initialization;
using Laser_Version2._0;
using Laser_Version2._0.UI;

namespace Laser_Build_1._0
{
    public partial class Main : Form
    {
        
        public Main()
        {            
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;//关闭CheckForIllegalCrossThreadCalls
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }                

        IO_Monitor iO_Monitor;//打开IO监视界面
        Laser_Program_Info Info_Display;//关于信息
        public static Dxf dxf;//Dxf处理页面
        Rtc_Handle Rtc_Manual;//振镜操作
        Menu_5_Axis_Handle menu_5_Axis_Handle;//轴手动操作页面
        ParameterSet parameterSet;    //参数设置界面
        Laser_Control_Panel Laser_Control_Window;//激光控制界面 
        public static Laser_Watt Laser_Watt_Window;//激光功率
        public static Camera_Capture pic_resolve = new Camera_Capture();
        //打开IO监视界面
        private void Status_Click(object sender, EventArgs e)
        {
            if (iO_Monitor == null)
            {
                iO_Monitor = new IO_Monitor();
                iO_Monitor.Show();
            }
            else
            {
                if (iO_Monitor.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    iO_Monitor = new IO_Monitor();
                    iO_Monitor.Show();
                }
                else
                {
                    iO_Monitor.Activate();//使子窗体获得焦点
                }
            }

            
        }

        //Information界面打开
        private void About_Info_Click(object sender, EventArgs e)
        {
            
            if (Info_Display == null)
            {
                Info_Display = new Laser_Program_Info();
                Info_Display.Show();
            }
            else
            {
                if (Info_Display.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    Info_Display = new Laser_Program_Info();
                    Info_Display.Show();
                }
                else
                {
                    Info_Display.Activate();//使子窗体获得焦点
                }
            }
        }
        
        //Dxf
        private void button9_Click(object sender, EventArgs e)
        {
            if (dxf == null)
            {
                dxf = new Dxf();
                dxf.Show();
            }
            else
            {
                if (dxf.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    dxf = new Dxf();
                    dxf.Show();
                }
                else
                {
                    dxf.Activate();//使子窗体获得焦点
                }
            }
        }
        //轴手动操作
        private void Menu_5_Handle_Click(object sender, EventArgs e)
        {            
            if (menu_5_Axis_Handle == null)
            {
                menu_5_Axis_Handle = new Menu_5_Axis_Handle();
                menu_5_Axis_Handle.Show();
            }
            else
            {
                if (menu_5_Axis_Handle.IsDisposed) //若子窗体关闭 则打开新子窗体 并显示
                {
                    menu_5_Axis_Handle = new Menu_5_Axis_Handle();
                    menu_5_Axis_Handle.Show();
                }
                else
                {
                    menu_5_Axis_Handle.Activate(); //使子窗体获得焦点
                }
            }
        }
        
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //释放GTS
            GTS_Fun.Factory.Free();

            //释放RTC控制卡
            RTC_Fun.Factory.Free();

            //保存设备参数至文件
            Para_List.Serialize_Parameter.Serialize("Para.xml");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                //读取通用输出的值            
                if (Prompt.Refresh.Lamp_control == 0)
                {
                    Prompt.Refresh.Lamp_control = 1;
                    button1.Text = "照明打开";
                }
                else
                {
                    Prompt.Refresh.Lamp_control = 0;
                    button1.Text = "照明关闭";
                }
            });
        }

       
        private void button2_Click(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (Prompt.Refresh.Cyc_control == 0)
                {
                    Prompt.Refresh.Cyc_control = 1;
                    button2.Text = "气缸伸出";
                }
                else
                {
                    Prompt.Refresh.Cyc_control = 0;
                    button2.Text = "气缸退回";
                }
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (Prompt.Refresh.Blow_control == 0)
                {
                    Prompt.Refresh.Blow_control = 1;
                    button3.Text = "吹气打开";
                }
                else
                {
                    Prompt.Refresh.Blow_control = 0;
                    button3.Text = "吹气关闭";
                }
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (Prompt.Refresh.Yellow_lamp == 0)
                {
                    Prompt.Refresh.Yellow_lamp = 1;
                    button4.Text = "灯塔黄打开";
                }
                else
                {
                    Prompt.Refresh.Yellow_lamp = 0;
                    button4.Text = "灯塔黄关闭";
                }
            });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (Prompt.Refresh.Green_lamp == 0)
                {
                    Prompt.Refresh.Green_lamp = 1;
                    button5.Text = "灯塔绿打开";
                }
                else
                {
                    Prompt.Refresh.Green_lamp = 0;
                    button5.Text = "灯塔绿关闭";
                }
            });
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (Prompt.Refresh.Red_lamp == 0)
                {
                    Prompt.Refresh.Red_lamp = 1;
                    button6.Text = "灯塔红打开";
                }
                else
                {
                    Prompt.Refresh.Red_lamp = 0;
                    button6.Text = "灯塔红关闭";
                }
            });
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (Prompt.Refresh.Beeze_Control == 0)
                {
                    Prompt.Refresh.Beeze_Control = 1;
                    button7.Text = "蜂鸣器打开";
                }
                else
                {
                    Prompt.Refresh.Beeze_Control = 0;
                    button7.Text = "蜂鸣器关闭";
                }
            });
        }


        private void Rtc_Mannual_Click(object sender, EventArgs e)
        {
            if (Rtc_Manual == null)
            {
                Rtc_Manual = new Laser_Version2._0.Rtc_Handle();
                Rtc_Manual.Show();
            }
            else
            {
                if (Rtc_Manual.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    Rtc_Manual = new Laser_Version2._0.Rtc_Handle();
                    Rtc_Manual.Show();
                }
                else
                {
                    Rtc_Manual.Activate();//使子窗体获得焦点
                }
            }
        }

        /// <summary>
        /// 菜单栏-设置-参数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_ParameterSet_Click(object sender, EventArgs e)
        {
            if (parameterSet == null)
            {
                parameterSet = new Laser_Version2._0.ParameterSet();
                parameterSet.Show();
            }
            else
            {
                if (parameterSet.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    parameterSet = new Laser_Version2._0.ParameterSet();
                    parameterSet.Show();
                }
                else
                {
                    parameterSet.Activate();//使子窗体获得焦点
                }
            }
        }
        /// <summary>
        /// 激光设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            if (Laser_Control_Window == null)
            {
                Laser_Control_Window = new Laser_Version2._0.Laser_Control_Panel();
                Laser_Control_Window.Show();
            }
            else
            {
                if (Laser_Control_Window.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    Laser_Control_Window = new Laser_Version2._0.Laser_Control_Panel();
                    Laser_Control_Window.Show();
                }
                else
                {
                    Laser_Control_Window.Activate();//使子窗体获得焦点
                }
            }
        }
        /// <summary>
        /// 激光功率计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Laser_Watt_Button_Click(object sender, EventArgs e)
        {
            if (Laser_Watt_Window == null)
            {
                Laser_Watt_Window = new Laser_Version2._0.UI.Laser_Watt();
                Laser_Watt_Window.Show();
            }
            else
            {
                if (Laser_Watt_Window.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    Laser_Watt_Window = new Laser_Version2._0.UI.Laser_Watt();
                    Laser_Watt_Window.Show();
                }
                else
                {
                    Laser_Watt_Window.Activate();//使子窗体获得焦点
                }
            }
        }
        /// <summary>
        /// 相机捕获界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Camera_Form_Click(object sender, EventArgs e)
        {
            if (pic_resolve == null)
            {
                pic_resolve = new Camera_Capture();
                pic_resolve.Show();
            }
            else
            {
                if (pic_resolve.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                {
                    pic_resolve = new Camera_Capture();
                    pic_resolve.Show();
                }
                else
                {
                    pic_resolve.Dispose();
                }
            }
        }
    }
}
