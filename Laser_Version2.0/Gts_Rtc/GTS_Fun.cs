using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using GTS;
using Laser_Build_1._0;
using Laser_Version2._0;
using Prompt;

namespace GTS_Fun
{

    //复位
    class Factory
    {
        //定义GTS函数调用返回值
        private static short Gts_Return;
        /// <summary>
        /// Gts控制卡初始化
        /// </summary>
        public static void Reset()
        {           
            //复位运动控制器
            Gts_Return = MC.GT_Reset();
            Log.Commandhandler("Gts_Initial---GT_Reset", Gts_Return);
            //延时
            Thread.Sleep(200);
            //配置运动控制器
            Gts_Return = MC.GT_LoadConfig("Axis.cfg");
            Log.Commandhandler("Gts_Initial--GT_LoadConfig", Gts_Return);
            //清除各轴的报警和限位
            Gts_Return = MC.GT_ClrSts(1, 4);
            Log.Commandhandler("Gts_Initial--清除各轴的报警和限位", Gts_Return);
            //轴使能
            Gts_Return = MC.GT_AxisOn(1);
            Gts_Return = MC.GT_AxisOn(2);
            //延时
            Thread.Sleep(200);

            //设置X轴误差带
            Gts_Return = MC.GT_SetAxisBand(1, Para_List.Parameter.Axis_X_Band, 4 * Para_List.Parameter.Axis_X_Time);//20-0.1um,4*2-250us
            Log.Commandhandler("X轴到位误差带", Gts_Return);

            //设置Y轴误差带
            Gts_Return = MC.GT_SetAxisBand(2, Para_List.Parameter.Axis_Y_Band, 4 * Para_List.Parameter.Axis_Y_Time);//20-0.1um,4*2-250us
            Log.Commandhandler("Y轴到位误差带", Gts_Return);
        }
        /// <summary>
        /// 关闭Gts控制卡
        /// </summary>
        public static void Free()
        {
            //关闭运动控制器
            Gts_Return = MC.GT_Close();
            Log.Commandhandler("Gts_Initial---GT_Close", Gts_Return);
        }
    }        
    //回原点
    class Axis_Home
    {   
        /// <summary>
        /// Gts工控卡 上位机Axes回零
        /// </summary>
        /// <param name="Axis"></param>
        /// <returns></returns>
        public static int Home_Upper_Monitor(short Axis)
        {
            //命令返回值
            short Gts_Return;
            short Capture;//捕获状态值
            MC.TTrapPrm Home_TrapPrm = new MC.TTrapPrm();
            int Axis_Sta;//轴状态
            uint Axis_Pclock;//轴时钟
            Int32 Axis_Pos;//回零是触发Home开关时的轴位置
            double prfPos;//回零运动过程中规划位置
            //double encPos;//回零运动过程中编码器位置
            //定义时钟
            uint pclock;

            //停止轴运动
            Gts_Return = MC.GT_Stop(1 << (Axis - 1), 0); //平滑停止轴运动
            Log.Commandhandler("Motion--停止轴运动", Gts_Return);

            //清除指定轴报警和限位
            Gts_Return = MC.GT_ClrSts(Axis, 1);
            Log.Commandhandler("Axis_Home----GT_ClrSts", Gts_Return);

            //回零准备，向正方向前进20mm，后触发回零
            //切换到点动模式
            Gts_Return = MC.GT_PrfTrap(Axis);
            Log.Commandhandler("Axis_Home----GT_PrfTrap", Gts_Return);

            //读取点动模式运动参数
            Gts_Return = MC.GT_GetTrapPrm(Axis, out Home_TrapPrm);
            Log.Commandhandler("Axis_Home----GT_GetTrapPrm", Gts_Return);

            //设置点动模式运动参数
            Home_TrapPrm.acc = Convert.ToDouble(Para_List.Parameter.Home_acc / Para_List.Parameter.Gts_Acc_reference);
            Home_TrapPrm.dec = Convert.ToDouble(Para_List.Parameter.Home_dcc / Para_List.Parameter.Gts_Acc_reference);
            Home_TrapPrm.smoothTime = Para_List.Parameter.Home_smoothTime;

            //设置点动模式运动参数
            Gts_Return = MC.GT_SetTrapPrm(Axis, ref Home_TrapPrm);
            Log.Commandhandler("Axis_Home----GT_SetTrapPrm", Gts_Return);

            //设置点动模式目标速度，即回原点速度
            Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(Para_List.Parameter.Home_High_Speed / Para_List.Parameter.Gts_Vel_reference));
            Log.Commandhandler("Axis_Home----GT_SetVel", Gts_Return);

            //读取当前规划位置
            Gts_Return = MC.GT_GetPrfPos(Axis, out prfPos, 1, out pclock);
            Log.Commandhandler("Motion--读取轴当前规划位置", Gts_Return);

            //设置点动模式目标位置，即原点准备距离 20mm
            Gts_Return = MC.GT_SetPos(Axis, Convert.ToInt32(Convert.ToDouble(20 * Para_List.Parameter.Gts_Pos_reference) + prfPos));
            Log.Commandhandler("Motion--设置目标位置", Gts_Return);

            //启动运动
            Gts_Return = MC.GT_Update(1 << (Axis - 1));
            Log.Commandhandler("Axis_Home----GT_Update", Gts_Return);

            do
            {
                //读取轴状态
                Gts_Return = MC.GT_GetSts(Axis, out Axis_Sta, 1, out Axis_Pclock);

            } while ((Axis_Sta & 0x400) != 0);

            //停止轴运动
            Gts_Return = MC.GT_Stop(1 << (Axis - 1), 0); //平滑停止轴运动
            Log.Commandhandler("Motion--停止轴运动", Gts_Return);

            //延时一段时间，等待电机稳定
            Thread.Sleep(200);//200ms

            //触发回零
            //启动Home捕捉
            Gts_Return = MC.GT_SetCaptureMode(Axis, MC.CAPTURE_HOME);
            Log.Commandhandler("Axis_Home----GT_SetCaptureMode", Gts_Return);

            //切换到点动模式
            Gts_Return = MC.GT_PrfTrap(Axis);
            Log.Commandhandler("Axis_Home----GT_PrfTrap", Gts_Return);

            //读取点动模式运动参数
            Gts_Return = MC.GT_GetTrapPrm(Axis, out Home_TrapPrm);
            Log.Commandhandler("Axis_Home----GT_GetTrapPrm", Gts_Return);

            //设置点动模式运动参数
            Home_TrapPrm.acc = Convert.ToDouble(Para_List.Parameter.Home_acc / Para_List.Parameter.Gts_Acc_reference);
            Home_TrapPrm.dec = Convert.ToDouble(Para_List.Parameter.Home_dcc / Para_List.Parameter.Gts_Acc_reference);
            Home_TrapPrm.smoothTime = Para_List.Parameter.Home_smoothTime;

            //设置点动模式运动参数
            Gts_Return = MC.GT_SetTrapPrm(Axis, ref Home_TrapPrm);
            Log.Commandhandler("Axis_Home----GT_SetTrapPrm", Gts_Return);

            //设置点动模式目标速度，即回原点速度
            Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(Para_List.Parameter.Home_High_Speed / Para_List.Parameter.Gts_Vel_reference));
            Log.Commandhandler("Axis_Home----GT_SetVel", Gts_Return);

            //设置点动模式目标位置，即原点搜索距离
            Gts_Return = MC.GT_SetPos(Axis, Convert.ToInt32(Para_List.Parameter.Search_Home * Para_List.Parameter.Gts_Pos_reference));
            Log.Commandhandler("Axis_Home----GT_SetPos", Gts_Return);

            //启动运动
            Gts_Return = MC.GT_Update(1 << (Axis - 1));
            Log.Commandhandler("Axis_Home----GT_Update", Gts_Return);


            do
            {
                //读取轴状态
                Gts_Return = MC.GT_GetSts(Axis, out Axis_Sta, 1, out Axis_Pclock);
                //读取捕获状态
                Gts_Return = MC.GT_GetCaptureStatus(Axis, out Capture, out Axis_Pos, 1, out Axis_Pclock);
                //读取编码器位置
                //Gts_Return = MC.GT_GetEncPos(Axis, out encPos, 1, out Axis_Pclock);
                //如果运动停止，返回出错信息
                if (0 == (Axis_Sta & 0x400))
                {
                    Log.Commandhandler("Axis_Home----No Home found!!", 1);
                    return 1;//整个过程Home信号一直没有触发，返回值为1                    
                }
            } while (Capture == 0);

            /********************************待评估***********************************/
            /*
            //清除捕捉状态
            //Gts_Return = MC.GT_ClearCaptureStatus(Axis);
            //Log.Commandhandler("Axis_Home----清除捕捉状态", Gts_Return);

            //设置捕捉Home 下降沿
            //Gts_Return = MC.GT_SetCaptureSense(Axis, MC.CAPTURE_HOME, 0);
            //Log.Commandhandler("Axis_Home----设置捕捉Home 下降沿", Gts_Return);

            //设定目标位置为捕获位置+偏移量
            Gts_Return = MC.GT_SetPos(Axis, Axis_Pos + Home_OffSet);
            Log.Commandhandler("Axis_Home----GT_SetPos", Gts_Return);

            //在运动状态下更新目标位置
            Gts_Return = MC.GT_Update(1 << (Axis - 1));
            Log.Commandhandler("Axis_Home----GT_Update", Gts_Return);              

            do
            {
                //读取轴状态
                Gts_Return = MC.GT_GetSts(Axis, out Axis_Sta, 1, out Axis_Pclock);
                //读取捕获状态
                Gts_Return = MC.GT_GetCaptureStatus(Axis, out Capture, out Axis_Pos, 1, out Axis_Pclock);
                //读取编码器位置
                //Gts_Return = MC.GT_GetEncPos(Axis, out encPos, 1, out Axis_Pclock);
                //如果运动停止，返回出错信息
                if (0 == (Axis_Sta & 0x400))
                {
                    Log.Commandhandler("Axis_Home----No Home found!!", 1);
                    //反转回零标志
                    Homing_Flag = !Homing_Flag;
                    return 1;//整个过程Home信号一直没有触发，返回值为1                    
                }
            } while (Capture ==0);
            */
            /********************************待评估***********************************/

            //停止轴运动
            Gts_Return = MC.GT_Stop(1 << (Axis - 1), 0); //平滑停止轴运动
            Log.Commandhandler("Motion--停止轴运动", Gts_Return);

            //延时一段时间，等待电机稳定
            Thread.Sleep(500);//200ms
            //位置清零            
            Gts_Return = MC.GT_ZeroPos(Axis, 1);
            Log.Commandhandler("Axis_Home----GT_ZeroPos", Gts_Return);

            /***************************Home_Offset偏置距离 开始********************************************/

            if (Para_List.Parameter.Home_OffSet !=0)
            {
                //切换到点动模式
                Gts_Return = MC.GT_PrfTrap(Axis);
                Log.Commandhandler("Axis_Home----GT_PrfTrap", Gts_Return);

                //读取点动模式运动参数
                Gts_Return = MC.GT_GetTrapPrm(Axis, out Home_TrapPrm);
                Log.Commandhandler("Axis_Home----GT_GetTrapPrm", Gts_Return);

                //设置点动模式运动参数
                Home_TrapPrm.acc = Convert.ToDouble(Para_List.Parameter.Home_acc / Para_List.Parameter.Gts_Acc_reference);
                Home_TrapPrm.dec = Convert.ToDouble(Para_List.Parameter.Home_dcc / Para_List.Parameter.Gts_Acc_reference);
                Home_TrapPrm.smoothTime = Para_List.Parameter.Home_smoothTime;

                //设置点动模式运动参数
                Gts_Return = MC.GT_SetTrapPrm(Axis, ref Home_TrapPrm);
                Log.Commandhandler("Axis_Home----GT_SetTrapPrm", Gts_Return);

                //设置点动模式目标速度，即回原点速度
                Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(Para_List.Parameter.Home_High_Speed / Para_List.Parameter.Gts_Vel_reference));
                Log.Commandhandler("Axis_Home----GT_SetVel", Gts_Return);

                //设置点动模式目标位置，即原点搜索距离
                Gts_Return = MC.GT_SetPos(Axis, Convert.ToInt32(Para_List.Parameter.Home_OffSet * Para_List.Parameter.Gts_Pos_reference));
                Log.Commandhandler("Axis_Home----GT_SetPos", Gts_Return);

                //启动运动
                Gts_Return = MC.GT_Update(1 << (Axis - 1));
                Log.Commandhandler("Axis_Home----GT_Update", Gts_Return);

                do
                {
                    //读取轴状态
                    Gts_Return = MC.GT_GetSts(Axis, out Axis_Sta, 1, out Axis_Pclock);
                    //读取规划位置
                    Gts_Return = MC.GT_GetPrfPos(Axis, out prfPos, 1, out Axis_Pclock);
                    //读取编码器位置
                    //Gts_Return = MC.GT_GetEncPos(Axis, out encPos, 1, out Axis_Pclock);

                } while ((Axis_Sta & 0x400) != 0);

                //检查是否到达 Home_OffSet
                if (prfPos != Convert.ToInt32(Para_List.Parameter.Home_OffSet * Para_List.Parameter.Gts_Pos_reference))
                {
                    Log.Commandhandler("Axis_Home----Move to Home_OffSet err!!", 1);
                    //置位Gts_Home标志
                    Refresh.Gts_Home_Flag = Refresh.Axis01_Limit_Up && Refresh.Axis01_Limit_Down && Refresh.Axis01_Alarm && Refresh.Axis01_MC_Err && Refresh.Axis01_IO_EMG && Refresh.Axis02_Limit_Up && Refresh.Axis02_Limit_Down && Refresh.Axis02_Alarm && Refresh.Axis02_MC_Err && Refresh.Axis02_IO_EMG && Refresh.EXI1;//任意（轴限位、报警、使能关闭、急停），致使原点标志丢失
                    return 2;
                }
                /***************************Home_Offset偏置距离 结束********************************************/
            }
            //延时一段时间，等待电机稳定
            Thread.Sleep(500);//200ms
            //位置清零            
            Gts_Return = MC.GT_ZeroPos(Axis, 1);
            Log.Commandhandler("Axis_Home----GT_ZeroPos", Gts_Return);
            return 0;
        }
        /// <summary>
        /// Gts工控卡 轴自身Axes回零
        /// </summary>
        /// <param name="Axis"></param>
        /// <returns></returns>
        public static int Axis01_Home_Down_Motor()
        {
            //命令返回值
            short Gts_Return = 0;

            //轴运行中，退出
            if (Prompt.Refresh.Axis01_Busy)
            {
                return 2;
            }

            //停止轴运动
            Gts_Return = MC.GT_Stop(1 << 0, 0); //平滑停止轴运动
            Log.Commandhandler("Motion--停止轴运动", Gts_Return);

            //清除指定轴报警和限位
            Gts_Return = MC.GT_ClrSts(1, 1);
            Log.Commandhandler("Axis_Home----GT_ClrSts", Gts_Return);

            //位置清零            
            Gts_Return = MC.GT_ZeroPos(1, 1);
            Log.Commandhandler("Axis_Home----GT_ZeroPos", Gts_Return);

            //触发回零外部信号
            if (Prompt.Refresh.Axis01_Home_Ex0_Control != 1) Prompt.Refresh.Axis01_Home_Ex0_Control = 1;

            //延时一段时间,等待信号生效
            Thread.Sleep(500); 

            //清除回零外部信号
            if (Prompt.Refresh.Axis01_Home_Ex0_Control != 0) Prompt.Refresh.Axis01_Home_Ex0_Control = 0;

            //捕获原点触发信号
            //等待完成
            Task.Factory.StartNew(() => {
                do
                {
                    //延时
                    Thread.Sleep(100);

                } while (!Prompt.Refresh.Axis01_Home);
            }).Wait(120 * 1000);//回零超时时长120s 2min
            if (!Prompt.Refresh.Axis01_Home)
            {
                MessageBox.Show("X轴回零超时！！！");
                return 1;
            }
            //清除回零外部信号
            if (Prompt.Refresh.Axis01_Home_Ex0_Control != 0) Prompt.Refresh.Axis01_Home_Ex0_Control = 0;

            //延时一段时间，等待电机稳定
            Thread.Sleep(500);//200ms

            //清除指定轴报警和限位
            Gts_Return = MC.GT_ClrSts(1, 1);
            Log.Commandhandler("Axis_Home----GT_ClrSts", Gts_Return);

            //位置清零            
            Gts_Return = MC.GT_ZeroPos(1, 1);
            Log.Commandhandler("Axis_Home----GT_ZeroPos", Gts_Return);

            return 0;
        }
        /// <summary>
        /// Gts工控卡 轴自身Axes回零
        /// </summary>
        /// <param name="Axis"></param>
        /// <returns></returns>
        public static int Axis02_Home_Down_Motor()
        {
            //命令返回值
            short Gts_Return = 0;

            //轴运行中，退出
            if (Prompt.Refresh.Axis02_Busy)
            {
                return 2;
            }

            //停止轴运动
            Gts_Return = MC.GT_Stop(1 << 1, 0); //平滑停止轴运动
            Log.Commandhandler("Motion--停止轴运动", Gts_Return);

            //清除指定轴报警和限位
            Gts_Return = MC.GT_ClrSts(2, 1);
            Log.Commandhandler("Axis_Home----GT_ClrSts", Gts_Return);

            //位置清零            
            Gts_Return = MC.GT_ZeroPos(2, 1);
            Log.Commandhandler("Axis_Home----GT_ZeroPos", Gts_Return);

            //触发回零外部信号
            if (Prompt.Refresh.Axis02_Home_Ex0_Control != 1) Prompt.Refresh.Axis02_Home_Ex0_Control = 1;

            //延时一段时间,等待信号生效
            Thread.Sleep(500);

            //清除回零外部信号
            if (Prompt.Refresh.Axis02_Home_Ex0_Control != 0) Prompt.Refresh.Axis02_Home_Ex0_Control = 0;

            //捕获原点触发信号
            //等待完成
            Task.Factory.StartNew(() => {
                do
                {
                    //延时
                    Thread.Sleep(100);
                } while (!Prompt.Refresh.Axis02_Home);
            }).Wait(120 * 1000);//回零超时时长120s 2min
            if (!Prompt.Refresh.Axis02_Home)
            {
                MessageBox.Show("Y轴回零超时！！！");
                return 1;
            }
            //清除回零外部信号
            if (Prompt.Refresh.Axis02_Home_Ex0_Control != 0) Prompt.Refresh.Axis02_Home_Ex0_Control = 0;

            //延时一段时间，等待电机稳定
            Thread.Sleep(500);//200ms

            //清除指定轴报警和限位
            Gts_Return = MC.GT_ClrSts(2, 1);
            Log.Commandhandler("Axis_Home----GT_ClrSts", Gts_Return);

            //位置清零            
            Gts_Return = MC.GT_ZeroPos(2, 1);
            Log.Commandhandler("Axis_Home----GT_ZeroPos", Gts_Return);

            return 0;
        }
    }


    class Motion
    {
        public static short Gts_Return;//指令返回变量 
        /// <summary>
        /// 绝对定位
        /// </summary>
        /// <param name="Axis"></param>
        /// <param name="acc"></param>
        /// <param name="dcc"></param>
        /// <param name="smoothTime"></param>
        /// <param name="pos"></param>
        /// <param name="vel"></param>
        public static void Abs(short Axis, decimal acc, decimal dcc, short smoothTime, decimal pos, decimal vel)
        {
            //定义点位运动参数变量
            MC.TTrapPrm trapPrm = new MC.TTrapPrm();
            //定义当前位置变量
            double prfpos;
            //定义时钟
            uint pclock;
            //定义轴状态
            int sts;
            //将轴设置为点位运动模式
            Gts_Return = MC.GT_PrfTrap(Axis);
            Log.Commandhandler("Motion--将轴设置为点位运动模式", Gts_Return);
            //读取点位运动运动参数
            Gts_Return = MC.GT_GetTrapPrm(Axis, out trapPrm);
            Log.Commandhandler("Motion--读取轴点位运动运动参数", Gts_Return);
            //设置要修改的参数
            trapPrm.acc = Convert.ToDouble(acc / Para_List.Parameter.Gts_Acc_reference);
            trapPrm.dec = Convert.ToDouble(dcc / Para_List.Parameter.Gts_Acc_reference);
            trapPrm.smoothTime = smoothTime;
            //设置点位运动参数
            Gts_Return = MC.GT_SetTrapPrm(Axis, ref trapPrm);
            Log.Commandhandler("Motion--读取轴设置点位运动参数", Gts_Return);

            //读取当前规划位置
            Gts_Return = MC.GT_GetPrfPos(Axis, out prfpos, 1, out pclock);
            Log.Commandhandler("Motion--读取轴当前规划位置", Gts_Return);

            //设置目标位置
            Gts_Return = MC.GT_SetPos(Axis, Convert.ToInt32(pos * Para_List.Parameter.Gts_Pos_reference));
            Log.Commandhandler("Motion--设置目标位置", Gts_Return);

            //设置目标速度
            Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(vel / Para_List.Parameter.Gts_Vel_reference));
            Log.Commandhandler("Motion--设置目标速度", Gts_Return);

            //启动轴运动
            Gts_Return = MC.GT_Update(1 << (Axis - 1));
            Log.Commandhandler("Motion--启动轴运动", Gts_Return);

            do
            {
                //读取轴状态
                Gts_Return = MC.GT_GetSts(Axis, out sts, 1, out pclock);
                Log.Commandhandler("Motion--读取轴状态", Gts_Return);
            } while ((sts & 0x400) != 0);//等待Axis规划停止
        }
        /// <summary>
        /// 相对定位
        /// </summary>
        /// <param name="Axis"></param>
        /// <param name="acc"></param>
        /// <param name="dcc"></param>
        /// <param name="smoothTime"></param>
        /// <param name="pos"></param>
        /// <param name="vel"></param>
        public static void Inc(short Axis, decimal acc, decimal dcc, short smoothTime, decimal pos, decimal vel)
        {
            //定义点位运动参数变量
            MC.TTrapPrm trapPrm = new MC.TTrapPrm();
            //定义当前位置变量
            double prfpos;
            //定义时钟
            uint pclock;
            //定义轴状态
            int sts;
            //将轴设置为点位运动模式
            Gts_Return = MC.GT_PrfTrap(Axis);
            Log.Commandhandler("Motion--将轴设置为点位运动模式", Gts_Return);
            //读取点位运动运动参数
            Gts_Return = MC.GT_GetTrapPrm(Axis, out trapPrm);
            Log.Commandhandler("Motion--读取轴点位运动运动参数", Gts_Return);
            //设置要修改的参数
            trapPrm.acc = Convert.ToDouble(acc / Para_List.Parameter.Gts_Acc_reference);
            trapPrm.dec = Convert.ToDouble(dcc / Para_List.Parameter.Gts_Acc_reference);
            trapPrm.smoothTime = smoothTime;
            //设置点位运动参数
            Gts_Return = MC.GT_SetTrapPrm(Axis, ref trapPrm);
            Log.Commandhandler("Motion--读取轴设置点位运动参数", Gts_Return);

            //读取当前规划位置
            Gts_Return = MC.GT_GetPrfPos(Axis, out prfpos, 1, out pclock);
            Log.Commandhandler("Motion--读取轴当前规划位置", Gts_Return);

            //设置目标位置
            Gts_Return = MC.GT_SetPos(Axis, Convert.ToInt32(Convert.ToDouble(pos * Para_List.Parameter.Gts_Pos_reference) + prfpos));
            Log.Commandhandler("Motion--设置目标位置", Gts_Return);

            //设置目标速度
            Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(vel / Para_List.Parameter.Gts_Vel_reference));
            Log.Commandhandler("Motion--设置目标速度", Gts_Return);

            //启动轴运动
            Gts_Return = MC.GT_Update(1 << (Axis - 1));
            Log.Commandhandler("Motion--启动轴运动", Gts_Return);

            do
            {
                //读取轴状态
                Gts_Return = MC.GT_GetSts(Axis, out sts, 1, out pclock);
                Log.Commandhandler("Motion--读取轴状态", Gts_Return);
            } while ((sts & 0x400) != 0);//等待Axis规划停止


        }
        /// <summary>
        /// Jog
        /// </summary>
        /// <param name="Axis"></param>
        /// <param name="dir"></param>
        /// <param name="JogVel"></param>
        /// <param name="JogAcc"></param>
        /// <param name="JogDcc"></param>
        public static void Jog(short Axis, short dir, decimal JogVel, decimal JogAcc, decimal JogDcc)
        {
            //定义Jog运动参数变量
            MC.TJogPrm prfJog = new MC.TJogPrm();
            //将轴设置为Jog模式
            Gts_Return = MC.GT_PrfJog(Axis);
            Log.Commandhandler("Motion--将轴设置为Jog模式", Gts_Return);
            //读取轴jog运动参数
            Gts_Return = MC.GT_GetJogPrm(Axis, out prfJog);
            Log.Commandhandler("Motion--读取轴jog运动参数", Gts_Return);

            //设置要修改的参数
            prfJog.acc = Convert.ToDouble(JogAcc / Para_List.Parameter.Gts_Acc_reference);//加速度
            prfJog.dec = Convert.ToDouble(JogDcc / Para_List.Parameter.Gts_Acc_reference);//减速度

            //设置jog运动参数
            Gts_Return = MC.GT_SetJogPrm(Axis, ref prfJog);
            Log.Commandhandler("Motion--设置jog运动参数", Gts_Return);

            //设置轴Jog运行速度
            if (dir == 0) //Jog+
            {
                Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(JogVel / Para_List.Parameter.Gts_Vel_reference));
                Log.Commandhandler("Motion--设置轴Jog运行速度", Gts_Return);
            }
            else    // Jog-
            {
                Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(-JogVel / Para_List.Parameter.Gts_Vel_reference));
                Log.Commandhandler("Motion--设置轴Jog运行速度", Gts_Return);
            }

            //启动轴运动
            Gts_Return = MC.GT_Update(1 << (Axis - 1));
            Log.Commandhandler("Motion--启动轴运动", Gts_Return);
        }
        /// <summary>
        /// 平滑停止轴运动
        /// </summary>
        /// <param name="Axis"></param>
        public static  void Smooth_Stop(short Axis)
        {
            //停止轴运动
            Gts_Return = MC.GT_Stop(1 << (Axis - 1), 0); //平滑停止轴运动
            Log.Commandhandler("Motion--停止轴运动", Gts_Return);
        }
        /// <summary>
        /// 紧急停止轴运动
        /// </summary>
        /// <param name="Axis"></param>
        public static  void Emg_Stop(short Axis)
        {
            //停止轴运动
            Gts_Return = MC.GT_Stop(1 << (Axis - 1), 1 << (Axis - 1)); //紧急停止轴运动
            Log.Commandhandler("Motion--停止轴运动", Gts_Return);
        }
    }

    class Interpolation
    {
        public static short Gts_Return;//指令返回变量 
        public static short run;//插补运行状态
        private static int segment;//插补剩余个数
        private static int Remain_Segment;//插补剩余个数 
        private static MC.TCrdData[] crdData = new MC.TCrdData[4096];
        static IntPtr Crd_IntPtr = new IntPtr();
        public static double[] Crd_Pos = new double[2];//坐标系位置
        public static List<Affinity_Matrix> affinity_Matrices = new List<Affinity_Matrix>();//校准数据集合
        public static List<Double_Fit_Data> Fit_Matrices_AM = new List<Double_Fit_Data>();//线性拟合校准数据集合 
        public static List<Double_Fit_Data> Fit_Matrices_MA = new List<Double_Fit_Data>();//线性拟合校准数据集合 
        /// <summary>
        /// 构造函数
        /// </summary>
        public Interpolation()
        {
            Load_Affinity_Matrix();
            
        }
        /// <summary>
        /// 加载矫正数组
        /// </summary>
        public static void Load_Affinity_Matrix()
        {
            string File_Name = "";
            string File_Name1 = "";
            string File_Name2 = "";
            if (Para_List.Parameter.Gts_Affinity_Type == 1)//点阵匹配
            {
                File_Name = "Gts_Affinity_Matrix_All.xml";
            }
            else if (Para_List.Parameter.Gts_Affinity_Type == 2)//直线拟合
            {
                File_Name1 = "Gts_Line_Fit_Data_AM.csv";
                File_Name2 = "Gts_Line_Fit_Data_MA.csv";
            }
            else
            {
                File_Name = "Gts_Affinity_Matrix_Three.xml";
            }
            //file path
            string File_Path = @"./\Config/" + File_Name;
            string File_Path1 = @"./\Config/" + File_Name1;
            string File_Path2 = @"./\Config/" + File_Name2;
            //read file
            if (Para_List.Parameter.Gts_Affinity_Type == 2)//直线拟合
            {
                if (File.Exists(File_Path1) && File.Exists(File_Path2))
                {
                    Fit_Matrices_AM = new List<Double_Fit_Data>(CSV_RW.DataTable_Double_Fit_Data(CSV_RW.OpenCSV(File_Path1)));
                    Fit_Matrices_MA = new List<Double_Fit_Data>(CSV_RW.DataTable_Double_Fit_Data(CSV_RW.OpenCSV(File_Path2)));
                    Log.Info("Gts AM 线性矫正文件加载成功！！！,数据数量：" + Fit_Matrices_AM.Count);
                    Log.Info("Gts MA 线性矫正文件加载成功！！！,数据数量：" + Fit_Matrices_AM.Count);
                }
                else
                {
                    Fit_Matrices_AM = new List<Double_Fit_Data>();
                    Fit_Matrices_MA = new List<Double_Fit_Data>();
                    MessageBox.Show("Gts AM/MA 线性矫正文件不存在，禁止加工，请检查！");
                    Log.Info("Gts AM/MA 线性矫正文件不存在，禁止加工，请检查！");
                }
            }
            else
            {
                if (File.Exists(File_Path))
                {
                    //获取矫正数据
                    if (Para_List.Parameter.Gts_Affinity_Type == 1)//点阵匹配
                    {
                        affinity_Matrices = new List<Affinity_Matrix>(Serialize_Data.Reserialize_Affinity_Matrix(File_Name));
                        Log.Info("Gts Affinity 矫正文件加载成功！！！,数据数量：" + affinity_Matrices.Count);
                    }
                    else
                    {
                        affinity_Matrices = new List<Affinity_Matrix>(Serialize_Data.Reserialize_Affinity_Matrix(File_Name));
                        Log.Info("Gts Affinity 矫正文件加载成功！！！,数据数量：" + affinity_Matrices.Count);
                    }
                }
                else
                {
                    affinity_Matrices = new List<Affinity_Matrix>();
                    MessageBox.Show("Gts Affinity 矫正文件文件不存在！！！，禁止加工，请检查！");
                    Log.Info("Gts Affinity 矫正文件文件不存在！！！，禁止加工，请检查！");
                }
            }            
        }
        /// <summary>
        /// 建立直角坐标系
        /// </summary>
        /// <param name="X_original"></param>
        /// <param name="Y_original"></param>
        public static void Coordination(decimal X_original, decimal Y_original)
        {
            //结构体变量，用于定义坐标系 
            //初始化结构体变量
            MC.TCrdPrm crdPrm = new MC.TCrdPrm
            {
                dimension = 2,                        // 建立三维的坐标系
                synVelMax = Convert.ToDouble(Para_List.Parameter.Syn_MaxVel / Para_List.Parameter.Gts_Vel_reference),                      // 坐标系的最大合成速度是: 500 pulse/ms   （0-32767）/ms
                synAccMax = Convert.ToDouble(Para_List.Parameter.Syn_MaxAcc / Para_List.Parameter.Gts_Acc_reference),                        // 坐标系的最大合成加速度是: 2 pulse/ms^2  （0-32767）/ms
                evenTime = Convert.ToInt16(Para_List.Parameter.Syn_EvenTime),                         // 坐标系的最小匀速时间为0
                profile1 = 1,                       // 规划器1对应到X轴                       
                profile2 = 2,                       // 规划器2对应到Y轴
                profile3 = 0,
                profile4 = 0,
                profile5 = 0,
                profile6 = 0,
                profile7 = 0,
                profile8 = 0,
                setOriginFlag = 1,                    // 需要设置加工坐标系原点位置
                originPos1 = Convert.ToInt32(X_original * Para_List.Parameter.Gts_Pos_reference),                     // 加工坐标系原点位置在(0,0)，即与机床坐标系原点重合
                originPos2 = Convert.ToInt32(Y_original * Para_List.Parameter.Gts_Pos_reference),
                originPos3 = 0,
                originPos4 = 0,
                originPos5 = 0,
                originPos6 = 0,
                originPos7 = 0,
                originPos8 = 0
            };

            //停止轴规划运动，停止坐标系运动
            Gts_Return = MC.GT_Stop(783, 0);//783--1-4轴全停止，坐标系1、2均停止；0-平滑停止运动，783-急停运动
            Log.Commandhandler("Establish_Coordinationg--GT_Stop", Gts_Return);

            //建立坐标系
            Gts_Return = MC.GT_SetCrdPrm(1, ref crdPrm);
            Log.Commandhandler("Establish_Coordinationg--GT_SetCrdPrm", Gts_Return);
        }
        /// <summary>
        /// 清空运动控制 FIFO
        /// </summary>
        public static void Clear_FIFO()
        {            
            
            //首先清除坐标系1、FIFO0中的数据
            Gts_Return = MC.GT_CrdClear(1, 0);
            Log.Commandhandler("Line_Interpolation--清除坐标系1、FIFO0中的数据", Gts_Return);
        }
        /// <summary>
        /// 直线插补 数据FIFO追加
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Line_FIFO(decimal x, decimal y)
        {
            //向缓存区写入一段插补数据.in
            Gts_Return = MC.GT_LnXY(
                1,//坐标系--1
                Convert.ToInt32(-x * Para_List.Parameter.Gts_Pos_reference),//插补X终点 [-1073741823,1073741823]
                Convert.ToInt32(-y * Para_List.Parameter.Gts_Pos_reference),//插补Y终点 [-1073741823,1073741823]
                Convert.ToDouble(Para_List.Parameter.Line_synVel / Para_List.Parameter.Gts_Vel_reference),//插补合成速度  [0-32767]
                Convert.ToDouble(Para_List.Parameter.Line_synAcc / Para_List.Parameter.Gts_Acc_reference),//插补合成加速度
                Convert.ToDouble(Para_List.Parameter.Line_endVel / Para_List.Parameter.Gts_Vel_reference),//插补终点速度
                0
                );
            Log.Commandhandler("Line_Interpolation--向缓存区写入一段直线插补数据", Gts_Return);
            
        }
        /// <summary>
        /// 整圆插补 数据FIFO追加
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Center_Start_x"></param>
        /// <param name="Center_Start_y"></param>
        /// <param name="dir"></param>
        public static void Circle_C_FIFO(decimal x, decimal y, decimal Center_Start_x, decimal Center_Start_y, short dir)
        {
            //向缓存区写入一段插补数据
            Gts_Return = MC.GT_ArcXYC(
                1,//坐标系--1
                Convert.ToInt32(-x * Para_List.Parameter.Gts_Pos_reference), Convert.ToInt32(-y * Para_List.Parameter.Gts_Pos_reference),//插补圆弧终点坐标 [-1073741823,1073741823]
                Convert.ToDouble(-Center_Start_x * Para_List.Parameter.Gts_Pos_reference), Convert.ToDouble(-Center_Start_y * Para_List.Parameter.Gts_Pos_reference),//插补圆弧圆心相对于 （刀具加工点）起点位置的偏移量
                dir,//圆弧方向0-顺时针，1-逆时针
                Convert.ToDouble(Para_List.Parameter.Circle_synVel / Para_List.Parameter.Gts_Vel_reference),//插补合成速度  [0-32767]
                Convert.ToDouble(Para_List.Parameter.Circle_synAcc / Para_List.Parameter.Gts_Acc_reference),//插补合成加速度
                Convert.ToDouble(Para_List.Parameter.Circle_endVel / Para_List.Parameter.Gts_Vel_reference),//插补终点速度
                0
                );
            Log.Commandhandler("Line_Interpolation--向缓存区写入一段圆心插补数据", Gts_Return);
        }
        /// <summary>
        /// 圆弧插补 不能用于描述整圆 数据FIFO追加
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="dir"></param>
        public static void Circle_R_FIFO(decimal x, decimal y, decimal radius, short dir)
        {
            //向缓存区写入一段插补数据
            Gts_Return = MC.GT_ArcXYR(
                1,//坐标系--1
                Convert.ToInt32(-x * Para_List.Parameter.Gts_Pos_reference), Convert.ToInt32(-y * Para_List.Parameter.Gts_Pos_reference),//插补圆弧终点坐标 [-1073741823,1073741823]
                Convert.ToDouble(radius * Para_List.Parameter.Gts_Pos_reference),//圆弧半径
                dir,//圆弧方向0-顺时针，1-逆时针
                Convert.ToDouble(Para_List.Parameter.Circle_synVel / Para_List.Parameter.Gts_Vel_reference),//插补合成速度  [0-32767]
                Convert.ToDouble(Para_List.Parameter.Circle_synAcc / Para_List.Parameter.Gts_Acc_reference),//插补合成加速度
                Convert.ToDouble(Para_List.Parameter.Circle_endVel / Para_List.Parameter.Gts_Vel_reference),//插补终点速度
                0
                );
            Log.Commandhandler("Line_Interpolation--向缓存区写入一段圆心插补数据", Gts_Return);
        }
        /// <summary>
        /// List<Interpolation_Data> 无前瞻和坐标矫正的  FIFO数据追加
        /// </summary>
        /// <param name="Concat_Datas"></param>
        public static void Tran_Data(List<Interpolation_Data> Concat_Datas)
        {
            //清除FIFO 0
            Clear_FIFO();

            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);
            
            foreach (var o in Concat_Datas) 
            {               
                //未矫正数据

                if (o.Type == 1)//直线
                {
                    Line_FIFO(o.End_x, o.End_y);//将直线插补数据写入
                }
                else if (o.Type == 2)//圆弧
                {
                    Circle_R_FIFO(o.End_x, o.End_y, o.Circle_radius, o.Circle_dir);//将圆弧插补写入
                }
                else if (o.Type == 3)//圆形
                {
                    Circle_C_FIFO(o.End_x, o.End_y, o.Center_Start_x, o.Center_Start_y, o.Circle_dir);//将圆形插补写入
                }

            }

            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return);

        } 
        /// <summary>
        /// List<Interpolation_Data> 有前瞻和坐标矫正的  FIFO数据追加
        /// </summary>
        /// <param name="Concat_Datas"></param>
        public static void Tran_Data_Correct (List<Interpolation_Data> Concat_Datas) 
        {
#if !DEBUG
            //清除FIFO 0
            Clear_FIFO();

            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);
#endif
            //定义处理的变量
            Vector Tmp_Point = new Vector();
            decimal Tmp_End_X = 0.0m;
            decimal Tmp_End_Y = 0.0m;
            decimal Tmp_Center_X = 0.0m;
            decimal Tmp_Center_Y = 0.0m;
            decimal Tmp_Center_Start_X = 0.0m;
            decimal Tmp_Center_Start_Y = 0.0m;
            foreach (var o in Concat_Datas)
            {
                if (Para_List.Parameter.Gts_Affinity_Type == 2)
                {
                    Tmp_Point = new Vector(Gts_Cal_Data_Resolve.Get_Line_Fit_Coordinate_AM(o.End_x,o.End_y, Fit_Matrices_AM));
                    Tmp_End_X = Tmp_Point.X;
                    Tmp_End_Y = Tmp_Point.Y;
                    Tmp_Point = new Vector(Gts_Cal_Data_Resolve.Get_Line_Fit_Coordinate_AM(o.Center_x, o.Center_y, Fit_Matrices_AM));
                    Tmp_Center_X = Tmp_Point.X;
                    Tmp_Center_Y = Tmp_Point.Y;
                }
                else
                {
                    //数据矫正
                    //终点计算
                    Tmp_Point = new Vector(Gts_Cal_Data_Resolve.Get_Affinity_Point(0, o.End_x, o.End_y, affinity_Matrices));
                    Tmp_End_X = Tmp_Point.X;
                    Tmp_End_Y = Tmp_Point.Y;
                    //圆心计算
                    Tmp_Point = new Vector(Gts_Cal_Data_Resolve.Get_Affinity_Point(0, o.Center_x, o.Center_y, affinity_Matrices));
                    Tmp_Center_X = Tmp_Point.X;
                    Tmp_Center_Y = Tmp_Point.Y;
                }  
                //圆心与差值计算
                Tmp_Center_Start_X = Tmp_Center_X - Tmp_End_X;
                Tmp_Center_Start_Y = Tmp_Center_X - Tmp_End_Y;

#if !DEBUG
                //替换数据
                if (o.Type == 1)//直线
                {
                    Line_FIFO(Tmp_End_X, Tmp_End_Y);//将直线插补数据写入
                }
                else if (o.Type == 2)//圆弧
                {
                    Circle_R_FIFO(Tmp_End_X, Tmp_End_Y, o.Circle_radius, o.Circle_dir);//将圆弧插补写入
                }
                else if (o.Type == 3)//圆形
                {
                    Circle_C_FIFO(Tmp_End_X, Tmp_End_Y, Tmp_Center_Start_X, Tmp_Center_Start_Y, o.Circle_dir);//将圆形插补写入
                }
#endif

            }
#if !DEBUG
            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return);
#endif
        }
        /// <summary>
        /// 获取当前点的坐标系坐标
        /// </summary>
        /// <param name="type"></param>
        /// 0 - NO COMPENSATION
        /// 1 - AFFINITY COMPENSATION
        /// 2 - LINE COMPENSATION
        /// <returns></returns>
        public static Vector Get_Coordinate(int type)
        {
            Vector Result = new Vector(0,0);
            double[] Curent_Pos=new double[2];
            MC.GT_GetCrdPos(1,out Curent_Pos[0]);
            Vector Tem_Pos = new Vector(-(decimal)Curent_Pos[0] / Para_List.Parameter.Gts_Pos_reference, -(decimal)Curent_Pos[1] / Para_List.Parameter.Gts_Pos_reference);
            //calculate data
            if (type == 0)
            {
                Result = new Vector(Tem_Pos);
            }
            else
            {
                if (Para_List.Parameter.Gts_Affinity_Type == 2)//线性
                {
                    Result = new Vector(Gts_Cal_Data_Resolve.Get_Line_Fit_Coordinate_MA(Tem_Pos.X, Tem_Pos.Y, Fit_Matrices_MA));
                }
                else
                {
                    Result = new Vector(Gts_Cal_Data_Resolve.Get_Affinity_Point(1,Tem_Pos.X, Tem_Pos.Y, affinity_Matrices));
                }
            }
            return Result;
        }
        /// <summary>
        /// 插补运动运行
        /// </summary>
        public static void Interpolation_Start()
        {
            //设置X轴误差带
            Gts_Return = MC.GT_SetAxisBand(1, Para_List.Parameter.Axis_X_Band, 4 * Para_List.Parameter.Axis_X_Time);//20-0.1um,4*2-250us
            Log.Commandhandler("X轴到位误差带", Gts_Return);
            //设置Y轴误差带
            Gts_Return = MC.GT_SetAxisBand(2, Para_List.Parameter.Axis_Y_Band, 4 * Para_List.Parameter.Axis_Y_Time);//20-0.1um,4*2-250us
            Log.Commandhandler("Y轴到位误差带", Gts_Return);

            //缓存区延时指令
            Gts_Return = MC.GT_BufDelay(1, 2, 0);//2ms
            Log.Commandhandler("Line_Interpolation--缓存区延时指令", Gts_Return);
            //启动坐标系1、FIFO0插补运动
            Gts_Return = MC.GT_CrdStart(1, 0);
            Log.Commandhandler("Line_Interpolation--启动坐标系1、FIFO0插补运动", Gts_Return);
            //脉冲输出
            do
            {
                //查询坐标系1、FIFO0插补运动状态
                Gts_Return = MC.GT_CrdStatus(
                    1,//坐标系1
                    out run,//插补运动状态
                    out segment,//当前已完成的插补段数
                    0
                    );

                //查询剩余插补段数
                Gts_Return = MC.GT_GetRemainderSegNum(
                    1,//坐标系1
                    out Remain_Segment,//剩余插补段数
                    0
                    );
                //获取坐标系位置
                Gts_Return = MC.GT_GetCrdPos(1, out Crd_Pos[0]);
                //延时
                Thread.Sleep(100);
            } while (run == 1);

            ////到位检测
            //do
            //{                
            //    //延时
            //    Thread.Sleep(100);
            //} while (!Prompt.Refresh.Axis01_Posed || !(Prompt.Refresh.Axis02_Posed));
            //延时
            Thread.Sleep(Para_List.Parameter.Posed_Time);
        }
        /// <summary>
        /// 停止轴运动
        /// </summary>
        public static void Interpolation_Stop()
        {
            //停止轴规划运动，停止坐标系运动
            Gts_Return = MC.GT_Stop(15, 0);//783-1-4轴全停止，坐标系1、2均停止,15-1-4轴全停止；0-平滑停止运动，783-急停运动
            Log.Commandhandler("Establish_Coordinationg--GT_Stop", Gts_Return);
        }
        /// <summary>
        /// XY平台运动到配合振镜切割准备点 无坐标矫正
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Gts_Ready(decimal x,decimal y)
        {  
            //无数据矫正
            //清除FIFO 0
            Clear_FIFO();
            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);

            //直线插补定位
            Line_FIFO(x, y);//将直线插补数据写入

            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return);
            //启动定位
            Interpolation_Start();
        }
        /// <summary>
        /// XY平台运动到配合振镜切割准备点 坐标矫正4
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Gts_Ready_Correct(decimal x, decimal y) 
        {
            
            //数据矫正
            Vector Tmp_Point = new Vector();
            //定义处理的变量
            decimal Tmp_X = 0.0m;
            decimal Tmp_Y = 0.0m;
            if (Para_List.Parameter.Gts_Affinity_Type == 2)
            {
                Tmp_Point = new Vector(Gts_Cal_Data_Resolve.Get_Line_Fit_Coordinate_AM(x, y, Fit_Matrices_AM));
                Tmp_X = Tmp_Point.X;
                Tmp_Y = Tmp_Point.Y;
            }
            else
            {
                //数据矫正
                Tmp_Point = new Vector(Gts_Cal_Data_Resolve.Get_Affinity_Point(0, x, y, affinity_Matrices));
                Tmp_X = Tmp_Point.X;
                Tmp_Y = Tmp_Point.Y;
            }            

#if !DEBUG
            //清除FIFO 0
            Clear_FIFO();
            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);
            //直线插补定位
            Line_FIFO(Tmp_X, Tmp_Y);//将直线插补数据写入
            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return);
#endif
            //启动定位
            Interpolation_Start();
        }
        public static void Gts_Ready_Test(decimal x, decimal y) 
        {

            //数据矫正
            Vector Tmp_Point = new Vector();
            //定义处理的变量
            decimal Tmp_X = 0.0m;
            decimal Tmp_Y = 0.0m;
            if (Para_List.Parameter.Gts_Affinity_Type == 2)
            {
                Tmp_Point = new Vector(Gts_Cal_Data_Resolve.Get_Line_Fit_Coordinate_AM(x, y, Fit_Matrices_AM));
                Tmp_X = Tmp_Point.X;
                Tmp_Y = Tmp_Point.Y;
            }
            else
            {
                //数据矫正
                Tmp_Point = new Vector(Gts_Cal_Data_Resolve.Get_Affinity_Point(0, x, y, affinity_Matrices));
                Tmp_X = Tmp_Point.X;
                Tmp_Y = Tmp_Point.Y;
            }
#if !DEBUG
            //清除FIFO 0
            Clear_FIFO();
            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);
            //直线插补定位
            Line_FIFO(Tmp_X, Tmp_Y);//将直线插补数据写入
            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return);
#endif
            //启动定位
            Interpolation_Start();

        }
        //XY平台运动到指定点位
        public static void Gts_Ready(Vector Point)
        {
            //清除FIFO 0
            Clear_FIFO();
            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);
            //直线插补定位
            Line_FIFO(Point.X, Point.Y);//将直线插补数据写入
            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return); 
            //启动定位
            Interpolation_Start();            
        }
        //XY平台运动到指定点位
        public static void Gts_Ready_Correct(Vector Point) 
        {
            //数据矫正
            Vector Tmp_Point = new Vector();
            //定义处理的变量
            decimal Tmp_X = 0.0m;
            decimal Tmp_Y = 0.0m;
            //终点计算
            //数据矫正
            Tmp_Point = new Vector(Gts_Cal_Data_Resolve.Get_Affinity_Point(0, Point.X, Point.Y, affinity_Matrices));
            Tmp_X = Tmp_Point.X;
            Tmp_Y = Tmp_Point.Y;
            //清除FIFO 0
            Clear_FIFO();
            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);
            //直线插补定位
            Line_FIFO(Tmp_X, Tmp_Y);//将直线插补数据写入
            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return);

            //启动定位
            Interpolation_Start();

        }
        //Gts插补 整合Rtc振镜 数据，执行
        public static void Integrate(List<List<Interpolation_Data>> Gts_Datas, UInt16 No) 
        {            
            //追加数据
            Tran_Data(Gts_Datas[No]);
            //启动定位
            Interpolation_Start();
        }
        //Gts插补 特定数据，执行
        public static void Integrate(List<Interpolation_Data> Gts_Datas)
        {
            //追加数据
            Tran_Data(Gts_Datas);
            //启动定位
            Interpolation_Start();
        }
        public static void Integrate_Correct(List<Interpolation_Data> Gts_Datas) 
        {
            //追加数据
            Tran_Data_Correct(Gts_Datas);
#if !DEBUG
            //启动定位
            Interpolation_Start();
#endif
        }
    }

}
