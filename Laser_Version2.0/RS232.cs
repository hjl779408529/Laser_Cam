using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communication.IO.Tools;

namespace Laser_Version2._0
{
    public delegate void Receive_Delegate();
    class RS232
    {
        //串口端口
        public SerialPort ComDevice = new SerialPort();
        //设备串口名List
        public List<string> PortName = new List<string>();
        //波特率
        public List<Int32> BaudRate = new List<Int32>() {300,600,1200,2400,4800,9600,19200,38400,43000,56000,57600,115200};
        //校验位
        public List<string> Parity = new List<string>() { "None", "Odd", "Even", "Mark", "Space" };
        //数据位
        public List<int> DataBits = new List<int>() {8, 7, 6};
        //停止位
        public List<int> StopBits = new List<int>() {1, 2, 3};
        public byte[] Receive_Byte = null;
        public bool Rec_Flag;//数据接收完成标志
        // Crc Computation Class
        private CRCTool compCRC = new CRCTool();
        //委托处理
        //接收数据数组
        public event Receive_Delegate Receive_Event;
        //构造函数
        public RS232()
        {
            //更新列表
            Refresh_Com_List();
            //绑定数据接收事件
            ComDevice.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);//绑定事件
        }
        //更新列表
        public void Refresh_Com_List()
        {
            //获取设备串口名
            PortName.Clear();
            PortName = SerialPort.GetPortNames().ToList<string>();
        }
        //串口打开
        public bool Open_Com(Int32 No)
        {
            if (PortName.Count < 0)
            {
                MessageBox.Show("没有发现串口,请检查线路！");
                return false;
            }

            if (ComDevice.IsOpen == false)
            {
                ComDevice.PortName = PortName[No];
                ComDevice.BaudRate = 115200;//波特率
                ComDevice.Parity = (Parity)Convert.ToInt32("0");//校验位 
                ComDevice.DataBits = 8;//数据位 8、7、6
                ComDevice.StopBits = (StopBits)Convert.ToInt32(1);
                try
                {
                    ComDevice.Open();
                }
                catch (Exception ex)
                {                    
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return true;
            }
            else
            {
                try
                {
                    ComDevice.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return false;
            }

        }
        public bool Open_Com(Int32 No, short baudrate_No)
        {
            if (PortName.Count < 0)
            {
                MessageBox.Show("没有发现串口,请检查线路！");
                return false;
            }

            if (ComDevice.IsOpen == false)
            {
                ComDevice.PortName = PortName[No];
                ComDevice.BaudRate = BaudRate[baudrate_No];//波特率
                ComDevice.Parity = (Parity)Convert.ToInt32("0");//校验位 
                ComDevice.DataBits = 8;//数据位 8、7、6
                ComDevice.StopBits = (StopBits)Convert.ToInt32(1);
                try
                {
                    ComDevice.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return true;
            }
            else
            {
                try
                {
                    ComDevice.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return false;
            }

        }
        //数据发送
        public bool Send_Data(string sendData)
        {
            //清除接收标志
            Rec_Flag = false;
            //发送的字节数组
            byte[] data = null;
            
            //将发送的字符串转化为byte,并追加终止符号
            data = StrCRC(sendData).Concat(new byte[] { 0x0D }).ToArray();
            //数据发送
            if (ComDevice.IsOpen)
            {
                try
                {
                    ComDevice.Write(data, 0, data.Length);//发送数据
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("串口未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
        //Hex字符串转换16进制字节数组 只支持为16进制数字的字符串
        public byte[] StrToHexByte(string hexString) 
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0) hexString = " " + hexString;
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length ; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Replace(" ", ""), 16);
            return returnBytes;
        }       
        //CRC数据校验值添加
        public byte[] StrCRC(string inStr) 
        {
            byte[] data = null;
            //Check_Sum
            ushort usCrc16 = (ushort)compCRC.Check_Sum(StrToHexByte(inStr));
            /*
            回车(Carriage Return)和换行(Line Feed)区别
            CR用符号\r表示, 十进制ASCII代码是13, 十六进制代码为0x0D
            LF使用\n符号表示, ASCII代码是10, 十六制为0x0A
            Dos / windows: 回车 + 换行CR / LF表示下一行,
            UNIX / linux: 换行符LF表示下一行，
            MAC OS: 回车符CR表示下一行.
            */
            //Prompt.Log.Info(inStr + Crc_Append_Str(usCrc16));
            //将字符串转换为Byte
            data = Encoding.ASCII.GetBytes((inStr + Crc_Append_Str(usCrc16)).Trim());
            return data;
        }
        //追加校准值
        public string Crc_Append_Str(UInt32 Num)
        {
            string Result = null;
            Result = string.Format("{0:X4}", Num);
            return Result;
        }
        //数据接收
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] ReDatas = new byte[ComDevice.BytesToRead];
            ComDevice.Read(ReDatas, 0, ReDatas.Length);//读取数据
            //接收数据处理 将ReDatas 转化为 String
            //该方式回丢弃数据
            //ASCII编码只能包含0-127的数据，高出的数据将丢弃
            //ReceiveData = new ASCIIEncoding().GetString(ReDatas);  
            //byte[] Rec_Data = null;
            //Rec_Data = Encoding.ASCII.GetBytes(ReceiveData.Trim());

            //接收的Byte数据 返回
            Receive_Byte = new byte[ReDatas.Length];
            Receive_Byte = (byte[])ReDatas.Clone();

            //异常输出
            if (Receive_Byte.Length==0)
            {
                Prompt.Log.Info("Rs232 通讯数据格式异常！！！");
            }
            else
            {
                //置位接收标志
                Rec_Flag = true;
                //执行数据处理
                Receive_Event?.Invoke();
            }            
        }
       
    }
}
