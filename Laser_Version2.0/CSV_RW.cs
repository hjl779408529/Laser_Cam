using Laser_Build_1._0;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laser_Version2._0
{
    class CSV_RW
    {
        /// <summary>
        /// 将DataTable中数据写入到CSV文件中
        /// </summary>
        /// <param name="dt">提供保存数据的DataTable</param>
        /// <param name="fileName">CSV的文件路径</param>
        public static void SaveCSV(DataTable dt, string fileName) 
        {
            string sdatetime = DateTime.Now.ToString("D");
            string[] tmp = fileName.Split('.');
            string fullPath = @"./\Config/" + tmp[0] + " " + sdatetime + ".csv";
            FileInfo fi = new FileInfo(fullPath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "";
            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);
            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string str = dt.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains(',') || str.Contains('"')
                        || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }
        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCSV(string filePath)
        {
            
            Encoding encoding = EncodingType.GetType(filePath); //获取编码格式
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            StreamReader sr = new StreamReader(fs, encoding);
            //string fileContent = sr.ReadToEnd();
            //encoding = sr.CurrentEncoding;
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                //strLine = Common.ConvertStringUTF8(strLine, encoding);
                //strLine = Common.ConvertStringUTF8(strLine);
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }
            sr.Close();
            fs.Close();
            return dt;
        }
        /// <summary>
        /// 将 datatable 转换为 List<Correct_Data> 数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fullPath"></param>
        public static List<Correct_Data> DataTable_Correct_Data(DataTable New_Data) 
        {
            List<Correct_Data> Result = new List<Correct_Data>();
            if (New_Data.Columns.Count == 4) //确定数据格式是否合适
            {
                for (int i = 0; i < New_Data.Rows.Count; i++)
                {                    
                    if ((decimal.TryParse(New_Data.Rows[i][0].ToString(), out decimal x0)) && (decimal.TryParse(New_Data.Rows[i][1].ToString(), out decimal y0)) && (decimal.TryParse(New_Data.Rows[i][2].ToString(), out decimal xm)) && (decimal.TryParse(New_Data.Rows[i][3].ToString(), out decimal ym)))
                    {
                        Result.Add(new Correct_Data(x0, y0, xm, ym));
                    }
                }
            }
            else
            {
                MessageBox.Show("数据格式异常！！！");
            }
            return Result;

        }
        /// <summary>
        /// 将 List<Correct_Data> 转换为 datatable 数据
        /// </summary>
        /// <param name="New_Data"></param>
        /// <returns></returns>
        public static DataTable Correct_Data_DataTable(List<Correct_Data> New_Data)
        {
            DataTable Result = new DataTable();
            Result.Columns.Add("实测值X", typeof(decimal));//添加列
            Result.Columns.Add("实测值Y", typeof(decimal));
            Result.Columns.Add("标准值X", typeof(decimal));
            Result.Columns.Add("标准值Y", typeof(decimal));
            //数据赋值
            for (int i = 0; i < New_Data.Count; i++)
            {
                Result.Rows.Add(new object[] { New_Data[i].Xo, New_Data[i].Yo, New_Data[i].Xm, New_Data[i].Ym });
            }
            //结果返回
            return Result;
        }

        /// <summary>
        /// 将 datatable 转换为 List<Affinity_Matrix> 数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fullPath"></param>
        public static List<Affinity_Matrix> DataTable_Affinity_Matrix(DataTable New_Data)
        {
            List<Affinity_Matrix> Result = new List<Affinity_Matrix>();
            if (New_Data.Columns.Count == 6) //确定数据格式是否合适
            {
                for (int i = 0; i < New_Data.Rows.Count; i++)
                {

                    if ((decimal.TryParse(New_Data.Rows[i][0].ToString(), out decimal Stretch_X)) && (decimal.TryParse(New_Data.Rows[i][1].ToString(), out decimal Distortion_X)) && (decimal.TryParse(New_Data.Rows[i][2].ToString(), out decimal Delta_X)) && (decimal.TryParse(New_Data.Rows[i][3].ToString(), out decimal Stretch_Y)) && (decimal.TryParse(New_Data.Rows[i][4].ToString(), out decimal Distortion_Y)) && (decimal.TryParse(New_Data.Rows[i][5].ToString(), out decimal Delta_Y)))
                    {
                        Result.Add(new Affinity_Matrix(Stretch_X, Distortion_X, Delta_X, Stretch_Y, Distortion_Y, Delta_Y));
                    }
                }
            }
            else
            {
                MessageBox.Show("数据格式异常！！！");
            }
            return Result;
        }
        /// <summary>
        /// 将 List<Affinity_Matrix> 转换为 datatable 数据
        /// </summary>
        /// <param name="New_Data"></param>
        /// <returns></returns>
        public static DataTable Affinity_Matrix_DataTable(List<Affinity_Matrix> New_Data)
        {
            DataTable Result = new DataTable();
            Result.Columns.Add("Stretch_X", typeof(decimal));//添加列
            Result.Columns.Add("Distortion_X", typeof(decimal));
            Result.Columns.Add("Delta_X", typeof(decimal));
            Result.Columns.Add("Stretch_Y", typeof(decimal));//添加列
            Result.Columns.Add("Distortion_Y", typeof(decimal));
            Result.Columns.Add("Delta_Y", typeof(decimal));
            //数据赋值
            for (int i = 0; i < New_Data.Count; i++)
            {
                Result.Rows.Add(new object[] { New_Data[i].Stretch_X, New_Data[i].Distortion_X, New_Data[i].Delta_X, New_Data[i].Stretch_Y, New_Data[i].Distortion_Y, New_Data[i].Delta_Y });
            }
            //结果返回
            return Result;
        }
        /// <summary>
        /// 将 datatable 转换为 List<Double_Fit_Data> 
        /// </summary>
        /// <param name="New_Data"></param>
        /// <returns></returns>
        public static List<Double_Fit_Data> DataTable_Double_Fit_Data(DataTable New_Data) 
        {
            List<Double_Fit_Data> Result = new List<Double_Fit_Data>();
            if (New_Data.Columns.Count == 10) //确定数据格式是否合适
            {
                for (int i = 0; i < New_Data.Rows.Count; i++)
                {

                    if ((decimal.TryParse(New_Data.Rows[i][0].ToString(), out decimal K_X1)) && (decimal.TryParse(New_Data.Rows[i][1].ToString(), out decimal K_X2)) && (decimal.TryParse(New_Data.Rows[i][2].ToString(), out decimal K_X3)) && (decimal.TryParse(New_Data.Rows[i][3].ToString(), out decimal K_X4)) && (decimal.TryParse(New_Data.Rows[i][4].ToString(), out decimal Delta_X)) && (decimal.TryParse(New_Data.Rows[i][5].ToString(), out decimal K_Y1)) && (decimal.TryParse(New_Data.Rows[i][6].ToString(), out decimal K_Y2)) && (decimal.TryParse(New_Data.Rows[i][7].ToString(), out decimal K_Y3)) && (decimal.TryParse(New_Data.Rows[i][8].ToString(), out decimal K_Y4)) && (decimal.TryParse(New_Data.Rows[i][9].ToString(), out decimal Delta_Y)))
                    {
                        Result.Add(new Double_Fit_Data(K_X1, K_X2, K_X3, K_X4, Delta_X, K_Y1, K_Y2, K_Y3, K_Y4, Delta_Y));
                    }
                }
            }
            else
            {
                MessageBox.Show("数据格式异常！！！");
            }
            return Result;
        }
        /// <summary>
        /// 将 List<Double_Fit_Data> 转换为 datatable 
        /// </summary>
        /// <param name="New_Data"></param>
        /// <returns></returns>
        public static DataTable Double_Fit_Data_DataTable(List<Double_Fit_Data> New_Data)
        {
            DataTable Result = new DataTable();
            Result.Columns.Add("X轴 1次系数", typeof(decimal));//添加列
            Result.Columns.Add("X轴 2次系数", typeof(decimal));
            Result.Columns.Add("X轴 3次系数", typeof(decimal));
            Result.Columns.Add("X轴 4次系数", typeof(decimal));
            Result.Columns.Add("X轴 常数", typeof(decimal));
            Result.Columns.Add("Y轴 1次系数", typeof(decimal));
            Result.Columns.Add("Y轴 2次系数", typeof(decimal));
            Result.Columns.Add("Y轴 3次系数", typeof(decimal));
            Result.Columns.Add("Y轴 4次系数", typeof(decimal));
            Result.Columns.Add("Y轴 常数", typeof(decimal));
            //数据赋值
            for (int i = 0; i < New_Data.Count; i++)
            {
                Result.Rows.Add(new object[] { New_Data[i].K_X1, New_Data[i].K_X2, New_Data[i].K_X3, New_Data[i].K_X4, New_Data[i].Delta_X, New_Data[i].K_Y1, New_Data[i].K_Y2, New_Data[i].K_Y3, New_Data[i].K_Y4, New_Data[i].Delta_Y });
            }
            //结果返回
            return Result;
        }
        /// <summary>
        /// 将 datatable 转换为 List<Fit_Data> 
        /// </summary>
        /// <param name="New_Data"></param>
        /// <returns></returns>
        public static List<Fit_Data> DataTable_Fit_Data(DataTable New_Data) 
        {
            List<Fit_Data> Result = new List<Fit_Data>();
            if (New_Data.Columns.Count == 10) //确定数据格式是否合适
            {
                for (int i = 0; i < New_Data.Rows.Count; i++)
                {
                    if ((decimal.TryParse(New_Data.Rows[i][0].ToString(), out decimal K1)) && (decimal.TryParse(New_Data.Rows[i][1].ToString(), out decimal K2)) && (decimal.TryParse(New_Data.Rows[i][2].ToString(), out decimal K3)) && (decimal.TryParse(New_Data.Rows[i][3].ToString(), out decimal K4)) && (decimal.TryParse(New_Data.Rows[i][4].ToString(), out decimal Delta)))
                    {
                        Result.Add(new Fit_Data(K1, K2, K3, K4, Delta));
                    }
                }
            }
            else
            {
                MessageBox.Show("数据格式异常！！！");
            }
            return Result;

        }
        /// <summary>
        /// 将 List<Fit_Data> 转换为 datatable 
        /// </summary>
        /// <param name="New_Data"></param>
        /// <returns></returns>
        public static DataTable Fit_Data_DataTable(List<Fit_Data> New_Data)
        {
            DataTable Result = new DataTable();
            Result.Columns.Add("1次系数", typeof(decimal));//添加列
            Result.Columns.Add("2次系数", typeof(decimal));
            Result.Columns.Add("3次系数", typeof(decimal));
            Result.Columns.Add("4次系数", typeof(decimal));
            Result.Columns.Add("常数", typeof(decimal));
            //数据赋值
            for (int i = 0; i < New_Data.Count; i++)
            {
                Result.Rows.Add(new object[] { New_Data[i].K1, New_Data[i].K2, New_Data[i].K3, New_Data[i].K4, New_Data[i].Delta});
            }
            //结果返回
            return Result;
        }

    }

    //编码问题目前为止，基本上没人解决，就连windows的IE的自动识别有时还识别错编码呢。--yongfa365   
    //如果文件有BOM则判断，如果没有就用系统默认编码，缺点：没有BOM的非系统编码文件会显示乱码。   
    //调用方法： EncodingType.GetType(filename) 
    public class EncodingType    
    {
        /// <summary>
        /// 获取文件编码格式
        /// </summary>
        /// <param name="FILE_NAME"></param>
        /// <returns></returns>
        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            System.Text.Encoding r = GetType(fs);
            fs.Close();
            return r;
        }
        /// <summary> 
        /// 通过给定的文件流，判断文件的编码类型 
        /// </summary> 
        /// <param name=“fs“>文件流</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
            Encoding reVal = Encoding.Default;
            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int.TryParse(fs.Length.ToString(), out int i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// <summary> 
        /// 判断是否是不带 BOM 的 UTF8 格式 
        /// </summary> 
        /// <param name=“data“></param> 
        /// <returns></returns> 
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
            byte curByte; //当前分析的字节. 
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前 
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }

    }
}

