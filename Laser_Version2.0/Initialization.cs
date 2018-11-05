using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GTS;
using RTC5Import;
using Laser_Version2._0;
using System.Windows.Forms;
using Laser_Build_1._0;
using Prompt;

namespace Initialization
{
    class Initial
    {
        //GTS初始化内容
        //定义GTS函数调用返回值
        short Com_Return;        
        //强制定义RS232端口
        public static RS232 Laser_Control_Com = new RS232(); //激光发生器 串口通讯
        public static Laser_Operation Laser_Operation_00 = new Laser_Operation(); //激光发生器 控制
        public static RS232 Laser_Watt_Com = new RS232(); //激光功率计 串口通讯
        public static Laser_Watt_Operation Laser_Watt_00 = new Laser_Watt_Operation();//激光功率计
        public static Double_Fit_Data Laser_Watt_Percent_Relate = new Double_Fit_Data();//激光功率与百分比对应关系
        //定义Tcp连接
        public static HPSocket_Communication T_Client = new HPSocket_Communication();
        public void Gts_Initial()
        {
            //打开运动控制器 
            Com_Return = MC.GT_Open(0, 0);
            Log.Commandhandler("Gts_Initial---GT_Open", Com_Return);
            //复位
            GTS_Fun.Factory.Reset();
            //Gts_Fun各功能初始化
            GTS_Fun.Axis_Home Gts_Fun_Axis_Home = new GTS_Fun.Axis_Home();
            GTS_Fun.Factory Gts_Fun_Factory = new GTS_Fun.Factory();
            GTS_Fun.Motion Gts_Fun_Motion = new GTS_Fun.Motion();
            GTS_Fun.Interpolation Gts_Fun_Interpolation = new GTS_Fun.Interpolation(); 

        }
        //激光器初始化内容
        public void Rtc_Initial()
        {
            //复位
            RTC_Fun.Factory.Reset();
            //Rtc_Fun各功能初始化
            RTC_Fun.Factory Rtc_Fun_Factory = new RTC_Fun.Factory();
            RTC_Fun.Motion Rtc_Fun_Motion = new RTC_Fun.Motion();
        }
        //公共初始化内容
        //文件目录指定  配置文件夹所在目录
        const string Dir = @"./\Config";//当前目录下的Config文件夹
        public void Common_Initial()
        {
            //建立配置文件存储目录
            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
            }
            //读取参数
            //配方数据读取
            Para_List.Serialize_Parameter.Reserialize("Para.xml");

        }
        //Rs232通讯初始化
        public void RS232_Initial() 
        {
            //激光控制器 232
            Laser_Control_Com.Receive_Event += new Receive_Delegate(Laser_Operation_00.Resolve_Com_Data);
            if (Para_List.Parameter.Laser_Control_Com_No < Laser_Control_Com.PortName.Count)
            {
                Laser_Control_Com.Open_Com(Para_List.Parameter.Laser_Control_Com_No);
            }
            else
            {
                MessageBox.Show("激光控制器通讯串口端口编号异常，请在激光控制面板选择正确的串口编号！！！");
            }
            //激光功率计 232
            Laser_Watt_Com.Receive_Event += new Receive_Delegate(Laser_Watt_00.Resolve_Com_Data);
            if (Para_List.Parameter.Laser_Watt_Com_No < Laser_Watt_Com.PortName.Count)
            {
                Laser_Watt_Com.Open_Com(Para_List.Parameter.Laser_Watt_Com_No, 3);
            }
            else
            {
                MessageBox.Show("激光功率计端口编号异常，请在激光功率计控制面板选择正确的串口编号！！！");
            }
            //加载功率 与 百分比校准文件
            Load_Watt_Percent_Relate();
        }
        public bool Load_Watt_Percent_Relate()
        {            
            string File_Name = "Laser_Watt_Percent_Relate.csv";
            string File_Path = @"./\Config/" + File_Name;
            if (File.Exists(File_Path))
            {
                //获取矫正数据
                if (CSV_RW.DataTable_Double_Fit_Data(CSV_RW.OpenCSV(File_Path)).Count >= 1)
                {
                    Laser_Watt_Percent_Relate = new Double_Fit_Data(CSV_RW.DataTable_Double_Fit_Data(CSV_RW.OpenCSV(File_Path))[0]);
                    Log.Info("Laser_Watt_Percent_Relate 矫正文件加载成功！！！");
                    return true;
                } 
                else
                {
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Laser_Watt_Percent_Relate 矫正文件不存在！！！，禁止加工，请检查！");
                Log.Info("Laser_Watt_Percent_Relate 矫正文件不存在！！！，禁止加工，请检查！");
                return false;
            }
            
        }
        //Tcp通讯初始化
        public void Tcp_Initial() 
        {
            T_Client.TCP_Start("127.0.0.1", Para_List.Parameter.Server_Port);
        }
        //laser 功率矫正初始化

    }


}