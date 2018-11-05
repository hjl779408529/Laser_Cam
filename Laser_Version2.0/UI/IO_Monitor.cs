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

namespace Laser_Build_1._0
{
    public partial class IO_Monitor : Form
    {
        public IO_Monitor()
        {
            InitializeComponent();
        }

        //定义IO_Monitor窗口刷新定时器
        System.Timers.Timer IO_Monitor_Timer = new System.Timers.Timer(200);
        private void IO_Monitor_Load(object sender, EventArgs e)
        {
            //启用定时器
            IO_Monitor_Timer.Elapsed += IO_Monitor_Timer_Elapsed;
            IO_Monitor_Timer.AutoReset = true;
            IO_Monitor_Timer.Enabled = true;
            IO_Monitor_Timer.Start();
        }

       
        //窗口打开，启用定时器刷新
        private void IO_Monitor_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (Prompt.Refresh.EXI1) { button1.BackColor = Color.Green; } else { button1.BackColor = SystemColors.Control; }//急停开关
                if (Prompt.Refresh.EXI2) { button2.BackColor = Color.Green; } else { button2.BackColor = SystemColors.Control; }//除尘气缸伸出传感器
                if (Prompt.Refresh.EXI3) { button3.BackColor = Color.Green; } else { button3.BackColor = SystemColors.Control; }//除尘气缸退回传感器
                if (Prompt.Refresh.EXI4) { button4.BackColor = Color.Green; } else { button4.BackColor = SystemColors.Control; }//左门禁传感器
                if (Prompt.Refresh.EXI5) { button5.BackColor = Color.Green; } else { button5.BackColor = SystemColors.Control; }//右门禁传感器
                if (Prompt.Refresh.EXI6) { button6.BackColor = Color.Green; } else { button6.BackColor = SystemColors.Control; }//启动按钮1
                if (Prompt.Refresh.EXI7) { button7.BackColor = Color.Green; } else { button7.BackColor = SystemColors.Control; }//启动按钮2
                if (Prompt.Refresh.EXO1) { button8.BackColor = Color.Green; } else { button8.BackColor = SystemColors.Control; }//三色灯塔黄
                if (Prompt.Refresh.EXO2) { button9.BackColor = Color.Green; } else { button9.BackColor = SystemColors.Control; }//三色灯塔绿
                if (Prompt.Refresh.EXO3) { button10.BackColor = Color.Green; } else { button10.BackColor = SystemColors.Control; }//三色灯塔红
                if (Prompt.Refresh.EXO4) { button11.BackColor = Color.Green; } else { button11.BackColor = SystemColors.Control; }//蜂鸣器
                if (Prompt.Refresh.EXO5) { button12.BackColor = Color.Green; } else { button12.BackColor = SystemColors.Control; }//除尘气缸伸出
                if (Prompt.Refresh.EXO6) { button13.BackColor = Color.Green; } else { button13.BackColor = SystemColors.Control; }//除尘气缸退回
                if (Prompt.Refresh.EXO7) { button14.BackColor = Color.Green; } else { button14.BackColor = SystemColors.Control; }//吹气打开
                if (Prompt.Refresh.EXO8) { button15.BackColor = Color.Green; } else { button15.BackColor = SystemColors.Control; }//照明灯
                if (Prompt.Refresh.EXO9) { button16.BackColor = Color.Green; } else { button16.BackColor = SystemColors.Control; }//启动按钮1灯
                if (Prompt.Refresh.EXO10) { button17.BackColor = Color.Green; } else { button17.BackColor = SystemColors.Control; }//启动按钮2灯
            });
        }

        //窗口关闭终止刷新
        private void IO_Monitor_FormClosed(object sender, FormClosedEventArgs e)
        {
            IO_Monitor_Timer.Close();
        }
    }
}