using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Management;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;
using System.Drawing;
using System.Threading;
using System.Xml.Serialization;
using System.Windows.Forms;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using Laser_Version2._0;
using System.Data;
using Initialization;
namespace Laser_Build_1._0
{
    //矫正数据存储
    [Serializable]
    public struct Correct_Data
    {
        //私有属性
        private decimal xo, yo;//x0,y0--基准坐标
        private decimal xm, ym;//x1,y1--轴实际坐标 

        //公开访问的属性
        public decimal Xo
        {
            get { return xo; }
            set { xo = value; }
        }
        public decimal Yo
        {
            get { return yo; }
            set { yo = value; }
        }
        public decimal Xm
        {
            get { return xm; }
            set { xm = value; }
        }
        public decimal Ym
        {
            get { return ym; }
            set { ym = value; }
        }


        //公开访问的方法
        //构造函数
        public Correct_Data(Correct_Data Ini)
        {
            this.xo = Ini.Xo;
            this.yo = Ini.Yo;
            this.xm = Ini.Xm;
            this.ym = Ini.Ym;
        }
        public Correct_Data(decimal xo, decimal yo, decimal xm, decimal ym)
        {
            this.xo = xo;
            this.yo = yo;
            this.xm = xm;
            this.ym = ym;
        }
        //清空数据
        public void Empty()
        {
            this.xo = 0;
            this.yo = 0;
            this.xm = 0;
            this.ym = 0;
        }
    }
    [Serializable]
    public struct Affinity_Matrix
    {
        //私有属性 
        private decimal stretch_x, distortion_x, delta_x;
        private decimal stretch_y, distortion_y, delta_y;
        //共有属性
        public decimal Stretch_X
        {
            get { return stretch_x; }
            set { stretch_x = value; }
        }
        public decimal Distortion_X
        {
            get { return distortion_x; }
            set { distortion_x = value; }
        }

        public decimal Delta_X
        {
            get { return delta_x; }
            set { delta_x = value; }
        }
        public decimal Stretch_Y
        {
            get { return stretch_y; }
            set { stretch_y = value; }
        }
        public decimal Distortion_Y
        {
            get { return distortion_y; }
            set { distortion_y = value; }
        }
        public decimal Delta_Y
        {
            get { return delta_y; }
            set { delta_y = value; }
        }
        //公开构造函数        
        //有参数
        public Affinity_Matrix(decimal stretch_x, decimal distortion_x, decimal delta_x, decimal stretch_y, decimal distortion_y, decimal delta_y)
        {
            this.stretch_x = stretch_x;
            this.distortion_x = distortion_x;
            this.stretch_y = stretch_y;
            this.distortion_y = distortion_y;
            this.delta_x = delta_x;
            this.delta_y = delta_y;
        }
        public Affinity_Matrix(Affinity_Matrix Ini)
        {
            this.stretch_x = Ini.Stretch_X;
            this.distortion_x = Ini.Distortion_X;
            this.delta_x = Ini.Delta_X;
            this.stretch_y = Ini.Stretch_Y;
            this.distortion_y = Ini.Distortion_Y;
            this.delta_y = Ini.Delta_Y;
        }
        //清空
        public void Empty()
        {
            this.stretch_x = 0;
            this.distortion_x = 0;
            this.delta_x = 0;
            this.stretch_y = 0;
            this.distortion_y = 0;
            this.delta_y = 0;
        }
    }
    public struct Fit_Data
    {
        //私有属性 
        private decimal k1, k2,k3,k4;//k1-1次系数，k2-2次系数，k3-3次系数，k4-4次系数
        private decimal delta;//坐标偏移值
        //共有属性
        public decimal K1
        {
            get { return k1; }
            set { k1 = value; }
        }
        public decimal K2
        {
            get { return k2; }
            set { k2 = value; }
        }
        public decimal K3
        {
            get { return k3; }
            set { k3 = value; }
        }
        public decimal K4
        {
            get { return k4; }
            set { k4 = value; }
        }
        public decimal Delta
        {
            get { return delta; }
            set { delta = value; }
        }
        //公开构造函数        
        //有参数
        public Fit_Data(decimal k1, decimal k2, decimal k3, decimal k4, decimal delta)
        {
            this.k1 = k1;
            this.k2 = k2;
            this.k3 = k3;
            this.k4 = k4;
            this.delta = delta;
        }
        public Fit_Data(Fit_Data Ini)
        {
            this.k1 = Ini.K1;
            this.k2 = Ini.K2;
            this.k3 = Ini.K3;
            this.k4 = Ini.K4;
            this.delta = Ini.Delta;
        }
        //清空
        public void Empty()
        {
            this.k1 = 0;
            this.k2 = 0;
            this.k3 = 0;
            this.k4 = 0;
            this.delta = 0;
        }
    }
    public struct Double_Fit_Data
    {
        //私有属性 
        private decimal k_x1, k_x2, k_x3, k_x4;//k_x1-1次系数，k_x2-2次系数，k_x3-3次系数，k_x4-4次系数
        private decimal delta_x;//坐标偏移值
        private decimal k_y1, k_y2, k_y3, k_y4;//k_y1-1次系数，k_y2-2次系数，k_y3-3次系数，k_y4-4次系数
        private decimal delta_y;//坐标偏移值
        //共有属性
        public decimal K_X1
        {
            get { return k_x1; }
            set { k_x1 = value; }
        }
        public decimal K_X2
        {
            get { return k_x2; }
            set { k_x2 = value; }
        }
        public decimal K_X3
        {
            get { return k_x3; }
            set { k_x3 = value; }
        }
        public decimal K_X4
        {
            get { return k_x4; }
            set { k_x4 = value; }
        }
        public decimal Delta_X
        {
            get { return delta_x; }
            set { delta_x = value; }
        }
        public decimal K_Y1
        {
            get { return k_y1; }
            set { k_y1 = value; }
        }
        public decimal K_Y2
        {
            get { return k_y2; }
            set { k_y2 = value; }
        }
        public decimal K_Y3
        {
            get { return k_y3; }
            set { k_y3 = value; }
        }
        public decimal K_Y4
        {
            get { return k_y4; }
            set { k_y4 = value; }
        }
        public decimal Delta_Y
        {
            get { return delta_y; }
            set { delta_y = value; }
        }
       
        //公开构造函数        
        //有参数
        public Double_Fit_Data(decimal k_x1, decimal k_x2, decimal k_x3, decimal k_x4, decimal delta_x, decimal k_y1, decimal k_y2, decimal k_y3, decimal k_y4, decimal delta_y)
        {
            this.k_x1 = k_x1;
            this.k_x2 = k_x2;
            this.k_x3 = k_x3;
            this.k_x4 = k_x4;
            this.delta_x = delta_x;
            this.k_y1 = k_y1;
            this.k_y2 = k_y2;
            this.k_y3 = k_y3;
            this.k_y4 = k_y4;
            this.delta_y = delta_y;
        }
        public Double_Fit_Data(Double_Fit_Data Ini)
        {
            this.k_x1 = Ini.K_X1;
            this.k_x2 = Ini.K_X2;
            this.k_x3 = Ini.K_X3;
            this.k_x4 = Ini.K_X4;
            this.delta_x = Ini.Delta_X;
            this.k_y1 = Ini.K_Y1;
            this.k_y2 = Ini.K_Y2;
            this.k_y3 = Ini.K_Y3;
            this.k_y4 = Ini.K_Y4;
            this.delta_y = Ini.Delta_Y;
        }
        //清空
        public void Empty()
        {
            this.k_x1 = 0;
            this.k_x2 = 0;
            this.k_x3 = 0;
            this.k_x4 = 0;
            this.delta_x = 0;
            this.k_y1 = 0;
            this.k_y2 = 0;
            this.k_y3 = 0;
            this.k_y4 = 0;
            this.delta_y = 0;
        }
    }
    public struct Tech_Parameter
    {
        //私有属性 
        private decimal scissors_type, watt, frequence, repeat;//scissors_type-刀具类型，watt-功率，frequence-频率，repeat-重复次数
        private decimal jump_speed, mark_speed, laser_on_delay, laser_off_delay;//jump_speed- 跳转速度，mark_speed- 打标速度，laser_on_delay - 开光延时，laser_off_delay - 关光延时
        private decimal jump_delay, mark_delay, polygon_delay;//jump_delay- 跳转延时，mark_delay-打标延时，polygon_delay- 折线延时
        //共有属性
        public decimal Scissors_Type
        {
            get { return scissors_type; }
            set { scissors_type = value; }
        }
        public decimal Watt
        {
            get { return watt; }
            set { watt = value; }
        }
        public decimal Frequence
        {
            get { return frequence; }
            set { frequence = value; }
        }
        public decimal Repeat
        {
            get { return repeat; }
            set { repeat = value; }
        }
        public decimal Jump_Speed
        {
            get { return jump_speed; }
            set { jump_speed = value; }
        }
        public decimal Mark_Speed
        {
            get { return mark_speed; }
            set { mark_speed = value; }
        }
        public decimal Laser_On_Delay
        {
            get { return laser_on_delay; }
            set { laser_on_delay = value; }
        }
        public decimal Laser_Off_Delay
        {
            get { return laser_off_delay; }
            set { laser_off_delay = value; }
        }
        public decimal Jump_Delay
        {
            get { return jump_delay; }
            set { jump_delay = value; }
        }
        public decimal Mark_Delay
        {
            get { return mark_delay; }
            set { mark_delay = value; }
        }
        public decimal Polygon_Delay
        {
            get { return polygon_delay; }
            set { polygon_delay = value; }
        }
        //公开构造函数        
        //有参数
        public Tech_Parameter(decimal scissors_type, decimal watt, decimal frequence, decimal repeat, decimal jump_speed, decimal mark_speed, decimal laser_on_delay, decimal laser_off_delay, decimal jump_delay, decimal mark_delay, decimal polygon_delay)
        {
            this.scissors_type = scissors_type;
            this.watt = watt;
            this.frequence = frequence;
            this.repeat = repeat;
            this.jump_speed = jump_speed;
            this.mark_speed = mark_speed;
            this.laser_on_delay = laser_on_delay;
            this.laser_off_delay = laser_off_delay;
            this.jump_delay = jump_delay;
            this.mark_delay = mark_delay;
            this.polygon_delay = polygon_delay;
        }
        public Tech_Parameter(Tech_Parameter Ini)
        {
            this.scissors_type = Ini.Scissors_Type;
            this.watt = Ini.Watt;
            this.frequence = Ini.Frequence;
            this.repeat = Ini.Repeat;
            this.jump_speed = Ini.Jump_Speed;
            this.mark_speed = Ini.Mark_Speed;
            this.laser_on_delay = Ini.Laser_On_Delay;
            this.laser_off_delay = Ini.Laser_Off_Delay;
            this.jump_delay = Ini.Jump_Delay;
            this.mark_delay = Ini.Mark_Delay;
            this.polygon_delay = Ini.Polygon_Delay;
        }
        //清空
        public void Empty()
        {
            this.scissors_type = 0;
            this.watt = 0;
            this.frequence = 0;
            this.repeat = 0;
            this.jump_speed = 0;
            this.mark_speed = 0;
            this.laser_on_delay = 0;
            this.laser_off_delay = 0;
            this.jump_delay = 0;
            this.mark_delay = 0;
            this.polygon_delay = 0;
        }
    }
    public class Calibration 
    {
        //定义退出变量
        public static bool Exit_Flag = false;
        public static List<Correct_Data> Get_Datas()
        {
            //建立变量
            List<Correct_Data> Result = new List<Correct_Data>();
            Correct_Data Temp_Correct_Data = new Correct_Data();
            //标定板数据采集
            DataTable Calibration_Data_Acquisition = new DataTable();
            Calibration_Data_Acquisition.Columns.Add("理论定位点X坐标", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("理论定位点Y坐标", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("相机采集X坐标", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("相机采集Y坐标", typeof(decimal));
            //建立变量
            Vector Cam=new Vector();//相机反馈的当前坐标
            Vector Cal_Actual_Point = new Vector();//当前平台坐标 对应的 标定板坐标
            int i = 0, j = 0;
            //建立直角坐标系
            GTS_Fun.Interpolation.Coordination(Para_List.Parameter.Work.X, Para_List.Parameter.Work.Y);
            //定位到加工坐标原点
            GTS_Fun.Interpolation.Gts_Ready(0, 0);
            //2.5mm步距进行数据提取和整合，使用INC指令
            for (i = 0; i < Para_List.Parameter.Gts_Calibration_Row; i++)
            {
                //1轴-x轴，2轴-y轴，X轴归零，y轴归 步距*i
                for (j = 0; j < Para_List.Parameter.Gts_Calibration_Col; j++)
                {
                    //清空Temp_Correct_Data
                    Temp_Correct_Data.Empty();
                    //插补运动实现
                    GTS_Fun.Interpolation.Gts_Ready(j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell);
                    //调用相机，获取对比的坐标信息
                    Thread.Sleep(500);
                    //相机反馈的当前坐标
                    Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Coordinate_Correct(1));//触发拍照 
                    if (Cam.Length == 0)
                    {
                        MessageBox.Show("相机坐标提取失败，请检查！！！");
                        return new List<Correct_Data>();
                    }
                    //相机测算的实际偏差值:(相机反馈的当前坐标) - (相机中心坐标)
                    //当前平台坐标 对应的 标定板坐标
                    Cal_Actual_Point = Get_Cal_Angle_Point(new Vector(j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell));
                    //数据保存
                    Temp_Correct_Data.Xo = Cal_Actual_Point.X + Cam.X;//相机实际X坐标
                    Temp_Correct_Data.Yo = Cal_Actual_Point.Y + Cam.Y;//相机实际Y坐标
                    Temp_Correct_Data.Xm = j * Para_List.Parameter.Gts_Calibration_Cell;//平台电机 理论X坐标
                    Temp_Correct_Data.Ym = i * Para_List.Parameter.Gts_Calibration_Cell;//平台电机 理论Y坐标
                    //标定板数据保存
                    Calibration_Data_Acquisition.Rows.Add(new object[] { j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell, Cam.X, Cam.Y});
                    //添加进入List
                    Result.Add(new Correct_Data(Temp_Correct_Data));
                    //线程终止
                    if (Exit_Flag)
                    {
                        Exit_Flag = false;
                        Serialize_Data.Serialize_Correct_Data(Result, "Gts_Correct_Data_01.xml");//采集数据以xml保存
                        CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Result), "Gts_Correct_Data_01");//采集数据转化为Csv保存
                        CSV_RW.SaveCSV(Calibration_Data_Acquisition, "Calibration_Data_Acquisition_01");//标定板原始数据采集
                        return Result;
                    }
                }
            }
            //保存文件至Config
            Serialize_Data.Serialize_Correct_Data(Result, "Gts_Correct_Data_01.xml");//采集数据以xml保存
            CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Result), "Gts_Correct_Data_01");//采集数据转化为Csv保存
            CSV_RW.SaveCSV(Calibration_Data_Acquisition, "Calibration_Data_Acquisition_01");//标定板原始数据采集
            MessageBox.Show("数据采集完成！！！");
            return Result;
        }
        /// <summary>
        /// 标定板数据测试功能
        /// </summary>
        /// <returns></returns>
        public static List<Correct_Data> Get_Datas_Test()
        {
            //建立变量
            List<Correct_Data> Result = new List<Correct_Data>();
            Correct_Data Temp_Correct_Data = new Correct_Data();
            //标定板数据采集
            DataTable Calibration_Data_Acquisition = CSV_RW.OpenCSV(@"./\Config/Calibration_Data_Acquisition.csv");
            //建立变量
            Vector Cam = new Vector();//相机反馈的当前坐标
            Vector Cal_Actual_Point = new Vector();//当前平台坐标 对应的 标定板坐标
            int i = 0, j = 0;
            //2.5mm步距进行数据提取和整合，使用INC指令
            for (i = 0; i < Para_List.Parameter.Gts_Calibration_Row; i++)
            {
                //1轴-x轴，2轴-y轴，X轴归零，y轴归 步距*i
                for (j = 0; j < Para_List.Parameter.Gts_Calibration_Col; j++)
                {
                    //清空Temp_Correct_Data
                    Temp_Correct_Data.Empty();
                    if((decimal.TryParse(Calibration_Data_Acquisition.Rows[i * Para_List.Parameter.Gts_Calibration_Row + j][2].ToString(), out decimal x0)) && (decimal.TryParse(Calibration_Data_Acquisition.Rows[i * Para_List.Parameter.Gts_Calibration_Row + j][3].ToString(), out decimal y0)))
                    {
                        //提取数据
                        Cam = new Vector(x0,y0);
                    }                    
                    //相机测算的实际偏差值:(相机反馈的当前坐标) - (相机中心坐标)
                    //当前平台坐标 对应的 标定板坐标
                    Cal_Actual_Point = Get_Cal_Angle_Point(new Vector(j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell));
                    //数据保存
                    Temp_Correct_Data.Xo = Cal_Actual_Point.X + Cam.X;//相机实际X坐标
                    Temp_Correct_Data.Yo = Cal_Actual_Point.Y + Cam.Y;//相机实际Y坐标
                    Temp_Correct_Data.Xm = j * Para_List.Parameter.Gts_Calibration_Cell;//平台电机 理论X坐标
                    Temp_Correct_Data.Ym = i * Para_List.Parameter.Gts_Calibration_Cell;//平台电机 理论Y坐标
                    //添加进入List
                    Result.Add(Temp_Correct_Data);
                    //线程终止
                    if (Exit_Flag)
                    {
                        Exit_Flag = false;
                        Serialize_Data.Serialize_Correct_Data(Result, "Gts_Correct_Data_Test.xml");//采集数据以xml保存
                        CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Result), "Gts_Correct_Data_Test");//采集数据转化为Csv保存
                        return Result;
                    }
                }
            }
            //保存文件至Config
            Serialize_Data.Serialize_Correct_Data(Result, "Gts_Correct_Data_Test.xml");//采集数据以xml保存
            CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Result), "Gts_Correct_Data_Test");//采集数据转化为Csv保存
            MessageBox.Show("数据采集完成！！！");
            return Result;
        }
        /// <summary>
        /// 标定板数据测试功能 标定板旋转
        /// </summary>
        /// <returns></returns>
        public static List<Correct_Data> Get_Datas_Angle() 
        {
            //建立变量
            List<Correct_Data> Result = new List<Correct_Data>();
            Correct_Data Temp_Correct_Data = new Correct_Data();
            //标定板数据采集
            DataTable Calibration_Data_Acquisition = CSV_RW.OpenCSV(@"./\Config/Calibration_Data_Acquisition.csv");
            //建立变量
            Vector Cam = new Vector();//相机反馈的当前坐标
            Vector Cal_Angle_Point = new Vector();//当前平台坐标 对应的 标定板坐标
            int i = 0, j = 0;
            //2.5mm步距进行数据提取和整合，使用INC指令
            for (i = 0; i < Para_List.Parameter.Gts_Calibration_Row; i++)
            {
                //1轴-x轴，2轴-y轴，X轴归零，y轴归 步距*i
                for (j = 0; j < Para_List.Parameter.Gts_Calibration_Col; j++)
                {
                    //清空Temp_Correct_Data
                    Temp_Correct_Data.Empty();
                    if ((decimal.TryParse(Calibration_Data_Acquisition.Rows[i * Para_List.Parameter.Gts_Calibration_Row + j][2].ToString(), out decimal x0)) && (decimal.TryParse(Calibration_Data_Acquisition.Rows[i * Para_List.Parameter.Gts_Calibration_Row + j][3].ToString(), out decimal y0)))
                    {
                        //提取数据
                        Cam = new Vector(x0, y0);
                    }
                    //当前平台坐标 对应的 标定板坐标
                    Cal_Angle_Point = Get_Cal_Angle_Point(new Vector(j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell));
                    //数据保存
                    Temp_Correct_Data.Xo = Cal_Angle_Point.X + Cam.X;//相机实际X坐标
                    Temp_Correct_Data.Yo = Cal_Angle_Point.Y + Cam.Y;//相机实际Y坐标
                    Temp_Correct_Data.Xm = j * Para_List.Parameter.Gts_Calibration_Cell;//平台电机 理论X坐标
                    Temp_Correct_Data.Ym = i * Para_List.Parameter.Gts_Calibration_Cell;//平台电机 理论Y坐标
                    //添加进入List
                    Result.Add(Temp_Correct_Data);
                    //线程终止
                    if (Exit_Flag)
                    {
                        Exit_Flag = false;
                        Serialize_Data.Serialize_Correct_Data(Result, "Gts_Correct_Data_Angle.xml");//采集数据以xml保存
                        CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Result), "Gts_Correct_Data_Angle");//采集数据转化为Csv保存
                        return Result;
                    }
                }
            }
            //保存文件至Config
            Serialize_Data.Serialize_Correct_Data(Result, "Gts_Correct_Data_Angle.xml");//采集数据以xml保存
            CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Result), "Gts_Correct_Data_Angle");//采集数据转化为Csv保存
            MessageBox.Show("数据采集完成！！！");
            return Result;
        }
        /// <summary>
        /// 标定板数据二次验证
        /// </summary>
        /// <returns></returns>
        public static List<Correct_Data> Get_Datas_Correct() 
        {
            //建立变量
            List<Correct_Data> Result = new List<Correct_Data>();
            Correct_Data Temp_Correct_Data = new Correct_Data();
            //标定板数据采集
            DataTable Calibration_Data_Acquisition = new DataTable();
            Calibration_Data_Acquisition.Columns.Add("理论定位点X坐标", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("理论定位点Y坐标", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("相机采集X坐标", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("相机采集Y坐标", typeof(decimal));
            //建立变量
            Vector Cam = new Vector();//相机反馈的当前坐标
            Vector Cal_Actual_Point = new Vector();//当前平台坐标 对应的 标定板坐标
            int i = 0, j = 0;
            //建立直角坐标系
            GTS_Fun.Interpolation.Coordination(Para_List.Parameter.Work.X, Para_List.Parameter.Work.Y);
            //定位到加工坐标原点
            GTS_Fun.Interpolation.Gts_Ready_Correct(0, 0);
            //2.5mm步距进行数据提取和整合，使用INC指令
            for (i = 0; i < Para_List.Parameter.Gts_Calibration_Row; i++)
            {
                //1轴-x轴，2轴-y轴，X轴归零，y轴归 步距*i
                for (j = 0; j < Para_List.Parameter.Gts_Calibration_Col; j++)
                {
                    //清空Temp_Correct_Data
                    Temp_Correct_Data.Empty();
                    //插补运动实现
                    GTS_Fun.Interpolation.Gts_Ready_Correct(j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell);
#if !DEBUG
                    //调用相机，获取对比的坐标信息
                    Thread.Sleep(500);
                    Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Coordinate_Correct(1));//触发拍照 
                    if (Cam.Length == 0)
                    {
                        MessageBox.Show("相机坐标提取失败，请检查！！！");
                        return new List<Correct_Data>();
                    }
                    //当前平台坐标 对应的 标定板坐标
                    Cal_Actual_Point = Get_Cal_Angle_Point(new Vector(j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell));
                    //数据保存
                    Temp_Correct_Data.Xo = Cal_Actual_Point.X + Cam.X;//相机实际X坐标
                    Temp_Correct_Data.Yo = Cal_Actual_Point.Y + Cam.Y;//相机实际Y坐标
                    Temp_Correct_Data.Xm = j * Para_List.Parameter.Gts_Calibration_Cell;//平台电机 理论X坐标
                    Temp_Correct_Data.Ym = i * Para_List.Parameter.Gts_Calibration_Cell;//平台电机 理论Y坐标

                    //标定板数据保存
                    Calibration_Data_Acquisition.Rows.Add(new object[] { j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell, Cam.X, Cam.Y });
                    //添加进入List
                    Result.Add(Temp_Correct_Data);
#endif
                    //线程终止
                    if (Exit_Flag)
                    {
                        Exit_Flag = false;
                        Serialize_Data.Serialize_Correct_Data(Result, "Gts_Correct_Data_02.xml");//采集数据以xml保存
                        CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Result), "Gts_Correct_Data_02");//采集数据转化为Csv保存
                        CSV_RW.SaveCSV(Calibration_Data_Acquisition, "Calibration_Data_Acquisition_02");//标定板原始数据采集
                        return Result;
                    }
                }
            }
            //保存文件至Config
            Serialize_Data.Serialize_Correct_Data(Result, "Gts_Correct_Data_02.xml");//采集数据以xml保存
            CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Result), "Gts_Correct_Data_02");//采集数据转化为Csv保存
            CSV_RW.SaveCSV(Calibration_Data_Acquisition, "Calibration_Data_Acquisition_02");//标定板原始数据采集
            MessageBox.Show("数据采集完成！！！");
            return Result;
        }
        /// <summary>
        /// 矫正原点
        /// </summary>
        public static bool Calibrate_Org()
        {
            //建立变量
            Vector Cam = new Vector();
            Vector Coodinate_Point;
            Vector Tem_Mark;
            UInt16 Counting = 0;           

            //矫正原点
            do
            {
                if (Counting == 0)
                {
                    //定位到ORG原点
                    Mark(new Vector(0, 0));
                }
                else
                {
                    //定位到矫正坐标
                    Mark(Para_List.Parameter.Cal_Org);
                }
                //调用相机，获取对比的坐标信息
                Thread.Sleep(1000);//延时200ms
                //Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Pixel_Correct(1));//触发拍照 
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Coordinate_Correct(1));//触发拍照 
                if (Cam.Length == 0)
                {
                    MessageBox.Show("相机坐标提取失败，请检查！！！");
                    return false;
                }
                //Cam = new Vector(Cam.X - 243 * Para_List.Parameter.Cam_Reference, Cam.Y - 324 * Para_List.Parameter.Cam_Reference);
                //获取坐标系平台坐标
                Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
                //计算偏移
                //Tem_Mark = new Vector(Coodinate_Point + Cam);
                Tem_Mark = new Vector(Coodinate_Point - Cam);
                //反馈回RTC_ORG数据
                Para_List.Parameter.Cal_Org = new Vector(Tem_Mark);
                //自增
                Counting++;
                //跳出
                if (Counting >= 10)
                {
                    MessageBox.Show("坐标原点矫正对齐失败！！！");
                    return false;
                }
            } while (!Differ_Deviation(Cam,Para_List.Parameter.Pos_Tolerance));
            Para_List.Parameter.Work = new Vector(Para_List.Parameter.Work.X - Para_List.Parameter.Cal_Org.X, Para_List.Parameter.Work.Y - Para_List.Parameter.Cal_Org.Y);
            return true;
            //数据矫正完成
        }
        /// <summary>
        /// 计算标定板旋转变换参数
        /// </summary>
        /// <returns></returns>
        public static bool Cal_Calibration_Angle_Matrix()
        {
            //不能使用平台坐标
            //建立变量
            Affinity_Matrix Result = new Affinity_Matrix();
            //定位点位确定
            Vector[] Cali_Mark = new Vector[2] { new Vector(0, 0), new Vector(350, 0)};
            //临时变量
            Vector Cam = new Vector();
            Vector Tem_Mark = new Vector();
            double[] temp_array;
            //标定板数据采集
            DataTable Calibration_Board_Angle = new DataTable();
            Calibration_Board_Angle.Columns.Add("理论定位点X坐标", typeof(decimal));
            Calibration_Board_Angle.Columns.Add("理论定位点Y坐标", typeof(decimal));
            Calibration_Board_Angle.Columns.Add("相机采集X坐标", typeof(decimal));
            Calibration_Board_Angle.Columns.Add("相机采集Y坐标", typeof(decimal));
            //标定板数据计算
            for (int i = 0; i < Cali_Mark.Length; i++)
            {
                Tem_Mark = new Vector(Cali_Mark[i]);//计算偏移 
                //定位到标定板数据实际点位i
                Mark(Tem_Mark);
                //调用相机，获取对比的坐标信息
                Thread.Sleep(1000);
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Coordinate_Correct(1));//触发拍照 
                if (Cam.Length == 0)
                {
                    MessageBox.Show("相机坐标数据提取错误，请检查！！！");
                    return false;
                }                
                //反馈回标定板数据实际点位i
                Cali_Mark[i] = new Vector(Tem_Mark + Cam);
                //数据保存
                Calibration_Board_Angle.Rows.Add(new object[] { Cali_Mark[i].X, Cali_Mark[i].Y, Cam.X, Cam.Y });
            }
            Mat rotateMat = new Mat();//定义旋转变换数组
            double angle = -(Math.Atan((double)((Cali_Mark[1].Y - Cali_Mark[0].Y) / (Cali_Mark[1].X - Cali_Mark[0].X))) * 180) / Math.PI;//旋转角度
            double scale = 1.0;//缩放因子
            PointF center = new PointF(0, 0);//旋转中心
            CvInvoke.GetRotationMatrix2D(center, angle, scale, rotateMat);
            //提取矩阵数据
            temp_array = rotateMat.GetDoubleArray();
            //获取仿射变换参数
            Result = Gts_Cal_Data_Resolve.Array_To_Affinity(temp_array);
            //获取仿射变换参数
            Para_List.Parameter.Cal_Trans_Angle = new Affinity_Matrix(Result);
            //保存数据
            CSV_RW.SaveCSV(Calibration_Board_Angle, "Calibration_Board_Angle_Data");
            //追加进入仿射变换List
            return true;
        }
        /// <summary>
        /// 计算标定板旋转变换后坐标值
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Vector Get_Cal_Angle_Point(Vector src)
        {
            return new Vector(src.X * Para_List.Parameter.Cal_Trans_Angle.Stretch_X + src.Y * Para_List.Parameter.Cal_Trans_Angle.Distortion_X + Para_List.Parameter.Cal_Trans_Angle.Delta_X, src.Y * Para_List.Parameter.Cal_Trans_Angle.Stretch_Y + src.X * Para_List.Parameter.Cal_Trans_Angle.Distortion_Y + Para_List.Parameter.Cal_Trans_Angle.Delta_Y);
        }
        /// <summary>
        /// 矫正Mark坐标
        ///
        /// </summary>
        /// <param name="type"></param>
        /// type - 0 初次校准
        /// type - 1 re_cal
        public static bool Calibrate_Mark(int type)
        {
            //建立变量
            Vector Cam = new Vector();
            Vector Coodinate_Point;
            Vector Tem_Mark;
            Vector Mark4_dif;
            UInt16 Counting;
            List<Vector> Mark_Datas = new List<Vector>();
            //abstract Mark Point
            if (type == 0)
            {
                Mark_Datas.Add(new Vector(Para_List.Parameter.Mark1));
                Mark_Datas.Add(new Vector(Para_List.Parameter.Mark2));
                Mark_Datas.Add(new Vector(Para_List.Parameter.Mark3));
                Mark_Datas.Add(new Vector(Para_List.Parameter.Mark4));
            }
            else
            {
                Mark_Datas.Add(new Vector(Gts_Cal_Data_Resolve.Get_Aff_After(Para_List.Parameter.Mark_Dxf1, Para_List.Parameter.Trans_Affinity)));
                Mark_Datas.Add(new Vector(Gts_Cal_Data_Resolve.Get_Aff_After(Para_List.Parameter.Mark_Dxf2, Para_List.Parameter.Trans_Affinity)));
                Mark_Datas.Add(new Vector(Gts_Cal_Data_Resolve.Get_Aff_After(Para_List.Parameter.Mark_Dxf3, Para_List.Parameter.Trans_Affinity)));
                Mark_Datas.Add(new Vector(Gts_Cal_Data_Resolve.Get_Aff_After(Para_List.Parameter.Mark_Dxf4, Para_List.Parameter.Trans_Affinity)));
            }            
            //process the mark point
            for (int i=0;i< Mark_Datas.Count;i++)
            {
                //矫正Mark           
                Counting = 0;
                do
                {
                    //定位到Mark点
                    Mark_Correct(Mark_Datas[i]);
                    //调用相机，获取对比的坐标信息
                    Thread.Sleep(200);//延时200ms
                    Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Coordinate_Correct(Para_List.Parameter.Camera_Mark_Type));//触发拍照 
                    if (Cam.Length == 0)
                    {
                         MessageBox.Show("相机坐标提取失败，请检查！！！");
                        return false;
                    }
                    //获取坐标系平台坐标
                    Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(1));
                    //计算偏移
                    Tem_Mark = new Vector(Coodinate_Point - Cam);
                    //反馈回Mark点
                    Mark_Datas[i] = new Vector(Tem_Mark);
                    //自增
                    Counting++;
                    //跳出
                    if (Counting >= 20)
                    {
                        MessageBox.Show(string.Format("Mark{0} 寻找失败!!!",i+1));
                        return false;
                    }
                } while (!Differ_Deviation(Cam, Para_List.Parameter.Pos_Tolerance));
                //获取理论坐标
                Mark_Datas[i] = new Vector(GTS_Fun.Interpolation.Get_Coordinate(1));
            }
            //cal Affinity matrics data 
            Para_List.Parameter.Trans_Affinity =new Affinity_Matrix(Gts_Cal_Data_Resolve.Cal_Affinity(Mark_Datas));

            //difference mark4 
            Tem_Mark = Gts_Cal_Data_Resolve.Get_Aff_After(Para_List.Parameter.Mark_Dxf4, Para_List.Parameter.Trans_Affinity);
            //caluate difference between theory mark4 and actual mark4
            Mark4_dif = new Vector(Tem_Mark - Mark_Datas[3]);
            //output result
            if (Differ_Deviation(Mark4_dif, Para_List.Parameter.Mark_Reference))
            {
                Prompt.Log.Info(String.Format("Mark4 验证OK！！！，X坐标偏差：{0}，Y坐标偏差：{1}", Mark4_dif.X, Mark4_dif.Y));
                MessageBox.Show(String.Format("Mark4 验证OK！！！，X坐标偏差：{0}，Y坐标偏差：{1}", Mark4_dif.X, Mark4_dif.Y));
            }
            else
            {
                Prompt.Log.Info(String.Format("Mark4 验证NG！！！，X坐标偏差：{0}，Y坐标偏差：{1}", Mark4_dif.X, Mark4_dif.Y));
                MessageBox.Show(String.Format("Mark4 验证NG！！！，X坐标偏差：{0}，Y坐标偏差：{1}", Mark4_dif.X, Mark4_dif.Y));
            }
            //返回
            return true;
        }
        /// <summary>
        /// 判别误差范围之内
        /// </summary>
        /// <param name="Indata"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static bool Differ_Deviation(Vector Indata,decimal reference)
        {
            if ((Math.Abs(Indata.X)<= reference) && (Math.Abs(Indata.Y) <= reference))
            {
                return true;
            }
            else
            {
                return false;
            }
         }       
        /// <summary>
        /// 矫正 振镜与ORG的距离
        /// </summary>
        public static bool Calibrate_RTC_ORG()
        {
            //生成RTC扫圆轨迹
            List<List<Interpolation_Data>> Calibrate_Data = Generate_Org_Rtc_Data(0.4m, 2.0m);
            //执行
            Integrated.Rts_Gts(Calibrate_Data);                   
            //建立变量
            Vector Cam = new Vector();
            Vector Coodinate_Point;
            Vector Tem_Mark;
            UInt16 Counting = 0;
            Tem_Mark = new Vector(Para_List.Parameter.Rtc_Org + Para_List.Parameter.Base_Gts);
            //矫正数据
            do
            {
                //定位到ORG矫正点
                Mark_Correct(Tem_Mark);
                //调用相机，获取对比的坐标信息
                Thread.Sleep(500);
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Pixel_Correct(2));//触发拍照 
                if (Cam.Length == 0)
                {
                    MessageBox.Show("相机坐标提取失败，请检查！！！");
                    return false;
                }
                Cam = new Vector(Cam.X - 243 * Para_List.Parameter.Cam_Reference, Cam.Y - 324 * Para_List.Parameter.Cam_Reference);
                //获取坐标系平台坐标
                Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(1));
                //计算偏移
                Tem_Mark = new Vector(Coodinate_Point + Cam);
                //自增
                Counting++;
                if (Counting>=10)
                {
                    return false;
                }

            } while (!Differ_Deviation(Cam, Para_List.Parameter.Pos_Tolerance));
            //获取实际坐标值            
            Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(1));//获取坐标系平台坐标
            Tem_Mark = new Vector(Coodinate_Point + Cam);
            Para_List.Parameter.Rtc_Org = new Vector(Tem_Mark - Para_List.Parameter.Base_Gts);
            //数据矫正完成
            return true;
        }
        /// <summary>
        /// 生成RTC 与 原点距离矫正 数据
        /// </summary>
        /// <param name="Radius"></param>
        /// <param name="Interval"></param>
        /// <returns></returns>
        public static List<List<Interpolation_Data>> Generate_Org_Rtc_Data(decimal Radius, decimal Interval)
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
            List<Interpolation_Data> Temp_Interpolation_List_Data = new List<Interpolation_Data>();//二级层
            Interpolation_Data Temp_Data = new Interpolation_Data();//一级层  
            decimal Gts_X = Para_List.Parameter.Base_Gts.X, Gts_Y = Para_List.Parameter.Base_Gts.Y;//X、Y坐标
            //decimal Radius = 1.0m;//半径
            //decimal Interval = 3.0m;//间距  
            //初始清除
            Result.Clear();
            Temp_Interpolation_List_Data.Clear();
            Temp_Data.Empty();

            //走刀至Gts 平台坐标

            //Gts 直线插补
            Temp_Data.Type = 1;
            //强制抬刀标志：1
            Temp_Data.Lift_flag = 1;
            //强制加工类型为Gts
            Temp_Data.Work = 10;
            //直线终点坐标
            Temp_Data.End_x = Gts_X;
            Temp_Data.End_y = Gts_Y;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //坐标原点 1半径的圆圈 1号圆
            //追加RTC加工数据
            //数据清空
            Temp_Data.Empty();
            //强制抬刀标志：0
            Temp_Data.Lift_flag = 0;
            //强制加工类型为RTC
            Temp_Data.Work = 20;
            //GTS平台配合坐标
            Temp_Data.Gts_x = Gts_X;
            Temp_Data.Gts_y = Gts_Y;
            //Rtc定位 激光加工起点坐标
            Temp_Data.Rtc_x = Radius;
            Temp_Data.Rtc_y = 0;
            //RTC arc_abs圆弧
            Temp_Data.Type = 11;
            //RTC 圆弧加工圆心坐标转换
            Temp_Data.Center_x = 0;
            Temp_Data.Center_y = 0;
            //圆弧角度
            Temp_Data.Angle = 370;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //处理二次结果，合并走直线的Gts数据，下次为Rtc加工，则变动该GTS数据终点坐标为RTC加工的gts基准位置
            for (int cal = 0; cal < Result.Count; cal++)
            {
                //当前序号 数量为1、加工类型1 直线、加工方式10 GTS
                //当前+1序号 数量大于1、加工方式20 RTX
                if ((cal < Result.Count - 1) && (Result[cal].Count == 1) && (Result[cal][0].Type == 1) && (Result[cal][0].Work == 10) && (Result[cal + 1].Count >= 1) && (Result[cal + 1][0].Work == 20))
                {
                    Temp_Data.Empty();
                    Temp_Data = Result[cal][0];
                    Temp_Data.End_x = Result[cal + 1][0].Gts_x;
                    Temp_Data.End_y = Result[cal + 1][0].Gts_y;
                    //重新赋值
                    Result[cal][0] = new Interpolation_Data(Temp_Data);
                }
            }
            //返回结果
            return Result;
        }
        /// <summary>
        /// 定位mark点 未矫正
        /// </summary>
        /// <param name="point"></param>
        public static void Mark(Vector point)
        {
            GTS_Fun.Interpolation.Gts_Ready(point.X,point.Y);
        }
        /// <summary>
        /// 定位mark点 矫正
        /// </summary>
        /// <param name="point"></param>
        public static void Mark_Correct(Vector point)
        {
            GTS_Fun.Interpolation.Gts_Ready_Correct(point.X, point.Y);
        }
        ///校准相机像素与物理距离的对应关系
        ///相机坐标系以相片中心为原点
        ///对应物理坐标系的坐标点
        ///实现像素与坐标的转换
        ///使用烧灼Mark点进行校准
        public static bool Cal_Cam_Affinity()
        {
            //生成打标数据
            //生成RTC扫圆轨迹
            List<List<Interpolation_Data>> Calibrate_Data = Generate_Org_Rtc_Data(0.4m, 2.0m);
            //执行打标数据
            Integrated.Rts_Gts(Calibrate_Data);

            //建立变量
            Affinity_Matrix Result = new Affinity_Matrix();
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义点位数组
            PointF[] srcTri = new PointF[3];//标准数据
            PointF[] dstTri = new PointF[3];//差异化数据 
            double[] temp_array;
            //定位点位计算标定板偏差
            Vector[] Cali_Mark_Src = new Vector[3] { new Vector(0, 0), new Vector(0, 2.0m), new Vector(1.5m, 0) };
            Vector[] Cali_Mark_Dst = new Vector[3] { new Vector(0, 0), new Vector(0, 2.0m), new Vector(1.5m, 0) };

            //矫正坐标中心对齐
            Vector Cam = new Vector();
            Vector Coodinate_Point;
            Vector Tem_Mark;
            UInt16 Counting = 0;
            //相对位移标定相机坐标系
            for (int i =0;i< Cali_Mark_Src.Length;i++)
            {                
                if (i==0)
                {
                    Tem_Mark = new Vector(Cali_Mark_Src[i] + Para_List.Parameter.Rtc_Org + Para_List.Parameter.Base_Gts);
                    do
                    {
                        //定位矫正点
                        Mark(Tem_Mark);
                        //调用相机，获取对比的坐标信息
                        Thread.Sleep(1000);
                        Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Pixel_Correct(2));//触发拍照 
                        if (Cam.Length == 0)
                        {
                            MessageBox.Show("相机坐标提取失败，请检查！！！");
                            return false;
                        }
                        Cam = new Vector(Cam.X - 243 * Para_List.Parameter.Cam_Reference, Cam.Y - 324 * Para_List.Parameter.Cam_Reference);
                        //获取坐标系平台坐标
                        Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
                        //计算偏移
                        Tem_Mark = new Vector(Coodinate_Point + Cam);
                        //自增
                        Counting++;
                        if (Counting >= 10)
                        {
                            MessageBox.Show("相机坐标系中心对齐失败！！！");
                            return false;
                        }
                    } while (!Differ_Deviation(Cam, Para_List.Parameter.Pos_Tolerance));

                    //获取坐标对齐mark的像素坐标
                    Thread.Sleep(1000);
                    Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Actual_Pixel(2));//触发拍照 
                    if (Cam.Length == 0)
                    {
                        MessageBox.Show("相机坐标提取失败，请检查！！！");
                        return false;
                    }
                    //反馈回相机实际坐标
                    Cali_Mark_Src[i] = new Vector(Cam);
                }
                else
                {
                    Tem_Mark = new Vector(new Vector(0,0) + Para_List.Parameter.Rtc_Org + Para_List.Parameter.Base_Gts);
                    do
                    {
                        //定位矫正点
                        Mark(Tem_Mark);
                        //调用相机，获取对比的坐标信息
                        Thread.Sleep(1000);
                        Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Pixel_Correct(2));//触发拍照 
                        if (Cam.Length == 0)
                        {
                            MessageBox.Show("相机坐标提取失败，请检查！！！");
                            return false;
                        }
                        Cam = new Vector(Cam.X - 243 * Para_List.Parameter.Cam_Reference, Cam.Y - 324 * Para_List.Parameter.Cam_Reference);
                        //获取坐标系平台坐标
                        Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
                        //计算偏移
                        Tem_Mark = new Vector(Coodinate_Point + Cam);
                        //自增
                        Counting++;
                        if (Counting >= 10)
                        {
                            MessageBox.Show("相机坐标系中心对齐失败！！！");
                            return false;
                        }
                    } while (!Differ_Deviation(Cam, Para_List.Parameter.Pos_Tolerance));

                    //进行位移
                    Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
                    Tem_Mark = new Vector(Coodinate_Point + Cali_Mark_Src[i]);
                    Mark(Tem_Mark);
                    //调用相机，获取对比的坐标信息
                    Thread.Sleep(1000);
                    Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Actual_Pixel(2));//触发拍照 
                    if (Cam.Length == 0)
                    {
                        MessageBox.Show("相机坐标提取失败，请检查！！！");
                        return false;
                    }
                    //反馈回相机实际坐标
                    Cali_Mark_Src[i] = new Vector(Cam);
                }     
            }

            //数据提取
            //标准数据
            srcTri[0] = new PointF((float)(Cali_Mark_Src[0].X), (float)(Cali_Mark_Src[0].Y));
            srcTri[1] = new PointF((float)(Cali_Mark_Src[1].X), (float)(Cali_Mark_Src[1].Y));
            srcTri[2] = new PointF((float)(Cali_Mark_Src[2].X), (float)(Cali_Mark_Src[2].Y));
            //仿射数据
            dstTri[0] = new PointF((float)(Cali_Mark_Dst[0].X), (float)(Cali_Mark_Dst[0].Y));
            dstTri[1] = new PointF((float)(Cali_Mark_Dst[1].X), (float)(Cali_Mark_Dst[1].Y));
            dstTri[2] = new PointF((float)(Cali_Mark_Dst[2].X), (float)(Cali_Mark_Dst[2].Y));
            //计算仿射变换矩阵
            mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
            //提取矩阵数据
            temp_array = mat.GetDoubleArray();
            //获取仿射变换参数
            Result = Gts_Cal_Data_Resolve.Array_To_Affinity(temp_array);
            Para_List.Parameter.Cam_Trans_Affinity = new Affinity_Matrix(Result);
            return true;
        }
        ///校准相机像素与物理距离的对应关系
        ///相机坐标系以相片中心为原点
        ///对应物理坐标系的坐标点
        ///实现像素与坐标的转换
        ///使用标定板原点进行校准
        public static bool Cal_Cam_Affinity_Test()
        {
            //建立变量
            Affinity_Matrix Result = new Affinity_Matrix();
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义点位数组
            PointF[] srcTri = new PointF[3];//标准数据
            PointF[] dstTri = new PointF[3];//差异化数据 
            double[] temp_array;
            //定位点位计算标定板偏差
            Vector[] Cali_Mark_Src = new Vector[3] { new Vector(0, 0), new Vector(0, 1.5m), new Vector(1.5m, 0) };
            Vector[] Cali_Mark_Dst = new Vector[3] { new Vector(0, 0), new Vector(0, 1.5m), new Vector(1.5m, 0) };

            //矫正坐标中心对齐
            Vector Cam = new Vector();
            Vector Coodinate_Point;
            Vector Tem_Mark;
            UInt16 Counting = 0;
            //相对位移标定相机坐标系
            for (int i = 0; i < Cali_Mark_Src.Length; i++)
            {
                Tem_Mark = new Vector(Cali_Mark_Src[i]);
                if (i == 0)
                {
                    do
                    {
                        //定位矫正点
                        Mark(Tem_Mark);
                        //调用相机，获取对比的坐标信息
                        Thread.Sleep(1000);
                        Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Pixel_Correct(1));//触发拍照 
                        if (Cam.Length == 0)
                        {
                            MessageBox.Show("相机坐标提取失败，请检查！！！");
                            return false;
                        }
                        Cam = new Vector(Cam.X - 243 * Para_List.Parameter.Cam_Reference, Cam.Y - 324 * Para_List.Parameter.Cam_Reference);
                        //获取坐标系平台坐标
                        Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
                        //计算偏移
                        Tem_Mark = new Vector(Coodinate_Point + Cam);
                        //自增
                        Counting++;
                        if (Counting >= 10)
                        {
                            MessageBox.Show("相机坐标系中心对齐失败！！！");
                            return false;
                        }
                    } while (!Differ_Deviation(Cam, Para_List.Parameter.Pos_Tolerance));

                    //获取坐标对齐mark的像素坐标
                    Thread.Sleep(2000);
                    Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Actual_Pixel(1));//触发拍照 
                    if (Cam.Length == 0)
                    {
                        MessageBox.Show("相机坐标提取失败，请检查！！！");
                        return false;
                    }
                    //反馈回相机实际坐标
                    Cali_Mark_Src[i] = new Vector(Cam);
                }
                else
                {
                    //定位坐标
                    Mark(Cali_Mark_Src[i]);
                    //调用相机，获取对比的坐标信息
                    Thread.Sleep(2000);
                    Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Actual_Pixel(1));//触发拍照 
                    if (Cam.Length == 0)
                    {
                        MessageBox.Show("相机坐标提取失败，请检查！！！");
                        return false;
                    }
                    //反馈回相机实际坐标
                    Cali_Mark_Src[i] = new Vector(Cam);
                }
            }

            //数据提取
            //标准数据
            srcTri[0] = new PointF((float)(Cali_Mark_Src[0].X), (float)(Cali_Mark_Src[0].Y));
            srcTri[1] = new PointF((float)(Cali_Mark_Src[1].X), (float)(Cali_Mark_Src[1].Y));
            srcTri[2] = new PointF((float)(Cali_Mark_Src[2].X), (float)(Cali_Mark_Src[2].Y));
            //仿射数据
            dstTri[0] = new PointF((float)(Cali_Mark_Dst[0].X), (float)(Cali_Mark_Dst[0].Y));
            dstTri[1] = new PointF((float)(Cali_Mark_Dst[1].X), (float)(Cali_Mark_Dst[1].Y));
            dstTri[2] = new PointF((float)(Cali_Mark_Dst[2].X), (float)(Cali_Mark_Dst[2].Y));
            //计算仿射变换矩阵
            mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
            //提取矩阵数据
            temp_array = mat.GetDoubleArray();
            //获取仿射变换参数
            Result = Gts_Cal_Data_Resolve.Array_To_Affinity(temp_array);
            Para_List.Parameter.Cam_Trans_Affinity = new Affinity_Matrix(Result);
            return true;
        }
        /// <summary>
        /// 计算振镜坐标系与平台坐标系的夹角
        /// </summary>
        public static bool Get_Rtc_Coordinate_Affinity()
        {
            //打标距离
            decimal Radius = 0.5m;//半径
            decimal Interval = 25m;//间距 
            //数据采集
            DataTable Calibration_Data_Acquisition = new DataTable();
            Calibration_Data_Acquisition.Columns.Add("振镜X坐标", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("振镜Y坐标", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("平台X坐标", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("平台Y坐标", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("X轴旋转角度", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("Y轴旋转角度", typeof(decimal));
            //生成RTC扫圆轨迹
            List<List<Interpolation_Data>> Calibrate_Data = Generate_Rtc_Cor_Data(Radius, Interval);
            //执行
            Integrated.Rts_Gts(Calibrate_Data);
            //建立变量
            Affinity_Matrix Result = new Affinity_Matrix();
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义点位数组
            PointF[] srcTri = new PointF[3];//标准数据
            PointF[] dstTri = new PointF[3];//差异化数据
            //定位点位初始化
            Vector[] Cali_Mark_Src = new Vector[4] { new Vector(0, 0), new Vector(Interval, 0), new Vector(0, Interval), new Vector(0, -Interval) };
            Vector[] Cali_Mark_Dst = new Vector[4] { new Vector(0, 0), new Vector(Interval, 0), new Vector(0, Interval), new Vector(0, -Interval) };
            Vector Gts_Point = new Vector(Para_List.Parameter.Base_Gts);
            //Dst定位点处理
            for (int i = 0;i< Cali_Mark_Dst.Length;i++)
            {
                Cali_Mark_Dst[i] = new Vector(Gts_Point + Cali_Mark_Dst[i] + Para_List.Parameter.Rtc_Org);
            }
            //仿射变换数组
            double[] temp_array;
            Vector Cam = new Vector();
            Vector Tem_Mark = new Vector();
            Vector Coordinate = new Vector();
            UInt16 Counting = 0;            

            //标定板数据计算
            for (int i = 0; i < Cali_Mark_Dst.Length; i++)
            {
                do
                {
                    //定位到标定板数据实际点位i
                    Mark_Correct(Cali_Mark_Dst[i]); 
                    //调用相机，获取对比的坐标信息
                    Thread.Sleep(500);//延时200ms
                    Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Coordinate_Correct(1));//触发拍照 
                    if (Cam.Length == 0)
                    {
                        MessageBox.Show("相机坐标提取失败，请检查！！！");
                        return false;
                    }
                    Coordinate = GTS_Fun.Interpolation.Get_Coordinate(1);
                    Tem_Mark = new Vector(Coordinate - Cam);//计算偏移
                    Cali_Mark_Dst[i] = new Vector(Tem_Mark);//反馈回标定板数据实际点位                                                            
                    Counting++;//自增
                    if (Counting >= 30)
                    {
                        MessageBox.Show("超出重试次数！！！");
                        return false;
                    }
                } while (!Differ_Deviation(Cam, Para_List.Parameter.Pos_Tolerance));            
                //获取实际坐标值
                Cali_Mark_Dst[i] = GTS_Fun.Interpolation.Get_Coordinate(1);
                //数据保存
                Calibration_Data_Acquisition.Rows.Add(new object[] { Cali_Mark_Src[i].X, Cali_Mark_Src[i].Y, Cali_Mark_Dst[i].X, Cali_Mark_Dst[i].Y });
            }
            //以原点为基准处理Dst点位
            Vector Cal_Standard = new Vector(Gts_Point + Para_List.Parameter.Rtc_Org);
            for (int i = 0; i < Cali_Mark_Dst.Length; i++)
            {
                Cali_Mark_Dst[i] = new Vector(Cali_Mark_Dst[i] - Cal_Standard);
            }
            //数据提取
            //标准数据  实测值
            srcTri[0] = new PointF((float)(Cali_Mark_Dst[1].X), (float)(Cali_Mark_Dst[1].Y));
            srcTri[1] = new PointF((float)(Cali_Mark_Dst[2].X), (float)(Cali_Mark_Dst[2].Y));
            srcTri[2] = new PointF((float)(Cali_Mark_Dst[3].X), (float)(Cali_Mark_Dst[3].Y));
            //仿射数据  振镜输出值
            dstTri[0] = new PointF((float)(Cali_Mark_Src[1].X), (float)(Cali_Mark_Src[1].Y));
            dstTri[1] = new PointF((float)(Cali_Mark_Src[2].X), (float)(Cali_Mark_Src[2].Y));
            dstTri[2] = new PointF((float)(Cali_Mark_Src[3].X), (float)(Cali_Mark_Src[3].Y));
            //计算仿射变换矩阵
            mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
            //提取矩阵数据
            temp_array = mat.GetDoubleArray();
            //获取仿射变换参数
            Result = Gts_Cal_Data_Resolve.Array_To_Affinity(temp_array);
            Para_List.Parameter.Rtc_Trans_Affinity = new Affinity_Matrix(Result);
            //计算角度
            //Affinity_Matrix Temp_para = new Affinity_Matrix();
            //Temp_para.Stretch_X = Cali_Mark_Dst[1].X/ Cali_Mark_Dst[1].Length;
            //Temp_para.Distortion_X = Cali_Mark_Dst[1].Y/ Cali_Mark_Dst[1].Length;
            //Temp_para.Stretch_Y = Cali_Mark_Dst[2].Y/ Cali_Mark_Dst[2].Length;
            //Temp_para.Distortion_Y = Cali_Mark_Dst[2].X/ Cali_Mark_Dst[2].Length;
            //Para_List.Parameter.Rtc_Trans_Angle = new Affinity_Matrix(Temp_para);
            //数据保存
            CSV_RW.SaveCSV(Calibration_Data_Acquisition, "Rtc_Cor_Data_Acquisition");
            return true;
        }
        /// <summary>
        /// 生成Rtc坐标系矫正 所需的加工轨迹
        /// </summary>
        /// <param name="Radius"></param>
        /// <param name="Interval"></param>
        /// <returns></returns>
        public static List<List<Interpolation_Data>> Generate_Rtc_Cor_Data(decimal Radius, decimal Interval)
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
            List<Interpolation_Data> Temp_Interpolation_List_Data = new List<Interpolation_Data>();//二级层
            Interpolation_Data Temp_Data = new Interpolation_Data();//一级层  
            decimal Gts_X = Para_List.Parameter.Base_Gts.X, Gts_Y = Para_List.Parameter.Base_Gts.Y;//X、Y坐标
            //decimal Radius = 1.0m;//半径
            //decimal Interval = 5.0m;//间距  
            //初始清除
            Result.Clear();
            Temp_Interpolation_List_Data.Clear();
            Temp_Data.Empty();

            //Gts 直线插补
            Temp_Data.Type = 1;
            //强制抬刀标志：1
            Temp_Data.Lift_flag = 1;
            //强制加工类型为Gts
            Temp_Data.Work = 10;
            //直线终点坐标
            Temp_Data.End_x = Gts_X;
            Temp_Data.End_y = Gts_Y;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //坐标原点  1号圆
            //追加RTC加工数据
            //数据清空
            Temp_Data.Empty();
            //强制抬刀标志：0
            Temp_Data.Lift_flag = 0;
            //强制加工类型为RTC
            Temp_Data.Work = 20;
            //GTS平台配合坐标
            Temp_Data.Gts_x = Gts_X;
            Temp_Data.Gts_y = Gts_Y;
            //Rtc定位 激光加工起点坐标
            Temp_Data.Rtc_x = Radius;
            Temp_Data.Rtc_y = 0;
            //RTC arc_abs圆弧
            Temp_Data.Type = 11;
            //RTC 圆弧加工圆心坐标转换
            Temp_Data.Center_x = 0;
            Temp_Data.Center_y = 0;
            //圆弧角度
            Temp_Data.Angle = 370;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //坐标原点  2号圆
            //追加RTC加工数据
            //数据清空
            Temp_Data.Empty();
            //强制抬刀标志：0
            Temp_Data.Lift_flag = 0;
            //强制加工类型为RTC
            Temp_Data.Work = 20;
            //GTS平台配合坐标
            Temp_Data.Gts_x = Gts_X;
            Temp_Data.Gts_y = Gts_Y;
            //Rtc定位 激光加工起点坐标
            Temp_Data.Rtc_x = Radius;
            Temp_Data.Rtc_y = Interval;
            //RTC arc_abs圆弧
            Temp_Data.Type = 11;
            //RTC 圆弧加工圆心坐标转换
            Temp_Data.Center_x = 0;
            Temp_Data.Center_y = Interval;
            //圆弧角度
            Temp_Data.Angle = 370;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();


            //坐标原点  3号圆
            //追加RTC加工数据
            //数据清空
            Temp_Data.Empty();
            //强制抬刀标志：0
            Temp_Data.Lift_flag = 0;
            //强制加工类型为RTC
            Temp_Data.Work = 20;
            //GTS平台配合坐标
            Temp_Data.Gts_x = Gts_X;
            Temp_Data.Gts_y = Gts_Y;
            //Rtc定位 激光加工起点坐标
            Temp_Data.Rtc_x = Radius + Interval;
            Temp_Data.Rtc_y = 0;
            //RTC arc_abs圆弧
            Temp_Data.Type = 11;
            //RTC 圆弧加工圆心坐标转换
            Temp_Data.Center_x = Interval;
            Temp_Data.Center_y = 0;
            //圆弧角度
            Temp_Data.Angle = 370;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();


            //坐标原点  4号圆
            //追加RTC加工数据
            //数据清空
            Temp_Data.Empty();
            //强制抬刀标志：0
            Temp_Data.Lift_flag = 0;
            //强制加工类型为RTC
            Temp_Data.Work = 20;
            //GTS平台配合坐标
            Temp_Data.Gts_x = Gts_X;
            Temp_Data.Gts_y = Gts_Y;
            //Rtc定位 激光加工起点坐标
            Temp_Data.Rtc_x = Radius;
            Temp_Data.Rtc_y = -Interval;
            //RTC arc_abs圆弧
            Temp_Data.Type = 11;
            //RTC 圆弧加工圆心坐标转换
            Temp_Data.Center_x = 0;
            Temp_Data.Center_y = -Interval;
            //圆弧角度
            Temp_Data.Angle = 370;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //处理二次结果，合并走直线的Gts数据，下次为Rtc加工，则变动该GTS数据终点坐标为RTC加工的gts基准位置
            for (int cal = 0; cal < Result.Count; cal++)
            {
                //当前序号 数量为1、加工类型1 直线、加工方式10 GTS
                //当前+1序号 数量大于1、加工方式20 RTX
                if ((cal < Result.Count - 1) && (Result[cal].Count == 1) && (Result[cal][0].Type == 1) && (Result[cal][0].Work == 10) && (Result[cal + 1].Count >= 1) && (Result[cal + 1][0].Work == 20))
                {
                    Temp_Data.Empty();
                    Temp_Data = Result[cal][0];
                    Temp_Data.End_x = Result[cal + 1][0].Gts_x;
                    Temp_Data.End_y = Result[cal + 1][0].Gts_y;
                    //重新赋值
                    Result[cal][0] = new Interpolation_Data(Temp_Data);
                }
            }
            //返回结果
            return Result;
        }
        /// <summary>
        /// 振镜数据采集
        /// </summary>
        /// <returns></returns>
        public static List<Vector> Rtc_Correct_Coordinate()
        {
            //生成相机采集数据点位
            List<Vector> Aquisition_Point = new List<Vector>();
            List<Vector> Rtc_Point = new List<Vector>();
            decimal Gts_X = Para_List.Parameter.Base_Gts.X, Gts_Y = Para_List.Parameter.Base_Gts.Y;//X、Y坐标
            Vector Cam = new Vector();
            Vector Tem_Mark = new Vector();
            Vector Coordinate = new Vector();
            Vector Tem_Datum = new Vector();
            //数据采集
            DataTable Temp_Acquisition = new DataTable();
            Temp_Acquisition.Columns.Add("振镜坐标X", typeof(decimal));
            Temp_Acquisition.Columns.Add("振镜坐标Y", typeof(decimal));
            Temp_Acquisition.Columns.Add("实际坐标X", typeof(decimal));
            Temp_Acquisition.Columns.Add("实际坐标Y", typeof(decimal));
            DataTable Calibration_Data_Acquisition = new DataTable();
            Calibration_Data_Acquisition.Columns.Add("振镜坐标X", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("振镜坐标Y", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("实际坐标X", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("实际坐标Y", typeof(decimal));
            //计算总间距
            Int16 No = (Int16)(Para_List.Parameter.Rtc_Distortion_Data_Limit / Para_List.Parameter.Rtc_Distortion_Data_Interval);
            ///Rtc要求数据 顺序：左上角-->右上角 Y坐标不变，依次变更X坐标
            ///Gts匹配 顺序：右上角-->右下角 X坐标不变，依次变更Y坐标 （Rts坐标轴交换，同时Rtc的X轴方向取反）
            for (int i = No / 2; i >= -No / 2; i--)
            {
                for (int j = No / 2; j >= -No / 2; j--)
                {
                    Aquisition_Point.Add(new Vector(Gts_X + i * Para_List.Parameter.Rtc_Distortion_Data_Interval, Gts_Y + j * Para_List.Parameter.Rtc_Distortion_Data_Interval));
                }
            }
            //振镜打标数据
            for (int i = -No / 2; i <= No / 2; i++)//Y坐标
            {
                for (int j = -No / 2; j <= No / 2; j++)//X坐标
                {
                    Rtc_Point.Add(new Vector(j * Para_List.Parameter.Rtc_Distortion_Data_Interval, -i * Para_List.Parameter.Rtc_Distortion_Data_Interval));
                }
            }
            //进行数据采集
            if (Rtc_Point.Count == Aquisition_Point.Count)
            {
                for (int i = 0; i < Aquisition_Point.Count; i++)
                {
                    Tem_Mark = new Vector(Aquisition_Point[i].X + Para_List.Parameter.Rtc_Org.X, Aquisition_Point[i].Y + Para_List.Parameter.Rtc_Org.Y);

                    if (Para_List.Parameter.Rtc_Get_Data_Align == 1)
                    {
                        //对齐中心
                        do
                        {
                            GTS_Fun.Interpolation.Gts_Ready_Test(Tem_Mark.X, Tem_Mark.Y);
                            //调用相机，获取对比的坐标信息
                            Thread.Sleep(500);
                            //相机反馈的当前坐标
                            Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Coordinate_Correct(1));//触发拍照 
                            if (Cam.Length == 0)
                            {
                                MessageBox.Show("相机坐标提取失败，请检查！！！");
                                CSV_RW.SaveCSV(Temp_Acquisition, "Rtc_Data_Aquisition_Temp_Fail");//原始数据保存
                                CSV_RW.SaveCSV(Calibration_Data_Acquisition, "Rtc_Data_Aquisition_Fail");//原始数据保存
                                return Aquisition_Point;
                            }
                            Coordinate = GTS_Fun.Interpolation.Get_Coordinate(1);
                            Tem_Mark = new Vector(Coordinate - Cam);//获取实际位置
                        } while (!Differ_Deviation(Cam, Para_List.Parameter.Pos_Tolerance));
                    }
                    else
                    {
                        //实际测量
                        GTS_Fun.Interpolation.Gts_Ready_Test(Tem_Mark.X, Tem_Mark.Y);
                        //调用相机，获取对比的坐标信息
                        Thread.Sleep(500);
                        //相机反馈的当前坐标
                        Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Coordinate_Correct(1));//触发拍照 
                        if (Cam.Length == 0)
                        {
                            MessageBox.Show("相机坐标提取失败，请检查！！！");
                            CSV_RW.SaveCSV(Temp_Acquisition, "Rtc_Data_Aquisition_Temp_Fail");//原始数据保存
                            CSV_RW.SaveCSV(Calibration_Data_Acquisition, "Rtc_Data_Aquisition_Fail");//原始数据保存
                            return Aquisition_Point;
                        }
                        Coordinate = GTS_Fun.Interpolation.Get_Coordinate(1);
                    }  
                    //添加数据
                    Temp_Acquisition.Rows.Add(new object[] { Rtc_Point[i].X, Rtc_Point[i].Y, Tem_Mark.Y, Tem_Mark.X });
                }
            }
            else
            {
                MessageBox.Show("打标数据不匹配！！！");
                return Aquisition_Point;
            }
            Tem_Datum =new Vector(Convert.ToDecimal(Temp_Acquisition.Rows[((No+1) * (No + 1) - 1) / 2][2].ToString()), Convert.ToDecimal(Temp_Acquisition.Rows[((No + 1) * (No + 1) - 1) / 2][3].ToString()));
            //数据处理
            for (int i =0;i< Temp_Acquisition.Rows.Count; i++)
            {
                decimal t1, t2, t3, t4;
                t1 = Convert.ToDecimal(Temp_Acquisition.Rows[i][0].ToString());
                t2 = Convert.ToDecimal(Temp_Acquisition.Rows[i][1].ToString());
                t3 = Convert.ToDecimal(Temp_Acquisition.Rows[i][2].ToString());
                t4 = Convert.ToDecimal(Temp_Acquisition.Rows[i][3].ToString());
                Calibration_Data_Acquisition.Rows.Add(new object[] { t1, t2, -(t3 - Tem_Datum.X), (t4 - Tem_Datum.Y) });
            }
            CSV_RW.SaveCSV(Temp_Acquisition, "Rtc_Data_Aquisition_Temp");//原始数据保存
            CSV_RW.SaveCSV(Calibration_Data_Acquisition, "Rtc_Data_Aquisition");//原始数据保存
            return Aquisition_Point;
        }
        /// <summary>
        /// 生成振镜矫正数据圆，或直线
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Radius"></param>
        /// <param name="Interval"></param>
        /// <param name="Limit"></param>
        /// <returns></returns>
        public static List<List<Interpolation_Data>> Generate_Rtc_Correct_Data(ushort Type,decimal Radius, decimal Interval,decimal Limit)
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
            List<Interpolation_Data> Temp_Interpolation_List_Data = new List<Interpolation_Data>();//二级层
            Interpolation_Data Temp_Data = new Interpolation_Data();//一级层  
            decimal Gts_X = Para_List.Parameter.Base_Gts.X, Gts_Y = Para_List.Parameter.Base_Gts.Y;//X、Y坐标
            //初始清除
            Result.Clear();
            Temp_Interpolation_List_Data.Clear();
            Temp_Data.Empty();
            //decimal Radius = 1.0m;//半径
            //decimal Interval = 5.0m;//间距 
            //计算总间距
            Int16 No = (Int16)(Limit / Interval);

            //Gts 直线插补
            Temp_Data.Type = 1;
            //强制抬刀标志：1
            Temp_Data.Lift_flag = 1;
            //强制加工类型为Gts
            Temp_Data.Work = 10;
            //直线终点坐标
            Temp_Data.End_x = Gts_X;
            Temp_Data.End_y = Gts_Y;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //网格数据
            if (Type == 1)
            {
                //行数据 起点X不变，Y步进
                for (int i = -No / 2; i <= No / 2; i++)
                {
                    //数据清空
                    Temp_Data.Empty();
                    //强制加工类型为RTC
                    Temp_Data.Work = 20;
                    //RTC mark_abs直线
                    Temp_Data.Type = 15;
                    //RTC加工，GTS平台配合坐标
                    //GTS平台配合坐标
                    Temp_Data.Gts_x = Gts_X;
                    Temp_Data.Gts_y = Gts_Y;
                    //Rtc定位 激光加工起点坐标
                    Temp_Data.Rtc_x = -Limit / 2M;
                    Temp_Data.Rtc_y = -Interval * i;
                    //坐标转换 将坐标转换为RTC坐标系坐标
                    Temp_Data.End_x = Limit / 2M;
                    Temp_Data.End_y = -Interval * i;
                    //追加修改的数据
                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                    Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                    Temp_Interpolation_List_Data.Clear();
                }
                //列数据 起点Y不变，X变化
                for (int i = -No / 2; i <= No / 2; i++)
                {                    
                    //数据清空
                    Temp_Data.Empty();
                    //强制加工类型为RTC
                    Temp_Data.Work = 20;
                    //RTC mark_abs直线
                    Temp_Data.Type = 15;
                    //RTC加工，GTS平台配合坐标
                    //GTS平台配合坐标
                    Temp_Data.Gts_x = Gts_X;
                    Temp_Data.Gts_y = Gts_Y;
                    //Rtc定位 激光加工起点坐标
                    Temp_Data.Rtc_x = Interval * i;
                    Temp_Data.Rtc_y = Limit / 2M;
                    //坐标转换 将坐标转换为RTC坐标系坐标
                    Temp_Data.End_x = Interval * i;
                    Temp_Data.End_y = -Limit / 2M;
                    //追加修改的数据
                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                    Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                    Temp_Interpolation_List_Data.Clear();
                }
            }
            else//其他整圆
            {
                for (int i = -No / 2; i <= No / 2; i++)//Y坐标
                {
                    for (int j = -No / 2; j <= No / 2; j++)//X坐标
                    {                        
                        //数据清空
                        Temp_Data.Empty();
                        //强制抬刀标志：0
                        Temp_Data.Lift_flag = 0;
                        //强制加工类型为RTC
                        Temp_Data.Work = 20;
                        //GTS平台配合坐标
                        Temp_Data.Gts_x = Gts_X;
                        Temp_Data.Gts_y = Gts_Y;
                        //Rtc定位 激光加工起点坐标
                        Temp_Data.Rtc_x = j * Interval + Radius;
                        Temp_Data.Rtc_y = -i * Interval;
                        //RTC arc_abs圆弧
                        Temp_Data.Type = 11;
                        //RTC 圆弧加工圆心坐标转换
                        Temp_Data.Center_x = j * Interval;
                        Temp_Data.Center_y = -i * Interval;
                        //圆弧角度
                        Temp_Data.Angle = 370;
                        //追加修改的数据
                        Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                        Temp_Interpolation_List_Data.Clear();
                    }
                }
            }
            //处理二次结果，合并走直线的Gts数据，下次为Rtc加工，则变动该GTS数据终点坐标为RTC加工的gts基准位置
            for (int cal = 0; cal < Result.Count; cal++)
            {
                //当前序号 数量为1、加工类型1 直线、加工方式10 GTS
                //当前+1序号 数量大于1、加工方式20 RTX
                if ((cal < Result.Count - 1) && (Result[cal].Count == 1) && (Result[cal][0].Type == 1) && (Result[cal][0].Work == 10) && (Result[cal + 1].Count >= 1) && (Result[cal + 1][0].Work == 20))
                {
                    Temp_Data.Empty();
                    Temp_Data = Result[cal][0];
                    Temp_Data.End_x = Result[cal + 1][0].Gts_x;
                    Temp_Data.End_y = Result[cal + 1][0].Gts_y;
                    //重新赋值
                    Result[cal][0] = new Interpolation_Data(Temp_Data);
                }
            }
            //返回结果
            return Result;
        }
        /// <summary>
        /// 振镜矩阵校准数据采集
        /// </summary>
        /// <returns></returns>
        public static List<Vector> Rtc_Correct_AFF_Coordinate() 
        {
            //生成相机采集数据点位
            List<Vector> Aquisition_Point = new List<Vector>();
            List<Vector> Rtc_Point = new List<Vector>();
            Vector Gts_Point = new Vector(Para_List.Parameter.Base_Gts);
            Vector Cam = new Vector();
            Vector Tem_Mark = new Vector();
            Vector Coordinate = new Vector();
            Vector Tem_Datum = new Vector();
            //数据采集
            DataTable Temp_Acquisition = new DataTable();
            Temp_Acquisition.Columns.Add("振镜坐标X", typeof(decimal));
            Temp_Acquisition.Columns.Add("振镜坐标Y", typeof(decimal));
            Temp_Acquisition.Columns.Add("实际坐标X", typeof(decimal));
            Temp_Acquisition.Columns.Add("实际坐标Y", typeof(decimal));
            DataTable Calibration_Data_Acquisition = new DataTable();
            Calibration_Data_Acquisition.Columns.Add("振镜坐标X", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("振镜坐标Y", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("实际坐标X", typeof(decimal));
            Calibration_Data_Acquisition.Columns.Add("实际坐标Y", typeof(decimal));
            //计算总间距
            if ((((Int16)(Para_List.Parameter.Rtc_Calibration_X_Len / Para_List.Parameter.Rtc_Calibration_Cell) % 2) != 0) || (((Int16)(Para_List.Parameter.Rtc_Calibration_Y_Len / Para_List.Parameter.Rtc_Calibration_Cell) % 2) != 0))
            {
                MessageBox.Show(string.Format("振镜矫正范围除以打标间距：X-{0}或Y-{1} 不为偶数，数据采集终止！！！", Para_List.Parameter.Rtc_Calibration_X_Len / Para_List.Parameter.Rtc_Calibration_Cell,Para_List.Parameter.Rtc_Calibration_Y_Len / Para_List.Parameter.Rtc_Calibration_Cell));
                return new List<Vector>();
            }
            Int16 No = (Int16)(Para_List.Parameter.Rtc_Calibration_X_Len / Para_List.Parameter.Rtc_Calibration_Cell);
            ///Affinity要求数据 顺序：左下角-->右下角 Y坐标不变，依次变更X坐标
            ///
            ///Gts匹配 顺序：左下角-->右下角 Y坐标不变，依次变更X坐标
            for (int i = -No / 2; i <= No / 2; i++)//Y坐标
            {
                for (int j = -No / 2; j <= No / 2; j++)//X坐标
                {
                    Aquisition_Point.Add(new Vector(j * Para_List.Parameter.Rtc_Calibration_Cell, i * Para_List.Parameter.Rtc_Calibration_Cell));
                }
            }
            //振镜打标数据
            for (int i = -No / 2; i <= No / 2; i++)//Y坐标
            {
                for (int j = -No / 2; j <= No / 2; j++)//X坐标
                {
                    Rtc_Point.Add(new Vector(j * Para_List.Parameter.Rtc_Calibration_Cell, i * Para_List.Parameter.Rtc_Calibration_Cell));
                }
            }
            //进行数据采集
            if (Rtc_Point.Count == Aquisition_Point.Count)
            {
                for (int i = 0; i < Aquisition_Point.Count; i++)
                {
                    Tem_Mark = new Vector(Aquisition_Point[i] + Para_List.Parameter.Rtc_Org + Gts_Point);
                    //定位到标定板数据实际点位i
                    Mark_Correct(Tem_Mark);
                    //调用相机，获取对比的坐标信息
                    Thread.Sleep(500);//延时200ms
                    Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation_Coordinate_Correct(1));//触发拍照 
                    if (Cam.Length == 0)
                    {
                        MessageBox.Show("相机坐标提取失败，请检查！！！");
                        CSV_RW.SaveCSV(Temp_Acquisition, "Rtc坐标系校准数据Temp_Fail");//原始数据保存
                        CSV_RW.SaveCSV(Calibration_Data_Acquisition, "Rtc坐标系校准数据_Fail");//原始数据保存
                        return Aquisition_Point;
                    }
                    Coordinate = GTS_Fun.Interpolation.Get_Coordinate(1);
                    Tem_Mark = new Vector(Coordinate + Cam);//计算偏移
                    Aquisition_Point[i] = Tem_Mark;
                    Temp_Acquisition.Rows.Add(new object[] { Tem_Mark.X, Tem_Mark.Y,Rtc_Point[i].X, Rtc_Point[i].Y});
                }
            }
            else
            {
                MessageBox.Show("打标数据不匹配！！！");
                return Aquisition_Point;
            }
            Tem_Datum = new Vector(Rtc_Point[((No + 1) * (No + 1) - 1) / 2] + Para_List.Parameter.Rtc_Org);//获取理论坐标原点
            //数据处理
            for (int i = 0; i < Temp_Acquisition.Rows.Count; i++)
            {
                decimal t1, t2, t3, t4;
                t1 = Convert.ToDecimal(Temp_Acquisition.Rows[i][0].ToString());
                t2 = Convert.ToDecimal(Temp_Acquisition.Rows[i][1].ToString());
                t3 = Convert.ToDecimal(Temp_Acquisition.Rows[i][2].ToString());
                t4 = Convert.ToDecimal(Temp_Acquisition.Rows[i][3].ToString());
                Calibration_Data_Acquisition.Rows.Add(new object[] { (t1 - Tem_Datum.X), (t2 - Tem_Datum.Y), t3, t4});
            }
            CSV_RW.SaveCSV(Temp_Acquisition, "Rtc坐标系校准数据Temp_OK");//原始数据保存
            CSV_RW.SaveCSV(Calibration_Data_Acquisition, "Rtc坐标系校准数据_OK");//原始数据保存
            Serialize_Data.Serialize_Correct_Data(CSV_RW.DataTable_Correct_Data(Calibration_Data_Acquisition), "Rtc坐标系校准数据_AFF.xml");//保存为Xml文件
            return Aquisition_Point;
        }
        /// <summary>
        /// 生成振镜矫正数据圆，或直线
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Radius"></param>
        /// <param name="Interval"></param>
        /// <param name="Limit"></param>
        /// <returns></returns>
        public static List<List<Interpolation_Data>> Generate_Rtc_AFF_Correct_Data(ushort Type, decimal Radius, decimal Interval, decimal Limit) 
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
            List<Interpolation_Data> Temp_Interpolation_List_Data = new List<Interpolation_Data>();//二级层
            Interpolation_Data Temp_Data = new Interpolation_Data();//一级层  
            decimal Gts_X = Para_List.Parameter.Base_Gts.X, Gts_Y = Para_List.Parameter.Base_Gts.Y;//X、Y坐标
            //初始清除
            Result.Clear();
            Temp_Interpolation_List_Data.Clear();
            Temp_Data.Empty();
            //计算总间距
            Int16 No = (Int16)(Limit / Interval);
            //Gts 直线插补
            Temp_Data.Type = 1;
            //强制抬刀标志：1
            Temp_Data.Lift_flag = 1;
            //强制加工类型为Gts
            Temp_Data.Work = 10;
            //直线终点坐标
            Temp_Data.End_x = Gts_X;
            Temp_Data.End_y = Gts_Y;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //网格数据
            if (Type == 1)
            {
                //行数据 起点Y不变，X步进
                for (int i = -No / 2; i <= No / 2; i++)
                {
                    //数据清空
                    Temp_Data.Empty();
                    //强制加工类型为RTC
                    Temp_Data.Work = 20;
                    //RTC mark_abs直线
                    Temp_Data.Type = 15;
                    //RTC加工，GTS平台配合坐标
                    //GTS平台配合坐标
                    Temp_Data.Gts_x = Gts_X;
                    Temp_Data.Gts_y = Gts_Y;
                    //Rtc定位 激光加工起点坐标
                    Temp_Data.Rtc_x = -Limit / 2M;
                    Temp_Data.Rtc_y = Interval * i;
                    //坐标转换 将坐标转换为RTC坐标系坐标
                    Temp_Data.End_x = Limit / 2M;
                    Temp_Data.End_y = Interval * i;
                    //追加修改的数据
                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                    Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                    Temp_Interpolation_List_Data.Clear();
                }
                //列数据 起点X不变，Y步进
                for (int i = -No / 2; i <= No / 2; i++)
                {
                    //数据清空
                    Temp_Data.Empty();
                    //强制加工类型为RTC
                    Temp_Data.Work = 20;
                    //RTC mark_abs直线
                    Temp_Data.Type = 15;
                    //RTC加工，GTS平台配合坐标
                    //GTS平台配合坐标
                    Temp_Data.Gts_x = Gts_X;
                    Temp_Data.Gts_y = Gts_Y;
                    //Rtc定位 激光加工起点坐标
                    Temp_Data.Rtc_x = Interval * i;
                    Temp_Data.Rtc_y = -Limit / 2M;
                    //坐标转换 将坐标转换为RTC坐标系坐标
                    Temp_Data.End_x = Interval * i;
                    Temp_Data.End_y = Limit / 2M;
                    //追加修改的数据
                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                    Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                    Temp_Interpolation_List_Data.Clear();
                }
            }
            else//其他整圆
            {
                for (int i = -No / 2; i <= No / 2; i++)//Y坐标
                {
                    for (int j = -No / 2; j <= No / 2; j++)//X坐标
                    {
                        //数据清空
                        Temp_Data.Empty();
                        //强制抬刀标志：0
                        Temp_Data.Lift_flag = 0;
                        //强制加工类型为RTC
                        Temp_Data.Work = 20;
                        //GTS平台配合坐标
                        Temp_Data.Gts_x = Gts_X;
                        Temp_Data.Gts_y = Gts_Y;
                        //Rtc定位 激光加工起点坐标
                        Temp_Data.Rtc_x = j * Interval + Radius;
                        Temp_Data.Rtc_y = i * Interval;
                        //RTC arc_abs圆弧
                        Temp_Data.Type = 11;
                        //RTC 圆弧加工圆心坐标转换
                        Temp_Data.Center_x = j * Interval;
                        Temp_Data.Center_y = i * Interval;
                        //圆弧角度
                        Temp_Data.Angle = 370;
                        //追加修改的数据
                        Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                        Temp_Interpolation_List_Data.Clear();
                    }
                }
            }
            //处理二次结果，合并走直线的Gts数据，下次为Rtc加工，则变动该GTS数据终点坐标为RTC加工的gts基准位置
            for (int cal = 0; cal < Result.Count; cal++)
            {
                //当前序号 数量为1、加工类型1 直线、加工方式10 GTS
                //当前+1序号 数量大于1、加工方式20 RTX
                if ((cal < Result.Count - 1) && (Result[cal].Count == 1) && (Result[cal][0].Type == 1) && (Result[cal][0].Work == 10) && (Result[cal + 1].Count >= 1) && (Result[cal + 1][0].Work == 20))
                {
                    Temp_Data.Empty();
                    Temp_Data = Result[cal][0];
                    Temp_Data.End_x = Result[cal + 1][0].Gts_x;
                    Temp_Data.End_y = Result[cal + 1][0].Gts_y;
                    //重新赋值
                    Result[cal][0] = new Interpolation_Data(Temp_Data);
                }
            }
            //返回结果
            return Result;
        }
    }
    //GTS校准数据处理 
    class Gts_Cal_Data_Resolve
    {        
        /// <summary>
        /// 处理相机与轴的数据，生成仿射变换数组，并保存进入文件
        /// </summary>
        /// <param name="correct_Datas"></param>
        /// <returns></returns>
        public static List<Affinity_Matrix> Resolve(List<Correct_Data> correct_Datas)
        {
            //建立变量
            List<Affinity_Matrix> Result = new List<Affinity_Matrix>();
            Affinity_Matrix Temp_Affinity_Matrix = new Affinity_Matrix();
            List<Double_Fit_Data> Line_Fit_Data_AM = new List<Double_Fit_Data>();
            Double_Fit_Data Temp_Fit_Data_AM = new Double_Fit_Data();
            List<Double_Fit_Data> Line_Fit_Data_MA = new List<Double_Fit_Data>();
            Double_Fit_Data Temp_Fit_Data_MA = new Double_Fit_Data();
            Int16 i, j;
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义仿射变换矩阵转换数组
            double[] temp_array;
            //拟合高阶次数
            short Line_Re = 4;
            //数据处理
            if (Para_List.Parameter.Gts_Calibration_Col * Para_List.Parameter.Gts_Calibration_Row == correct_Datas.Count)//矫正和差异数据完整
            {
                if (Para_List.Parameter.Gts_Affinity_Type == 1)//全部点对
                {
                    //定义点位数组 
                    PointF[] srcTri = new PointF[Para_List.Parameter.Gts_Calibration_Col * Para_List.Parameter.Gts_Calibration_Row];//标准数据
                    PointF[] dstTri = new PointF[Para_List.Parameter.Gts_Calibration_Col * Para_List.Parameter.Gts_Calibration_Row];//差异化数据
                    //所有点对
                    for (i = 0; i < correct_Datas.Count; i++)
                    {
                        srcTri[i] = new PointF((float)correct_Datas[i].Xo, (float)correct_Datas[i].Yo);
                        dstTri[i] = new PointF((float)correct_Datas[i].Xm, (float)correct_Datas[i].Ym);
                    }
                    mat = CvInvoke.EstimateRigidTransform(srcTri, dstTri, false);
                    //提取矩阵数据
                    temp_array = mat.GetDoubleArray();
                    //获取仿射变换参数
                    Temp_Affinity_Matrix = Array_To_Affinity(temp_array);
                    //追加进入仿射变换List
                    Result.Add(new Affinity_Matrix(Temp_Affinity_Matrix));
                    //清除变量
                    Temp_Affinity_Matrix.Empty();
                    //保存为文件
                    Serialize_Data.Serialize_Affinity_Matrix(Result, "Gts_Affinity_Matrix_All.xml");
                }
                else if (Para_List.Parameter.Gts_Affinity_Type == 2)//线性拟合
                {
                    //初始化数据 M_A
                    double[] src_x_MA = new double[Para_List.Parameter.Gts_Calibration_Col];
                    double[] dst_x_MA = new double[Para_List.Parameter.Gts_Calibration_Col];
                    Tuple<double, double> R_X_MA = new Tuple<double, double>(0, 0);
                    double[] src_y_MA = new double[Para_List.Parameter.Gts_Calibration_Row];
                    double[] dst_y_MA = new double[Para_List.Parameter.Gts_Calibration_Row];
                    Tuple<double, double> R_Y_MA = new Tuple<double, double>(0, 0);
                    //初始化数据 A_M
                    double[] src_x_AM = new double[Para_List.Parameter.Gts_Calibration_Col];
                    double[] dst_x_AM = new double[Para_List.Parameter.Gts_Calibration_Col];
                    Tuple<double, double> R_X_AM = new Tuple<double, double>(0, 0);
                    double[] src_y_AM = new double[Para_List.Parameter.Gts_Calibration_Row];
                    double[] dst_y_AM = new double[Para_List.Parameter.Gts_Calibration_Row];
                    Tuple<double, double> R_Y_AM = new Tuple<double, double>(0, 0);                    
                    //拟合数据
                    for (i = 0; i < Para_List.Parameter.Gts_Calibration_Col; i++)
                    {
                        for (j = 0; j < Para_List.Parameter.Gts_Calibration_Row; j++)
                        {
                            //提取X轴拟合数据 M_A
                            src_x_MA[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xm);
                            dst_x_MA[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xo);
                            //提取Y轴拟合数据
                            src_y_MA[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Ym);
                            dst_y_MA[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Yo);

                            //提取X轴拟合数据 A_M
                            src_x_AM[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xo);
                            dst_x_AM[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xm);
                            //提取Y轴拟合数据
                            src_y_AM[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Yo);
                            dst_y_AM[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Ym);
                        }
                        //高阶曲线拟合
                        if (Line_Re == 4)
                        {
                            double[] Res_x_MA = Fit.Polynomial(src_x_MA, dst_x_MA, Line_Re);
                            double[] Res_y_MA = Fit.Polynomial(src_y_MA, dst_y_MA, Line_Re);
                            double[] Res_x_AM = Fit.Polynomial(src_y_AM, dst_x_AM, Line_Re);
                            double[] Res_y_AM = Fit.Polynomial(src_x_AM, dst_y_AM, Line_Re);
                            //提取拟合直线数据 AM
                            Temp_Fit_Data_AM = new Double_Fit_Data
                            {
                                K_X4 = (decimal)Res_x_AM[4],
                                K_X3 = (decimal)Res_x_AM[3],
                                K_X2 = (decimal)Res_x_AM[2],
                                K_X1 = (decimal)Res_x_AM[1],
                                Delta_X = (decimal)Res_x_AM[0],
                                K_Y4 = (decimal)Res_y_AM[4],
                                K_Y3 = (decimal)Res_y_AM[3],
                                K_Y2 = (decimal)Res_y_AM[2],
                                K_Y1 = (decimal)Res_y_AM[1],
                                Delta_Y = (decimal)Res_y_AM[0]
                            };
                            //提取拟合直线数据 MA
                            Temp_Fit_Data_MA = new Double_Fit_Data
                            {
                                K_X4 = (decimal)Res_x_MA[4],
                                K_X3 = (decimal)Res_x_MA[3],
                                K_X2 = (decimal)Res_x_MA[2],
                                K_X1 = (decimal)Res_x_MA[1],
                                Delta_X = (decimal)Res_x_MA[0],
                                K_Y4 = (decimal)Res_y_MA[4],
                                K_Y3 = (decimal)Res_y_MA[3],
                                K_Y2 = (decimal)Res_y_MA[2],
                                K_Y1 = (decimal)Res_y_MA[1],
                                Delta_Y = (decimal)Res_y_MA[0]
                            };
                        }
                        else if (Line_Re == 3)
                        {
                            double[] Res_x_MA = Fit.Polynomial(src_x_MA, dst_x_MA, Line_Re);
                            double[] Res_y_MA = Fit.Polynomial(src_y_MA, dst_y_MA, Line_Re);
                            double[] Res_x_AM = Fit.Polynomial(src_y_AM, dst_x_AM, Line_Re);
                            double[] Res_y_AM = Fit.Polynomial(src_x_AM, dst_y_AM, Line_Re);
                            //提取拟合直线数据 AM
                            Temp_Fit_Data_AM = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = (decimal)Res_x_AM[3],
                                K_X2 = (decimal)Res_x_AM[2],
                                K_X1 = (decimal)Res_x_AM[1],
                                Delta_X = (decimal)Res_x_AM[0],
                                K_Y4 = (decimal)Res_y_AM[4],
                                K_Y3 = (decimal)Res_y_AM[3],
                                K_Y2 = (decimal)Res_y_AM[2],
                                K_Y1 = (decimal)Res_y_AM[1],
                                Delta_Y = (decimal)Res_y_AM[0]
                            };
                            //提取拟合直线数据 MA
                            Temp_Fit_Data_MA = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = (decimal)Res_x_MA[3],
                                K_X2 = (decimal)Res_x_MA[2],
                                K_X1 = (decimal)Res_x_MA[1],
                                Delta_X = (decimal)Res_x_MA[0],
                                K_Y4 = 0,
                                K_Y3 = (decimal)Res_y_MA[3],
                                K_Y2 = (decimal)Res_y_MA[2],
                                K_Y1 = (decimal)Res_y_MA[1],
                                Delta_Y = (decimal)Res_y_MA[0]
                            };
                        }
                        else if (Line_Re == 2)
                        {
                            double[] Res_x_MA = Fit.Polynomial(src_x_MA, dst_x_MA, Line_Re);
                            double[] Res_y_MA = Fit.Polynomial(src_y_MA, dst_y_MA, Line_Re);
                            double[] Res_x_AM = Fit.Polynomial(src_y_AM, dst_x_AM, Line_Re);
                            double[] Res_y_AM = Fit.Polynomial(src_x_AM, dst_y_AM, Line_Re);
                            //提取拟合直线数据 AM
                            Temp_Fit_Data_AM = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = 0,
                                K_X2 = (decimal)Res_x_AM[2],
                                K_X1 = (decimal)Res_x_AM[1],
                                Delta_X = (decimal)Res_x_AM[0],
                                K_Y4 = 0,
                                K_Y3 = 0,
                                K_Y2 = (decimal)Res_y_AM[2],
                                K_Y1 = (decimal)Res_y_AM[1],
                                Delta_Y = (decimal)Res_y_AM[0]
                            };
                            //提取拟合直线数据 MA
                            Temp_Fit_Data_MA = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = 0,
                                K_X2 = (decimal)Res_x_MA[2],
                                K_X1 = (decimal)Res_x_MA[1],
                                Delta_X = (decimal)Res_x_MA[0],
                                K_Y4 = 0,
                                K_Y3 = 0,
                                K_Y2 = (decimal)Res_y_MA[2],
                                K_Y1 = (decimal)Res_y_MA[1],
                                Delta_Y = (decimal)Res_y_MA[0]
                            };
                        }
                        else if (Line_Re == 1)
                        {
                            double[] Res_x_MA = Fit.Polynomial(src_x_MA, dst_x_MA, Line_Re);
                            double[] Res_y_MA = Fit.Polynomial(src_y_MA, dst_y_MA, Line_Re);
                            double[] Res_x_AM = Fit.Polynomial(src_y_AM, dst_x_AM, Line_Re);
                            double[] Res_y_AM = Fit.Polynomial(src_x_AM, dst_y_AM, Line_Re);
                            //提取拟合直线数据 AM
                            Temp_Fit_Data_AM = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = 0,
                                K_X2 = 0,
                                K_X1 = (decimal)Res_x_AM[1],
                                Delta_X = (decimal)Res_x_AM[0],
                                K_Y4 = 0,
                                K_Y3 = 0,
                                K_Y2 = 0,
                                K_Y1 = (decimal)Res_y_AM[1],
                                Delta_Y = (decimal)Res_y_AM[0]
                            };
                            //提取拟合直线数据 MA
                            Temp_Fit_Data_MA = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = 0,
                                K_X2 = 0,
                                K_X1 = (decimal)Res_x_MA[1],
                                Delta_X = (decimal)Res_x_MA[0],
                                K_Y4 = 0,
                                K_Y3 = 0,
                                K_Y2 = 0,
                                K_Y1 = (decimal)Res_y_MA[1],
                                Delta_Y = (decimal)Res_y_MA[0]
                            };
                        }
                        else
                        {
                            R_X_MA = Fit.Line(src_x_MA, dst_x_MA); 
                            R_Y_MA = Fit.Line(src_y_MA, dst_y_MA);
                            R_X_AM = Fit.Line(src_y_AM, dst_x_AM);
                            R_Y_AM = Fit.Line(src_x_AM, dst_y_AM);
                            //提取拟合直线数据
                            Temp_Fit_Data_AM = new Double_Fit_Data
                            {
                                K_X1 = (decimal)R_X_AM.Item2,
                                Delta_X = (decimal)R_X_AM.Item1,
                                K_Y1 = (decimal)R_Y_AM.Item2,
                                Delta_Y = (decimal)R_Y_AM.Item1
                            };
                            //提取拟合直线数据
                            Temp_Fit_Data_MA = new Double_Fit_Data
                            {
                                K_X1 = (decimal)R_X_MA.Item2,
                                Delta_X = (decimal)R_X_MA.Item1,
                                K_Y1 = (decimal)R_Y_MA.Item2,
                                Delta_Y = (decimal)R_Y_MA.Item1
                            };
                        }                        
                        //保存进入Line_Fit_Data
                        Line_Fit_Data_AM.Add(new Double_Fit_Data(Temp_Fit_Data_AM));
                        //清空数据
                        Temp_Fit_Data_AM.Empty();
                        //保存进入Line_Fit_Data
                        Line_Fit_Data_MA.Add(new Double_Fit_Data(Temp_Fit_Data_MA));
                        //清空数据
                        Temp_Fit_Data_MA.Empty();
                    }
                    //保存轴拟合数据
                    CSV_RW.SaveCSV(CSV_RW.Double_Fit_Data_DataTable(Line_Fit_Data_AM), "Gts_Line_Fit_Data_AM");
                    //保存轴拟合数据
                    CSV_RW.SaveCSV(CSV_RW.Double_Fit_Data_DataTable(Line_Fit_Data_MA), "Gts_Line_Fit_Data_MA");
                }
                else
                {
                    //定义点位数组 
                    PointF[] srcTri = new PointF[3];//标准数据
                    PointF[] dstTri = new PointF[3];//差异化数据
                    //数据处理
                    for (i = 0; i < Para_List.Parameter.Gts_Calibration_Col - 1; i++)
                    {
                        for (j = 0; j < Para_List.Parameter.Gts_Calibration_Row - 1; j++)
                        {

                            //标准数据  平台坐标
                            srcTri[0] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Yo));
                            srcTri[1] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Yo));
                            srcTri[2] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Yo));
                            //srcTri[3] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Ym));

                            //仿射数据  测量坐标
                            dstTri[0] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Ym));
                            dstTri[1] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Ym));
                            dstTri[2] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Ym));
                            //dstTri[3] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Yo));


                            //计算仿射变换矩阵
                            mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
                            //mat = CvInvoke.EstimateRigidTransform(srcTri, dstTri, false);
                            //提取矩阵数据
                            temp_array = mat.GetDoubleArray();
                            //获取仿射变换参数
                            Temp_Affinity_Matrix = Array_To_Affinity(temp_array);
                            //追加进入仿射变换List
                            Result.Add(new Affinity_Matrix(Temp_Affinity_Matrix));
                            //清除变量
                            Temp_Affinity_Matrix.Empty();
                        }
                    }
                    //保存为文件
                    Serialize_Data.Serialize_Affinity_Matrix(Result, "Gts_Affinity_Matrix_Three.xml");
                }   
            }
            return Result;

        }
        /// <summary>
        /// abstract affinity parameter from array
        /// </summary>
        /// <param name="temp_array"></param>
        /// <returns></returns>
        public static Affinity_Matrix Array_To_Affinity(double[] temp_array)
        {
            Affinity_Matrix Result = new Affinity_Matrix
            {
                //获取仿射变换参数
                Stretch_X = Convert.ToDecimal(temp_array[0]),
                Distortion_X = Convert.ToDecimal(temp_array[1]),
                Delta_X = Convert.ToDecimal(temp_array[2]),//x方向偏移
                Stretch_Y = Convert.ToDecimal(temp_array[4]),
                Distortion_Y = Convert.ToDecimal(temp_array[3]),
                Delta_Y = Convert.ToDecimal(temp_array[5])//y方向偏移
            };
            //返回结果
            return Result;
        }
        /// <summary>
        /// 定位坐标 X
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public static Int16 Seek_X_Pos(decimal Pos)
        {
            Int16 Result = 0;
            Result = (Int16)Math.Floor((Pos / Para_List.Parameter.Gts_Calibration_Cell) + 0.01m);
            if (Result >= Para_List.Parameter.Gts_Affinity_Row)
            {
                Result = (Int16)(Para_List.Parameter.Gts_Affinity_Row - 1);
            }
            else if (Result <= 0)
            {
                Result = 0;
            }
            return Result;
        }
        /// <summary>
        /// 定位坐标 Y
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public static Int16 Seek_Y_Pos(decimal Pos)
        {
            Int16 Result = 0;
            Result = (Int16)Math.Floor((Pos / Para_List.Parameter.Gts_Calibration_Cell) + 0.01m);
            if (Result >= Para_List.Parameter.Gts_Affinity_Col)
            {
                Result = (Int16)(Para_List.Parameter.Gts_Affinity_Col - 1);
            }
            else if (Result <= 0)
            {
                Result = 0;
            }
            return Result;
        }
        /// <summary>
        /// dxf 仿射变换 求DX，DY，Dct(sin \cos)
        /// </summary>
        /// <returns></returns>
        public static Affinity_Matrix Cal_Affinity()
        {
            //建立变量
            Affinity_Matrix Result = new Affinity_Matrix();
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义点位数组
            PointF[] srcTri = new PointF[3];//标准数据
            PointF[] dstTri = new PointF[3];//差异化数据 
            double[] temp_array;
            //数据提取
            //标准数据
            srcTri[0] = new PointF((float)(Para_List.Parameter.Mark_Dxf1.X), (float)(Para_List.Parameter.Mark_Dxf1.Y));
            srcTri[1] = new PointF((float)(Para_List.Parameter.Mark_Dxf2.X), (float)(Para_List.Parameter.Mark_Dxf2.Y));
            srcTri[2] = new PointF((float)(Para_List.Parameter.Mark_Dxf3.X), (float)(Para_List.Parameter.Mark_Dxf3.Y));
            //仿射数据
            dstTri[0] = new PointF((float)(Para_List.Parameter.Mark1.X), (float)(Para_List.Parameter.Mark1.Y));
            dstTri[1] = new PointF((float)(Para_List.Parameter.Mark2.X), (float)(Para_List.Parameter.Mark2.Y));
            dstTri[2] = new PointF((float)(Para_List.Parameter.Mark3.X), (float)(Para_List.Parameter.Mark3.Y));
            //计算仿射变换矩阵
            mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
            //提取矩阵数据
            temp_array = mat.GetDoubleArray();
            //获取仿射变换参数
            Result = Array_To_Affinity(temp_array);

            //保存为文件
            string sdatetime = DateTime.Now.ToString("D");
            string filePath = "";
            string delimiter = ",";
            string strHeader = "";
            filePath = @"./\Config/" + "Cal_Affinity.csv";
            strHeader += "Stretch_X,Distortion_X,Delat_X,Stretch_Y,Distortion_Y,Delta_Y";
            bool isExit = File.Exists(filePath);
            StreamWriter sw = new StreamWriter(filePath, true, Encoding.GetEncoding("gb2312"));
            if (!isExit)
            {
                sw.WriteLine(strHeader);
            }
            //output rows data
            string strRowValue = "";
            strRowValue += Result.Stretch_X + delimiter
                         + Result.Distortion_X + delimiter
                         + Result.Delta_X + delimiter
                         + Result.Stretch_Y + delimiter
                         + Result.Distortion_Y + delimiter
                         + Result.Delta_Y;
            sw.WriteLine(strRowValue);
            sw.Close();

            //追加进入仿射变换List
            return Result;
        }
        /// <summary>
        /// dxf 仿射变换 求DX，DY，Dct(sin \cos)
        /// </summary>
        /// <param name="indata"></param>
        /// <returns></returns>
        public static Affinity_Matrix Cal_Affinity(List<Vector> indata)
        {
            //建立变量
            Affinity_Matrix Result = new Affinity_Matrix();
            if (indata.Count>=3)
            {
                //定义仿射变换数组 
                Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
                ///定义点位数组
                PointF[] srcTri = new PointF[3];//标准数据
                PointF[] dstTri = new PointF[3];//差异化数据 
                double[] temp_array;
                //数据提取
                //标准数据
                srcTri[0] = new PointF((float)(Para_List.Parameter.Mark_Dxf1.X), (float)(Para_List.Parameter.Mark_Dxf1.Y));
                srcTri[1] = new PointF((float)(Para_List.Parameter.Mark_Dxf2.X), (float)(Para_List.Parameter.Mark_Dxf2.Y));
                srcTri[2] = new PointF((float)(Para_List.Parameter.Mark_Dxf3.X), (float)(Para_List.Parameter.Mark_Dxf3.Y));
                //仿射数据
                dstTri[0] = new PointF((float)(indata[0].X), (float)(indata[0].Y));
                dstTri[1] = new PointF((float)(indata[1].X), (float)(indata[1].Y));
                dstTri[2] = new PointF((float)(indata[2].X), (float)(indata[2].Y));
                //计算仿射变换矩阵
                mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
                //提取矩阵数据
                temp_array = mat.GetDoubleArray();
                //获取仿射变换参数
                Result = Array_To_Affinity(temp_array);

                //保存为文件
                string sdatetime = DateTime.Now.ToString("D");
                string filePath = "";
                string delimiter = ",";
                string strHeader = "";
                filePath = @"./\Config/" + "Cal_Affinity.csv";
                strHeader += "Stretch_X,Distortion_X,Delat_X,Stretch_Y,Distortion_Y,Delta_Y";
                bool isExit = File.Exists(filePath);
                StreamWriter sw = new StreamWriter(filePath, true, Encoding.GetEncoding("gb2312"));
                if (!isExit)
                {
                    sw.WriteLine(strHeader);
                }
                //output rows data
                string strRowValue = "";
                strRowValue += Result.Stretch_X + delimiter
                             + Result.Distortion_X + delimiter
                             + Result.Delta_X + delimiter
                             + Result.Stretch_Y + delimiter
                             + Result.Distortion_Y + delimiter
                             + Result.Delta_Y;
                sw.WriteLine(strRowValue);
                sw.Close();

                //追加进入仿射变换List
                return Result;
            }
            else
            {
                return Result;
            }
            
        }
        /// <summary>
        /// 获取线性补偿坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="line_fit_data"></param>
        /// <returns></returns>
        public static Vector Get_Line_Fit_Coordinate_AM(decimal x,decimal y,List<Double_Fit_Data> line_fit_data)  
        {
            Vector Result = new Vector();
            //临时定位变量
            Int16 m, n;
            decimal X_per, Y_per;
            decimal K_x1, K_x2, K_x3, K_x4, B_x;
            decimal K_y1, K_y2, K_y3, K_y4, B_y; 

            //获取落点
            m = Seek_X_Pos(y);
            n = Seek_Y_Pos(x);
            //计算比率
            X_per = Math.Abs(y - m * Para_List.Parameter.Gts_Calibration_Cell) / Para_List.Parameter.Gts_Calibration_Cell;
            Y_per = Math.Abs(x - n * Para_List.Parameter.Gts_Calibration_Cell) / Para_List.Parameter.Gts_Calibration_Cell;
            //计算拟合参数
            K_x1 = (line_fit_data[m + 1].K_X1 - line_fit_data[m].K_X1) * X_per + line_fit_data[m].K_X1;
            K_x2 = (line_fit_data[m + 1].K_X2 - line_fit_data[m].K_X2) * X_per + line_fit_data[m].K_X2;
            K_x3 = (line_fit_data[m + 1].K_X3 - line_fit_data[m].K_X3) * X_per + line_fit_data[m].K_X3;
            K_x4 = (line_fit_data[m + 1].K_X4 - line_fit_data[m].K_X4) * X_per + line_fit_data[m].K_X4;
            B_x = (line_fit_data[m + 1].Delta_X - line_fit_data[m].Delta_X) * X_per + line_fit_data[m].Delta_X;
            K_y1 = (line_fit_data[n + 1].K_Y1 - line_fit_data[n].K_Y1) * Y_per + line_fit_data[n].K_Y1;
            K_y2 = (line_fit_data[n + 1].K_Y2 - line_fit_data[n].K_Y2) * Y_per + line_fit_data[n].K_Y2;
            K_y3 = (line_fit_data[n + 1].K_Y3 - line_fit_data[n].K_Y3) * Y_per + line_fit_data[n].K_Y3;
            K_y4 = (line_fit_data[n + 1].K_Y4 - line_fit_data[n].K_Y4) * Y_per + line_fit_data[n].K_Y4;
            B_y = (line_fit_data[n + 1].Delta_Y - line_fit_data[n].Delta_Y) * Y_per + line_fit_data[n].Delta_Y;
            //计算结果
            Result = new Vector(K_x4 * x * x * x * x + K_x3 * x * x * x + K_x2 * x * x + K_x1 * x + B_x, K_y4 * y * y * y * y + K_y3 * y * y * y + K_y2 * y * y + K_y1 * y + B_y);
#if DEBUG
            string sdatetime = DateTime.Now.ToString("D");
            string delimiter = ",";
            string strHeader = "";
            //保存的位置和文件名称
            string File_Path = @"./\Config/" + "Gts_Correct_Line_Fit " + sdatetime + ".csv";
            strHeader += "原X坐标,原Y坐标,补偿后X坐标,补偿后Y坐标,补偿前后X差值,补偿前后Y差值,取值坐标X位置,取值坐标Y位置";
            bool isExit = File.Exists(File_Path);
            StreamWriter sw = new StreamWriter(File_Path, true, Encoding.GetEncoding("gb2312"));
            if (!isExit)
            {
                sw.WriteLine(strHeader);
            }
            //output rows data
            string strRowValue = "";
            strRowValue += x + delimiter
                            + y + delimiter
                            + Result.X+ delimiter
                            + Result.Y + delimiter
                            + (Result.X - x) + delimiter
                            + (Result.Y - y) + delimiter
                            + m + delimiter
                            + n + delimiter;
            sw.WriteLine(strRowValue);
            sw.Close();
#endif
            //返回实际坐标
            return Result;
        }
        /// <summary>
        /// 获取线性补偿坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="line_fit_data"></param>
        /// <returns></returns>
        public static Vector Get_Line_Fit_Coordinate_MA(decimal x, decimal y, List<Double_Fit_Data> line_fit_data)
        {
            Vector Result = new Vector();
            //临时定位变量
            Int16 m, n;
            decimal X_per, Y_per;
            decimal K_x1, K_x2, K_x3, K_x4, B_x;
            decimal K_y1, K_y2, K_y3, K_y4, B_y;

            //获取落点
            m = Seek_X_Pos(y);
            n = Seek_Y_Pos(x);
            //计算比率
            X_per = Math.Abs(y - m * Para_List.Parameter.Gts_Calibration_Cell) / Para_List.Parameter.Gts_Calibration_Cell;
            Y_per = Math.Abs(x - n * Para_List.Parameter.Gts_Calibration_Cell) / Para_List.Parameter.Gts_Calibration_Cell;
            //计算拟合参数
            K_x1 = (line_fit_data[m + 1].K_X1 - line_fit_data[m].K_X1) * X_per + line_fit_data[m].K_X1;
            K_x2 = (line_fit_data[m + 1].K_X2 - line_fit_data[m].K_X2) * X_per + line_fit_data[m].K_X2;
            K_x3 = (line_fit_data[m + 1].K_X3 - line_fit_data[m].K_X3) * X_per + line_fit_data[m].K_X3;
            K_x4 = (line_fit_data[m + 1].K_X4 - line_fit_data[m].K_X4) * X_per + line_fit_data[m].K_X4;
            B_x = (line_fit_data[m + 1].Delta_X - line_fit_data[m].Delta_X) * X_per + line_fit_data[m].Delta_X;
            K_y1 = (line_fit_data[n + 1].K_Y1 - line_fit_data[n].K_Y1) * Y_per + line_fit_data[n].K_Y1;
            K_y2 = (line_fit_data[n + 1].K_Y2 - line_fit_data[n].K_Y2) * Y_per + line_fit_data[n].K_Y2;
            K_y3 = (line_fit_data[n + 1].K_Y3 - line_fit_data[n].K_Y3) * Y_per + line_fit_data[n].K_Y3;
            K_y4 = (line_fit_data[n + 1].K_Y4 - line_fit_data[n].K_Y4) * Y_per + line_fit_data[n].K_Y4;
            B_y = (line_fit_data[n + 1].Delta_Y - line_fit_data[n].Delta_Y) * Y_per + line_fit_data[n].Delta_Y;
            //计算结果
            Result = new Vector(K_x4 * x * x * x * x + K_x3 * x * x * x + K_x2 * x * x + K_x1 * x + B_x, K_y4 * y * y * y * y + K_y3 * y * y * y + K_y2 * y * y + K_y1 * y + B_y);
#if DEBUG
            string sdatetime = DateTime.Now.ToString("D");
            string delimiter = ",";
            string strHeader = "";
            //保存的位置和文件名称
            string File_Path = @"./\Config/" + "Gts_Correct_Line_Fit " + sdatetime + ".csv";
            strHeader += "原X坐标,原Y坐标,补偿后X坐标,补偿后Y坐标,补偿前后X差值,补偿前后Y差值,取值坐标X位置,取值坐标Y位置";
            bool isExit = File.Exists(File_Path);
            StreamWriter sw = new StreamWriter(File_Path, true, Encoding.GetEncoding("gb2312"));
            if (!isExit)
            {
                sw.WriteLine(strHeader);
            }
            //output rows data
            string strRowValue = "";
            strRowValue += x + delimiter
                            + y + delimiter
                            + Result.X+ delimiter
                            + Result.Y + delimiter
                            + (Result.X - x) + delimiter
                            + (Result.Y - y) + delimiter
                            + m + delimiter
                            + n + delimiter;
            sw.WriteLine(strRowValue);
            sw.Close();
#endif
            //返回实际坐标
            return Result;
        }
        /// <summary>
        /// get point affinity point'
        /// </summary>
        /// <param name="src"></param>
        /// <param name="affinity_Matrices"></param>
        /// <returns></returns>
        public static Vector Get_Aff_After(Vector src,Affinity_Matrix affinity_Matrices)
        { 
            return new Vector(src.X * affinity_Matrices.Stretch_X + src.Y * affinity_Matrices.Distortion_X + affinity_Matrices.Delta_X,src.Y * affinity_Matrices.Stretch_Y + src.X * affinity_Matrices.Distortion_Y + affinity_Matrices.Delta_Y);
        }
        /// <summary>
        /// 获取Affinity补偿坐标
        /// </summary>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="affinity_Matrices"></param>
        /// <returns></returns>
        public static Vector Get_Affinity_Point(int type,decimal x, decimal y, List<Affinity_Matrix> affinity_Matrices)//0-A_M,1-M_A
        {
            Vector Result = new Vector();
            //临时定位变量
            Int16 m, n;
            //获取落点
            m = Seek_X_Pos(x);
            n = Seek_Y_Pos(y);



            if (type == 1) //Motor_Coordinate ---- Actual_Coordinate
            {

                //计算逆矩阵
                var matrix1 = new DenseMatrix(3, 3); 
                //矩阵赋值
                if (affinity_Matrices.Count > 1)
                {
                    matrix1[0, 0] = (Double)affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Stretch_X;
                    matrix1[0, 1] = (Double)affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Distortion_X;
                    matrix1[0, 2] = (Double)affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Delta_X;
                    matrix1[1, 0] = (Double)affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Distortion_Y;
                    matrix1[1, 1] = (Double)affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Stretch_Y;
                    matrix1[1, 2] = (Double)affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Delta_Y;
                    matrix1[2, 0] = 0;
                    matrix1[2, 1] = 0;
                    matrix1[2, 2] = 1;
                }
                else if ((affinity_Matrices.Count > 0) && (affinity_Matrices.Count == 1))
                {
                    matrix1[0, 0] = (Double)affinity_Matrices[0].Stretch_X;
                    matrix1[0, 1] = (Double)affinity_Matrices[0].Distortion_X;
                    matrix1[0, 2] = (Double)affinity_Matrices[0].Delta_X;
                    matrix1[1, 0] = (Double)affinity_Matrices[0].Distortion_Y;
                    matrix1[1, 1] = (Double)affinity_Matrices[0].Stretch_Y;
                    matrix1[1, 2] = (Double)affinity_Matrices[0].Delta_Y;
                    matrix1[2, 0] = 0;
                    matrix1[2, 1] = 0;
                    matrix1[2, 2] = 1;
                }
                //逆矩阵
                var matrix2 = matrix1.Inverse();
                Affinity_Matrix Tmp = new Affinity_Matrix((decimal)matrix2[0, 0], (decimal)matrix2[0, 1], (decimal)matrix2[0, 2], (decimal)matrix2[1, 1], (decimal)matrix2[1, 0], (decimal)matrix2[1, 2]);
                //终点计算
                Result = new Vector(x * Tmp.Stretch_X + y * Tmp.Distortion_X + Tmp.Delta_X, y * Tmp.Stretch_Y + x * Tmp.Distortion_Y + Tmp.Delta_Y);
            }
            else
            {
                //终点计算
                if (affinity_Matrices.Count > 1)
                {
                    Result = new Vector(x * affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Stretch_X + y * affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Distortion_X + affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Delta_X, y * affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Stretch_Y + x * affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Distortion_Y + affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Delta_Y);
                }
                else if ((affinity_Matrices.Count > 0) && (affinity_Matrices.Count == 1))
                {
                    Result = new Vector(x * affinity_Matrices[0].Stretch_X + y * affinity_Matrices[0].Distortion_X + affinity_Matrices[0].Delta_X, y * affinity_Matrices[0].Stretch_Y + x * affinity_Matrices[0].Distortion_Y + affinity_Matrices[0].Delta_Y);
                }
            }
            
#if DEBUG
            string sdatetime = DateTime.Now.ToString("D");
            string delimiter = ",";
            string strHeader = "";
            //保存的位置和文件名称
            string File_Path = @"./\Config/" + "Gts_Correct_Line_Fit " + sdatetime + ".csv";
            strHeader += "原X坐标,原Y坐标,补偿后X坐标,补偿后Y坐标,补偿前后X差值,补偿前后Y差值,取值坐标X位置,取值坐标Y位置";
            bool isExit = File.Exists(File_Path);
            StreamWriter sw = new StreamWriter(File_Path, true, Encoding.GetEncoding("gb2312"));
            if (!isExit)
            {
                sw.WriteLine(strHeader);
            }
            //output rows data
            string strRowValue = "";
            strRowValue += x + delimiter
                            + y + delimiter
                            + Result.X+ delimiter
                            + Result.Y + delimiter
                            + (Result.X - x) + delimiter
                            + (Result.Y - y) + delimiter
                            + m + delimiter
                            + n + delimiter;
            sw.WriteLine(strRowValue);
            sw.Close();
#endif
            //返回实际坐标
            return Result;
        }
    }
    //RTC校准数据处理
    class Rtc_Cal_Data_Resolve
    {
        /// <summary>
        /// 处理测量值与实际值 数据，生成仿射变换数组，并保存进入文件
        /// </summary>
        /// <param name="correct_Datas"></param>
        /// <returns></returns>
        public static List<Affinity_Matrix> Resolve(List<Correct_Data> correct_Datas)
        {
            //建立变量
            List<Affinity_Matrix> Result = new List<Affinity_Matrix>();
            Affinity_Matrix Temp_Affinity_Matrix = new Affinity_Matrix();
            List<Double_Fit_Data> Line_Fit_Data = new List<Double_Fit_Data>();
            Double_Fit_Data Temp_Fit_Data = new Double_Fit_Data();
            Int16 i, j;
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义仿射变换矩阵转换数组
            double[] temp_array;
            //拟合高阶次数
            short Line_Re = 1;
            //数据处理
            if (Para_List.Parameter.Rtc_Calibration_Col * Para_List.Parameter.Rtc_Calibration_Row == correct_Datas.Count)//矫正和差异数据完整
            {
                if (Para_List.Parameter.Rtc_Affinity_Type == 1)//全部点对
                {
                    //定义点位数组 
                    PointF[] srcTri = new PointF[Para_List.Parameter.Rtc_Calibration_Col * Para_List.Parameter.Rtc_Calibration_Row];//标准数据
                    PointF[] dstTri = new PointF[Para_List.Parameter.Rtc_Calibration_Col * Para_List.Parameter.Rtc_Calibration_Row];//差异化数据
                    //所有点对
                    for (i = 0; i < correct_Datas.Count; i++)
                    {
                        srcTri[i] = new PointF((float)correct_Datas[i].Xo, (float)correct_Datas[i].Yo);
                        dstTri[i] = new PointF((float)correct_Datas[i].Xm, (float)correct_Datas[i].Ym);
                    }
                    mat = CvInvoke.EstimateRigidTransform(srcTri, dstTri, false);
                    //提取矩阵数据
                    temp_array = mat.GetDoubleArray();
                    //获取仿射变换参数
                    Temp_Affinity_Matrix = Gts_Cal_Data_Resolve.Array_To_Affinity(temp_array);
                    //追加进入仿射变换List
                    Result.Add(new Affinity_Matrix(Temp_Affinity_Matrix));
                    //清除变量
                    Temp_Affinity_Matrix.Empty();
                    //保存为文件
                    Serialize_Data.Serialize_Affinity_Matrix(Result, "Rtc_Affinity_Matrix_All.xml");
                }
                else if (Para_List.Parameter.Rtc_Affinity_Type == 2)//线性拟合
                {
                    //初始化数据
                    double[] src_x = new double[Para_List.Parameter.Rtc_Calibration_Col];
                    double[] dst_x = new double[Para_List.Parameter.Rtc_Calibration_Col];
                    Tuple<double, double> R_X = new Tuple<double, double>(0, 0);
                    double[] src_y = new double[Para_List.Parameter.Rtc_Calibration_Row];
                    double[] dst_y = new double[Para_List.Parameter.Rtc_Calibration_Row];
                    Tuple<double, double> R_Y = new Tuple<double, double>(0, 0);
                    //拟合数据
                    for (i = 0; i < Para_List.Parameter.Rtc_Calibration_Col; i++)
                    {
                        for (j = 0; j < Para_List.Parameter.Rtc_Calibration_Row; j++)
                        {
                            //提取X轴拟合数据
                            src_x[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col].Xo);
                            dst_x[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col].Xm);
                            //提取Y轴拟合数据
                            src_y[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col].Yo);
                            dst_y[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col].Ym);                            
                        }
                        //高阶曲线拟合
                        if (Line_Re == 4)
                        {
                            double[] Res_x = Fit.Polynomial(src_x, dst_x, Line_Re);
                            double[] Res_y = Fit.Polynomial(src_y, dst_y, Line_Re);
                            //提取拟合直线数据
                            Temp_Fit_Data = new Double_Fit_Data
                            {
                                K_X4 = (decimal)Res_x[4],
                                K_X3 = (decimal)Res_x[3],
                                K_X2 = (decimal)Res_x[2],
                                K_X1 = (decimal)Res_x[1],
                                Delta_X = (decimal)Res_x[0],
                                K_Y4 = (decimal)Res_y[4],
                                K_Y3 = (decimal)Res_y[3],
                                K_Y2 = (decimal)Res_y[2],
                                K_Y1 = (decimal)Res_y[1],
                                Delta_Y = (decimal)Res_y[0]
                            };
                        }
                        else if (Line_Re == 3)
                        {
                            double[] Res_x = Fit.Polynomial(src_x, dst_x, Line_Re);
                            double[] Res_y = Fit.Polynomial(src_y, dst_y, Line_Re);
                            //提取拟合直线数据
                            Temp_Fit_Data = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = (decimal)Res_x[3],
                                K_X2 = (decimal)Res_x[2],
                                K_X1 = (decimal)Res_x[1],
                                Delta_X = (decimal)Res_x[0],
                                K_Y4 = 0,
                                K_Y3 = (decimal)Res_y[3],
                                K_Y2 = (decimal)Res_y[2],
                                K_Y1 = (decimal)Res_y[1],
                                Delta_Y = (decimal)Res_y[0]
                            };
                        }
                        else if (Line_Re == 2)
                        {
                            double[] Res_x = Fit.Polynomial(src_x, dst_x, Line_Re);
                            double[] Res_y = Fit.Polynomial(src_y, dst_y, Line_Re);
                            //提取拟合直线数据
                            Temp_Fit_Data = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = 0,
                                K_X2 = (decimal)Res_x[2],
                                K_X1 = (decimal)Res_x[1],
                                Delta_X = (decimal)Res_x[0],
                                K_Y4 = 0,
                                K_Y3 = 0,
                                K_Y2 = (decimal)Res_y[2],
                                K_Y1 = (decimal)Res_y[1],
                                Delta_Y = (decimal)Res_y[0]
                            };
                        }
                        else if (Line_Re == 1)
                        {
                            double[] Res_x = Fit.Polynomial(src_x, dst_x, Line_Re);
                            double[] Res_y = Fit.Polynomial(src_y, dst_y, Line_Re);
                            //提取拟合直线数据
                            Temp_Fit_Data = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = 0,
                                K_X2 = 0,
                                K_X1 = (decimal)Res_x[1],
                                Delta_X = (decimal)Res_x[0],
                                K_Y4 = 0,
                                K_Y3 = 0,
                                K_Y2 = 0,
                                K_Y1 = 0,
                                Delta_Y = (decimal)Res_y[0]
                            };
                        }
                        else//1阶线性拟合
                        {
                            R_X = Fit.Line(src_x, dst_x);
                            R_Y = Fit.Line(src_y, dst_y);
                            //提取拟合直线数据
                            Temp_Fit_Data = new Double_Fit_Data
                            {
                                K_X1 = (decimal)R_X.Item2,
                                Delta_X = (decimal)R_X.Item1,
                                K_Y1 = (decimal)R_Y.Item2,
                                Delta_Y = (decimal)R_Y.Item1
                            };
                        }
                        //保存进入Line_Fit_Data
                        Line_Fit_Data.Add(new Double_Fit_Data(Temp_Fit_Data));
                        //清空数据
                        Temp_Fit_Data.Empty();
                    }
                    //保存轴拟合数据
                    CSV_RW.SaveCSV(CSV_RW.Double_Fit_Data_DataTable(Line_Fit_Data), "Rtc_Line_Fit_Data");
                }
                else
                {
                    //定义点位数组 
                    PointF[] srcTri = new PointF[3];//标准数据
                    PointF[] dstTri = new PointF[3];//差异化数据
                    //数据处理
                    for (i = 0; i < Para_List.Parameter.Rtc_Calibration_Col - 1; i++)
                    {
                        for (j = 0; j < Para_List.Parameter.Rtc_Calibration_Row - 1; j++)
                        {
                            //标准数据  定位坐标
                            srcTri[0] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col].Yo));
                            srcTri[1] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row].Yo));
                            srcTri[2] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row + 1].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row + 1].Yo));//计算仿射变换矩阵

                            //仿射数据  测量坐标
                            dstTri[0] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col].Ym));
                            dstTri[1] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row].Ym));
                            dstTri[2] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row + 1].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row + 1].Ym));

                            //计算仿射变换矩阵
                            mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
                            //提取矩阵数据
                            temp_array = mat.GetDoubleArray();
                            //获取仿射变换参数
                            Temp_Affinity_Matrix = Gts_Cal_Data_Resolve.Array_To_Affinity(temp_array);
                            //追加进入仿射变换List
                            Result.Add(new Affinity_Matrix(Temp_Affinity_Matrix));
                            //清除变量
                            Temp_Affinity_Matrix.Empty();
                        }
                    }
                    //保存为文件
                    Serialize_Data.Serialize_Affinity_Matrix(Result, "Rtc_Affinity_Matrix_Three.xml");
                }
            }
            return Result;
        }
        /// <summary>
        /// 定位坐标 X
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public static Int16 Seek_X_Pos(decimal Pos)
        {
            Int16 Result = 0;
            Result = (Int16)Math.Floor(((Pos + Para_List.Parameter.Rtc_Calibration_X_Len / 2m) / Para_List.Parameter.Rtc_Calibration_Cell) + 0.01m);
            if (Result >= Para_List.Parameter.Rtc_Affinity_Row)
            {
                Result = (Int16)(Para_List.Parameter.Rtc_Affinity_Row - 1);
            }
            else if (Result <= 0)
            {
                Result = 0;
            }
            return Result;
        }
        /// <summary>
        /// 定位坐标 Y
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public static Int16 Seek_Y_Pos(decimal Pos)
        {
            Int16 Result = 0;
            Result = (Int16)Math.Floor(((Pos + Para_List.Parameter.Rtc_Calibration_Y_Len / 2m) / Para_List.Parameter.Rtc_Calibration_Cell) + 0.01m);
            if (Result >= Para_List.Parameter.Rtc_Affinity_Col)
            {
                Result = (Int16)(Para_List.Parameter.Rtc_Affinity_Col - 1);
            }
            else if (Result <= 0)
            {
                Result = 0;
            }
            return Result;
        }
        /// <summary>
        /// 获取线性补偿后的坐标值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="line_fit_data"></param>
        /// <returns></returns>
        public static Vector Get_Line_Fit_Coordinate(decimal x, decimal y, List<Double_Fit_Data> line_fit_data)
        {
            //临时定位变量
            Int16 m, n;
            decimal X_per, Y_per;
            decimal K_x1, K_x2, K_x3, K_x4, B_x;
            decimal K_y1, K_y2, K_y3, K_y4, B_y; 
            //获取落点
            m = Seek_X_Pos(y);
            n = Seek_Y_Pos(x);
            //计算比率
            X_per = Math.Abs(y - m * Para_List.Parameter.Rtc_Calibration_Cell) / Para_List.Parameter.Rtc_Calibration_Cell;
            Y_per = Math.Abs(x - m * Para_List.Parameter.Rtc_Calibration_Cell) / Para_List.Parameter.Rtc_Calibration_Cell;
            //计算实际 线性拟合数据
            K_x1 = (line_fit_data[m + 1].K_X1 - line_fit_data[m].K_X1) * X_per + line_fit_data[m].K_X1;
            K_x2 = (line_fit_data[m + 1].K_X2 - line_fit_data[m].K_X2) * X_per + line_fit_data[m].K_X2;
            K_x3 = (line_fit_data[m + 1].K_X3 - line_fit_data[m].K_X3) * X_per + line_fit_data[m].K_X3;
            K_x4 = (line_fit_data[m + 1].K_X4 - line_fit_data[m].K_X4) * X_per + line_fit_data[m].K_X4;
            B_x = (line_fit_data[m + 1].Delta_X - line_fit_data[m].Delta_X) * X_per + line_fit_data[m].Delta_X;
            K_y1 = (line_fit_data[m + 1].K_Y1 - line_fit_data[m].K_Y1) * Y_per + line_fit_data[m].K_Y1;
            K_y2 = (line_fit_data[m + 1].K_Y2 - line_fit_data[m].K_Y2) * Y_per + line_fit_data[m].K_Y2;
            K_y3 = (line_fit_data[m + 1].K_Y3 - line_fit_data[m].K_Y3) * Y_per + line_fit_data[m].K_Y3;
            K_y4 = (line_fit_data[m + 1].K_Y4 - line_fit_data[m].K_Y4) * Y_per + line_fit_data[m].K_Y4;
            B_y = (line_fit_data[m + 1].Delta_Y - line_fit_data[m].Delta_Y) * Y_per + line_fit_data[m].Delta_Y;
            //返回实际坐标
            return new Vector(K_x4 * x * x * x * x + K_x3 * x * x * x + K_x2 * x * x + K_x1 * x + B_x, K_y4 * y * y * y * y + K_y3 * y * y * y + K_y2 * y * y + K_y1 * y + B_y);
        }
        /// <summary>
        /// 获取Affinity补偿坐标
        /// </summary>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="affinity_Matrices"></param>
        /// <returns></returns>
        public static Vector Get_Affinity_Point(int type, decimal x, decimal y, List<Affinity_Matrix> affinity_Matrices)//0-A_M,1-M_A
        {
            Vector Result = new Vector();
            //临时定位变量
            Int16 m, n;
            //获取落点
            m = Seek_X_Pos(x);
            n = Seek_Y_Pos(y);
            //坐标转换
            if (type == 1) //Motor_Coordinate ---- Actual_Coordinate
            {
                //计算逆矩阵
                var matrix1 = new DenseMatrix(3, 3);
                //矩阵赋值
                if (affinity_Matrices.Count > 1)
                {
                    matrix1[0, 0] = (Double)affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Stretch_X;
                    matrix1[0, 1] = (Double)affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Distortion_X;
                    matrix1[0, 2] = (Double)affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Delta_X;
                    matrix1[1, 0] = (Double)affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Distortion_Y;
                    matrix1[1, 1] = (Double)affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Stretch_Y;
                    matrix1[1, 2] = (Double)affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Delta_Y;
                    matrix1[2, 0] = 0;
                    matrix1[2, 1] = 0;
                    matrix1[2, 2] = 1;
                }
                else if ((affinity_Matrices.Count > 0) && (affinity_Matrices.Count == 1))
                {
                    matrix1[0, 0] = (Double)affinity_Matrices[0].Stretch_X;
                    matrix1[0, 1] = (Double)affinity_Matrices[0].Distortion_X;
                    matrix1[0, 2] = (Double)affinity_Matrices[0].Delta_X;
                    matrix1[1, 0] = (Double)affinity_Matrices[0].Distortion_Y;
                    matrix1[1, 1] = (Double)affinity_Matrices[0].Stretch_Y;
                    matrix1[1, 2] = (Double)affinity_Matrices[0].Delta_Y;
                    matrix1[2, 0] = 0;
                    matrix1[2, 1] = 0;
                    matrix1[2, 2] = 1;
                }
                //逆矩阵
                var matrix2 = matrix1.Inverse();
                Affinity_Matrix Tmp = new Affinity_Matrix((decimal)matrix2[0, 0], (decimal)matrix2[0, 1], (decimal)matrix2[0, 2], (decimal)matrix2[1, 1], (decimal)matrix2[1, 0], (decimal)matrix2[1, 2]);
                //终点计算
                Result = new Vector(x * Tmp.Stretch_X + y * Tmp.Distortion_X + Tmp.Delta_X, y * Tmp.Stretch_Y + x * Tmp.Distortion_Y + Tmp.Delta_Y);
            }
            else
            {
                //终点计算
                if (affinity_Matrices.Count > 1)
                {
                    Result = new Vector(x * affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Stretch_X + y * affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Distortion_X + affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Delta_X, y * affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Stretch_Y + x * affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Distortion_Y + affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Delta_Y);
                }
                else if ((affinity_Matrices.Count > 0) && (affinity_Matrices.Count == 1))
                {
                    Result = new Vector(x * affinity_Matrices[0].Stretch_X + y * affinity_Matrices[0].Distortion_X + affinity_Matrices[0].Delta_X, y * affinity_Matrices[0].Stretch_Y + x * affinity_Matrices[0].Distortion_Y + affinity_Matrices[0].Delta_Y);
                }
            }
            //返回实际坐标
            return Result;
        }
        /// <summary>
        /// 矫正振镜坐标系夹角 返回矫正后的坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector Correct_Rtc_Axes(decimal x, decimal y)
        {
            return new Vector(x * Para_List.Parameter.Rtc_Trans_Affinity.Stretch_X + y * Para_List.Parameter.Rtc_Trans_Affinity.Distortion_X + Para_List.Parameter.Rtc_Trans_Affinity.Delta_X, y * Para_List.Parameter.Rtc_Trans_Affinity.Stretch_Y + x * Para_List.Parameter.Rtc_Trans_Affinity.Distortion_Y + Para_List.Parameter.Rtc_Trans_Affinity.Delta_Y);
        }
        /// <summary>
        /// 矫正振镜坐标系夹角 返回矫正后的坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector Correct_Rtc_Axes_Angle(decimal x, decimal y)
        {
            return new Vector(x * Para_List.Parameter.Rtc_Trans_Angle.Stretch_X - y * Para_List.Parameter.Rtc_Trans_Angle.Distortion_X + Para_List.Parameter.Rtc_Trans_Angle.Delta_X, y * Para_List.Parameter.Rtc_Trans_Angle.Stretch_Y + x * Para_List.Parameter.Rtc_Trans_Angle.Distortion_Y + Para_List.Parameter.Rtc_Trans_Angle.Delta_Y);
        }
        
    }
    class Laser_Watt_Cal
    {
        /// <summary>
        /// 矫正数据采集
        /// </summary>
        public static void Acquisition_Data()
        {
            //数据变量
            DataTable Laser_Watt_Percent_Data = new DataTable();
            Laser_Watt_Percent_Data.Columns.Add("激光输出百分比", typeof(decimal));
            Laser_Watt_Percent_Data.Columns.Add("激光实际功率", typeof(decimal));
            if ((Initial.Laser_Control_Com.ComDevice.IsOpen) && (Initial.Laser_Watt_Com.ComDevice.IsOpen))
            {
                for (int i = 0; i <= 100; i++)
                {
                    Initial.Laser_Operation_00.Change_Pec(i);
                    Thread.Sleep(500);
                    RTC_Fun.Motion.Open_Laser();
                    Thread.Sleep(1000);//读数稳定时间
                    Laser_Watt_Percent_Data.Rows.Add(new object[] { i, Initial.Laser_Watt_00.Current_Watt});
                    RTC_Fun.Motion.Close_Laser();
                    Thread.Sleep(5000 * i );//冷却时间
                }
            }
            else
            {
                RTC_Fun.Motion.Close_Laser();
                MessageBox.Show("激光控制器或激光功率计串口未打开！！！");
                return;
            }
            CSV_RW.SaveCSV(Laser_Watt_Percent_Data, "Laser_Watt_Percent_Data");
        }
        /// <summary>
        /// 生成激光 百分比 与 功率的对应关系
        /// </summary>
        /// <param name="New_Data"></param>
        /// <returns></returns>
        public static List<Double_Fit_Data> Resolve(DataTable New_Data)
        {

            //建立变量
            List<Double_Fit_Data> Result = new List<Double_Fit_Data>();
            Double_Fit_Data Temp_Fit_Data = new Double_Fit_Data();
            Int16 i;
            //拟合高阶次数
            short Line_Re = 4;
            //初始化数据
            double[] src = new double[New_Data.Rows.Count];
            double[] dst = new double[New_Data.Rows.Count];
            //数据处理
            for (i = 0; i < New_Data.Rows.Count; i++)
            {                
                if ((decimal.TryParse(New_Data.Rows[i][0].ToString(), out decimal X )) && (decimal.TryParse(New_Data.Rows[i][1].ToString(), out decimal Y)))
                {
                    src[i] = (float)X;
                    dst[i] = (float)Y;
                }                
            }
            //高阶曲线拟合
            if (Line_Re == 4)
            {
                double[] Res_00 = Fit.Polynomial(src, dst, Line_Re);
                double[] Res_01 = Fit.Polynomial(dst, src, Line_Re);
                //提取拟合直线数据
                Temp_Fit_Data = new Double_Fit_Data
                {
                    K_X4 = (decimal)Res_00[4],
                    K_X3 = (decimal)Res_00[3],
                    K_X2 = (decimal)Res_00[2],
                    K_X1 = (decimal)Res_00[1],
                    Delta_X = (decimal)Res_00[0],
                    K_Y4 = (decimal)Res_01[4],
                    K_Y3 = (decimal)Res_01[3],
                    K_Y2 = (decimal)Res_01[2],
                    K_Y1 = (decimal)Res_01[1],
                    Delta_Y = (decimal)Res_01[0]
                };
            }
            else if (Line_Re == 3)
            {
                double[] Res_00 = Fit.Polynomial(src, dst, Line_Re);
                double[] Res_01 = Fit.Polynomial(dst, src, Line_Re);
                //提取拟合直线数据
                Temp_Fit_Data = new Double_Fit_Data
                {
                    K_X4 = 0,
                    K_X3 = (decimal)Res_00[3],
                    K_X2 = (decimal)Res_00[2],
                    K_X1 = (decimal)Res_00[1],
                    Delta_X = (decimal)Res_00[0],
                    K_Y4 = 0,
                    K_Y3 = (decimal)Res_01[3],
                    K_Y2 = (decimal)Res_01[2],
                    K_Y1 = (decimal)Res_01[1],
                    Delta_Y = (decimal)Res_01[0]
                };
            }
            else if (Line_Re == 2)
            {
                double[] Res_00 = Fit.Polynomial(src, dst, Line_Re);
                double[] Res_01 = Fit.Polynomial(dst, src, Line_Re);
                //提取拟合直线数据
                Temp_Fit_Data = new Double_Fit_Data
                {
                    K_X4 = 0,
                    K_X3 = 0,
                    K_X2 = (decimal)Res_00[2],
                    K_X1 = (decimal)Res_00[1],
                    Delta_X = (decimal)Res_00[0],
                    K_Y4 = 0,
                    K_Y3 = 0,
                    K_Y2 = (decimal)Res_01[2],
                    K_Y1 = (decimal)Res_01[1],
                    Delta_Y = (decimal)Res_01[0]
                };
            }
            else if (Line_Re == 1)
            {
                double[] Res_00 = Fit.Polynomial(src, dst, Line_Re);
                double[] Res_01 = Fit.Polynomial(dst, src, Line_Re);
                //提取拟合直线数据
                Temp_Fit_Data = new Double_Fit_Data
                {
                    K_X4 = 0,
                    K_X3 = 0,
                    K_X2 = 0,
                    K_X1 = (decimal)Res_00[1],
                    Delta_X = (decimal)Res_00[0],
                    K_Y4 = 0,
                    K_Y3 = 0,
                    K_Y2 = 0,
                    K_Y1 = 0,
                    Delta_Y = (decimal)Res_01[0]
                };
            }
            else//1阶线性拟合
            {

                Tuple<double, double> R_00 = new Tuple<double, double>(0, 0);
                Tuple<double, double> R_01 = new Tuple<double, double>(0, 0);
                R_00 = Fit.Line(src, dst);
                R_01 = Fit.Line(dst, src);
                //提取拟合直线数据
                Temp_Fit_Data = new Double_Fit_Data
                {
                    K_X1 = (decimal)R_00.Item2,
                    Delta_X = (decimal)R_00.Item1,
                    K_Y1 = (decimal)R_01.Item2,
                    Delta_Y = (decimal)R_01.Item1
                };
            }
            //结果追加
            Result.Add(new Double_Fit_Data(Temp_Fit_Data));
            //清空数据
            Temp_Fit_Data.Empty();
            //保存功率矫正拟合数据
            CSV_RW.SaveCSV(CSV_RW.Double_Fit_Data_DataTable(Result), "Laser_Watt_Percent_Relate");
            //返回结果
            return Result;
        }
        /// <summary>
        /// 功率 转换为 百分比
        /// </summary>
        /// <param name="watt"></param>
        /// <returns></returns>
        public static decimal Watt_To_Percent(decimal watt)
        {
            return Initial.Laser_Watt_Percent_Relate.K_X4 * watt * watt * watt * watt + Initial.Laser_Watt_Percent_Relate.K_X3 * watt * watt * watt + Initial.Laser_Watt_Percent_Relate.K_X2 * watt * watt + Initial.Laser_Watt_Percent_Relate.K_X1 * watt + Initial.Laser_Watt_Percent_Relate.Delta_X;
        }
        /// <summary>
        /// 百分比 转换为 功率
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal Percent_To_Watt(decimal percent)
        {
            return Initial.Laser_Watt_Percent_Relate.K_Y4 * percent * percent * percent * percent + Initial.Laser_Watt_Percent_Relate.K_Y3 * percent * percent * percent + Initial.Laser_Watt_Percent_Relate.K_Y2 * percent * percent + Initial.Laser_Watt_Percent_Relate.K_Y1 * percent + Initial.Laser_Watt_Percent_Relate.Delta_Y;
        }
        /// <summary>
        /// 功率 转换为 功率
        /// </summary>
        /// <param name="watt"></param>
        /// <returns></returns>
        public static decimal Watt_To_Watt(decimal watt) 
        {
            return Initial.Laser_Watt_Percent_Relate.K_X4 * watt * watt * watt * watt + Initial.Laser_Watt_Percent_Relate.K_X3 * watt * watt * watt + Initial.Laser_Watt_Percent_Relate.K_X2 * watt * watt + Initial.Laser_Watt_Percent_Relate.K_X1 * watt + Initial.Laser_Watt_Percent_Relate.Delta_X;
        }
    }      
    public class Serialize_Data 
    {
        /// <summary>
        /// List<Correct_Data> 数据序列化
        /// </summary>
        /// <param name="list"></param>
        /// <param name="txtFile"></param>
        public static void Serialize_Correct_Data(List<Correct_Data> list,string txtFile)
        {
            //写入文件
            string File_Path = @"./\Config/" + txtFile;
            using (FileStream fs = new FileStream(File_Path, FileMode.Create,FileAccess.ReadWrite))
            {
                //二进制 序列化
                //BinaryFormatter bf = new BinaryFormatter();
                //xml 序列化
                XmlSerializer bf = new XmlSerializer(typeof(List<Correct_Data>));
                bf.Serialize(fs, list);
            }
        }
        /// <summary>
        ///  List<Correct_Data> 数据反序列化
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<Correct_Data> Reserialize_Correct_Data (string fileName)
        {            

            //读取文件
            string File_Path = @"./\Config/" + fileName;
            using (FileStream fs = new FileStream(File_Path, FileMode.Open,FileAccess.Read))
            {
                //二进制 反序列化
                //BinaryFormatter bf = new BinaryFormatter();
                //xml 反序列化
                XmlSerializer bf = new XmlSerializer(typeof(List<Correct_Data>));
                List<Correct_Data> list = (List<Correct_Data>)bf.Deserialize(fs);
                return list;
            }
        }
        /// <summary>
        /// List<Affinity_Matrix> 数据序列化
        /// </summary>
        /// <param name="list"></param>
        /// <param name="txtFile"></param>
        public static void Serialize_Affinity_Matrix(List<Affinity_Matrix> list, string txtFile) 
        {
            //写入文件
            string File_Path = @"./\Config/" + txtFile;
            using (FileStream fs = new FileStream(File_Path, FileMode.Create, FileAccess.ReadWrite))
            {
                //保存参数至文件 二进制
                //BinaryFormatter bf = new BinaryFormatter();
                //保存为xml
                XmlSerializer bf = new XmlSerializer(typeof(List<Affinity_Matrix>));
                bf.Serialize(fs, list);
            }
        }

        /// <summary>
        /// List<Affinity_Matrix> 数据反序列化
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<Affinity_Matrix> Reserialize_Affinity_Matrix(string fileName)  
        {

            //读取文件
            string File_Path = @"./\Config/" + fileName;
            using (FileStream fs = new FileStream(File_Path, FileMode.Open, FileAccess.Read))
            {
                //二进制 反序列化
                //BinaryFormatter bf = new BinaryFormatter();
                //xml 反序列化
                XmlSerializer bf = new XmlSerializer(typeof(List<Affinity_Matrix>));
                List<Affinity_Matrix> list = (List<Affinity_Matrix>)bf.Deserialize(fs);
                return list;
            }
        }
    }
}
