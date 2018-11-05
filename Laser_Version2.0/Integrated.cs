using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Laser_Build_1._0
{
    class Integrated
    {
        //退出执行
        public static bool Exit_Flag = false;
        public bool Exit_Flag_00 = false; 
        public bool Exit_Flag_01 = false;
        public bool Exit_Flag_02 = false;
        //该函数，利用传入的数据，将RTC和GTS数据配合，进行裁切
        /// <summary>
        /// 平台不补偿 + 振镜不补偿
        /// </summary>
        /// <param name="List_Datas"></param>
        public static void Rts_Gts(List<List<Interpolation_Data>> List_Datas)
        {
            //初始振镜回Home
            RTC_Fun.Motion.Home();
            //临时变量
            int i = 0, j = 0,repeat = 0;
            //数据处理
            for (i = 0; i < List_Datas.Count; i++)//外层数据
            {
                for (j = 0; j < List_Datas[i].Count; j++)//内层数据
                {
                    if (List_Datas[i][j].Work == 10)//Gts加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate(List_Datas[i]);
                        }
                        else
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //定位到零点
                            RTC_Fun.Motion.Home();
                            //重复加工
                            for (repeat = 0; repeat < Para_List.Parameter.Gts_Repeat; repeat++)
                            {
                                //Gts移动到启动位置 上一list数据的结尾数据或本次的结尾或本次序号0的start;待测试
                                GTS_Fun.Interpolation.Gts_Ready(List_Datas[i][0].Start_x, List_Datas[i][0].Start_y);
                                //打开激光
                                RTC_Fun.Motion.Open_Laser();
                                //启动Gts运动
                                GTS_Fun.Interpolation.Integrate(List_Datas[i]);
                                //关闭激光
                                RTC_Fun.Motion.Close_Laser();
                                //退出执行
                                if (Exit_Flag)
                                {
                                    Exit_Flag = false;
                                    RTC_Fun.Motion.Home();
                                    return;
                                }
                            } 
                        }
                    }
                    else if (List_Datas[i][j].Work == 20)//Rtc加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //启动RTC加工
                            RTC_Fun.Motion.Draw_Jump(List_Datas[i], 1); 
                        }
                        else
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
                            //重复加工
                            for (repeat = 0; repeat < Para_List.Parameter.Rtc_Repeat; repeat++)
                            {
                                //启动加工
                                RTC_Fun.Motion.Draw(List_Datas[i], 1);
                                //退出执行
                                if (Exit_Flag)
                                {
                                    Exit_Flag = false;
                                    RTC_Fun.Motion.Home();
                                    return;
                                }
                            }
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                        }
                    }
                    break;
                }
                //退出执行
                if (Exit_Flag)
                {
                    Exit_Flag = false;
                    RTC_Fun.Motion.Home();
                    return;
                }                
            }
            RTC_Fun.Motion.Home();
        }
        /// <summary>
        /// 平台补偿 + 振镜补偿
        /// </summary>
        /// <param name="List_Datas"></param>
        public static void Rts_Gts_Correct(List<List<Interpolation_Data>> List_Datas)
        {
            //临时变量
            int i = 0, j = 0;
            //数据处理
            for (i = 0; i < List_Datas.Count; i++)//外层数据
            {
                for (j = 0; j < List_Datas[i].Count; j++)//内层数据
                {
                    if (List_Datas[i][j].Work == 10)//Gts加工数据
                    {
                        
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
#endif
                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate_Correct(List_Datas[i]);
                        }
                        else
                        {
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //定位到零点
                            RTC_Fun.Motion.Home();
#endif
                            //Gts移动到启动位置 上一list数据的结尾数据或本次的结尾;待测试
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Start_x, List_Datas[i][0].Start_y);
                            //打开激光
#if !DEBUG
                            RTC_Fun.Motion.Open_Laser();
#endif
                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate_Correct(List_Datas[i]);
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
#endif
                        }

                    }
                    else if (List_Datas[i][j].Work == 20)//Rtc加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
                            Thread.Sleep(500);
                            //启动RTC加工
                            RTC_Fun.Motion.Draw_Jump_Correct(List_Datas[i], 1);
#endif
                        }
                        else
                        {
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
#endif
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
                            Thread.Sleep(500);
#if !DEBUG
                            //启动加工
                            RTC_Fun.Motion.Draw_Rtc_Correct(List_Datas[i], 1);
#endif
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
#endif

                        }
                    }
                    break;
                }
                //退出执行
                if (Exit_Flag)
                {
                    Exit_Flag = false;
                    RTC_Fun.Motion.Home();
                    return;
                }
            }
            RTC_Fun.Motion.Home();
        }
        /// <summary>
        /// 平台补偿 + 不补偿且坐标系方向不纠正 生成RTC校准坐标 用于生RTC板卡校准文件
        /// </summary>
        /// <param name="List_Datas"></param>
        public static void Rts_Gts_Cal_Rtc(List<List<Interpolation_Data>> List_Datas) 
        {
            //初始振镜回Home
            RTC_Fun.Motion.Home();
            //临时变量
            int i = 0, j = 0;
            //数据处理
            for (i = 0; i < List_Datas.Count; i++)//外层数据
            {
                for (j = 0; j < List_Datas[i].Count; j++)//内层数据
                {
                    if (List_Datas[i][j].Work == 10)//Gts加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate_Correct(List_Datas[i]);
                        }
                        else
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //定位到零点
                            RTC_Fun.Motion.Home();
                            //Gts移动到启动位置 上一list数据的结尾数据或本次的结尾或本次序号0的start;待测试
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Start_x, List_Datas[i][0].Start_y);
                            //打开激光
                            RTC_Fun.Motion.Open_Laser();

                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate_Correct(List_Datas[i]);

                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();

                        }
                    }
                    else if (List_Datas[i][j].Work == 20)//Rtc加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //启动RTC加工
                            RTC_Fun.Motion.Draw_Cal(List_Datas[i], 1);
                        }
                        else
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
                            //启动加工
                            RTC_Fun.Motion.Draw_Cal(List_Datas[i], 1);
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                        }
                    }
                    break;
                }
                //退出执行
                if (Exit_Flag)
                {
                    Exit_Flag = false;
                    RTC_Fun.Motion.Home();
                    return;
                }
            }
            RTC_Fun.Motion.Home();
        }
        /// <summary>
        /// 平台补偿 + 振镜坐标系仿射变换补偿
        /// </summary>
        /// <param name="List_Datas"></param>
        public static void Rts_Gts_Cal_Rtc_Affinity(List<List<Interpolation_Data>> List_Datas)
        {
            //临时变量
            int i = 0, j = 0;
            //数据处理
            for (i = 0; i < List_Datas.Count; i++)//外层数据
            {
                for (j = 0; j < List_Datas[i].Count; j++)//内层数据
                {
                    if (List_Datas[i][j].Work == 10)//Gts加工数据
                    {

                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
#endif
                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate_Correct(List_Datas[i]);
                        }
                        else
                        {
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //定位到零点
                            RTC_Fun.Motion.Home();
#endif
                            //Gts移动到启动位置 上一list数据的结尾数据或本次的结尾;待测试
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Start_x, List_Datas[i][0].Start_y);
                            //打开激光
#if !DEBUG
                            RTC_Fun.Motion.Open_Laser();
#endif
                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate_Correct(List_Datas[i]);
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
#endif
                        }

                    }
                    else if (List_Datas[i][j].Work == 20)//Rtc加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
                            //启动RTC加工
                            RTC_Fun.Motion.Draw_Jump_Correct_AFF(List_Datas[i], 1);
#endif
                        }
                        else
                        {
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
#endif
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
#if !DEBUG
                            //启动加工
                            RTC_Fun.Motion.Draw_Rtc_Correct_AFF(List_Datas[i], 1);
#endif
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
#endif

                        }
                    }
                    break;
                }
                //退出执行
                if (Exit_Flag)
                {
                    Exit_Flag = false;
                    RTC_Fun.Motion.Home();
                    return;
                }
            }
            RTC_Fun.Motion.Home();
        }
        /// <summary>
        /// 平台补偿 + 振镜不补偿 用于加工Rtc坐标系矩阵补偿数据 或 验证平台补偿和不补偿的差异
        /// </summary>
        /// <param name="List_Datas"></param>
        public static void Rts_No_Gts_Yes_Correct(List<List<Interpolation_Data>> List_Datas)
        {
            //初始振镜回Home
            RTC_Fun.Motion.Home();
            //临时变量
            int i = 0, j = 0, repeat = 0;
            //数据处理
            for (i = 0; i < List_Datas.Count; i++)//外层数据
            {
                for (j = 0; j < List_Datas[i].Count; j++)//内层数据
                {
                    if (List_Datas[i][j].Work == 10)//Gts加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate_Correct(List_Datas[i]);
                        }
                        else
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //定位到零点
                            RTC_Fun.Motion.Home();
                            //重复加工
                            for (repeat = 0; repeat < Para_List.Parameter.Gts_Repeat; repeat++)
                            {
                                //Gts移动到启动位置 上一list数据的结尾数据或本次的结尾或本次序号0的start;待测试
                                GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Start_x, List_Datas[i][0].Start_y);
                                //打开激光
                                RTC_Fun.Motion.Open_Laser();
                                //启动Gts运动
                                GTS_Fun.Interpolation.Integrate_Correct(List_Datas[i]);
                                //关闭激光
                                RTC_Fun.Motion.Close_Laser();
                                //退出执行
                                if (Exit_Flag)
                                {
                                    Exit_Flag = false;
                                    RTC_Fun.Motion.Home();
                                    return;
                                }
                            }
                        }
                    }
                    else if (List_Datas[i][j].Work == 20)//Rtc加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
                            //启动RTC加工
                            RTC_Fun.Motion.Draw_Jump(List_Datas[i], 1);
                        }
                        else
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
                            //重复加工
                            for (repeat = 0; repeat < Para_List.Parameter.Rtc_Repeat; repeat++)
                            {
                                //启动加工
                                RTC_Fun.Motion.Draw(List_Datas[i], 1);
                                //退出执行
                                if (Exit_Flag)
                                {
                                    Exit_Flag = false;
                                    RTC_Fun.Motion.Home();
                                    return;
                                }
                            }
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                        }
                    }
                    break;
                }
                //退出执行
                if (Exit_Flag)
                {
                    Exit_Flag = false;
                    RTC_Fun.Motion.Home();
                    return;
                }
            }
            RTC_Fun.Motion.Home();
        }

        
        /// <summary>
        /// 平台补偿 + 振镜矩阵补偿 异
        /// </summary>
        /// <param name="List_Datas"></param>
        public static void Rtc_Mat_Gts_Yes_Correct(List<List<Interpolation_Data>> List_Datas)
        {
            //初始振镜回Home
            RTC_Fun.Motion.Home();
            //临时变量
            int i = 0, j = 0, repeat = 0;
            //数据处理
            for (i = 0; i < List_Datas.Count; i++)//外层数据
            {
                for (j = 0; j < List_Datas[i].Count; j++)//内层数据
                {
                    if (List_Datas[i][j].Work == 10)//Gts加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate_Correct(List_Datas[i]);
                        }
                        else
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //定位到零点
                            RTC_Fun.Motion.Home();
                            //重复加工
                            for (repeat = 0; repeat < Para_List.Parameter.Gts_Repeat; repeat++)
                            {
                                //Gts移动到启动位置 上一list数据的结尾数据或本次的结尾或本次序号0的start;待测试
                                GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Start_x, List_Datas[i][0].Start_y);
                                //打开激光
                                RTC_Fun.Motion.Open_Laser();
                                //启动Gts运动
                                GTS_Fun.Interpolation.Integrate_Correct(List_Datas[i]);
                                //关闭激光
                                RTC_Fun.Motion.Close_Laser();
                                //退出执行
                                if (Exit_Flag)
                                {
                                    Exit_Flag = false;
                                    RTC_Fun.Motion.Home();
                                    return;
                                }
                            }
                        }
                    }
                    else if (List_Datas[i][j].Work == 20)//Rtc加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
                            //启动RTC加工
                            RTC_Fun.Motion.Draw_Jump(List_Datas[i], 1);
                        }
                        else
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
                            //重复加工
                            for (repeat = 0; repeat < Para_List.Parameter.Rtc_Repeat; repeat++)
                            {
                                //启动加工
                                RTC_Fun.Motion.Draw_Matrix_Correct(List_Datas[i], 1);
                                //退出执行
                                if (Exit_Flag)
                                {
                                    Exit_Flag = false;
                                    RTC_Fun.Motion.Home();
                                    return;
                                }
                            }
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                        }
                    }
                    break;
                }
                //退出执行
                if (Exit_Flag)
                {
                    Exit_Flag = false;
                    RTC_Fun.Motion.Home();
                    return;
                }
            }
            RTC_Fun.Motion.Home();
        }
    }
}
