using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using netDxf;
using netDxf.Header;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Objects;
using netDxf.Tables;
using netDxf.Units;
using System.Xml.Serialization;
using Laser_Version2._0;
using Prompt;

namespace Laser_Build_1._0
{
    class Data_Resolve
    {
        /***************************************************数据提取***************************************************************/
        //读取文件
        public DxfDocument Read_File(string filename)
        {
            //定义文件名
            string Dxf_filename = null;
            if (filename==null)
            {
                Dxf_filename = "Sample.dxf";
            }
            else
            {
                Dxf_filename = filename;
            }
            DxfDocument Result = new DxfDocument();
            //检查文件是否存在
            FileInfo fileInfo = new FileInfo(Dxf_filename);
            if (!fileInfo.Exists)
            {
                Main.dxf.appendInfo(Dxf_filename + "  文件不存在！！！");
                Log.Error(Dxf_filename + "  文件不存在！！！" + "\r\n");
                return Result;
            }
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(Dxf_filename, out bool isBinary);

            // 检查Dxf文件版本是否正确
            if (dxfVersion < DxfVersion.AutoCad2000)
            {
                Main.dxf.appendInfo(Dxf_filename + "  文件版本不支持");
                Log.Error(Dxf_filename + "  文件版本不支持" + "\r\n");
                return Result;
            }

            //读取Dxf文件
            Result = DxfDocument.Load(Dxf_filename);
            // check if there has been any problems loading the file,
            // this might be the case of a corrupt file or a problem in the library
            if (Result == null)
            {
                Main.dxf.appendInfo(Dxf_filename + "  Dxf文件读取失败");
                Log.Error(Dxf_filename + "  Dxf文件读取失败" + "\r\n");
                return Result;
            }
            //返回读取结果
            return Result;
        }
        /**
         * 将一个list均分成n个list
         * @param source
         * @return
         */
        public List<List<T>> AverageAssign<T>(List<T> source, int n)
        {
            List<List<T>> result = new List<List<T>>();
            int remaider = source.Count % n;  //(先计算出余数)
            int number = source.Count / n;  //然后是商
            int offset = 0;//偏移量
            for (int i = 0; i < n; i++)
            {
                List<T> value = null;
                if (remaider > 0)
                {
                    value =new List<T>(source.Skip(i * number + offset).Take(number + 1).ToList());
                    remaider--;
                    offset++;
                }
                else
                {
                    value = new List<T>(source.Skip(i * number + offset).Take(number).ToList()); 
                }
                result.Add(new List<T>(value));
            }
            return result;
        }
        //处理Dxf得到圆弧和线段数据
        public List<Entity_Data> Resolve_Arc_Line(DxfDocument dxf)
        {
            List<Entity_Data> Result = new List<Entity_Data>();
            //建立临时Entity数据
            Entity_Data Temp_Entity_Data = new Entity_Data();
            //临时变量
            int i = 0;
            //圆弧数据读取
            for (i = 0; i < dxf.Arcs.Count; i++)
            {
                if (dxf.Arcs[i].Layer.Name == "Laser")
                {
                    Temp_Entity_Data.Type = 2;//圆弧
                    //起点计算
                    Temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.Arcs[i].StartPoint.X);
                    Temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.Arcs[i].StartPoint.Y);
                    //终点计算
                    Temp_Entity_Data.End_x = Convert.ToDecimal(dxf.Arcs[i].EndPoint.X);
                    Temp_Entity_Data.End_y = Convert.ToDecimal(dxf.Arcs[i].EndPoint.Y);
                    //起始和终止角度提取
                    Temp_Entity_Data.Cir_Start_Angle = Convert.ToDecimal(dxf.Arcs[i].StartAngle);
                    Temp_Entity_Data.Cir_End_Angle = Convert.ToDecimal(dxf.Arcs[i].EndAngle);
                    //角度处理
                    if (Temp_Entity_Data.Cir_Start_Angle >= 359.99m)
                    {
                        Temp_Entity_Data.Cir_Start_Angle = 0.0m;
                    }                    
                    if (Temp_Entity_Data.Cir_End_Angle <= 0.01m)
                    {
                        Temp_Entity_Data.Cir_End_Angle = 360.0m;
                    }
                    //圆心计算
                    Temp_Entity_Data.Center_x = Convert.ToDecimal(dxf.Arcs[i].Center.X);
                    Temp_Entity_Data.Center_y = Convert.ToDecimal(dxf.Arcs[i].Center.Y);
                    Temp_Entity_Data.Circle_radius = Convert.ToDecimal(dxf.Arcs[i].Radius);
                    //提交进入Arc_Data
                    Result.Add(new Entity_Data(Temp_Entity_Data));
                    Temp_Entity_Data.Empty();
                 }
            }
            //直线数据读取
            for (i = 0; i < dxf.Lines.Count; i++)
            {
                if (dxf.Lines[i].Layer.Name == "Laser")
                {                    
                    Temp_Entity_Data.Type = 1;
                    //起点计算
                    Temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.Lines[i].StartPoint.X);
                    Temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.Lines[i].StartPoint.Y);
                    //终点计算
                    Temp_Entity_Data.End_x = Convert.ToDecimal(dxf.Lines[i].EndPoint.X);
                    Temp_Entity_Data.End_y = Convert.ToDecimal(dxf.Lines[i].EndPoint.Y);

                    //提交进入Arc_Data
                    Result.Add(new Entity_Data(Temp_Entity_Data));
                    Temp_Entity_Data.Empty();
                }
            }
            //返回结果
            return Result;
        }
        //处理Dxf得到多边形数据
        public List<List<Entity_Data>> Resolve_LWPolyline(DxfDocument dxf) 
        {
            List<List<Entity_Data>> Result = new List<List<Entity_Data>>();
            List<Entity_Data> Temp_List = new List<Entity_Data>(); 
            //建立临时Entity数据
            Entity_Data Temp_Entity_Data = new Entity_Data();
            //临时变量
            int i = 0, j = 0;
            //LightWeightPolyline 多边形读取
            if (dxf.LwPolylines.Count > 0)
            {
                for (i = 0; i < dxf.LwPolylines.Count; i++)
                {
                    if (dxf.LwPolylines[i].Layer.Name == "Laser")
                    {
                        for (j = 0; j < dxf.LwPolylines[i].Vertexes.Count; j++)
                        {
                            
                            if (j <= dxf.LwPolylines[i].Vertexes.Count - 2)
                            {
                                if (!(dxf.LwPolylines[i].Vertexes[j].Position.X == dxf.LwPolylines[i].Vertexes[j + 1].Position.X) || !(dxf.LwPolylines[i].Vertexes[j].Position.Y == dxf.LwPolylines[i].Vertexes[j + 1].Position.Y))
                                {
                                    Temp_Entity_Data.Type = 1;//直线插补
                                    ///起点计算
                                    Temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j].Position.X);
                                    Temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j].Position.Y);
                                    Temp_Entity_Data.End_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j + 1].Position.X);
                                    Temp_Entity_Data.End_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j + 1].Position.Y);
                                    //提交进入LwPolylines_Entity_Data
                                    Temp_List.Add(new Entity_Data(Temp_Entity_Data));
                                }                               
                            }
                            else if (j == (dxf.LwPolylines[i].Vertexes.Count - 1))
                            {
                                if (!(dxf.LwPolylines[i].Vertexes[0].Position.X == dxf.LwPolylines[i].Vertexes[j].Position.X) || !(dxf.LwPolylines[i].Vertexes[0].Position.Y == dxf.LwPolylines[i].Vertexes[j].Position.Y))
                                {
                                    Temp_Entity_Data.Type = 1;//直线插补
                                    ///起点计算
                                    Temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j].Position.X);
                                    Temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j].Position.Y);
                                    Temp_Entity_Data.End_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[0].Position.X);
                                    Temp_Entity_Data.End_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[0].Position.Y);
                                    //提交进入LwPolylines_Entity_Data
                                    Temp_List.Add(new Entity_Data(Temp_Entity_Data));
                                }                                
                            }                            
                            Temp_Entity_Data.Empty();
                        }
                        //追加一个LW
                        Result.Add(new List<Entity_Data>(Temp_List));
                        //清空数据
                        Temp_List.Clear();
                    }
                }
            }
            //返回结果
            return Result;
        }
        //处理Dxf得到Circle数据
        public List<Entity_Data> Resolve_Circle(DxfDocument dxf)
        {
            List<Entity_Data> Result = new List<Entity_Data>();
            //建立临时Entity数据
            Entity_Data Temp_Entity_Data = new Entity_Data();
            //临时变量
            int i = 0;
            //圆数据读取
            if (dxf.Circles.Count > 0)
            {
                for (i = 0; i < dxf.Circles.Count; i++)
                {
                    if (dxf.Circles[i].Layer.Name == "Laser")
                    {
                        Temp_Entity_Data.Type = 3;//圆                    
                        //圆心计算
                        Temp_Entity_Data.Center_x = Convert.ToDecimal(dxf.Circles[i].Center.X);
                        Temp_Entity_Data.Center_y = Convert.ToDecimal(dxf.Circles[i].Center.Y);
                        Temp_Entity_Data.Circle_radius = Convert.ToDecimal(dxf.Circles[i].Radius);
                        Temp_Entity_Data.Start_x = Temp_Entity_Data.Center_x + Temp_Entity_Data.Circle_radius;
                        Temp_Entity_Data.Start_y = Temp_Entity_Data.Center_y;
                        Temp_Entity_Data.End_x = Temp_Entity_Data.Center_x;
                        Temp_Entity_Data.End_y = Temp_Entity_Data.Center_y + Temp_Entity_Data.Circle_radius;
                        //画圆方向
                        Temp_Entity_Data.Circle_dir = 0;//顺时针画圆
                        //起始和终止角度
                        Temp_Entity_Data.Cir_End_Angle = 0.0m;
                        Temp_Entity_Data.Cir_Start_Angle = 360.0m;
                        //提交进入Circle_Entity_Data
                        Result.Add(new Entity_Data(Temp_Entity_Data));
                        Temp_Entity_Data.Empty();
                    }
                }
            }
            //返回结果
            return Result;
        }
        //处理Dxf得到Mark数据
        public List<Entity_Data> Resolve_Mark_Point(DxfDocument dxf)
        {
            List<Entity_Data> Result = new List<Entity_Data>();
            //建立临时Entity数据
            Entity_Data Temp_Entity_Data = new Entity_Data();
            //临时变量
            int i = 0;
            //Mark数据读取
            if (dxf.Circles.Count > 0)
            {
                for (i = 0; i < dxf.Circles.Count; i++)
                {
                    if (dxf.Circles[i].Layer.Name == "Mark")  //Mark点 数据收集
                    {
                        //原始数据
                        Temp_Entity_Data.Type = 0;
                        Temp_Entity_Data.Center_x = Convert.ToDecimal(dxf.Circles[i].Center.X);
                        Temp_Entity_Data.Center_y = Convert.ToDecimal(dxf.Circles[i].Center.Y);
                        Temp_Entity_Data.Circle_radius = Convert.ToDecimal(dxf.Circles[i].Radius);
                        //提交进入Circle_Entity_Data
                        Result.Add(new Entity_Data(Temp_Entity_Data));
                        Temp_Entity_Data.Empty();
                    }
                    else if (dxf.Circles[i].Layer.Name == "Mark_Focus")  //Mark点 数据收集
                    {
                        //原始数据
                        Temp_Entity_Data.Type = 10;
                        Temp_Entity_Data.Center_x = Convert.ToDecimal(dxf.Circles[i].Center.X);
                        Temp_Entity_Data.Center_y = Convert.ToDecimal(dxf.Circles[i].Center.Y);
                        Temp_Entity_Data.Circle_radius = Convert.ToDecimal(dxf.Circles[i].Radius);
                        //提交进入Circle_Entity_Data
                        Result.Add(new Entity_Data(Temp_Entity_Data));
                        Temp_Entity_Data.Empty();
                    }
                }
            }
            //返回结果
            return Result;
        }
        //计算并排序Mark坐标点
        public List<Vector> Mark_Calculate(List<Entity_Data> Mark_Datas_Collection)
        {
            //定义返回值
            List<Vector> Result = new List<Vector>();
            List<Vector> Mark_Datas = new List<Vector>();
            //abstract Mark Point
            for (int i = 0;i< Mark_Datas_Collection.Count;i++)
            {
                if (Mark_Datas_Collection[i].Type == 10)
                {
                    Mark_Datas.Add(new Vector(Mark_Datas_Collection[i].Center_x, Mark_Datas_Collection[i].Center_y));
                }
            }
            //排序
            Mark_Datas = Mark_Datas.OrderBy(a =>a.X).ThenByDescending(a => a.Y).ToList();
            //点位输出
            //左下点
            Para_List.Parameter.Mark_Dxf1 = Mark_Datas[0];
            Result.Add(new Vector(Mark_Datas[0]));
            //左上点
            Para_List.Parameter.Mark_Dxf2 = new Vector(Mark_Datas[1]);
            Result.Add(new Vector(Mark_Datas[1]));
            //右上点
            Para_List.Parameter.Mark_Dxf3 = new Vector(Mark_Datas[3]);
            Result.Add(new Vector(Mark_Datas[3]));
            //右下点
            Para_List.Parameter.Mark_Dxf4 = new Vector(Mark_Datas[2]);
            Result.Add(new Vector(Mark_Datas[2]));
            //返回结果
            return Result;
        }

        /***************************************************数据矫正***************************************************************/
        //Entity数据提取完成后，使用mark点计算的仿射变换参数处理数据，获取Dxf在平台坐标系的位置、同时补偿振镜中心与坐标系原点的差值
        public List<Entity_Data> Calibration_Entity(List<Entity_Data> entity_Datas, Affinity_Matrix Mark_affinity_Matrices) 
        {
            //建立变量 
            List<Entity_Data> Result = new List<Entity_Data>();
            Entity_Data Temp_Data = new Entity_Data();
            
            foreach (var O in entity_Datas)
            {
                //先清空
                Temp_Data.Empty();
                //后赋值
                Temp_Data = O;
                //加工起始位置选择
                if (Para_List.Parameter.Calibration_Type == 0) //非Mark点矫正，从原点起始加工
                {
                    //sin取正  (当前坐标系采用) 已验证
                    //起点计算
                    Temp_Data.Start_x = O.Start_x - Para_List.Parameter.Rtc_Org.X;
                    Temp_Data.Start_y = O.Start_y - Para_List.Parameter.Rtc_Org.Y;
                    //终点计算
                    Temp_Data.End_x = O.End_x - Para_List.Parameter.Rtc_Org.X;
                    Temp_Data.End_y = O.End_y - Para_List.Parameter.Rtc_Org.Y;
                    //圆心计算
                    Temp_Data.Center_x = O.Center_x - Para_List.Parameter.Rtc_Org.X;
                    Temp_Data.Center_y = O.Center_y - Para_List.Parameter.Rtc_Org.Y;
                }
                else//Mark点矫正，从矫正位置起始加工
                {
                    //sin取正  (当前坐标系采用) 已验证
                    //起点计算
                    Temp_Data.Start_x = O.Start_x * Mark_affinity_Matrices.Stretch_X + O.Start_y * Mark_affinity_Matrices.Distortion_X + Mark_affinity_Matrices.Delta_X - Para_List.Parameter.Rtc_Org.X;
                    Temp_Data.Start_y = O.Start_y * Mark_affinity_Matrices.Stretch_Y + O.Start_x * Mark_affinity_Matrices.Distortion_Y + Mark_affinity_Matrices.Delta_Y - Para_List.Parameter.Rtc_Org.Y;
                    //终点计算
                    Temp_Data.End_x = O.End_x * Mark_affinity_Matrices.Stretch_X + O.End_y * Mark_affinity_Matrices.Distortion_X + Mark_affinity_Matrices.Delta_X - Para_List.Parameter.Rtc_Org.X;
                    Temp_Data.End_y = O.End_y * Mark_affinity_Matrices.Stretch_Y + O.End_x * Mark_affinity_Matrices.Distortion_Y + Mark_affinity_Matrices.Delta_Y - Para_List.Parameter.Rtc_Org.Y;
                    //圆心计算
                    Temp_Data.Center_x = O.Center_x * Mark_affinity_Matrices.Stretch_X + O.Center_y * Mark_affinity_Matrices.Distortion_X + Mark_affinity_Matrices.Delta_X - Para_List.Parameter.Rtc_Org.X;
                    Temp_Data.Center_y = O.Center_y * Mark_affinity_Matrices.Stretch_Y + O.Center_x * Mark_affinity_Matrices.Distortion_Y + Mark_affinity_Matrices.Delta_Y - Para_List.Parameter.Rtc_Org.Y;

                }
                //追加数据至Result
                Result.Add(new Entity_Data(Temp_Data));
                //清空Temp_Data
                Temp_Data.Empty();

            }
            return Result;
        }
        //Entity数据提取完成后，使用mark点计算的仿射变换参数处理数据，获取Dxf在平台坐标系的位置、同时补偿振镜中心与坐标系原点的差值
        public List<List<Entity_Data>> Calibration_List_Entity(List<List<Entity_Data>> List_Datas, Affinity_Matrix Mark_affinity_Matrices)
        {
            //建立变量 
            List<List<Entity_Data>> Result = new List<List<Entity_Data>>();
            List<Entity_Data> Temp_List = new List<Entity_Data>(); 
            Entity_Data Temp_Data = new Entity_Data();

            foreach (var entity_Datas in List_Datas)
            {
                foreach (var O in entity_Datas)
                {
                    //先清空
                    Temp_Data.Empty();
                    //后赋值
                    Temp_Data = O;
                    //加工起始位置选择
                    if (Para_List.Parameter.Calibration_Type==0) //非Mark点矫正，从原点起始加工
                    {
                        //sin取正  (当前坐标系采用) 已验证
                        //起点计算
                        Temp_Data.Start_x = O.Start_x - Para_List.Parameter.Rtc_Org.X;
                        Temp_Data.Start_y = O.Start_y - Para_List.Parameter.Rtc_Org.Y;
                        //终点计算
                        Temp_Data.End_x = O.End_x - Para_List.Parameter.Rtc_Org.X;
                        Temp_Data.End_y = O.End_y - Para_List.Parameter.Rtc_Org.Y;
                        //圆心计算
                        Temp_Data.Center_x = O.Center_x - Para_List.Parameter.Rtc_Org.X;
                        Temp_Data.Center_y = O.Center_y - Para_List.Parameter.Rtc_Org.Y;
                    }
                    else //Mark点矫正，从矫正位置起始加工
                    {
                        //sin取正  (当前坐标系采用) 已验证
                        //起点计算
                        Temp_Data.Start_x = O.Start_x * Mark_affinity_Matrices.Stretch_X + O.Start_y * Mark_affinity_Matrices.Distortion_X + Mark_affinity_Matrices.Delta_X - Para_List.Parameter.Rtc_Org.X;
                        Temp_Data.Start_y = O.Start_y * Mark_affinity_Matrices.Stretch_Y + O.Start_x * Mark_affinity_Matrices.Distortion_Y + Mark_affinity_Matrices.Delta_Y - Para_List.Parameter.Rtc_Org.Y;
                        //终点计算
                        Temp_Data.End_x = O.End_x * Mark_affinity_Matrices.Stretch_X + O.End_y * Mark_affinity_Matrices.Distortion_X + Mark_affinity_Matrices.Delta_X - Para_List.Parameter.Rtc_Org.X;
                        Temp_Data.End_y = O.End_y * Mark_affinity_Matrices.Stretch_Y + O.End_x * Mark_affinity_Matrices.Distortion_Y + Mark_affinity_Matrices.Delta_Y - Para_List.Parameter.Rtc_Org.Y;
                        //圆心计算
                        Temp_Data.Center_x = O.Center_x * Mark_affinity_Matrices.Stretch_X + O.Center_y * Mark_affinity_Matrices.Distortion_X + Mark_affinity_Matrices.Delta_X - Para_List.Parameter.Rtc_Org.X;
                        Temp_Data.Center_y = O.Center_y * Mark_affinity_Matrices.Stretch_Y + O.Center_x * Mark_affinity_Matrices.Distortion_Y + Mark_affinity_Matrices.Delta_Y - Para_List.Parameter.Rtc_Org.Y;

                    }
                    //追加数据至Temp_List
                    Temp_List.Add(new Entity_Data(Temp_Data));
                    //清空Temp_Data
                    Temp_Data.Empty();
                }
                //追加至结果数据
                Result.Add(new List<Entity_Data>(Temp_List));
                //清空数据
                Temp_List.Clear();
            }
            
            return Result;
        }       

        /***************************************************轨迹生成***************************************************************/
        public List<List<Interpolation_Data>> Integrate_Arc_Line_Parrallel(List<Entity_Data> Arc_Line_Datas)
        {
            //排序
            Arc_Line_Datas = Arc_Line_Datas.OrderBy(a => a.Start_x).ThenBy(a =>a.Start_y).ToList();
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();
            List<Interpolation_Data> Single_Data = new List<Interpolation_Data>(); //辅助运算 用途:提取顺序的衔接和处理
            //临时变量
            List<Interpolation_Data> Temp_List_Data = new List<Interpolation_Data>();
            Interpolation_Data Temp_Data = new Interpolation_Data();
            int Num = 0;
            int Del_Num_01 = 0;
            bool Del_Flag_01 = false;
            int Del_Num_02 = 0;
            bool Del_Flag_02 = false;
            //初始清除
            Single_Data.Clear();
            Temp_List_Data.Clear();
            Temp_Data.Empty();

            //处理Line_Arc生成加工数据 初始数据  属于切入加工起点，故强制使用
            //直线插补走刀
            //强制生成独立的 List<Interpolation_Data>，并将其写入独立运行数据块 List<List<Interpolation_Data>>
            if (Arc_Line_Datas.Count > 0)
            {
                //选择任意切入点
                Temp_Data.Type = 1;//直线插补
                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                Temp_Data.Lift_flag = 1;//抬刀标志
                //强制约束接入点为直线
                if (Arc_Line_Datas.Min(o => o.Type) == 1)
                {
                    Parallel.For(0, Arc_Line_Datas.Count,(Index,Sta)=> 
                    {
                        if (Arc_Line_Datas[Index].Type == 1)
                        {
                            Temp_Data.End_x = Arc_Line_Datas[Index].Start_x;
                            Temp_Data.End_y = Arc_Line_Datas[Index].Start_y;
                            Sta.Stop();
                        }
                    });
                }
                else
                {
                    Temp_Data.End_x = Arc_Line_Datas[0].Start_x;
                    Temp_Data.End_y = Arc_Line_Datas[0].Start_y;
                }

                //提交进入Arc_Data
                Single_Data.Add(new Interpolation_Data(Temp_Data));
                //整合数据生成代码
                Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                //清空数据
                Temp_Data.Empty();
                Temp_List_Data.Clear();

                //整理数据
                do
                {
                    Num = Arc_Line_Datas.Count;//记录当前Arc_Line_Datas.Count,用于判断数据是否处理完或封闭寻找结束
                    Del_Num_01 = 0;
                    Del_Flag_01 = false;
                    Del_Num_02 = 0;
                    Del_Flag_02 = false;
                    Parallel.For(0, Arc_Line_Datas.Count, (Pal_i, Pal_Sta) =>
                    {
                        if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, Arc_Line_Datas[Pal_i].End_x, Arc_Line_Datas[Pal_i].End_y))//当前插补终点是 数据处理终点 同CAD文件规定方向相反
                        {                            
                            Del_Num_01 = Pal_i;
                            Del_Flag_01 = true;
                            Pal_Sta.Stop();
                        }
                        else if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, Arc_Line_Datas[Pal_i].Start_x, Arc_Line_Datas[Pal_i].Start_y)) //当前插补终点是 数据处理起点 同CAD文件规定方向相同
                        {                            
                            Del_Num_02 = Pal_i;
                            Del_Flag_02 = true;
                            Pal_Sta.Stop();
                        }
                    });
                    //数据处理
                    if (Del_Flag_01)
                    {
                        Temp_Data.Lift_flag = 0;//抬刀标志
                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                                            //插补起点坐标
                        Temp_Data.Start_x = Arc_Line_Datas[Del_Num_01].End_x;
                        Temp_Data.Start_y = Arc_Line_Datas[Del_Num_01].End_y;
                        //插补终点坐标
                        Temp_Data.End_x = Arc_Line_Datas[Del_Num_01].Start_x;
                        Temp_Data.End_y = Arc_Line_Datas[Del_Num_01].Start_y;

                        if (Arc_Line_Datas[Del_Num_01].Type == 1)//直线
                        {
                            Temp_Data.Type = 1;//直线插补
                        }
                        else if (Arc_Line_Datas[Del_Num_01].Type == 2)//圆弧
                        {
                            Temp_Data.Type = 2;//圆弧插补
                                               //圆弧插补 圆心坐标 减去 插补起点坐标
                            Temp_Data.Center_Start_x = Temp_Data.Center_x - Temp_Data.Start_x;
                            Temp_Data.Center_Start_y = Temp_Data.Center_y - Temp_Data.Start_y;
                            //计算圆弧角度
                            Temp_Data.Angle = Arc_Line_Datas[Del_Num_01].Cir_End_Angle - Arc_Line_Datas[Del_Num_01].Cir_Start_Angle;
                            //圆弧方向
                            Temp_Data.Circle_dir = 0;
                            //圆弧圆心
                            Temp_Data.Center_x = Arc_Line_Datas[Del_Num_01].Center_x;
                            Temp_Data.Center_y = Arc_Line_Datas[Del_Num_01].Center_y;
                            //圆弧半径
                            Temp_Data.Circle_radius = Arc_Line_Datas[Del_Num_01].Circle_radius;
                        }

                        //提交进入Arc_Data
                        Single_Data.Add(new Interpolation_Data(Temp_Data));

                        //整合数据生成代码
                        Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据

                        //清空数据
                        Temp_Data.Empty();
                        //删除数据
                        Arc_Line_Datas.RemoveAt(Del_Num_01);
                        //清除标志
                        Del_Num_01 = 0;
                        Del_Flag_01 = false;
                        Del_Num_02 = 0;
                        Del_Flag_02 = false;
                    }
                    else if (!Del_Flag_01 && Del_Flag_02)
                    {
                        Temp_Data.Lift_flag = 0;//抬刀标志
                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                                            //插补起点坐标
                        Temp_Data.Start_x = Arc_Line_Datas[Del_Num_02].Start_x;
                        Temp_Data.Start_y = Arc_Line_Datas[Del_Num_02].Start_y;
                        //插补终点坐标
                        Temp_Data.End_x = Arc_Line_Datas[Del_Num_02].End_x;
                        Temp_Data.End_y = Arc_Line_Datas[Del_Num_02].End_y;

                        if (Arc_Line_Datas[Del_Num_02].Type == 1)//直线
                        {
                            Temp_Data.Type = 1;//直线插补 
                        }
                        else if (Arc_Line_Datas[Del_Num_02].Type == 2)//圆弧
                        {
                            Temp_Data.Type = 2;//圆弧插补
                                               //圆弧插补 圆心坐标 减去 插补起点坐标
                            Temp_Data.Center_Start_x = Temp_Data.Center_x - Temp_Data.Start_x;
                            Temp_Data.Center_Start_y = Temp_Data.Center_y - Temp_Data.Start_y;
                            //计算圆弧角度
                            Temp_Data.Angle = Arc_Line_Datas[Del_Num_02].Cir_Start_Angle - Arc_Line_Datas[Del_Num_02].Cir_End_Angle;
                            //圆弧方向
                            Temp_Data.Circle_dir = 1;
                            //圆弧圆心
                            Temp_Data.Center_x = Arc_Line_Datas[Del_Num_02].Center_x;
                            Temp_Data.Center_y = Arc_Line_Datas[Del_Num_02].Center_y;
                            //圆弧半径
                            Temp_Data.Circle_radius = Arc_Line_Datas[Del_Num_02].Circle_radius;
                        }
                        //提交进入Arc_Data
                        Single_Data.Add(new Interpolation_Data(Temp_Data));
                        //整合数据生成代码
                        Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据                                                                              
                        Temp_Data.Empty();//清空数据

                        //删除数据
                        Arc_Line_Datas.RemoveAt(Del_Num_02);
                        //清除标志
                        Del_Num_01 = 0;
                        Del_Flag_01 = false;
                        Del_Num_02 = 0;
                        Del_Flag_02 = false;
                    }

                    //寻找结束点失败，意味着重新开始新的 线段或圆弧
                    if ((Arc_Line_Datas.Count != 0) && (Num != 0) && (Num == Arc_Line_Datas.Count))
                    {
                        //整合数据生成代码 当前结束的封闭图形加工数据
                        Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                        //清空数据
                        Temp_Data.Empty();
                        Temp_List_Data.Clear();

                        //跳刀直接使用直线插补走刀
                        //插补进入新的目标起点
                        Temp_Data.Type = 1;//直线插补
                        Temp_Data.Lift_flag = 1;//抬刀标志
                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        //强制约束接入点为直线
                        if (Arc_Line_Datas.Min(o => o.Type) == 1)
                        {
                            Parallel.For(0, Arc_Line_Datas.Count, (Index, Sta) =>
                            {
                                if (Arc_Line_Datas[Index].Type == 1)
                                {
                                    Temp_Data.End_x = Arc_Line_Datas[Index].Start_x;
                                    Temp_Data.End_y = Arc_Line_Datas[Index].Start_y;
                                    Sta.Stop();
                                }
                            });
                        }
                        else
                        {
                            Temp_Data.End_x = Arc_Line_Datas[0].Start_x;
                            Temp_Data.End_y = Arc_Line_Datas[0].Start_y;
                        }

                        //提交进入Arc_Data
                        Single_Data.Add(new Interpolation_Data(Temp_Data));

                        //整合数据生成代码
                        Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                        Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                        //清空数据
                        Temp_Data.Empty();
                        Temp_List_Data.Clear();
                    }
                    else if ((Arc_Line_Datas.Count == 0) && (Num == 1))
                    {
                        //整合数据生成代码 当前结束的封闭图形加工数据
                        Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                        //清空数据
                        Temp_List_Data.Clear();
                    }

                } while (Arc_Line_Datas.Count > 0);//实体Line_Arc数据未清空完
            }
            //返回结果
            return Result;
        }
        //数据处理 生成Arc_Line整合数据  振镜和平台联合加工
        public List<List<Interpolation_Data>> Integrate_Arc_Line(List<Entity_Data> Arc_Line_Datas)
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();
            List<Interpolation_Data> Single_Data = new List<Interpolation_Data>(); //辅助运算 用途:提取顺序的衔接和处理
            //临时变量
            List<Interpolation_Data> Temp_List_Data = new List<Interpolation_Data>();
            Interpolation_Data Temp_Data = new Interpolation_Data();
            int i = 0;
            int Num = 0;
            //初始清除
            Single_Data.Clear();
            Temp_List_Data.Clear();
            Temp_Data.Empty();

            //处理Line_Arc生成加工数据 初始数据  属于切入加工起点，故强制使用
            //直线插补走刀
            //强制生成独立的 List<Interpolation_Data>，并将其写入独立运行数据块 List<List<Interpolation_Data>>
            if (Arc_Line_Datas.Count > 0)
            {
                //选择任意切入点
                Temp_Data.Type = 1;//直线插补
                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                Temp_Data.Lift_flag = 1;//抬刀标志
                //强制约束接入点为直线
                if (Arc_Line_Datas.Min(o => o.Type) == 1)
                {
                    for (i = 0; i < Arc_Line_Datas.Count; i++)
                    {
                        if (Arc_Line_Datas[i].Type == 1)
                        {
                            Temp_Data.End_x = Arc_Line_Datas[i].Start_x;
                            Temp_Data.End_y = Arc_Line_Datas[i].Start_y;
                            break;
                        }
                    }
                }
                else
                {
                    Temp_Data.End_x = Arc_Line_Datas[0].Start_x;
                    Temp_Data.End_y = Arc_Line_Datas[0].Start_y;
                }

                //提交进入Arc_Data
                Single_Data.Add(new Interpolation_Data(Temp_Data));
                //整合数据生成代码
                Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                //清空数据
                Temp_Data.Empty();
                Temp_List_Data.Clear();

                //整理数据
                do
                {
                    Num = Arc_Line_Datas.Count;//记录当前Arc_Line_Datas.Count,用于判断数据是否处理完或封闭寻找结束
                    for (i = 0; i < Arc_Line_Datas.Count; i++)
                    {
                        if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, Arc_Line_Datas[i].End_x, Arc_Line_Datas[i].End_y))//当前插补终点是 数据处理终点 同CAD文件规定方向相反
                        {
                            Temp_Data.Lift_flag = 0;//抬刀标志
                            Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            //插补起点坐标
                            Temp_Data.Start_x = Arc_Line_Datas[i].End_x;
                            Temp_Data.Start_y = Arc_Line_Datas[i].End_y;
                            //插补终点坐标
                            Temp_Data.End_x = Arc_Line_Datas[i].Start_x;
                            Temp_Data.End_y = Arc_Line_Datas[i].Start_y;                            

                            if (Arc_Line_Datas[i].Type == 1)//直线
                            {
                                Temp_Data.Type = 1;//直线插补
                            }
                            else if (Arc_Line_Datas[i].Type == 2)//圆弧
                            {
                                Temp_Data.Type = 2;//圆弧插补
                                //圆弧插补 圆心坐标 减去 插补起点坐标
                                Temp_Data.Center_Start_x = Temp_Data.Center_x - Temp_Data.Start_x;
                                Temp_Data.Center_Start_y = Temp_Data.Center_y - Temp_Data.Start_y;                                
                                //计算圆弧角度
                                Temp_Data.Angle = (Arc_Line_Datas[i].Cir_End_Angle - Arc_Line_Datas[i].Cir_Start_Angle);
                                //圆弧方向
                                Temp_Data.Circle_dir = 0;
                                //圆弧圆心
                                Temp_Data.Center_x = Arc_Line_Datas[i].Center_x;
                                Temp_Data.Center_y = Arc_Line_Datas[i].Center_y;
                                //圆弧半径
                                Temp_Data.Circle_radius = Arc_Line_Datas[i].Circle_radius;
                            }                           

                            //提交进入Arc_Data
                            Single_Data.Add(new Interpolation_Data(Temp_Data));

                            //整合数据生成代码
                            Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据

                            //清空数据
                            Temp_Data.Empty();

                            //删除当前的Entity数据
                            Arc_Line_Datas.RemoveAt(i);
                            break;
                        }
                        else if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, Arc_Line_Datas[i].Start_x, Arc_Line_Datas[i].Start_y)) //当前插补终点是 数据处理起点 同CAD文件规定方向相同
                        {
                            Temp_Data.Lift_flag = 0;//抬刀标志
                            Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            //插补起点坐标
                            Temp_Data.Start_x = Arc_Line_Datas[i].Start_x;
                            Temp_Data.Start_y = Arc_Line_Datas[i].Start_y;
                            //插补终点坐标
                            Temp_Data.End_x = Arc_Line_Datas[i].End_x;
                            Temp_Data.End_y = Arc_Line_Datas[i].End_y;

                            if (Arc_Line_Datas[i].Type == 1)//直线
                            {
                                Temp_Data.Type = 1;//直线插补 
                            }
                            else if (Arc_Line_Datas[i].Type == 2)//圆弧
                            {
                                Temp_Data.Type = 2;//圆弧插补
                                //圆弧插补 圆心坐标 减去 插补起点坐标
                                Temp_Data.Center_Start_x = Temp_Data.Center_x - Temp_Data.Start_x;
                                Temp_Data.Center_Start_y = Temp_Data.Center_y - Temp_Data.Start_y;
                                //计算圆弧角度
                                Temp_Data.Angle = Arc_Line_Datas[i].Cir_Start_Angle - Arc_Line_Datas[i].Cir_End_Angle;
                                //圆弧方向
                                Temp_Data.Circle_dir = 1;
                                //圆弧圆心
                                Temp_Data.Center_x = Arc_Line_Datas[i].Center_x;
                                Temp_Data.Center_y = Arc_Line_Datas[i].Center_y;
                                //圆弧半径
                                Temp_Data.Circle_radius = Arc_Line_Datas[i].Circle_radius;
                            }                            

                            //提交进入Arc_Data
                            Single_Data.Add(new Interpolation_Data(Temp_Data));
                            //整合数据生成代码
                            Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                            //清空数据
                            Temp_Data.Empty();

                            //删除当前的Entity数据
                            Arc_Line_Datas.RemoveAt(i);
                            break;
                        }
                    }

                    //寻找结束点失败，意味着重新开始新的 线段或圆弧
                    if ((Arc_Line_Datas.Count != 0) && (Num !=0) && (Num == Arc_Line_Datas.Count))
                    {
                        //整合数据生成代码 当前结束的封闭图形加工数据
                        Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                        //清空数据
                        Temp_Data.Empty();
                        Temp_List_Data.Clear();

                        //跳刀直接使用直线插补走刀
                        //插补进入新的目标起点
                        Temp_Data.Type = 1;//直线插补
                        Temp_Data.Lift_flag = 1;//抬刀标志
                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        //强制约束接入点为直线
                        if (Arc_Line_Datas.Min(o => o.Type) == 1)
                        {
                            for (i = 0; i < Arc_Line_Datas.Count; i++)
                            {
                                if (Arc_Line_Datas[i].Type == 1)
                                {
                                    Temp_Data.End_x = Arc_Line_Datas[i].Start_x;
                                    Temp_Data.End_y = Arc_Line_Datas[i].Start_y;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Temp_Data.End_x = Arc_Line_Datas[0].Start_x;
                            Temp_Data.End_y = Arc_Line_Datas[0].Start_y;
                        }
                        //提交进入Arc_Data
                        Single_Data.Add(new Interpolation_Data(Temp_Data));
                        //整合数据生成代码
                        Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                        Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                        //清空数据
                        Temp_Data.Empty();
                        Temp_List_Data.Clear();
                    }
                    else if ((Arc_Line_Datas.Count == 0) && (Num == 1))
                    {
                        //整合数据生成代码 当前结束的封闭图形加工数据
                        Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                        //清空数据
                        Temp_List_Data.Clear();
                    }
                } while (Arc_Line_Datas.Count > 0);//实体Line_Arc数据未清空完
            }
            //返回结果
            return Result;
        }        
        //数据处理 生成LWPolyline整合数据  振镜和平台联合加工
        public List<List<Interpolation_Data>> Integrate_LWPolyline(List<Entity_Data> LWPolyline_Datas)
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();
            List<Interpolation_Data> Single_Data = new List<Interpolation_Data>(); //辅助运算 用途:提取顺序的衔接和处理
            //临时变量
            List<Interpolation_Data> Temp_List_Data = new List<Interpolation_Data>();
            Interpolation_Data Temp_Data = new Interpolation_Data();
            int i = 0;
            int Num = 0;
            //初始清除
            Single_Data.Clear();
            Temp_List_Data.Clear();
            Temp_Data.Empty();

            //处理LWPolyline生成加工数据 初始数据  属于切入加工起点，故强制使用
            //直线插补走刀
            //强制生成独立的 List<Interpolation_Data>，并将其写入独立运行数据块 List<List<Interpolation_Data>>
            if (LWPolyline_Datas.Count > 0)
            {
                //选择任意切入点
                Temp_Data.Type = 1;//直线插补
                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                Temp_Data.Lift_flag = 1;//抬刀标志
                Temp_Data.End_x = LWPolyline_Datas[0].Start_x;
                Temp_Data.End_y = LWPolyline_Datas[0].Start_y;

                //提交进入Arc_Data
                Single_Data.Add(new Interpolation_Data(Temp_Data));
                //整合数据生成代码
                Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                //清空数据
                Temp_Data.Empty();
                Temp_List_Data.Clear();

                //整理数据
                do
                {
                    Num = LWPolyline_Datas.Count;//记录当前LWPolyline_Datas.Count,用于判断数据是否处理完或封闭寻找结束
                    for (i = 0; i < LWPolyline_Datas.Count; i++)
                    {
                        if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, LWPolyline_Datas[i].End_x, LWPolyline_Datas[i].End_y))//当前插补终点是 数据处理终点 同CAD文件规定方向相反
                        {
                            Temp_Data.Type = 1;//直线插补

                            Temp_Data.Lift_flag = 0;//抬刀标志
                            Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            //插补起点
                            Temp_Data.Start_x = LWPolyline_Datas[i].End_x;
                            Temp_Data.Start_y = LWPolyline_Datas[i].End_y;
                            //插补终点坐标
                            Temp_Data.End_x = LWPolyline_Datas[i].Start_x;
                            Temp_Data.End_y = LWPolyline_Datas[i].Start_y;

                            //提交进入Single_Data
                            Single_Data.Add(new Interpolation_Data(Temp_Data));

                            //整合数据生成代码
                            Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据

                            //清空数据
                            Temp_Data.Empty();

                            //删除当前的Entity数据
                            LWPolyline_Datas.RemoveAt(i);
                            break;
                        }
                        else if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, LWPolyline_Datas[i].Start_x, LWPolyline_Datas[i].Start_y)) //当前插补终点是 数据处理起点 同CAD文件规定方向相同
                        {
                            Temp_Data.Type = 1;//直线插补 

                            Temp_Data.Lift_flag = 0;//抬刀标志
                            Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            //插补起点
                            Temp_Data.Start_x = LWPolyline_Datas[i].Start_x;
                            Temp_Data.Start_y = LWPolyline_Datas[i].Start_y;
                            //插补终点坐标
                            Temp_Data.End_x = LWPolyline_Datas[i].End_x;
                            Temp_Data.End_y = LWPolyline_Datas[i].End_y;

                            //提交进入Single_Data
                            Single_Data.Add(new Interpolation_Data(Temp_Data));

                            //整合数据生成代码
                            Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据

                            //清空数据
                            Temp_Data.Empty();

                            //删除当前的Entity数据
                            LWPolyline_Datas.RemoveAt(i);
                            break;
                        }
                    }

                    //寻找结束点失败，意味着重新开始新的 线段或圆弧
                    if ((LWPolyline_Datas.Count != 0) && (Num != 0) && (Num == LWPolyline_Datas.Count))
                    {
                        //整合数据生成代码 当前结束的封闭图形加工数据
                        Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                        //清空数据
                        Temp_Data.Empty();
                        Temp_List_Data.Clear();

                        //跳刀直接使用直线插补走刀
                        //插补进入新的目标起点
                        Temp_Data.Type = 1;//直线插补
                        Temp_Data.Lift_flag = 1;//抬刀标志
                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        Temp_Data.End_x = LWPolyline_Datas[0].Start_x;
                        Temp_Data.End_y = LWPolyline_Datas[0].Start_y;

                        //提交进入Arc_Data
                        Single_Data.Add(new Interpolation_Data(Temp_Data));

                        //整合数据生成代码
                        Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                        Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                        //清空数据
                        Temp_Data.Empty();
                        Temp_List_Data.Clear();
                    }
                    else if ((LWPolyline_Datas.Count == 0) && (Num == 1))
                    {
                        //整合数据生成代码 当前结束的封闭图形加工数据
                        Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                        //清空数据
                        Temp_List_Data.Clear();
                    }
                } while (LWPolyline_Datas.Count > 0);//实体LWPolyline数据未清空完
            }
            //返回结果
            return Result;
        }
        //数据处理 生成LWPolyline整合数据  振镜和平台联合加工
        public List<List<Interpolation_Data>> Integrate_LWPolyline(List<List<Entity_Data>> LWPolyline_Datas)
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();
            //临时变量
            List<Interpolation_Data> Temp_List_Data = new List<Interpolation_Data>();
            Interpolation_Data Temp_Data = new Interpolation_Data();
            int i = 0;
            int j = 0;
            //初始清除
            Temp_List_Data.Clear();
            Temp_Data.Empty();

            //处理LWPolyline生成加工数据 初始数据  属于切入加工起点，故强制使用
            //直线插补走刀
            //强制生成独立的 List<Interpolation_Data>，并将其写入独立运行数据块 List<List<Interpolation_Data>>
            if (LWPolyline_Datas.Count > 0)
            {
                //选择任意切入点
                Temp_Data.Type = 1;//直线插补
                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                Temp_Data.Lift_flag = 1;//抬刀标志
                Temp_Data.End_x = LWPolyline_Datas[0][0].Start_x;
                Temp_Data.End_y = LWPolyline_Datas[0][0].Start_y;
                //整合数据生成代码
                Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                //清空数据
                Temp_Data.Empty();
                Temp_List_Data.Clear();

                //整理数据
                do
                {
                    
                    for (i = 0;i< LWPolyline_Datas.Count;i++)
                    {
                        for (j = 0; j < LWPolyline_Datas[i].Count; j++)
                        {
                            Temp_Data.Type = 1;//直线插补
                            Temp_Data.Lift_flag = 0;//抬刀标志
                            Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            Temp_Data.Start_x = LWPolyline_Datas[i][j].Start_x;
                            Temp_Data.Start_y = LWPolyline_Datas[i][j].Start_y;
                            Temp_Data.End_x = LWPolyline_Datas[i][j].End_x;
                            Temp_Data.End_y = LWPolyline_Datas[i][j].End_y;
                            //整合数据生成代码
                            Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                            Temp_Data.Empty(); //清空数据
                        }
                        Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                        //清空数据
                        Temp_Data.Empty();
                        Temp_List_Data.Clear();
                        //删除数据
                        LWPolyline_Datas.RemoveAt(i);

                        if (LWPolyline_Datas.Count >= 1)
                        { 
                            //跳刀直接使用直线插补走刀
                            //插补进入新的目标起点
                            Temp_Data.Type = 1;//直线插补
                            Temp_Data.Lift_flag = 1;//抬刀标志
                            Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            Temp_Data.End_x = LWPolyline_Datas[0][0].Start_x;
                            Temp_Data.End_y = LWPolyline_Datas[0][0].Start_y;
                            //整合数据生成代码
                            Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                            Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                            //清空数据
                            Temp_Data.Empty();
                            Temp_List_Data.Clear();
                        }
                        //跳出for循环
                        break;
                    }                    
                    
                } while (LWPolyline_Datas.Count > 0);//实体LWPolyline数据未清空完
            }
            //返回结果
            return Result;
        }
        //数据处理 生成Circle整合数据  振镜和平台联合加工
        public List<List<Interpolation_Data>> Integrate_Circle(List<Entity_Data> Circle_Datas)
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();
            List<Interpolation_Data> Single_Data = new List<Interpolation_Data>(); //辅助运算 用途:提取顺序的衔接和处理
            //临时变量
            List<Interpolation_Data> Temp_List_Data = new List<Interpolation_Data>();
            Interpolation_Data Temp_Data = new Interpolation_Data();
            int i = 0;
            //初始清除
            Single_Data.Clear();
            Temp_List_Data.Clear();
            Temp_Data.Empty();

            //处理Circle生成加工数据 初始数据  属于切入加工起点，故强制使用
            //直线插补走刀
            //强制生成独立的 List<Interpolation_Data>，并将其写入独立运行数据块 List<List<Interpolation_Data>>
            if (Circle_Datas.Count > 0)
            {
                //选择任意切入点
                Temp_Data.Type = 1;//直线插补
                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                Temp_Data.Lift_flag = 1;//抬刀标志
                Temp_Data.End_x = Circle_Datas[0].End_x;
                Temp_Data.End_y = Circle_Datas[0].End_y; 

                //整合数据生成代码
                Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                //清空数据
                Temp_Data.Empty();
                Temp_List_Data.Clear();

                //整理数据
                do
                {
                    for ( i = 0;i<Circle_Datas.Count; i++ )
                    {
                        Temp_Data.Type = 3;//圆形插补
                        Temp_Data.Lift_flag = 0;//抬刀标志
                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        //圆形半径
                        Temp_Data.Circle_radius = Circle_Datas[i].Circle_radius;
                        //圆形起点坐标
                        Temp_Data.Start_x = Circle_Datas[i].End_x;
                        Temp_Data.Start_y = Circle_Datas[i].End_y;
                        //插补终点坐标
                        Temp_Data.End_x = Circle_Datas[i].End_x;
                        Temp_Data.End_y = Circle_Datas[i].End_y;
                        //圆心坐标
                        Temp_Data.Center_x = Circle_Datas[i].Center_x;
                        Temp_Data.Center_y = Circle_Datas[i].Center_y;
                        //圆弧插补 圆心坐标 减去 插补起点坐标
                        Temp_Data.Center_Start_x = Temp_Data.Center_x - Temp_Data.End_x;
                        Temp_Data.Center_Start_y = Temp_Data.Center_y - Temp_Data.End_y;
                        //圆形方向
                        Temp_Data.Circle_dir = Circle_Datas[i].Circle_dir;

                        //整合数据生成代码
                        Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                        Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                        //清空数据
                        Temp_Data.Empty();
                        Temp_List_Data.Clear();

                        //删除当前的Entity数据
                        Circle_Datas.RemoveAt(i);

                        if (Circle_Datas.Count>=1)
                        {
                            //跳刀直接使用直线插补走刀
                            //插补进入新的目标起点
                            Temp_Data.Type = 1;//直线插补
                            Temp_Data.Lift_flag = 1;//抬刀标志
                            Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            Temp_Data.End_x = Circle_Datas[0].End_x;
                            Temp_Data.End_y = Circle_Datas[0].End_y;

                            //整合数据生成代码
                            Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
                            Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                            //清空数据
                            Temp_Data.Empty();
                            Temp_List_Data.Clear();
                        }
                        
                        //跳出For循环
                        break;
                    }

                } while (Circle_Datas.Count > 0);//实体Circle数据未清空完
            }
                                                                           
            ////处理Circle生成加工数据 初始数据  属于切入加工起点，故强制使用
            ////直线插补走刀
            ////强制生成独立的 List<Interpolation_Data>，并将其写入独立运行数据块 List<List<Interpolation_Data>>
            //if (Circle_Datas.Count > 0)
            //{
            //    //选择任意切入点
            //    Temp_Data.Type = 1;//直线插补
            //    Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
            //    Temp_Data.Lift_flag = 1;//抬刀标志
            //    Temp_Data.Repeat = 0;//重复次数
            //    Temp_Data.End_x = Circle_Datas[0].Start_x;
            //    Temp_Data.End_y = Circle_Datas[0].Start_y; ;

            //    //提交进入Arc_Data
            //    Single_Data.Add(new Interpolation_Data(Temp_Data));
            //    //整合数据生成代码
            //    Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
            //    Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

            //    //清空数据
            //    Temp_Data.Empty();
            //    Temp_List_Data.Clear();

            //    //整理数据
            //    do
            //    {
            //        Num = Circle_Datas.Count;//记录当前Circle_Datas.Count,用于判断数据是否处理完或封闭寻找结束
            //        for (i = 0; i < Circle_Datas.Count; i++)
            //        {
            //            if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, Circle_Datas[i].End_x, Circle_Datas[i].End_y))//当前插补终点是 数据处理终点 同CAD文件规定方向相反
            //            {
            //                Temp_Data.Type = 3;//圆形插补
            //                Temp_Data.Lift_flag = 0;//抬刀标志
            //                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
            //                Temp_Data.Repeat = 0;//重复次数
            //                //圆形半径
            //                Temp_Data.Circle_radius = Circle_Datas[i].Circle_radius;
            //                //圆形起点坐标
            //                Temp_Data.Start_x = Circle_Datas[i].End_x;
            //                Temp_Data.Start_y = Circle_Datas[i].End_y;
            //                //插补终点坐标
            //                Temp_Data.End_x = Circle_Datas[i].End_x;
            //                Temp_Data.End_y = Circle_Datas[i].End_y;
            //                //圆心坐标
            //                Temp_Data.Center_x = Circle_Datas[i].Center_x;
            //                Temp_Data.Center_y = Circle_Datas[i].Center_y;

            //                //圆弧插补 圆心坐标 减去 插补起点坐标
            //                Temp_Data.Center_Start_x = Temp_Data.Center_x - Temp_Data.End_x;
            //                Temp_Data.Center_Start_y = Temp_Data.Center_y - Temp_Data.End_y;

            //                //圆形方向
            //                Temp_Data.Circle_dir = Circle_Datas[i].Circle_dir;

            //                //提交进入Arc_Data
            //                Single_Data.Add(new Interpolation_Data(Temp_Data));

            //                //整合数据生成代码
            //                Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据

            //                //清空数据
            //                Temp_Data.Empty();

            //                //删除当前的Entity数据
            //                Circle_Datas.RemoveAt(i);
            //                break;
            //            }
            //            else if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, Circle_Datas[i].Start_x, Circle_Datas[i].Start_y)) //当前插补终点是 数据处理起点 同CAD文件规定方向相同
            //            {

            //                Temp_Data.Type = 3;//圆形插补
            //                Temp_Data.Lift_flag = 0;//抬刀标志
            //                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
            //                Temp_Data.Repeat = 0;//重复次数
            //                //圆形半径
            //                Temp_Data.Circle_radius = Circle_Datas[i].Circle_radius;
            //                //圆形起点坐标
            //                Temp_Data.Start_x = Circle_Datas[i].Start_x;
            //                Temp_Data.Start_y = Circle_Datas[i].Start_y;
            //                //插补终点坐标
            //                Temp_Data.End_x = Circle_Datas[i].Start_x;
            //                Temp_Data.End_y = Circle_Datas[i].Start_y;
            //                //圆心坐标
            //                Temp_Data.Center_x = Circle_Datas[i].Center_x;
            //                Temp_Data.Center_y = Circle_Datas[i].Center_y;

            //                //圆弧插补 圆心坐标 减去 插补起点坐标
            //                Temp_Data.Center_Start_x = Temp_Data.Center_x - Temp_Data.End_x;
            //                Temp_Data.Center_Start_y = Temp_Data.Center_y - Temp_Data.End_y;

            //                //圆形方向
            //                Temp_Data.Circle_dir = Circle_Datas[i].Circle_dir;

            //                //提交进入Arc_Data
            //                Single_Data.Add(new Interpolation_Data(Temp_Data));
            //                //整合数据生成代码
            //                Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据

            //                //清空数据
            //                Temp_Data.Empty();

            //                //删除当前的Entity数据
            //                Circle_Datas.RemoveAt(i);
            //                break;
            //            }
            //        }

            //        //寻找结束点失败，意味着重新开始新的 线段或圆弧
            //        if ((Circle_Datas.Count != 0) && (Num != 0) && (Num == Circle_Datas.Count))
            //        {

            //            //整合数据生成代码 当前结束的封闭图形加工数据
            //            Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
            //            //清空数据
            //            Temp_Data.Empty();
            //            Temp_List_Data.Clear();

            //            //跳刀直接使用直线插补走刀
            //            //插补进入新的目标起点
            //            Temp_Data.Type = 1;//直线插补
            //            Temp_Data.Lift_flag = 1;//抬刀标志
            //            Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
            //            Temp_Data.Repeat = 0;//重复次数
            //            Temp_Data.End_x = Circle_Datas[0].Start_x;
            //            Temp_Data.End_y = Circle_Datas[0].Start_y;

            //            //提交进入Arc_Data
            //            Single_Data.Add(new Interpolation_Data(Temp_Data));

            //            //整合数据生成代码
            //            Temp_List_Data.Add(new Interpolation_Data(Temp_Data));//追加数据
            //            Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

            //            //清空数据
            //            Temp_Data.Empty();
            //            Temp_List_Data.Clear();
            //        }
            //        else if ((Circle_Datas.Count == 0) && (Num == 1))
            //        {
            //            //整合数据生成代码 当前结束的封闭图形加工数据
            //            Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
            //           //清空数据
            //            Temp_List_Data.Clear();
            //        }

            //    } while (Circle_Datas.Count > 0);//实体Circle数据未清空完
            //}
            //返回结果
            return Result;
        }
        //数据整合
        public List<List<Interpolation_Data>> Converge_Data(List<List<Interpolation_Data>> Arc_Line_Data, List<List<Interpolation_Data>> LW_Data, List<List<Interpolation_Data>> Circle_Data)
        {
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();
            Result.AddRange(new List<List<Interpolation_Data>>(Arc_Line_Data));
            Result.AddRange(new List<List<Interpolation_Data>>(LW_Data));
            Result.AddRange(new List<List<Interpolation_Data>>(Circle_Data));
            //临时数据
            Interpolation_Data Temp_Data = new Interpolation_Data();
            //获取坐标点
            Vector Start_Coordinate = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
            //数据处理合并为走刀直线添加起点坐标
            for (int cal = 0; cal < Result.Count; cal++)
            {
                //当前序号 数量为1、加工类型1 直线、加工方式10 GTS
                //当前+1序号 数量大于1、加工方式20 RTX
                if (cal==0)
                {
                    if ((Result[cal].Count == 1) && (Result[cal][0].Type==1) && (Result[cal][0].Lift_flag == 1))
                    {
                        Temp_Data.Empty();
                        Temp_Data = Result[cal][0];
                        Temp_Data.Start_x = Start_Coordinate.X;
                        Temp_Data.Start_y = Start_Coordinate.Y;
                        //重新赋值
                        Result[cal][0] = new Interpolation_Data(Temp_Data);
                    }                    
                }
                else
                {
                    if ((Result[cal].Count == 1) && (Result[cal][0].Type == 1) && (Result[cal][0].Lift_flag == 1))
                    {
                        Temp_Data.Empty();
                        Temp_Data = Result[cal][0];
                        Temp_Data.Start_x = Result[cal - 1][Result[cal - 1].Count - 1].End_x;
                        Temp_Data.Start_y = Result[cal - 1][Result[cal - 1].Count - 1].End_y;
                        //重新赋值
                        Result[cal][0] = new Interpolation_Data(Temp_Data);
                    }
                }
            }
            //返回结果
            return Result;
        }

        /***************************************************刀具补偿****************************************************************/
        public List<List<Interpolation_Data>> Cutter_Compensation(List<List<Interpolation_Data>> In_Data)
        {
            //整体原则 不修改传入数据，直接将结果反馈回Result
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
            List<Interpolation_Data> Temp_Interpolation_List_Data = new List<Interpolation_Data>();//二级层
            Interpolation_Data Temp_Data = new Interpolation_Data();//一级层  
            Interpolation_Data Temp_Cal_Data = new Interpolation_Data();//临时计算数据
            decimal Trail_Center_X = 0, Trail_Center_Y = 0;//轨迹中心坐标  
            //临时变量
            int i = 0;
            int j = 0;
            int m = 0;
            Vector Temp_Vector = new Vector();//用于获取交点的数据
            //初始清除
            Result.Clear();
            Temp_Interpolation_List_Data.Clear();
            Temp_Data.Empty();

            //数据处理部分
            for (i = 0; i < In_Data.Count; i++)
            {
                //清除数据
                Temp_Interpolation_List_Data.Clear();
                //获取封闭图形中心坐标
                Trail_Center_X = (In_Data[i].Max(o => o.End_x) + In_Data[i].Min(o => o.End_x)) / 2m;//RTC坐标X基准
                Trail_Center_Y = (In_Data[i].Max(o => o.End_y) + In_Data[i].Min(o => o.End_y)) / 2m;//RTC坐标Y基准
                if ((In_Data[i].Count > 0) && (In_Data[i].Count == 1)) //二级层，整合元素数量小于2，说明数量为1
                {                    
                    for (j = 0; j < In_Data[i].Count; j++)
                    {
                        //获取赋值
                        Temp_Data =new Interpolation_Data(In_Data[i][j]);
                        //数据处理
                        if (In_Data[i][j].Type == 1)//直线  修正终点 end_x,end_y
                        {
                            if (i <= In_Data.Count - 2) //确保不超出List索引
                            {
                                if ((In_Data[i + 1].Count >0 ) && (In_Data[i+1].Count<=1)) //单级数据
                                {
                                    if (In_Data[i+1][0].Type == 1)//直线
                                    {
                                        //追加数据
                                        Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                        //追加返回值
                                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                        //数据清空
                                        Temp_Data.Empty();
                                        Temp_Interpolation_List_Data.Clear();
                                    }
                                    else if (In_Data[i + 1][0].Type == 2)// 圆弧
                                    {
                                        Temp_Vector = Cal_Cross_Data(In_Data[i][j], In_Data[i + 1][0]);//计算当前层 与 下级层第一个切入圆弧的交点                                        
                                        Temp_Data.End_x = Temp_Vector.X;
                                        Temp_Data.End_y = Temp_Vector.Y;
                                        //追加数据
                                        Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                        //追加返回值
                                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                        //数据清空
                                        Temp_Data.Empty();
                                        Temp_Interpolation_List_Data.Clear();
                                    }
                                    else if (In_Data[i + 1][0].Type == 3)//整圆
                                    {
                                        //临时计算数据赋值
                                        Temp_Cal_Data = new Interpolation_Data(In_Data[i + 1][0]);
                                        //计算起点                          
                                        if (Math.Abs(Temp_Cal_Data.End_x - In_Data[i + 1][0].Circle_radius - In_Data[i + 1][0].Center_x) <= Para_List.Parameter.Pos_Tolerance)
                                        {
                                            Temp_Cal_Data.End_x = In_Data[i + 1][0].End_x + Para_List.Parameter.Cutter_Radius;
                                            Temp_Cal_Data.End_y = In_Data[i + 1][0].End_y;
                                        }
                                        else if (Math.Abs(Temp_Cal_Data.End_y - In_Data[i + 1][0].Circle_radius - In_Data[i + 1][0].Center_y) <= Para_List.Parameter.Pos_Tolerance)
                                        {
                                            Temp_Cal_Data.End_x = In_Data[i + 1][0].End_x;
                                            Temp_Cal_Data.End_y = In_Data[i + 1][0].End_y + Para_List.Parameter.Cutter_Radius;
                                        }

                                        //修正直线数据
                                        Temp_Data.End_x = Temp_Cal_Data.End_x;
                                        Temp_Data.End_y = Temp_Cal_Data.End_y;
                                        //追加数据
                                        Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                        //追加返回值
                                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                        //数据清空
                                        Temp_Data.Empty();
                                        Temp_Interpolation_List_Data.Clear();
                                    }
                                }
                                else//多级数据
                                {
                                    //判断是否是封闭图形
                                    if (Differ_Err(In_Data[i + 1][0].Start_x, In_Data[i + 1][0].Start_y, In_Data[i + 1][In_Data[i + 1].Count-1].End_x, In_Data[i + 1][In_Data[i + 1].Count - 1].End_y))
                                    {
                                        Temp_Vector = Cal_Cross_Data(In_Data[i + 1][In_Data[i + 1].Count - 1], In_Data[i + 1][0]);//计算当前层 与 下级层
                                        Temp_Data.End_x = Temp_Vector.X;
                                        Temp_Data.End_y = Temp_Vector.Y;
                                        //追加数据
                                        Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                        //追加返回值
                                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                        //数据清空
                                        Temp_Data.Empty();
                                        Temp_Interpolation_List_Data.Clear();
                                    }
                                    else
                                    {
                                        //追加返回值
                                        Result.Add(new List<Interpolation_Data>(In_Data[i]));
                                        //数据清空
                                        Temp_Data.Empty();
                                        Temp_Interpolation_List_Data.Clear();
                                    }                                    
                                }
                            }
                            else if (i == In_Data.Count - 1)//最后一段数据
                            {
                                //追加返回值
                                Result.Add(new List<Interpolation_Data>(In_Data[i]));
                                //数据清空
                                Temp_Data.Empty();
                                Temp_Interpolation_List_Data.Clear();
                            }
                        }
                        else if (In_Data[i][j].Type == 2)// 圆弧  圆弧暂不处理
                        {
                            //追加数据
                            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                            //追加返回值
                            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                            //数据清空
                            Temp_Data.Empty();
                            Temp_Interpolation_List_Data.Clear();
                        }
                        else if (In_Data[i][j].Type == 3)//整圆
                        {
                            //获取中心
                            Temp_Data.Trail_Center = new Vector(In_Data[i][j].Center_x, In_Data[i][j].Center_y);
                            //刀具补偿类型 0-不补偿、1-钻孔 -Radius、2-落料 +Radius
                            decimal Radius = 0.0m;//定义刀具补偿半径
                            if (Para_List.Parameter.Cutter_Type == 0)
                            {
                                Radius = 0.0m;
                            }
                            else if (Para_List.Parameter.Cutter_Type == 1)
                            {
                                Radius = -Para_List.Parameter.Cutter_Radius;
                            }
                            else if (Para_List.Parameter.Cutter_Type == 2)
                            {
                                Radius = Para_List.Parameter.Cutter_Radius;
                            }
                            //整圆半径修正
                            Temp_Data.Circle_radius = Temp_Data.Circle_radius + Radius;
                            //整圆Center_Start 原点坐标与起点坐标的差值                          
                            if (Math.Abs(Temp_Data.End_x - In_Data[i][j].Circle_radius - In_Data[i][j].Center_x) <=Para_List.Parameter.Pos_Tolerance)
                            {   
                                Temp_Data.End_x = In_Data[i][j].End_x + Radius;
                                Temp_Data.End_y = In_Data[i][j].End_y;
                                Temp_Data.Center_Start_x = In_Data[i][j].Center_Start_x + Radius;
                                Temp_Data.Center_Start_y = In_Data[i][j].Center_Start_y;
                            }
                            else if (Math.Abs(Temp_Data.End_y - In_Data[i][j].Circle_radius - In_Data[i][j].Center_y) <= Para_List.Parameter.Pos_Tolerance)
                            {
                                Temp_Data.End_x = In_Data[i][j].End_x;
                                Temp_Data.End_y = In_Data[i][j].End_y + Radius;
                                Temp_Data.Center_Start_x = In_Data[i][j].Center_Start_x;
                                Temp_Data.Center_Start_y = In_Data[i][j].Center_Start_y + Radius;
                            }
                            //追加数据
                            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                            //追加返回值
                            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                            //数据清空
                            Temp_Data.Empty();
                            Temp_Interpolation_List_Data.Clear();
                        }
                    }
                }
                else if (In_Data[i].Count >= 2) //二级层，整合元素数量大于等于2，说明封闭图形
                {                    
                    //判断是否是封闭图形
                    if (Differ_Err(In_Data[i][0].Start_x, In_Data[i][0].Start_y, In_Data[i][In_Data[i].Count - 1].End_x, In_Data[i][In_Data[i].Count - 1].End_y))
                    {
                        for (m = 0; m < In_Data[i].Count; m++)
                        {
                            if (m == 0)
                            {
                                //获取赋值
                                Temp_Data = new Interpolation_Data(In_Data[i][0]);
                                Temp_Vector =new Vector(Cal_Cross_Data(In_Data[i][In_Data[i].Count - 1], In_Data[i][0]));//计算结尾层 与 0层  起始点
                                Temp_Data.Start_x = Temp_Vector.X;
                                Temp_Data.Start_y = Temp_Vector.Y;
                                Temp_Vector = new Vector(Cal_Cross_Data(In_Data[i][0], In_Data[i][1]));//计算0层 与 1层  终止点
                                Temp_Data.End_x = Temp_Vector.X;
                                Temp_Data.End_y = Temp_Vector.Y;
                                //追加数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                            }
                            else if ((m > 0) && (m<= In_Data[i].Count-2))
                            {
                                //获取赋值
                                Temp_Data = new Interpolation_Data(In_Data[i][m]);
                                Temp_Vector = new Vector(Cal_Cross_Data(In_Data[i][m-1], In_Data[i][m]));//计算当前层 与 上一层  起始点
                                Temp_Data.Start_x = Temp_Vector.X;
                                Temp_Data.Start_y = Temp_Vector.Y;
                                Temp_Vector = new Vector(Cal_Cross_Data(In_Data[i][m], In_Data[i][m+1]));//计算当前层 与 +1层  终止点
                                Temp_Data.End_x = Temp_Vector.X;
                                Temp_Data.End_y = Temp_Vector.Y;
                                //追加数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                            }
                            else if (m == (In_Data[i].Count - 1))
                            {
                                //获取赋值
                                Temp_Data = new Interpolation_Data(In_Data[i][m]);
                                Temp_Vector = new Vector(Cal_Cross_Data(In_Data[i][m - 1], In_Data[i][m]));//计算m层 与 上1层  起始点
                                Temp_Data.Start_x = Temp_Vector.X;
                                Temp_Data.Start_y = Temp_Vector.Y;
                                Temp_Vector = new Vector(Cal_Cross_Data(In_Data[i][m], In_Data[i][0]));//计算m层 与 下一层  终止点
                                Temp_Data.End_x = Temp_Vector.X;
                                Temp_Data.End_y = Temp_Vector.Y;
                                //追加数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                            }
                        }
                    }
                    else
                    {
                        Temp_Interpolation_List_Data = new List<Interpolation_Data>(In_Data[i]);
                    }
                    //追加返回值
                    Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                    //数据清空
                    Temp_Data.Empty();
                    Temp_Interpolation_List_Data.Clear();
                }
            }
            //返回结果
            return Result;
        }

        /***************************************************轨迹拆分***************************************************************/
        //将加工数据切分为RTC和GTS加工  该函数的对象是：直线、圆弧和整圆
        public List<List<Interpolation_Data>> Separate_Rtc_Gts(List<List<Interpolation_Data>> In_Data)
        { 
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
            List<Interpolation_Data> Temp_Interpolation_List_Data = new List<Interpolation_Data>();//二级层
            Interpolation_Data Temp_Data = new Interpolation_Data();//一级层  
            decimal Delta_X = 0, Delta_Y = 0;//X、Y坐标极值差值
            decimal Rtc_Cal_X=0,Rtc_Cal_Y=0;//RTC坐标计算基准                  
            int i = 0;
            int j = 0;
            int m = 0;
            //初始清除
            Result.Clear();
            Temp_Interpolation_List_Data.Clear();
            Temp_Data.Empty();
            //数据处理部分
            for (i = 0; i < In_Data.Count; i++)
            {
                //清除数据
                Temp_Interpolation_List_Data.Clear();
                if ((In_Data[i].Count > 0) && (In_Data[i].Count < 2)) //二级层，整合元素数量小于2，说明只有一个跳刀
                {
                    for (j = 0; j < In_Data[i].Count; j++)
                    {
                        //直线、整圆拆分，整理成GTS和RTC加工数据
                        if (In_Data[i][j].Type == 1)//直线
                        {
                            if (In_Data[i][j].Lift_flag==1)//抬刀标志
                            {
                                Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                            }
                            else
                            {
                                //整合数据加工范围判断 取Max Min,Delta_X,Delta_Y长度均在50mm以内
                                //数据计算
                                Delta_X = Convert.ToDecimal(Math.Abs(In_Data[i].Max(o => o.End_x) - In_Data[i].Min(o => o.End_x)));//X坐标极值范围
                                Delta_Y = Convert.ToDecimal(Math.Abs(In_Data[i].Max(o => o.End_y) - In_Data[i].Min(o => o.End_y)));//Y坐标极值范围
                                //获取封闭图形中心坐标
                                Rtc_Cal_X = (In_Data[i].Max(o => o.End_x) + In_Data[i].Min(o => o.End_x)) / 2m;//RTC坐标X基准
                                Rtc_Cal_Y = (In_Data[i].Max(o => o.End_y) + In_Data[i].Min(o => o.End_y)) / 2m;//RTC坐标Y基准
                                //范围判断
                                if ((Delta_X >= Para_List.Parameter.Rtc_Limit.X) || (Delta_Y > Para_List.Parameter.Rtc_Limit.Y))//X、Y坐标极值范围大于等于48mm，由GTS加工，否则由RTC加工
                                {
                                    Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                                }
                                else
                                {
                                    //数据清空
                                    Temp_Data.Empty();
                                    //数据赋值
                                    Temp_Data = In_Data[i][j];
                                    //强制抬刀标志：0
                                    Temp_Data.Lift_flag = 0;
                                    //强制加工类型为RTC
                                    Temp_Data.Work = 20;
                                    //RTC加工，GTS平台配合坐标
                                    if (j == 0)
                                    {
                                        //GTS平台配合坐标
                                        Temp_Data.Gts_x = Rtc_Cal_X;
                                        Temp_Data.Gts_y = Rtc_Cal_Y;
                                        //Rtc定位 激光加工起点坐标
                                        Temp_Data.Rtc_x = In_Data[i][j].Start_x - Rtc_Cal_X;
                                        Temp_Data.Rtc_y = In_Data[i][j].Start_y - Rtc_Cal_Y;
                                    }
                                    //RTC mark_abs直线
                                    Temp_Data.Type = 15;
                                    //坐标转换 将坐标转换为RTC坐标系坐标
                                    Temp_Data.End_x = In_Data[i][j].End_x - Rtc_Cal_X;
                                    Temp_Data.End_y = In_Data[i][j].End_y - Rtc_Cal_Y;
                                    //追加修改的数据
                                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                    Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                }
                            }
                                 
                        }
                        else if (In_Data[i][j].Type == 2)// 圆弧
                        {
                            if (Temp_Data.Circle_radius >= 20)//圆弧半径大于等于20mm
                            {
                                Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                            }
                            else
                            {
                                //生成Rtc加工数据
                                Temp_Data = In_Data[i][j];
                                //RTC arc_abs圆弧
                                Temp_Data.Type = 11;
                                //强制抬刀标志：0
                                Temp_Data.Lift_flag = 0;
                                //强制加工类型为RTC
                                Temp_Data.Work = 20;
                                //RTC加工，GTS平台配合坐标
                                Temp_Data.Gts_x = In_Data[i][j].Center_x;
                                Temp_Data.Gts_y = In_Data[i][j].Center_y;
                                //Rtc定位 激光加工起点坐标
                                Temp_Data.Rtc_x = In_Data[i][j].Start_x - In_Data[i][j].Center_x;
                                Temp_Data.Rtc_y = In_Data[i][j].Start_y - In_Data[i][j].Center_y;
                                //RTC 圆弧加工圆心坐标转换
                                Temp_Data.Center_x = In_Data[i][j].Center_x - Temp_Data.Gts_x;
                                Temp_Data.Center_y = In_Data[i][j].Center_y - Temp_Data.Gts_y;
                                //坐标转换 将坐标转换为RTC坐标系坐标
                                Temp_Data.End_x = In_Data[i][j].End_x - In_Data[i][j].Center_x;
                                Temp_Data.End_y = In_Data[i][j].End_y - In_Data[i][j].Center_y;
                                //追加修改的数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                //清空数据
                                Temp_Data.Empty();
                                Temp_Interpolation_List_Data.Clear();

                                //再追加一组直线插补，让GTS平台跳至该圆弧终点
                                Temp_Data.Type = 1;//直线插补
                                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                                Temp_Data.Lift_flag = 1;//抬刀标志
                                Temp_Data.End_x = In_Data[i][j].End_x;
                                Temp_Data.End_y = In_Data[i][j].End_y;

                                //追加修改的数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                //清空数据
                                Temp_Data.Empty();
                                Temp_Interpolation_List_Data.Clear();
                            }
                            
                        }
                        else if (In_Data[i][j].Type == 3)//整圆
                        {
                            //判断整圆大小
                            if (In_Data[i][j].Circle_radius >= 24) //整圆半径大于24mm，GTS加工
                            {
                                Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                            }
                            else //整圆半径小于24mm，RTC加工
                            {
                                //数据赋值
                                Temp_Data = In_Data[i][j];
                                //RTC arc_abs圆弧
                                Temp_Data.Type = 11;                                
                                //强制抬刀标志：0
                                Temp_Data.Lift_flag = 0;
                                //强制加工类型为RTC
                                Temp_Data.Work = 20;
                                //RTC加工，GTS平台配合坐标
                                Temp_Data.Gts_x = In_Data[i][j].Center_x;
                                Temp_Data.Gts_y = In_Data[i][j].Center_y;                                
                                //RTC 圆弧加工圆心坐标转换
                                Temp_Data.Center_x = 0;
                                Temp_Data.Center_y = 0;
                                //RTC加工切入点
                                Temp_Data.End_x = Temp_Data.Center_x;
                                Temp_Data.End_y = Temp_Data.Center_y + In_Data[i][j].Circle_radius;
                                //RTC加工整圆角度
                                // arc angle in ° as a 64 - bit IEEE floating point value
                                // (positive angle values correspond to clockwise angles);
                                // allowed range: [–3600.0° … +3600.0°] (±10 full circles);
                                // out-of-range values will be edge-clipped.
                                Temp_Data.Angle = 370;//这个参数得看RTC手册，整圆的旋转角度

                                //Rtc定位 激光加工起点坐标
                                Temp_Data.Rtc_x = Temp_Data.Center_x;
                                Temp_Data.Rtc_y = Temp_Data.Center_y + In_Data[i][j].Circle_radius;

                                //追加修改的数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                            }
                            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                        }
                    }
                }
                else if (In_Data[i].Count >= 2) //二级层，整合元素数量大于等于2，说明封闭图形
                {
                    //整合数据加工范围判断 取Max Min,Delta_X,Delta_Y长度均在50mm以内
                    //数据计算
                    Delta_X = Convert.ToDecimal(Math.Abs(In_Data[i].Max(o => o.End_x) - In_Data[i].Min(o => o.End_x)));//X坐标极值范围
                    Delta_Y = Convert.ToDecimal(Math.Abs(In_Data[i].Max(o => o.End_y) - In_Data[i].Min(o => o.End_y)));//Y坐标极值范围
                    //获取封闭图形中心坐标
                    Rtc_Cal_X = (In_Data[i].Max(o => o.End_x) + In_Data[i].Min(o => o.End_x)) / 2m;//RTC坐标X基准
                    Rtc_Cal_Y = (In_Data[i].Max(o => o.End_y) + In_Data[i].Min(o => o.End_y)) / 2m;//RTC坐标Y基准
                    //范围判断
                    if ((Delta_X > Para_List.Parameter.Rtc_Limit.X) || (Delta_Y > Para_List.Parameter.Rtc_Limit.Y))//X、Y坐标极值范围大于等于48mm，由GTS加工，否则由RTC加工
                    {
                        //不考虑圆弧半径大小，全部由Gts加工
                        //Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                        //考虑圆弧半径大小，Radius >= 20mm由Gts加工，Radius < 20mm由Rtc加工
                        for (m=0;m< In_Data[i].Count; m++)
                        {
                            //数据清空
                            Temp_Data.Empty();
                            //数据赋值
                            Temp_Data = In_Data[i][m];
                            //当前数据类型判断
                            if (Temp_Data.Type ==1)//直线
                            {
                                //追加修改的数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                            }
                            else if (Temp_Data.Type == 2)//圆弧
                            {
                                if (Temp_Data.Circle_radius >= 20)//圆弧半径大于等于20mm
                                {
                                    //追加修改的数据
                                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                }
                                else//圆弧半径小于20mm
                                {
                                    //从起始到当前
                                    if (Temp_Interpolation_List_Data.Count>0)
                                    {
                                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                    }
                                    //清空数据  为生成Rtc数据做准备
                                    Temp_Interpolation_List_Data.Clear();

                                    //先计算圆心与切点的直线
                                    if (m>0)
                                    {
                                        //圆弧起点 与 圆心 的直线参数
                                        decimal k1 = (Temp_Data.Center_y - In_Data[i][m - 1].End_y) / (Temp_Data.Center_x - In_Data[i][m - 1].End_x);
                                        decimal b1 = Temp_Data.Center_y - k1 * Temp_Data.Center_x;
                                        //计算起点偏移点
                                        decimal x1 = Temp_Data.Center_x + 2;
                                        decimal y1 = x1 * k1 + b1;
                                        //圆弧终点 与 圆心 的直线参数
                                        decimal k2 = (Temp_Data.Center_y - Temp_Data.End_y) / (Temp_Data.Center_x - Temp_Data.End_x);
                                        decimal b2 = Temp_Data.Center_y - k2 * Temp_Data.Center_x;
                                        //计算终点偏移点
                                        decimal x2 = Temp_Data.Center_x + 2;
                                        decimal y2 = x2 * k2 + b2;

                                        //生成Rtc加工数据
                                        //RTC arc_abs圆弧
                                        Temp_Data.Type = 11;
                                        //强制抬刀标志：0
                                        Temp_Data.Lift_flag = 0;
                                        //强制加工类型为RTC
                                        Temp_Data.Work = 20;
                                        //RTC加工，GTS平台配合坐标
                                        Temp_Data.Gts_x = In_Data[i][m - 1].End_x;
                                        Temp_Data.Gts_y = In_Data[i][m - 1].End_y;
                                        //Rtc定位 激光加工起点坐标
                                        Temp_Data.Rtc_x = 0;
                                        Temp_Data.Rtc_y = 0;
                                        //RTC 圆弧加工圆心坐标转换
                                        Temp_Data.Center_x = In_Data[i][m].Center_x - Temp_Data.Gts_x;
                                        Temp_Data.Center_y = In_Data[i][m].Center_y - Temp_Data.Gts_y;
                                        //坐标转换 将坐标转换为RTC坐标系坐标
                                        Temp_Data.End_x = In_Data[i][m].End_x - Temp_Data.Gts_x;
                                        Temp_Data.End_y = In_Data[i][m].End_y - Temp_Data.Gts_y;
                                        //追加修改的数据
                                        Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                        //清空数据
                                        Temp_Data.Empty();
                                        Temp_Interpolation_List_Data.Clear();

                                        //再追加一组直线插补，让GTS平台跳至该圆弧终点
                                        Temp_Data.Type = 1;//直线插补
                                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                                        Temp_Data.Lift_flag = 1;//抬刀标志
                                        Temp_Data.End_x = In_Data[i][m].End_x;
                                        Temp_Data.End_y = In_Data[i][m].End_y;

                                        //追加修改的数据
                                        Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                        //清空数据
                                        Temp_Data.Empty();
                                        Temp_Interpolation_List_Data.Clear();

                                    }
                                    else//肯定有切入点
                                    {
                                        if (i > 0)
                                        {
                                            //圆弧起点 与 圆心 的直线参数
                                            decimal k1 = (Temp_Data.Center_y - In_Data[i - 1][In_Data[i - 1].Count - 1].End_y) / (Temp_Data.Center_x - In_Data[i - 1][In_Data[i - 1].Count - 1].End_x);
                                            decimal b1 = Temp_Data.Center_y - k1 * Temp_Data.Center_x;
                                            //计算起点偏移点
                                            decimal x1 = Temp_Data.Center_x + 2;
                                            decimal y1 = x1 * k1 + b1;
                                            //圆弧终点 与 圆心 的直线参数
                                            decimal k2 = (Temp_Data.Center_y - Temp_Data.End_y) / (Temp_Data.Center_x - Temp_Data.End_x);
                                            decimal b2 = Temp_Data.Center_y - k2 * Temp_Data.Center_x;
                                            //计算终点偏移点
                                            decimal x2 = Temp_Data.Center_x + 2;
                                            decimal y2 = x2 * k2 + b2;

                                            //生成Rtc加工数据
                                            //RTC arc_abs圆弧
                                            Temp_Data.Type = 11;
                                            //强制抬刀标志：0
                                            Temp_Data.Lift_flag = 0;
                                            //强制加工类型为RTC
                                            Temp_Data.Work = 20;
                                            //RTC加工，GTS平台配合坐标
                                            Temp_Data.Gts_x = In_Data[i - 1][In_Data[i - 1].Count - 1].End_x;
                                            Temp_Data.Gts_y = In_Data[i - 1][In_Data[i - 1].Count - 1].End_y;
                                            //Rtc定位 激光加工起点坐标
                                            Temp_Data.Rtc_x = 0;
                                            Temp_Data.Rtc_y = 0;
                                            //RTC 圆弧加工圆心坐标转换
                                            Temp_Data.Center_x = In_Data[i][m].Center_x - Temp_Data.Gts_x;
                                            Temp_Data.Center_y = In_Data[i][m].Center_y - Temp_Data.Gts_y;
                                            //坐标转换 将坐标转换为RTC坐标系坐标
                                            Temp_Data.End_x = In_Data[i][m].End_x - Temp_Data.Gts_x;
                                            Temp_Data.End_y = In_Data[i][m].End_y - Temp_Data.Gts_y;
                                            //追加修改的数据
                                            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                            //清空数据
                                            Temp_Data.Empty();
                                            Temp_Interpolation_List_Data.Clear();

                                            //再追加一组直线插补，让GTS平台跳至该圆弧终点
                                            Temp_Data.Type = 1;//直线插补
                                            Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                                            Temp_Data.Lift_flag = 1;//抬刀标志
                                            Temp_Data.End_x = In_Data[i][m].End_x;
                                            Temp_Data.End_y = In_Data[i][m].End_y;

                                            //追加修改的数据
                                            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                            //清空数据
                                            Temp_Data.Empty();
                                            Temp_Interpolation_List_Data.Clear();
                                        }
                                        else
                                        {
                                            MessageBox.Show("整合数据异常，终止！！！");
                                        }
                                        
                                    }
                                    
                                }
                            }

                            //遍历结束
                            if ((Temp_Interpolation_List_Data.Count > 0) && (m == In_Data[i].Count-1))
                            {
                                Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                //清空数据  为生成Rtc数据做准备
                                Temp_Interpolation_List_Data.Clear();
                            }                            

                        }

                    }
                    else
                    {
                        for (j = 0; j < In_Data[i].Count; j++)
                        {
                            //数据清空
                            Temp_Data.Empty();
                            //数据赋值
                            Temp_Data = In_Data[i][j];
                            //强制抬刀标志：0
                            Temp_Data.Lift_flag = 0;
                            //强制加工类型为RTC
                            Temp_Data.Work = 20;
                            //RTC加工，GTS平台配合坐标
                            if (j == 0)
                            {
                                //GTS平台配合坐标
                                Temp_Data.Gts_x = Rtc_Cal_X;
                                Temp_Data.Gts_y = Rtc_Cal_Y;
                                //Rtc定位 激光加工起点坐标
                                Temp_Data.Rtc_x = In_Data[i][0].Start_x - Rtc_Cal_X;
                                Temp_Data.Rtc_y = In_Data[i][0].Start_y - Rtc_Cal_Y;
                            }
                            //直线、圆弧拆分，整理成RTC加工数据
                            if (Temp_Data.Type==1)//直线
                            {
                                //RTC mark_abs直线
                                Temp_Data.Type = 15;                                
                            }
                            else if (Temp_Data.Type == 2)//圆弧
                            {
                                //RTC arc_abs圆弧
                                Temp_Data.Type = 11;
                                //RTC 圆弧加工圆心坐标转换
                                Temp_Data.Center_x = In_Data[i][j].Center_x - Rtc_Cal_X;
                                Temp_Data.Center_y = In_Data[i][j].Center_y - Rtc_Cal_Y;
                                if (In_Data[i][j].Circle_dir == 1)
                                {
                                    Temp_Data.Angle = In_Data[i][j].Angle;
                                }
                                else if (In_Data[i][j].Circle_dir == 0)
                                {
                                    Temp_Data.Angle = -In_Data[i][j].Angle;
                                }
                            }
                            //坐标转换 将坐标转换为RTC坐标系坐标
                            Temp_Data.End_x = In_Data[i][j].End_x - Rtc_Cal_X;
                            Temp_Data.End_y = In_Data[i][j].End_y - Rtc_Cal_Y;
                            //追加修改的数据
                            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                        }
                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                    }
                }
            }

            //处理二次结果，合并走直线的Gts数据，下次为Rtc加工，则变动该GTS数据终点坐标为RTC加工的gts基准位置
            for (int cal = 0; cal < Result.Count; cal++)
            {
                //当前序号 数量为1、加工类型1 直线、加工方式10 GTS
                //当前+1序号 数量大于1、加工方式20 RTX
                if ((cal< Result.Count-1) && (Result[cal].Count==1) && (Result[cal][0].Type==1) && (Result[cal][0].Work == 10) && (Result[cal+1].Count >= 1) && (Result[cal+1][0].Work == 20))
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

        //将加工数据切分为RTC和GTS加工  该函数的对象是：直线、圆弧和整圆
        public List<List<Interpolation_Data>> Separate_Rtc_Gts_Limit(List<List<Interpolation_Data>> In_Data) 
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
            List<Interpolation_Data> Temp_Interpolation_List_Data = new List<Interpolation_Data>();//二级层
            Interpolation_Data Temp_Data = new Interpolation_Data();//一级层  
            decimal Delta_X = 0, Delta_Y = 0;//X、Y坐标极值差值
            decimal Rtc_Cal_X = 0, Rtc_Cal_Y = 0;//RTC坐标计算基准                  
            int i = 0;
            int j = 0;
            int m = 0;
            //初始清除
            Result.Clear();
            Temp_Interpolation_List_Data.Clear();
            Temp_Data.Empty();
            //计算极值范围
            Extreme Range_XY = new Extreme(Cal_Max_Min(In_Data));
            if ((Range_XY.Delta_X > Para_List.Parameter.Rtc_Limit.X) || (Range_XY.Delta_Y > Para_List.Parameter.Rtc_Limit.Y)) //加工覆盖范围之外，强制拆分，GTS平台移动配合RTC完成加工
            {
                //数据处理部分
                for (i = 0; i < In_Data.Count; i++)
                {
                    //清除数据
                    Temp_Interpolation_List_Data.Clear();
                    if ((In_Data[i].Count > 0) && (In_Data[i].Count < 2)) //二级层，整合元素数量小于2，说明只有一个跳刀
                    {
                        for (j = 0; j < In_Data[i].Count; j++)
                        {
                            //直线、整圆拆分，整理成GTS和RTC加工数据
                            if (In_Data[i][j].Type == 1)//直线
                            {
                                //if (In_Data[i][j].Lift_flag == 1)//抬刀标志
                                //{
                                    Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                                //}
                                //else
                                //{
                                //    //数据计算
                                //    Delta_X = Convert.ToDecimal(Math.Abs(In_Data[i].Max(o => o.End_x) - In_Data[i].Min(o => o.End_x)));//X坐标极值范围
                                //    Delta_Y = Convert.ToDecimal(Math.Abs(In_Data[i].Max(o => o.End_y) - In_Data[i].Min(o => o.End_y)));//Y坐标极值范围
                                //   //获取封闭图形中心坐标
                                //    Rtc_Cal_X = (In_Data[i].Max(o => o.End_x) + In_Data[i].Min(o => o.End_x)) / 2m;//RTC坐标X基准
                                //    Rtc_Cal_Y = (In_Data[i].Max(o => o.End_y) + In_Data[i].Min(o => o.End_y)) / 2m;//RTC坐标Y基准
                                //    //范围判断
                                //    if ((Delta_X > Para_List.Parameter.Rtc_Limit.X) || (Delta_Y > Para_List.Parameter.Rtc_Limit.Y))//X、Y坐标极值范围大于等于48mm，由GTS加工，否则由RTC加工
                                //    {
                                //        Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                                //    }
                                //    else
                                //    {
                                //        //数据清空
                                //        Temp_Data.Empty();
                                //        //数据赋值
                                //        Temp_Data = In_Data[i][j];
                                //        //强制抬刀标志：0
                                //        Temp_Data.Lift_flag = 0;
                                //        //强制加工类型为RTC
                                //        Temp_Data.Work = 20;
                                //        //RTC加工，GTS平台配合坐标
                                //        if (j == 0)
                                //        {
                                //            //GTS平台配合坐标
                                //            Temp_Data.Gts_x = Rtc_Cal_X;
                                //            Temp_Data.Gts_y = Rtc_Cal_Y;
                                //            //Rtc定位 激光加工起点坐标
                                //            Temp_Data.Rtc_x = In_Data[i][j].Start_x - Rtc_Cal_X;
                                //            Temp_Data.Rtc_y = In_Data[i][j].Start_y - Rtc_Cal_Y;
                                //        }
                                //        //RTC mark_abs直线
                                //        Temp_Data.Type = 15;
                                //        //坐标转换 将坐标转换为RTC坐标系坐标
                                //        Temp_Data.End_x = In_Data[i][j].End_x - Rtc_Cal_X;
                                //        Temp_Data.End_y = In_Data[i][j].End_y - Rtc_Cal_Y;
                                //        //追加修改的数据
                                //        Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                //        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                //    }
                                //}
                            }
                            else if (In_Data[i][j].Type == 2)// 圆弧
                            {
                                if (In_Data[i][j].Circle_radius >= 20)//圆弧半径大于等于20mm
                                {
                                    Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                                }
                                else
                                {
                                    //生成Rtc加工数据
                                    Temp_Data = In_Data[i][j];
                                    //RTC arc_abs圆弧
                                    Temp_Data.Type = 11;
                                    //强制抬刀标志：0
                                    Temp_Data.Lift_flag = 0;
                                    //强制加工类型为RTC
                                    Temp_Data.Work = 20;
                                    //RTC加工，GTS平台配合坐标
                                    Temp_Data.Gts_x = In_Data[i][j].Center_x;
                                    Temp_Data.Gts_y = In_Data[i][j].Center_y;
                                    //Rtc定位 激光加工起点坐标
                                    Temp_Data.Rtc_x = In_Data[i][j].Start_x - In_Data[i][j].Center_x;
                                    Temp_Data.Rtc_y = In_Data[i][j].Start_y - In_Data[i][j].Center_y;
                                    //RTC 圆弧加工圆心坐标转换
                                    Temp_Data.Center_x = In_Data[i][j].Center_x - Temp_Data.Gts_x;
                                    Temp_Data.Center_y = In_Data[i][j].Center_y - Temp_Data.Gts_y;
                                    //坐标转换 将坐标转换为RTC坐标系坐标
                                    Temp_Data.End_x = In_Data[i][j].End_x - In_Data[i][j].Center_x;
                                    Temp_Data.End_y = In_Data[i][j].End_y - In_Data[i][j].Center_y;
                                    //追加修改的数据
                                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                    Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                    //清空数据
                                    Temp_Data.Empty();
                                    Temp_Interpolation_List_Data.Clear();

                                    //再追加一组直线插补，让GTS平台跳至该圆弧终点
                                    Temp_Data.Type = 1;//直线插补
                                    Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                                    Temp_Data.Lift_flag = 1;//抬刀标志
                                    Temp_Data.End_x = In_Data[i][j].End_x;
                                    Temp_Data.End_y = In_Data[i][j].End_y;

                                    //追加修改的数据
                                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                    Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                    //清空数据
                                    Temp_Data.Empty();
                                    Temp_Interpolation_List_Data.Clear();
                                }

                            }
                            else if (In_Data[i][j].Type == 3)//整圆
                            {
                                //判断整圆大小
                                if (In_Data[i][j].Circle_radius >= 24) //整圆半径大于24mm，GTS加工
                                {
                                    Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                                }
                                else //整圆半径小于24mm，RTC加工
                                {
                                    //数据赋值
                                    Temp_Data = In_Data[i][j];
                                    //RTC arc_abs圆弧
                                    Temp_Data.Type = 11;
                                    //强制抬刀标志：0
                                    Temp_Data.Lift_flag = 0;
                                    //强制加工类型为RTC
                                    Temp_Data.Work = 20;
                                    //RTC加工，GTS平台配合坐标
                                    Temp_Data.Gts_x = In_Data[i][j].Center_x;
                                    Temp_Data.Gts_y = In_Data[i][j].Center_y;
                                    //RTC 圆弧加工圆心坐标转换
                                    Temp_Data.Center_x = 0;
                                    Temp_Data.Center_y = 0;
                                    //RTC加工切入点
                                    Temp_Data.End_x = Temp_Data.Center_x;
                                    Temp_Data.End_y = Temp_Data.Center_y + In_Data[i][j].Circle_radius;
                                    //RTC加工整圆角度
                                    // arc angle in ° as a 64 - bit IEEE floating point value
                                    // (positive angle values correspond to clockwise angles);
                                    // allowed range: [–3600.0° … +3600.0°] (±10 full circles);
                                    // out-of-range values will be edge-clipped.
                                    Temp_Data.Angle = 370;//这个参数得看RTC手册，整圆的旋转角度

                                    //Rtc定位 激光加工起点坐标
                                    Temp_Data.Rtc_x = Temp_Data.Center_x;
                                    Temp_Data.Rtc_y = Temp_Data.Center_y + In_Data[i][j].Circle_radius;

                                    //追加修改的数据
                                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                }
                                Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                            }
                        }
                    }
                    else if (In_Data[i].Count >= 2) //二级层，整合元素数量大于等于2，说明封闭图形
                    {
                        //整合数据加工范围判断 取Max Min,Delta_X,Delta_Y长度均在50mm以内
                        //数据计算
                        Delta_X = Convert.ToDecimal(Math.Abs(In_Data[i].Max(o => o.End_x) - In_Data[i].Min(o => o.End_x)));//X坐标极值范围
                        Delta_Y = Convert.ToDecimal(Math.Abs(In_Data[i].Max(o => o.End_y) - In_Data[i].Min(o => o.End_y)));//Y坐标极值范围
                        //获取封闭图形中心坐标
                        Rtc_Cal_X = (In_Data[i].Max(o => o.End_x) + In_Data[i].Min(o => o.End_x)) / 2m;//RTC坐标X基准
                        Rtc_Cal_Y = (In_Data[i].Max(o => o.End_y) + In_Data[i].Min(o => o.End_y)) / 2m;//RTC坐标Y基准
                        //范围判断
                        if ((Delta_X > Para_List.Parameter.Rtc_Limit.X) || (Delta_Y > Para_List.Parameter.Rtc_Limit.Y))//X、Y坐标极值范围大于等于48mm，由GTS加工，否则由RTC加工
                        {
                            //不考虑圆弧半径大小，全部由Gts加工
                            //Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                            //考虑圆弧半径大小，Radius >= 20mm由Gts加工，Radius < 20mm由Rtc加工
                            for (m = 0; m < In_Data[i].Count; m++)
                            {
                                //数据清空
                                Temp_Data.Empty();
                                //数据赋值
                                Temp_Data = In_Data[i][m];
                                //当前数据类型判断
                                if (Temp_Data.Type == 1)//直线
                                {
                                    //追加修改的数据
                                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                }
                                else if (Temp_Data.Type == 2)//圆弧
                                {
                                    if (Temp_Data.Circle_radius >= 20)//圆弧半径大于等于20mm
                                    {
                                        //追加修改的数据
                                        Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                    }
                                    else//圆弧半径小于20mm
                                    {
                                        //从起始到当前
                                        if (Temp_Interpolation_List_Data.Count > 0)
                                        {
                                            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                        }
                                        //清空数据  为生成Rtc数据做准备
                                        Temp_Interpolation_List_Data.Clear();

                                        //先计算圆心与切点的直线
                                        if (m > 0)
                                        {
                                            //生成Rtc加工数据
                                            //RTC arc_abs圆弧
                                            Temp_Data.Type = 11;
                                            //强制抬刀标志：0
                                            Temp_Data.Lift_flag = 0;
                                            //强制加工类型为RTC
                                            Temp_Data.Work = 20;
                                            //RTC加工，GTS平台配合坐标
                                            Temp_Data.Gts_x = In_Data[i][m - 1].End_x;
                                            Temp_Data.Gts_y = In_Data[i][m - 1].End_y;
                                            //Rtc定位 激光加工起点坐标
                                            Temp_Data.Rtc_x = 0;
                                            Temp_Data.Rtc_y = 0;
                                            //RTC 圆弧加工圆心坐标转换
                                            Temp_Data.Center_x = In_Data[i][m].Center_x - Temp_Data.Gts_x;
                                            Temp_Data.Center_y = In_Data[i][m].Center_y - Temp_Data.Gts_y;
                                            //坐标转换 将坐标转换为RTC坐标系坐标
                                            Temp_Data.End_x = In_Data[i][m].End_x - Temp_Data.Gts_x;
                                            Temp_Data.End_y = In_Data[i][m].End_y - Temp_Data.Gts_y;
                                            //追加修改的数据
                                            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                            //清空数据
                                            Temp_Data.Empty();
                                            Temp_Interpolation_List_Data.Clear();

                                            //再追加一组直线插补，让GTS平台跳至该圆弧终点
                                            Temp_Data.Type = 1;//直线插补
                                            Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                                            Temp_Data.Lift_flag = 1;//抬刀标志
                                            Temp_Data.End_x = In_Data[i][m].End_x;
                                            Temp_Data.End_y = In_Data[i][m].End_y;

                                            //追加修改的数据
                                            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                            //清空数据
                                            Temp_Data.Empty();
                                            Temp_Interpolation_List_Data.Clear();

                                        }
                                        else//肯定有切入点
                                        {
                                            if (i > 0)
                                            {

                                                //生成Rtc加工数据
                                                //RTC arc_abs圆弧
                                                Temp_Data.Type = 11;
                                                //强制抬刀标志：0
                                                Temp_Data.Lift_flag = 0;
                                                //强制加工类型为RTC
                                                Temp_Data.Work = 20;
                                                //RTC加工，GTS平台配合坐标
                                                Temp_Data.Gts_x = In_Data[i - 1][In_Data[i - 1].Count - 1].End_x;
                                                Temp_Data.Gts_y = In_Data[i - 1][In_Data[i - 1].Count - 1].End_y;
                                                //Rtc定位 激光加工起点坐标
                                                Temp_Data.Rtc_x = 0;
                                                Temp_Data.Rtc_y = 0;
                                                //RTC 圆弧加工圆心坐标转换
                                                Temp_Data.Center_x = In_Data[i][m].Center_x - Temp_Data.Gts_x;
                                                Temp_Data.Center_y = In_Data[i][m].Center_y - Temp_Data.Gts_y;
                                                //坐标转换 将坐标转换为RTC坐标系坐标
                                                Temp_Data.End_x = In_Data[i][m].End_x - Temp_Data.Gts_x;
                                                Temp_Data.End_y = In_Data[i][m].End_y - Temp_Data.Gts_y;
                                                //追加修改的数据
                                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                                Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                                //清空数据
                                                Temp_Data.Empty();
                                                Temp_Interpolation_List_Data.Clear();

                                                //再追加一组直线插补，让GTS平台跳至该圆弧终点
                                                Temp_Data.Type = 1;//直线插补
                                                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                                                Temp_Data.Lift_flag = 1;//抬刀标志
                                                Temp_Data.End_x = In_Data[i][m].End_x;
                                                Temp_Data.End_y = In_Data[i][m].End_y;

                                                //追加修改的数据
                                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                                Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                                //清空数据
                                                Temp_Data.Empty();
                                                Temp_Interpolation_List_Data.Clear();
                                            }
                                            else
                                            {
                                                MessageBox.Show("整合数据异常，终止！！！");
                                            }

                                        }

                                    }
                                }

                                //遍历结束
                                if ((Temp_Interpolation_List_Data.Count > 0) && (m == In_Data[i].Count - 1))
                                {
                                    Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                    //清空数据  为生成Rtc数据做准备
                                    Temp_Interpolation_List_Data.Clear();
                                }

                            }
                        }
                        else
                        {
                            for (j = 0; j < In_Data[i].Count; j++)
                            {
                                //数据清空
                                Temp_Data.Empty();
                                //数据赋值
                                Temp_Data = In_Data[i][j];
                                //强制抬刀标志：0
                                Temp_Data.Lift_flag = 0;
                                //强制加工类型为RTC
                                Temp_Data.Work = 20;
                                //RTC加工，GTS平台配合坐标
                                if (j == 0)
                                {
                                    //GTS平台配合坐标
                                    Temp_Data.Gts_x = Rtc_Cal_X;
                                    Temp_Data.Gts_y = Rtc_Cal_Y;
                                    //Rtc定位 激光加工起点坐标
                                    Temp_Data.Rtc_x = In_Data[i][0].Start_x - Rtc_Cal_X;
                                    Temp_Data.Rtc_y = In_Data[i][0].Start_y - Rtc_Cal_Y;
                                }
                                //直线、圆弧拆分，整理成RTC加工数据
                                if (Temp_Data.Type == 1)//直线
                                {
                                    //RTC mark_abs直线
                                    Temp_Data.Type = 15;
                                }
                                else if (Temp_Data.Type == 2)//圆弧
                                {
                                    //RTC arc_abs圆弧
                                    Temp_Data.Type = 11;
                                    //RTC 圆弧加工圆心坐标转换
                                    Temp_Data.Center_x = In_Data[i][j].Center_x - Rtc_Cal_X;
                                    Temp_Data.Center_y = In_Data[i][j].Center_y - Rtc_Cal_Y;
                                    if (In_Data[i][j].Circle_dir == 1)
                                    {
                                        Temp_Data.Angle = In_Data[i][j].Angle;
                                    }
                                    else if (In_Data[i][j].Circle_dir == 0)
                                    {
                                        Temp_Data.Angle = In_Data[i][j].Angle;
                                    }
                                }
                                //坐标转换 将坐标转换为RTC坐标系坐标
                                Temp_Data.End_x = In_Data[i][j].End_x - Rtc_Cal_X;
                                Temp_Data.End_y = In_Data[i][j].End_y - Rtc_Cal_Y;
                                //追加修改的数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                            }
                            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                        }
                    }
                }
            }
            else//加工覆盖范围48X48之内，完全RTC加工    角度取反暂时不允许使用
            {
                //计算封闭图形中心坐标
                Rtc_Cal_X = (Range_XY.X_Max + Range_XY.X_Min) / 2m;//RTC坐标X基准
                Rtc_Cal_Y = (Range_XY.Y_Max + Range_XY.Y_Min) / 2m;//RTC坐标Y基准
                //数据处理部分
                for (i = 0; i < In_Data.Count; i++)
                {
                    //清除数据
                    Temp_Interpolation_List_Data.Clear();
                    if ((In_Data[i].Count > 0) && (In_Data[i].Count < 2)) //二级层，整合元素数量小于2，说明只有一个跳刀
                    {
                        for (j = 0; j < In_Data[i].Count; j++)
                        {
                            //直线、整圆拆分，整理成GTS和RTC加工数据
                            if (In_Data[i][j].Type == 1)//直线
                            {
                                if (In_Data[i][j].Lift_flag == 1)//抬刀标志
                                {
                                    //数据清空
                                    Temp_Data.Empty();
                                    //数据赋值
                                    Temp_Data = In_Data[i][j];
                                    //强制加工类型为RTC
                                    Temp_Data.Work = 20;
                                    //RTC加工，GTS平台配合坐标
                                    if (j == 0)
                                    {
                                        //GTS平台配合坐标
                                        Temp_Data.Gts_x = Rtc_Cal_X;
                                        Temp_Data.Gts_y = Rtc_Cal_Y;
                                        //Rtc定位 激光加工起点坐标
                                        Temp_Data.Rtc_x = 0;
                                        Temp_Data.Rtc_y = 0;
                                    }
                                    //RTC jump_abs直线
                                    Temp_Data.Type = 13;
                                    //坐标转换 将坐标转换为RTC坐标系坐标
                                    Temp_Data.End_x = In_Data[i][j].End_x - Rtc_Cal_X;
                                    Temp_Data.End_y = In_Data[i][j].End_y - Rtc_Cal_Y;
                                    //追加修改的数据
                                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                    Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                    //Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                                }
                                else
                                {
                                    //数据清空
                                    Temp_Data.Empty();
                                    //数据赋值
                                    Temp_Data = In_Data[i][j];
                                    //强制加工类型为RTC
                                    Temp_Data.Work = 20;
                                    //RTC加工，GTS平台配合坐标
                                    if (j == 0)
                                    {
                                        //GTS平台配合坐标
                                        Temp_Data.Gts_x = Rtc_Cal_X;
                                        Temp_Data.Gts_y = Rtc_Cal_Y;
                                        //Rtc定位 激光加工起点坐标
                                        Temp_Data.Rtc_x = In_Data[i][j].Start_x - Rtc_Cal_X;
                                        Temp_Data.Rtc_y = In_Data[i][j].Start_y - Rtc_Cal_Y;
                                    }
                                    //RTC mark_abs直线
                                    Temp_Data.Type = 15;
                                    //坐标转换 将坐标转换为RTC坐标系坐标
                                    Temp_Data.End_x = In_Data[i][j].End_x - Rtc_Cal_X;
                                    Temp_Data.End_y = In_Data[i][j].End_y - Rtc_Cal_Y;
                                    //追加修改的数据
                                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                    Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                }                                
                            }
                            else if (In_Data[i][j].Type == 2)// 圆弧
                            {                                
                                //数据清空
                                Temp_Data.Empty();
                                //生成Rtc加工数据
                                Temp_Data = In_Data[i][j];
                                //RTC arc_abs圆弧
                                Temp_Data.Type = 11;
                                //强制抬刀标志：0
                                Temp_Data.Lift_flag = 0;
                                //强制加工类型为RTC
                                Temp_Data.Work = 20;
                                //RTC加工，GTS平台配合坐标
                                Temp_Data.Gts_x = Rtc_Cal_X;
                                Temp_Data.Gts_y = Rtc_Cal_Y;
                                //RTC 圆弧加工圆心坐标转换
                                Temp_Data.Center_x = In_Data[i][j].Center_x - Temp_Data.Gts_x;
                                Temp_Data.Center_y = In_Data[i][j].Center_y - Temp_Data.Gts_y;
                                //Rtc定位 激光加工起点坐标
                                Temp_Data.Rtc_x = In_Data[i][j].Start_x - Temp_Data.Gts_x;
                                Temp_Data.Rtc_y = In_Data[i][j].Start_y - Temp_Data.Gts_y;                                    
                                //坐标转换 将坐标转换为RTC坐标系坐标
                                Temp_Data.End_x = In_Data[i][j].End_x - Temp_Data.Gts_x;
                                Temp_Data.End_y = In_Data[i][j].End_y - Temp_Data.Gts_y;
                                //角度处理
                                if (In_Data[i][j].Circle_dir == 1)
                                {
                                    Temp_Data.Angle = In_Data[i][j].Angle;
                                }
                                else if (In_Data[i][j].Circle_dir == 0)
                                {
                                    Temp_Data.Angle = In_Data[i][j].Angle;
                                }
                                //追加修改的数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                //清空数据
                                Temp_Data.Empty();
                                Temp_Interpolation_List_Data.Clear();
                            }
                            else if (In_Data[i][j].Type == 3)//整圆
                            {                                
                                //数据赋值
                                Temp_Data = In_Data[i][j];
                                //RTC arc_abs圆弧
                                Temp_Data.Type = 11;
                                //强制抬刀标志：0
                                Temp_Data.Lift_flag = 0;
                                //强制加工类型为RTC
                                Temp_Data.Work = 20;
                                //RTC加工，GTS平台配合坐标
                                Temp_Data.Gts_x = Rtc_Cal_X;
                                Temp_Data.Gts_y = Rtc_Cal_Y;
                                //RTC 圆弧加工圆心坐标转换
                                Temp_Data.Center_x = In_Data[i][j].Center_x - Temp_Data.Gts_x;
                                Temp_Data.Center_y = In_Data[i][j].Center_y - Temp_Data.Gts_y;
                                //RTC加工切入点
                                Temp_Data.End_x = Temp_Data.Center_x;
                                Temp_Data.End_y = Temp_Data.Center_y + In_Data[i][j].Circle_radius;
                                //RTC加工整圆角度
                                // arc angle in ° as a 64 - bit IEEE floating point value
                                // (positive angle values correspond to clockwise angles);
                                // allowed range: [–3600.0° … +3600.0°] (±10 full circles);
                                // out-of-range values will be edge-clipped.
                                Temp_Data.Angle = 370;//这个参数得看RTC手册，整圆的旋转角度

                                //Rtc定位 激光加工起点坐标
                                Temp_Data.Rtc_x = Temp_Data.Center_x;
                                Temp_Data.Rtc_y = Temp_Data.Center_y + In_Data[i][j].Circle_radius;

                                //追加修改的数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                            }
                        }
                    }
                    else if (In_Data[i].Count >= 2) //二级层，整合元素数量大于等于2
                    {
                        for (j = 0; j < In_Data[i].Count; j++)
                        {
                            //数据清空
                            Temp_Data.Empty();
                            //数据赋值
                            Temp_Data = In_Data[i][j];
                            //强制抬刀标志：0
                            Temp_Data.Lift_flag = 0;
                            //强制加工类型为RTC
                            Temp_Data.Work = 20;
                            //RTC加工，GTS平台配合坐标
                            if (j == 0)
                            {
                                //GTS平台配合坐标
                                Temp_Data.Gts_x = Rtc_Cal_X;
                                Temp_Data.Gts_y = Rtc_Cal_Y;
                                //Rtc定位 激光加工起点坐标
                                Temp_Data.Rtc_x = In_Data[i][0].Start_x - Rtc_Cal_X;
                                Temp_Data.Rtc_y = In_Data[i][0].Start_y - Rtc_Cal_Y;
                            }
                            //直线、圆弧拆分，整理成RTC加工数据
                            if (Temp_Data.Type == 1)//直线
                            {
                                //RTC mark_abs直线
                                Temp_Data.Type = 15;
                            }
                            else if (Temp_Data.Type == 2)//圆弧
                            {
                                //RTC arc_abs圆弧
                                Temp_Data.Type = 11;
                                //RTC 圆弧加工圆心坐标转换
                                Temp_Data.Center_x = In_Data[i][j].Center_x - Rtc_Cal_X;
                                Temp_Data.Center_y = In_Data[i][j].Center_y - Rtc_Cal_Y;
                                if (In_Data[i][j].Circle_dir == 1)
                                {
                                    Temp_Data.Angle = In_Data[i][j].Angle;
                                }
                                else if (In_Data[i][j].Circle_dir == 0)
                                {                                   
                                    Temp_Data.Angle = In_Data[i][j].Angle;
                                }
                            }
                            //坐标转换 将坐标转换为RTC坐标系坐标
                            Temp_Data.End_x = In_Data[i][j].End_x - Rtc_Cal_X;
                            Temp_Data.End_y = In_Data[i][j].End_y - Rtc_Cal_Y;
                            //追加修改的数据
                            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                        }
                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));                        
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
        /***************************************************辅助函数***************************************************************/
        //计算等距线交点 
        public Vector Cal_Cross_Data(Interpolation_Data Indata_1,Interpolation_Data Indata_2)
        {
            //直线使用 矢量的单位向量 
            Vector_Calculate Vec_Cal = new Vector_Calculate();
            Vector Result = new Vector(); 
            decimal Radius = 0.0m;
            
            //钻孔、落料、无三种模式切换
            //刀具补偿类型 0-不补偿、1-钻孔 -Radius、2-落料 +Radius
            if (Para_List.Parameter.Cutter_Type==0)
            {
                Radius = 0.0m;
                Log.Info("无补偿，返回原坐标！！！");
                Result = new Vector(Indata_1.End_x, Indata_1.End_y);
                return Result;
            }
            else if (Para_List.Parameter.Cutter_Type == 1)
            {
                Radius = - Para_List.Parameter.Cutter_Radius;
            }
            else if (Para_List.Parameter.Cutter_Type == 2)
            {
                Radius = Para_List.Parameter.Cutter_Radius;
            }
            //数据处理划分
            if ((Indata_1.Type==1) && (Indata_2.Type == 1)) //直线和直线
            {
                //向量以交点为起始点，分别指向当前刀具加工方向，下一段刀具加工方向
                //使用单位矢量
                Vector Line1 = new Vector((Indata_1.End_x - Indata_1.Start_x) / (new Vector(Indata_1.End_x - Indata_1.Start_x, Indata_1.End_y - Indata_1.Start_y).Length), (Indata_1.End_y - Indata_1.Start_y) / (new Vector(Indata_1.End_x - Indata_1.Start_x, Indata_1.End_y - Indata_1.Start_y).Length));
                Vector Line2 = new Vector((Indata_2.End_x - Indata_2.Start_x) / (new Vector(Indata_2.End_x - Indata_2.Start_x, Indata_2.End_y - Indata_2.Start_y).Length), (Indata_2.End_y - Indata_2.Start_y) / (new Vector(Indata_2.End_x - Indata_2.Start_x, Indata_2.End_y - Indata_2.Start_y).Length));
                decimal Angle = Vec_Cal.AngleBetweenVector(Line1, Line2);
                //计算等距线交点
                if (Angle == 180) //角度为180,共线且方向相反
                {
                    Log.Info("两直线所在向量夹角为180，沿用原坐标！！！");
                    Result = new Vector(Indata_1.End_x, Indata_1.End_y);
                }
                else if ((Angle == 0) || (Angle == 360)) //角度为0 或360,共线同向
                {
                    Result = new Vector(Indata_1.End_x - Radius * Line2.Y, Indata_1.End_y+ Radius * Line2.X);
                }
                else
                {
                    Result = new Vector(Indata_1.End_x + Radius * (Line2.X - Line1.X) / (Line1.X * Line2.Y - Line2.X * Line1.Y), Indata_1.End_y + Radius * (Line2.Y - Line1.Y) / (Line1.X * Line2.Y - Line2.X * Line1.Y));
                }
            }
            else if ((Indata_1.Type == 1) && (Indata_2.Type == 2))//直线和圆弧
            {
                //向量以交点为起始点，分别指向当前刀具加工方向，下一段刀具加工方向
                //使用单位矢量
                Vector Line = new Vector((Indata_1.End_x - Indata_1.Start_x) / (new Vector(Indata_1.End_x - Indata_1.Start_x, Indata_1.End_y - Indata_1.Start_y).Length), (Indata_1.End_y - Indata_1.Start_y) / (new Vector(Indata_1.End_x - Indata_1.Start_x, Indata_1.End_y - Indata_1.Start_y).Length));
                //保持坐标值 矢量
                Vector Center_Start = new Vector(Indata_2.Center_x - Indata_2.Start_x, Indata_2.Center_y - Indata_2.Start_y);
                //圆弧方向  顺圆弧、逆圆弧
                decimal R = 0.0m;
                if (Indata_2.Circle_dir == 0)
                {
                    R = -Indata_2.Circle_radius;
                }
                else
                {
                    R = Indata_2.Circle_radius;
                }
                //圆弧方向矢量 
                Vector Tangent_Line = new Vector(-Center_Start.Y / R, Center_Start.X / R);
                //decimal F =(decimal)Math.Sqrt((double)((R + Radius) * (R + Radius) - (Tangent_Line.Y * Line.X - Tangent_Line.X * Line.Y - Radius) * (Tangent_Line.Y * Line.X - Tangent_Line.X * Line.Y - Radius)));
                //计算(直线矢量 与 圆弧径向矢量) 夹角
                decimal Angle_Line_Radial = Vec_Cal.AngleBetweenVector(Line, Center_Start);
                //计算(直线矢量 与 圆弧切线矢量) 夹角
                decimal Angle_Line_Tangent = Vec_Cal.AngleBetweenVector(Line, Tangent_Line);
                //目前只考虑 外切
                if (Angle_Line_Tangent == 180) //角度为180,共线且方向相反
                {
                    Log.Info("直线与圆弧切线方向夹角为 180，沿用原坐标！！！");
                    Result = new Vector(Indata_1.End_x - Radius * Tangent_Line.Y, Indata_1.End_y + Radius * Tangent_Line.X);                   
                }
                else if ((Angle_Line_Tangent == 0) || (Angle_Line_Tangent == 360)) //角度为0 或360,共线同向
                {
                    Result = new Vector(Indata_1.End_x, Indata_1.End_y);
                }
                else
                {
                    Log.Info("直线与圆弧非外切，沿用原坐标！！！");
                    Result = new Vector(Indata_1.End_x, Indata_1.End_y);
                }
            }
            else if ((Indata_1.Type == 2) && (Indata_2.Type == 1))//圆弧和直线
            {
                //向量以交点为起始点，分别指向当前刀具加工方向，下一段刀具加工方向
                //使用单位矢量
                Vector Line = new Vector((Indata_2.End_x - Indata_2.Start_x) / (new Vector(Indata_2.End_x - Indata_2.Start_x, Indata_2.End_y - Indata_2.Start_y).Length), (Indata_2.End_y - Indata_2.Start_y) / (new Vector(Indata_2.End_x - Indata_2.Start_x, Indata_2.End_y - Indata_2.Start_y).Length));
                //保持坐标值 矢量
                Vector Center_Start = new Vector(Indata_1.Center_x - Indata_1.Start_x, Indata_1.Center_y - Indata_1.Start_y);
                Vector Line_01 = new Vector(Indata_1.Start_x + Center_Start.X - Indata_2.Start_x, Indata_1.Start_y + Center_Start.Y - Indata_2.Start_y);
                //圆弧方向  顺圆弧、逆圆弧
                decimal R = 0.0m;
                if (Indata_1.Circle_dir == 0)
                {
                    R = -Indata_1.Circle_radius;
                }
                else
                {
                    R = Indata_1.Circle_radius;
                }
                //圆弧方向矢量 
                Vector Tangent_Line = new Vector(- Line_01.Y / R, Line_01.X / R);
                //decimal F = (decimal)Math.Sqrt((double)((R + Radius) * (R + Radius) - (Tangent_Line.Y * Line.X - Tangent_Line.X * Line.Y - Radius) * (Tangent_Line.Y * Line.X - Tangent_Line.X * Line.Y - Radius)));
                //计算(圆弧径向矢量 与 直线矢量) 夹角
                decimal Angle_Line_Radial = Vec_Cal.AngleBetweenVector(Center_Start, Line);
                //计算(圆弧切线的矢量 与 直线矢量) 夹角
                decimal Angle_Line_Tangent = Vec_Cal.AngleBetweenVector(Tangent_Line, Line);
                //目前只考虑 外切
                if (Angle_Line_Tangent == 180) //角度为180,共线且方向相反
                {
                    Log.Info("圆弧切线方向与直线夹角为 180，沿用原坐标！！！");
                    Result = new Vector(Indata_1.End_x - Radius * Tangent_Line.Y, Indata_1.End_y + Radius * Tangent_Line.X);
                }
                else if ((Angle_Line_Tangent == 0) || (Angle_Line_Tangent == 360)) //角度为0 或360,共线同向
                {
                    Result = new Vector(Indata_1.End_x, Indata_1.End_y);
                }
                else
                {
                    Log.Info("圆弧与直线非外切，沿用原坐标！！！");
                    Result = new Vector(Indata_1.End_x, Indata_1.End_y);
                }
            }
            else if ((Indata_1.Type == 2) && (Indata_2.Type == 2))//圆弧和圆弧
            {
                //圆弧1的矢量
                //保持坐标值 矢量
                Vector Center_Start_01 = new Vector(Indata_1.Center_x - Indata_1.Start_x, Indata_1.Center_y - Indata_1.Start_y);
                Vector Line_01 = new Vector(Indata_1.Start_x + Center_Start_01.X - Indata_1.End_x, Indata_1.Start_y + Center_Start_01.Y - Indata_1.End_y);
                //圆弧方向  顺圆弧、逆圆弧
                decimal R1 = 0.0m;
                if (Indata_1.Circle_dir == 0) 
                {
                    R1 = -Indata_1.Circle_radius;
                }
                else
                {
                    R1 = Indata_1.Circle_radius;
                }
                //圆弧1切线矢量 
                Vector Tangent_Line_01 = new Vector(-Line_01.Y / R1, Line_01.X / R1);

                //圆弧2的矢量
                //保持坐标值 矢量
                Vector Center_Start_02 = new Vector(Indata_2.Center_x - Indata_2.Start_x, Indata_2.Center_y - Indata_2.Start_y);
                //圆弧方向  顺圆弧、逆圆弧
                decimal R2 = 0.0m;
                if (Indata_2.Circle_dir == 0)
                {
                    R2 = -Indata_2.Circle_radius;
                }
                else
                {
                    R2 = Indata_2.Circle_radius;
                }
                //圆弧2切线矢量 
                Vector Tangent_Line_02 = new Vector(Center_Start_02.X, Center_Start_02.Y);  

                Log.Info("圆弧与圆弧 数据不处理，沿用原坐标！！！");
                Result = new Vector(Indata_1.End_x, Indata_1.End_y);
            }  
            //结果返回
            return Result;
        }

        //计算极值范围
        public Extreme Cal_Max_Min(List<List<Interpolation_Data>> In_Data)
        {
            List<decimal> Tem_Dat_X = new List<decimal>();
            List<decimal> Tem_Dat_Y = new List<decimal>();
            for (int i = 0;i< In_Data.Count;i++)
            {
                Tem_Dat_X.Add(In_Data[i].Max(o => o.End_x));
                Tem_Dat_X.Add(In_Data[i].Min(o => o.End_x));
                Tem_Dat_Y.Add(In_Data[i].Max(o => o.End_y));
                Tem_Dat_Y.Add(In_Data[i].Min(o => o.End_y));
            }
            return new Extreme(Tem_Dat_X.Max(), Tem_Dat_X.Min(), Tem_Dat_Y.Max(), Tem_Dat_Y.Min(),Math.Abs(Tem_Dat_X.Max() - Tem_Dat_X.Min()), Math.Abs(Tem_Dat_Y.Max() - Tem_Dat_Y.Min()));
        }
        //反序列化 标定板标定数据 
        private List<Affinity_Matrix> Reserialize_Affinity_Matrix(string fileName)
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
        //生成RTC校准数据
        public List<List<Interpolation_Data>> Generate_Calibration_Data(decimal Radius, decimal Interval)
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

            //坐标原点 2.5半径的圆圈 X+ 2号圆
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
            Temp_Data.Rtc_x = (Interval + Radius);
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

            //坐标原点 2.5半径的圆圈 X- 3号圆
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
            Temp_Data.Rtc_x = -(Interval + Radius);
            Temp_Data.Rtc_y = 0;
            //RTC arc_abs圆弧
            Temp_Data.Type = 11;
            //RTC 圆弧加工圆心坐标转换
            Temp_Data.Center_x = -Interval;
            Temp_Data.Center_y = 0;
            //圆弧角度
            Temp_Data.Angle = 370;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //坐标原点 2.5半径的圆圈 Y+ 4号圆
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
            Temp_Data.Rtc_x = 0;
            Temp_Data.Rtc_y = (Interval + Radius); ;
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

            //坐标原点 2.5半径的圆圈 Y- 5号圆
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
            Temp_Data.Rtc_x = 0;
            Temp_Data.Rtc_y = -(Interval + Radius); ;
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

            //返回结果
            return Result;
        }        
        //坐标误差容许判断
        private bool Differ_Err(decimal x1, decimal y1, decimal x2, decimal y2)
        {
            if (Math.Sqrt((double)((x1 - x2) * (x1 - x2)) + (double)((y1 - y2) * (y1 - y2))) <= (double)Para_List.Parameter.Pos_Tolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
            //if ((x1 == x2) && (y1 == y2))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
        //坐标误差容许判断
        private bool Differ_Err(Vector Point1, Vector Point2)
        {
            if ((Point1.X == Point2.X) && (Point1.Y == Point2.Y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
       
        
        //角度补偿 放置角度，偏移量　Entity数据处理　自定义仿射变换系数
        public List<Interpolation_Data> Compensation_Seperate(List<Interpolation_Data> In_Data, Affinity_Rate Rate)
        {
            decimal Arc = Convert.ToDecimal(Math.PI) * (Rate.Angle / 180.0m);
            decimal Cos_Arc = Convert.ToDecimal(Math.Cos(Convert.ToDouble(Arc)));
            decimal Sin_Arc = Convert.ToDecimal(Math.Sin(Convert.ToDouble(Arc)));

            List<Interpolation_Data> Result = new List<Interpolation_Data>();
            Interpolation_Data Temp_interpolation_Data = new Interpolation_Data();
            for (int i = 0; i < In_Data.Count; i++)
            {

                //数据处理
                Temp_interpolation_Data.Empty();
                Temp_interpolation_Data = In_Data[i];
                Temp_interpolation_Data.End_x = In_Data[i].End_x * Cos_Arc - In_Data[i].End_y * Sin_Arc + Rate.Delta_X;//坐标原点的坐标X
                Temp_interpolation_Data.End_y = In_Data[i].End_y * Cos_Arc + In_Data[i].End_x * Sin_Arc + Rate.Delta_Y;//坐标原点的坐标Y
                Temp_interpolation_Data.Center_Start_x = In_Data[i].Center_Start_x * Cos_Arc - In_Data[i].Center_Start_y * Sin_Arc;
                Temp_interpolation_Data.Center_Start_y = In_Data[i].Center_Start_y * Cos_Arc + In_Data[i].Center_Start_x * Sin_Arc;

                Result.Add(Temp_interpolation_Data);
            }
            return Result;
        }
        //角度补偿 放置角度，偏移量　整合数据处理　自定义仿射变换系数
        public List<List<Interpolation_Data>> Compensation_Integrate(List<List<Interpolation_Data>> In_Data, Affinity_Rate Rate)
        {
            decimal Arc = Convert.ToDecimal(Math.PI) * (Rate.Angle / 180.0m);
            decimal Cos_Arc = Convert.ToDecimal(Math.Cos(Convert.ToDouble(Arc)));
            decimal Sin_Arc = Convert.ToDecimal(Math.Sin(Convert.ToDouble(Arc)));

            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
            List<Interpolation_Data> Temp_interpolation_List_Data = new List<Interpolation_Data>();//二级层
            Interpolation_Data Temp_interpolation_Data = new Interpolation_Data();//一级层            

            for (int i = 0; i < In_Data.Count; i++)
            {
                Temp_interpolation_List_Data.Clear();
                for (int j = 0; j < In_Data[i].Count; j++)
                {
                    //数据处理
                    Temp_interpolation_Data.Empty();
                    Temp_interpolation_Data = In_Data[i][j];
                    Temp_interpolation_Data.End_x = In_Data[i][j].End_x * Cos_Arc - In_Data[i][j].End_y * Sin_Arc + Rate.Delta_X;//相对于坐标原点的坐标X
                    Temp_interpolation_Data.End_y = In_Data[i][j].End_y * Cos_Arc + In_Data[i][j].End_x * Sin_Arc + Rate.Delta_Y;//相对于坐标原点的坐标Y
                    Temp_interpolation_Data.Center_Start_x = In_Data[i][j].Center_Start_x * Cos_Arc - In_Data[i][j].Center_Start_y * Sin_Arc;
                    Temp_interpolation_Data.Center_Start_y = In_Data[i][j].Center_Start_y * Cos_Arc + In_Data[i][j].Center_Start_x * Sin_Arc;
                    Temp_interpolation_List_Data.Add(Temp_interpolation_Data);
                }
                Result.Add(new List<Interpolation_Data>(Temp_interpolation_List_Data));
            }
            return Result;
        }
    }
}
