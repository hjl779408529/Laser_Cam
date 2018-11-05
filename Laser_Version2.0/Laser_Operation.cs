using Communication.IO.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laser_Version2._0
{
    struct Laser_CC_Data
    {
        public string RW;//读写标志：Read和Write (01-read；00-write),Response for Read和Write (03-for read；02-for write)；
        public string DataSize;//数据大小 ASCII:0-255；度取命令时始终为0
        public string Address;//地址
        public string Com_Control;//控制命令
        public string Data;//数据
        public string Crc;//CRC校验值
        public string Sum;//数据整合
        public UInt16[] Rec;//接收数据，指向数组的头部的指针
        public byte[] Rec_Byte;//接收数据byte，指向数组的头部的指针

        public void Empty()
        {
            RW = null;//读写标志：Read和Write (01-read；00-write),Response for Read和Write (03-for read；02-for write)；
            DataSize = null;//数据大小 ASCII:0-255；
            Address = null;//地址
            Com_Control = null;//控制命令
            Data = null;//数据
            Crc = null;//CRC校验值
            Sum = null;//数据整合
            Rec = null;
            Rec_Byte = null;
        }
    }
    
    class Laser_Operation
    {
        // Crc Computation Class
        private CRCTool compCRC = new CRCTool();
        public Laser_CC_Data Resolve_Rec = new Laser_CC_Data();//接收数据解析        
        //构造函数
        public Laser_Operation()
        {
            
        }        
        //读取数据
        public void Read(string Address,string CC)//读取数据没有D1-Dn
        {
            Laser_CC_Data CC_Data = new Laser_CC_Data();
            CC_Data.RW = "01";//读取标志
            CC_Data.DataSize = "00";//读取数据，DataSize大小强制为0
            CC_Data.Address = Address;//地址
            CC_Data.Com_Control = CC;//控制指令
            //整合指令
            CC_Data.Sum = CC_Data.RW + CC_Data.DataSize + CC_Data.Address + CC_Data.Com_Control + CC_Data.Data;
            //MessageBox.Show(CC_Data.Sum);
            //发送数据
            Initialization.Initial.Laser_Control_Com.Send_Data(CC_Data.Sum);
            //等待数据读取完成
            Thread.Sleep(200);
        }
        //写入数据
        public void Write(string Address, string CC,string Data)//写入数据，这就包含写入数据的参数：D1-Dn 
        {
            Laser_CC_Data CC_Data = new Laser_CC_Data();
            CC_Data.RW = "00";//写入标志
            CC_Data.DataSize = Cal_Data_Size(Convert.ToUInt32(Data.Length/2));//写入数据，DataSize
            CC_Data.Address = Address;//地址
            CC_Data.Com_Control = CC;//控制指令
            CC_Data.Data = Data;//数据
            //整合指令
            CC_Data.Sum = CC_Data.RW + CC_Data.DataSize + CC_Data.Address + CC_Data.Com_Control + CC_Data.Data;
            //MessageBox.Show(CC_Data.Sum);
            //发送数据
            Initialization.Initial.Laser_Control_Com.Send_Data(CC_Data.Sum);
            //等待数据读取完成
            Thread.Sleep(200);
        }
        //将数值10进制转16进制，再将16进制转换为字符串返回 中心是byte转化为ASCII
        public string Cal_Data_Size(UInt32 Num) 
        {
            string tempStr = null;
            if (Num <= 255)
            {
                tempStr = string.Format("{0:X2}", Num);
            }
            else
            {
                MessageBox.Show("发送数据长度异常！！！");
                return tempStr;
            }
            return tempStr;
        }
        //将数值10进制转16进制，再将16进制转换为字符串返回 中心是byte转化为ASCII  5000 - 1388
        public string Uint_To_Str(UInt32 Num) 
        {
            string tempStr = null;
            tempStr = string.Format("{0:X4}", Num);
            if (tempStr.Length == 3)
            {
                tempStr = "0" + tempStr;
            }
            else if (tempStr.Length == 2)
            {
                tempStr = "00" + tempStr;
            }
            else if (tempStr.Length == 1)
            {
                tempStr = "000" + tempStr;
            }
            else if (tempStr.Length == 0)
            {
                tempStr = "0000";
            }
            return tempStr;
        }
        //将数值10进制转16进制，再将16进制转换为字符串返回 中心是byte转化为ASCII  5000 - 1388
        public string PRF_To_Str(UInt32 Num) 
        {
            string tempStr = null;
            tempStr = string.Format("{0:X4}", Num);
            //MessageBox.Show(tempStr);
            if (tempStr.Length==8)
            {
                tempStr = tempStr.Substring(1);
                tempStr = tempStr.Substring(1);
            }
            else if (tempStr.Length == 7)
            {
                tempStr = tempStr.Substring(1);
            }
            else if (tempStr.Length == 5)
            {
                tempStr = "0" + tempStr;
            }
            else if (tempStr.Length == 4)
            {
                tempStr = "00" + tempStr;
            }
            else if (tempStr.Length == 3)
            {
                tempStr = "000" + tempStr;
            }
            //补位
            if ((tempStr.Length % 2) != 0) tempStr = "0" + tempStr;
            //MessageBox.Show(tempStr);
            return tempStr;
        }
        public void Resolve_Com_Data()
        {
            byte[] Rec_Data = new byte[Initialization.Initial.Laser_Control_Com.Receive_Byte.Length];
            Rec_Data = (byte[])Initialization.Initial.Laser_Control_Com.Receive_Byte.Clone();
            //清空数据
            Resolve_Rec.Empty();
            //数据拆分
            if (Rec_Data.Length >= 13)
            {
                Resolve_Rec.RW = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[0], Rec_Data[1] });
                Resolve_Rec.DataSize = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[2], Rec_Data[3] });
                Resolve_Rec.Address = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[4], Rec_Data[5] });
                Resolve_Rec.Com_Control = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[6], Rec_Data[7] });
            }
            //检查格式
            if ((Resolve_Rec.RW == "03") || (Resolve_Rec.RW == "02")) //03-Read,02-Write
            {
                if (uint.TryParse(Resolve_Rec.DataSize, out uint tmp))//接收数据大小
                {
                    if ((13 + tmp * 2) == Rec_Data.Length)//校验长度
                    {
                        if (tmp > 0)//DataSize>0 
                        {
                            for (int i = 0; i < tmp * 2; i++)
                            {
                                Resolve_Rec.Data = Resolve_Rec.Data + System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[8 + i] });//获取Data数据
                            }
                            Resolve_Rec.Rec = Str_To_Uint16(Resolve_Rec.Data);//获取D1...Dn Uinte16格式
                            Resolve_Rec.Rec_Byte = StrToHexByte(Resolve_Rec.Data);//获取D1...Dn Byte值
                            Array.Reverse(Resolve_Rec.Rec_Byte);
                            Resolve_Rec.Crc = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[8 + tmp * 2], Rec_Data[8 + tmp * 2 + 1], Rec_Data[8 + tmp * 2 + 2], Rec_Data[8 + tmp * 2 + 3] });//获取Crc校验
                        }
                        else
                        {
                            Resolve_Rec.Data = null;
                            Resolve_Rec.Crc = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[8 + tmp * 2], Rec_Data[8 + tmp * 2 + 1], Rec_Data[8 + tmp * 2 + 2], Rec_Data[8 + tmp * 2 + 3] });//获取Crc校验
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                //接收数据组合
                Resolve_Rec.Sum = Resolve_Rec.RW + Resolve_Rec.DataSize + Resolve_Rec.Address + Resolve_Rec.Com_Control + Resolve_Rec.Data;
                //校验数据完整性
                if (Crc_Append_Str((ushort)compCRC.Check_Sum(StrToHexByte(Resolve_Rec.Sum))) == Resolve_Rec.Crc)
                {
                    Prompt.Log.Info("激光控制接收数据 校验OK！！！");
                }
            }
            else
            {
                Prompt.Log.Info("激光控制接收数据格式异常！！！");
            }
        }
        //将string转换为Uint16
        public UInt16[] Str_To_Uint16(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0) hexString = " " + hexString;
            UInt16[] Result = new UInt16[hexString.Length / 2];
            for (int i = 0; i < Result.Length; i++)
            {
                if (UInt16.TryParse(hexString.Substring(i * 2, 2).Replace(" ", ""), out UInt16 tmp))//判断是否可以转换
                {
                    Result[i] = tmp;
                }
            }
            return Result;
        }
        //追加校准值
        public string Crc_Append_Str(UInt32 Num)
        {
            string Result = null;
            Result = string.Format("{0:X4}", Num);
            //MessageBox.Show(Result);
            return Result;
        }
        //Hex字符串转换16进制字节数组 只支持为16进制数字的字符串
        public byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0) hexString = " " + hexString;
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Replace(" ", ""), 16);
            return returnBytes;
        }
        //激光功率写入
        public void Change_Pec(decimal pec)
        {
            if (Math.Abs(pec) < 100)
            {
                Write("00", "55", Uint_To_Str((UInt16)(pec * 10)));
            }
            else
            {
                MessageBox.Show("激光功率设置值超出允许范围！！！");
            }
        }

        
    }
}
