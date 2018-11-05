using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.IO;
using GTS;
using RTC5Import;
using System.Timers;
using System.Threading;

namespace Laser_Build_1._0
{
    class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary> 
        [STAThread]
        
        static void Main()
        {

            //运动控制卡初始化
            Initialization.Initial Laser_initial = new Initialization.Initial();
            Laser_initial.Common_Initial();//公共初始化
            Thread.Sleep(200);
            Laser_initial.RS232_Initial();//RS232初始化
            Laser_initial.Tcp_Initial();//TCP初始化
            Laser_initial.Gts_Initial();//工控卡初始化
            Laser_initial.Rtc_Initial();//振镜初始化
            
            //运动控制卡IO监视
            Prompt.Refresh Gts_IO_RE = new Prompt.Refresh();
            System.Timers.Timer Gts_IO_Refresh_Timer = new System.Timers.Timer(10);//10ms刷新一次         

            //启用定时器
            Gts_IO_Refresh_Timer.Elapsed += Gts_IO_RE.Refresh_IO_Thread;
            Gts_IO_Refresh_Timer.AutoReset = true;
            Gts_IO_Refresh_Timer.Enabled = true;
            Gts_IO_Refresh_Timer.Start();

            System.Timers.Timer Refresh_Timer_1s = new System.Timers.Timer(900);//1s刷新一次
            //启用定时器
            Refresh_Timer_1s.Elapsed += Gts_IO_RE.Timer_1s;
            Refresh_Timer_1s.AutoReset = true;
            Refresh_Timer_1s.Enabled = true;
            Refresh_Timer_1s.Start();

            //窗体启动
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());     
        }
    }
}


public struct Entity_Data 
{
    public short Type;//插补代号：1-直线插补，2-圆弧插补（圆心描述），3-圆弧插补（半径描述，不可描述整圆）
    public short Backup;//备用
    public decimal Start_x;//插补起点X坐标
    public decimal Start_y;//插补起点Y坐标
    public decimal End_x;//插补终点X坐标
    public decimal End_y;//插补终点Y坐标
    public decimal Center_x;//圆心坐标x
    public decimal Center_y;//圆心坐标y
    public decimal Center_Start_x;//圆心坐标x-起点坐标x
    public decimal Center_Start_y;//圆心坐标y-起点坐标y
    public decimal Cir_Start_Angle;//圆弧起始角度
    public decimal Cir_End_Angle;//圆弧终止角度
    public decimal Circle_radius;//圆弧半径
    public short Circle_dir;//圆弧方向：顺时针-0，逆时针-1
    public Entity_Data(Entity_Data Ini)
    {
        this.Type = Ini.Type;//插补代号：1-直线插补，2-圆弧插补（圆心描述），3-圆弧插补（半径描述，不可描述整圆）
        this.Backup = Ini.Backup;//备用
        this.Start_x = Ini.Start_x;//插补起点X坐标
        this.Start_y = Ini.Start_y;//插补起点Y坐标
        this.End_x = Ini.End_x;//插补终点X坐标
        this.End_y = Ini.End_y;//插补终点Y坐标
        this.Center_x = Ini.Center_x;//圆心坐标x
        this.Center_y = Ini.Center_y;//圆心坐标y
        this.Center_Start_x = Ini.Center_Start_x;//圆心坐标x-起点坐标x
        this.Center_Start_y = Ini.Center_Start_y;//圆心坐标y-起点坐标y
        this.Cir_Start_Angle = Ini.Cir_Start_Angle;//圆弧起始角度
        this.Cir_End_Angle = Ini.Cir_End_Angle;//圆弧终止角度
        this.Circle_radius = Ini.Circle_radius;//圆弧半径
        this.Circle_dir = Ini.Circle_dir;//圆弧方向：顺时针-0，逆时针-1
    }
    public void Empty()
    {
        Type = 0;//插补代号：1-直线插补，2-圆弧插补（圆心描述），3-圆弧插补（半径描述，不可描述整圆）
        Backup = 0;//备用
        Start_x = 0m;//插补起点X坐标
        Start_y = 0m;//插补起点Y坐标
        End_x = 0m;//插补终点X坐标
        End_y = 0m;//插补终点Y坐标
        Center_x = 0m;//圆心坐标x
        Center_y = 0m;//圆心坐标y
        Center_Start_x = 0m;//圆心坐标x-起点坐标x
        Center_Start_y = 0m;//圆心坐标y-起点坐标y 
        Cir_Start_Angle = 0m;//圆弧起始角度
        Cir_End_Angle = 0m;//圆弧终止角度 
        Circle_radius = 0m;//圆弧半径
        Circle_dir = 0;//圆弧方向：顺时针-0，逆时针-1
    }
}
//Gts插补数据
public struct Interpolation_Data
{
    public short Type;//插补代号：1-直线插补，2-圆弧插补（圆心描述），3-圆弧插补（半径描述，不可描述整圆）
    public short Lift_flag;//抬刀标志：0-抬刀，等待刀具抬起标志；1-刀具下刀切割标志
    public short Work;//工作区域选择，10-GTS，20-RTC
    public decimal Start_x;//插补起点X坐标 保留参数
    public decimal Start_y;//插补起点Y坐标 保留参数
    public decimal End_x;//插补终点X坐标
    public decimal End_y;//插补终点Y坐标
    public Vector Trail_Center;//封闭图形中心坐标X坐标 
    public decimal Gts_x;//Gts定位置激光加工中心X坐标
    public decimal Gts_y;//Gts定位置激光加工中心Y坐标
    public decimal Rtc_x;//Rtc定位 激光加工起点X坐标
    public decimal Rtc_y;//Rtc定位 激光加工起点Y坐标
    public decimal Center_x;//圆心坐标x
    public decimal Center_y;//圆心坐标y
    public decimal Center_Start_x;//圆心坐标x-起点坐标x
    public decimal Center_Start_y;//圆心坐标y-起点坐标y 
    public decimal Angle;//旋转角度 
    public decimal Circle_radius;//圆弧半径
    public short Circle_dir;//圆弧方向：顺时针-0，逆时针-1
    public Interpolation_Data (Interpolation_Data Ini)
    {
        this.Type = Ini.Type;//插补代号：1-直线插补，2-圆弧插补（圆心描述），3-圆弧插补（半径描述，不可描述整圆）
        this.Lift_flag = Ini.Lift_flag;//抬刀标志：0-抬刀，等待刀具抬起标志；1-刀具下刀切割标志
        this.Work = Ini.Work;//工作区域选择，10-GTS，20-RTC
        this.Start_x = Ini.Start_x;//插补起点X坐标 保留参数
        this.Start_y = Ini.Start_y;//插补起点Y坐标 保留参数
        this.End_x = Ini.End_x;//插补终点X坐标
        this.End_y = Ini.End_y;//插补终点Y坐标
        this.Trail_Center=new Vector(Ini.Trail_Center);//封闭图形中心坐标X坐标 
        this.Gts_x = Ini.Gts_x;//Gts定位置激光加工中心X坐标
        this.Gts_y = Ini.Gts_y;//Gts定位置激光加工中心Y坐标
        this.Rtc_x = Ini.Rtc_x;//Rtc定位 激光加工起点X坐标
        this.Rtc_y = Ini.Rtc_y;//Rtc定位 激光加工起点Y坐标
        this.Center_x = Ini.Center_x;//圆心坐标x
        this.Center_y = Ini.Center_y;//圆心坐标y
        this.Center_Start_x = Ini.Center_Start_x;//圆心坐标x-起点坐标x
        this.Center_Start_y = Ini.Center_Start_y;//圆心坐标y-起点坐标y 
        this.Angle = Ini.Angle;//旋转角度 
        this.Circle_radius = Ini.Circle_radius;//圆弧半径
        this.Circle_dir = Ini.Circle_dir;//圆弧方向：顺时针-0，逆时针-1
    }

    public void Empty()
    {
        Type = 0;//插补代号：1-直线插补，2-圆弧插补（圆心描述），3-圆弧插补（半径描述，不可描述整圆）
        Lift_flag = 0;//抬刀标志：0-抬刀，等待刀具抬起标志；1-刀具下刀切割标志
        Work=0;//工作区域选择，10-GTS，20-RTC
        Start_x = 0m;//插补起点X坐标
        Start_y = 0m;//插补起点Y坐标
        End_x = 0m;//插补终点X坐标
        End_y = 0m;//插补终点Y坐标
        Gts_x =0m;//Gts定位置激光加工中心X坐标
        Gts_y=0m;//Gts定位置激光加工中心Y坐标
        Rtc_x = 0m;//Rtc定位 激光加工起点X坐标
        Rtc_y = 0m;//Rtc定位 激光加工起点Y坐标
        Center_x = 0m;//圆心坐标x
        Center_y = 0m;//圆心坐标y
        Center_Start_x = 0m;//圆心坐标x-起点坐标x
        Center_Start_y = 0m;//圆心坐标y-起点坐标y 
        Angle=0m;//旋转角度 
        Circle_radius = 0m;//圆弧半径
        Circle_dir = 0;//圆弧方向：顺时针-0，逆时针-1
    }
}
//Rtc振镜单封闭图形数据
public struct Rtc_Data 
{
    public short Type;//插补代号：1-直线插补，2-圆弧插补（圆心描述），3-圆弧插补（半径描述，不可描述整圆）
    public short Lift_flag;//抬刀标志：0-抬刀，等待刀具抬起标志；1-刀具下刀切割标志
    public short Repeat;//重复加工次数
    public decimal Start_x;//插补起点X坐标 保留参数
    public decimal Start_y;//插补起点Y坐标 保留参数
    public decimal End_x;//终点X坐标
    public decimal End_y;//终点Y坐标
    public decimal Center_x;//圆心坐标x
    public decimal Center_y;//圆心坐标y
    public decimal Angle;//旋转角度
    public void Empty()
    {
        Type = 0;//插补代号：1-直线插补，2-圆弧插补（圆心描述），3-圆弧插补（半径描述，不可描述整圆）
        Lift_flag = 0;//抬刀标志：0-抬刀，等待刀具抬起标志；1-刀具下刀切割标志
        Repeat = 0;//重复加工次数
        Start_x = 0m;//插补起点X坐标
        Start_y = 0m;//插补起点Y坐标
        End_x = 0m;//插补终点X坐标
        End_y = 0m;//插补终点Y坐标
        Center_x = 0m;//圆心坐标x
        Center_y = 0m;//圆心坐标y
        Angle = 0m;//旋转角度
    }
}
//向量点位
[Serializable]
public struct Vector 
{
    private decimal x;//
    private decimal y;//

    public decimal X { get => x; set => x = value; }
    public decimal Y { get => y; set => y = value; }

    public Vector(decimal x,decimal y)
    {
        this.x = x;
        this.y = y;
    }
    public Vector(Vector Ini)
    {
        this.x = Ini.X;
        this.y = Ini.Y;
    }
    public void Empty()
    {
        this.x = 0;
        this.y = 0;
    }
    public decimal Length
    {
        get => (decimal)Math.Sqrt((double)(this.x * this.x + this.y * this.y));
    }
    public static Vector operator -(Vector a, Vector b)
    {
        return new Vector(a.X-b.X,a.Y-b.Y);
    }
    public static Vector operator +(Vector a, Vector b)
    {
        return new Vector(a.X + b.X, a.Y + b.Y);
    }


}
//Max 和 Min 判断
[Serializable]
public struct Extreme
{
    private decimal x_max;
    private decimal x_min;
    private decimal y_max;
    private decimal y_min;
    private decimal delta_x;
    private decimal delta_y;

    public decimal X_Max { get => x_max; set => x_max = value; }
    public decimal X_Min { get => x_min; set => x_min = value; }
    public decimal Y_Max { get => y_max; set => y_max = value; }
    public decimal Y_Min { get => y_min; set => y_min = value; }
    public decimal Delta_X { get => delta_x; set => delta_x = value; }
    public decimal Delta_Y { get => delta_y; set => delta_y = value; } 

    public Extreme(decimal x_max, decimal x_min, decimal y_max, decimal y_min, decimal delta_x, decimal delta_y)
    {
        this.x_max = x_max;
        this.x_min = x_min;
        this.y_max = y_max;
        this.y_min = y_min;
        this.delta_x = delta_x;
        this.delta_y = delta_y;
    }
    public Extreme(Extreme Ini)
    {
        this.x_max = Ini.x_max;
        this.x_min = Ini.x_min;
        this.y_max = Ini.y_max;
        this.y_min = Ini.y_min;
        this.delta_x = Ini.delta_x;
        this.delta_y = Ini.delta_y;
    }
    public void Empt_min()
    {
        this.x_max = 0;
        this.x_min = 0;
        this.y_max = 0;
        this.y_min = 0;
        this.delta_x = 0;
        this.delta_y = 0;
    }
}
//仿射变换
[Serializable]
public struct Affinity_Rate 
{
    private decimal delta_x;// 
    private decimal delta_y;//
    private decimal angle;// 

    public decimal Delta_X { get => delta_x; set => delta_x = value; }
    public decimal Delta_Y { get => delta_y; set => delta_y = value; }
    public decimal Angle { get => angle; set => angle = value; }

    public Affinity_Rate(decimal delta_x, decimal delta_y, decimal angle) 
    { 
        this.delta_x = delta_x;
        this.delta_y = delta_y;
        this.angle = angle;
    }
    public void Empty()
    {
        this.delta_x = 0;
        this.delta_y = 0;
        this.angle = 0;
    }
}
//类比 Thread.Sleep(200);
//尽量简化程序 使用现成的工具或轮子
namespace Common_Method
{
    class Delay_Time
    {
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {
                Application.DoEvents();
            }
        }
    }
}