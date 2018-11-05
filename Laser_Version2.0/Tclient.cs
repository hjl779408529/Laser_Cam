using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using Prompt;
using Laser_Version2._0;
using System.Threading.Tasks;

namespace Laser_Build_1._0
{
    public class Tclient
    {
        public NetworkStream stream;
        public TcpClient clien = null;
        public int totalCount = 0;
        public const int bufferSize = 1024;
        public byte[] buffer=new byte[bufferSize];
        public string readType = null;
        public StringBuilder messageBuffe = new StringBuilder();
        //返回值
        int Bis_result = 9;
        TcpClient client = null;
        public ManualResetEvent connectDone = new ManualResetEvent(false);
        public Vector Receive_Cordinate = new Vector();//接收的数据 相机转换为坐标
        public bool Rec_Ok;//接收完成标志
        //无参数 构造函数
        public Tclient()
        {
            Rec_Ok = false;
        }
        //超时事件
        private void Tcp_Rec_TimeOut()
        {
            MessageBox.Show("Tcp 数据接收超时！！！！");
        }
        public void TCP_Start()
        {
            string ip = "127.0.0.1";
            client = new TcpClient();
            client.ReceiveTimeout = 10;
            connectDone.Reset();
            client.BeginConnect(IPAddress.Parse(ip), Convert.ToInt32("6230"), new AsyncCallback(ClientAccpent), client);
            connectDone.WaitOne();
            if (client != null && client.Connected)
            {
                asyncread(client);
                Log.Error("相机Tcp 连接成功！！！");

            }
            else
            {
                Log.Error("相机Tcp 连接失败！！！");
            }
        }
        public void Tcp_Close()
        {
            Rec_Ok = false;
            client.Close();
        }
        private void ClientAccpent(IAsyncResult ar)
        {
            connectDone.Set();
            client = (TcpClient)ar.AsyncState;
            try
            {
                if (client.Connected)
                {;
                    client.EndConnect(ar);
                }
                else
                {
                    client.EndConnect(ar);
                }
            }
            catch (Exception ex)
            {
                Prompt.Log.Info(ex.Message);
            }

        }

        private void asyncread(TcpClient sock)
        {
            Tclient tcpclient = new Tclient();
            tcpclient.clien = sock;

            tcpclient.stream = sock.GetStream();
            if (tcpclient.stream.CanRead)
            {
                try
                {
                    IAsyncResult ar = tcpclient.stream.BeginRead(tcpclient.buffer, 0, Tclient.bufferSize, new AsyncCallback(TCPReadCallBack), tcpclient);
                }
                catch (Exception ex)
                {
                    Prompt.Log.Info(ex.Message);
                }
            }
        }

        private void TCPReadCallBack(IAsyncResult ar)
        {
            Tclient state = (Tclient)ar.AsyncState;
            if (state.clien == null || !state.clien.Connected)
                return;
            int rec;
            NetworkStream steam = state.stream;
            rec = state.stream.EndRead(ar);
            state.totalCount += rec;
            if (rec > 0)
            {
                if (state.stream.CanRead)
                {
                    byte[] bytedata = new byte[rec];
                    Array.Copy(state.buffer, 0, bytedata, 0, rec);
                    string data = BitConverter.ToString(bytedata);
                    data = Encoding.ASCII.GetString(bytedata);

                    string[] tmp = data.Split(',');
                    if ((decimal.TryParse(tmp[0], out decimal d_tmp_x)) && (decimal.TryParse(tmp[1], out decimal d_tmp_y)))
                    {
                        Receive_Cordinate = new Vector(d_tmp_x, d_tmp_y);
                        Rec_Ok = true;
                        MessageBox.Show(string.Format("(X:{0},Y:{1})", Receive_Cordinate.X, Receive_Cordinate.Y));
                    }
                    else
                    {
                        MessageBox.Show("相机坐标提取格式失败！！！！");
                    }
                    //Senddata(Bis_result);
                    state.stream.BeginRead(state.buffer, 0, Tclient.bufferSize, new AsyncCallback(TCPReadCallBack), state);
                }
            }
            else
            {
                Senddata(Bis_result);
            }
        }
        /// <summary>
        /// 触发拍照
        /// </summary>
        /// <param name="Bis_result">< 1：标定 2：Mark点/param>
        public void Senddata(int Bis_result)
        {
            string msg = Bis_result.ToString();
            NetworkStream stream = client.GetStream();
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(msg);
            byte[] buf = BitConverter.GetBytes(31);
            stream.Write(buffer, 0, buffer.Length);
            Rec_Ok = false;
        }
        //获取校准值
        public Vector Get_Cam_Deviation(int order)
        {
            Vector Result;            
            //发送指令
            Senddata(order);
            //等待完成
            Task.Factory.StartNew(() => { do { } while (!Rec_Ok); }).Wait(5 * 1000);//5 * 1000,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            //换算数据
            if ((Rec_Ok) && !(Receive_Cordinate.X == 999) && !((Receive_Cordinate.Y == 999)))
            {
                Result = new Vector(Receive_Cordinate.X * Para_List.Parameter.Cam_Reference, Receive_Cordinate.Y * Para_List.Parameter.Cam_Reference);
            }
            else
            {
                Result = new Vector(999, 999);//异常接收退出
                Log.Error("相机数据获取超时！！！");
            }            
            //返回数据
            return Result;
        }
        public Vector Get_Cam_Deviation_Test(int order)
        {
            Vector Result;
            //发送指令
            Senddata(order);
            //等待完成
            Task.Factory.StartNew(() => { do { } while (!Rec_Ok); }).Wait(5 * 1000);//5 * 1000,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            //换算数据
            if ((Rec_Ok) && !(Receive_Cordinate.X == 999) && !((Receive_Cordinate.Y == 999)))
            {
                Result = new Vector(Receive_Cordinate.X, Receive_Cordinate.Y);
            }
            else
            {
                Result = new Vector(999, 999);//异常接收退出
                Log.Error("相机数据获取超时！！！");
            }
            //返回数据
            return Result;
        }
        public Vector Get_Cam_Deviation_Test_00(int order) 
        {
            Vector Result;
            //发送指令
            Senddata(order);
            //等待完成
            Task.Factory.StartNew(() => { do { } while (!Rec_Ok); }).Wait(5 * 1000);//5 * 1000,该时间范围内：代码段完成 或 超出该时间范围 返回并继续向下执行
            //换算数据
            if ((Rec_Ok) && !(Receive_Cordinate.X == 999) && !((Receive_Cordinate.Y == 999)))
            {
                Result = new Vector(Get_Cam_Actual_Point(Receive_Cordinate.X, Receive_Cordinate.Y));
            }
            else
            {
                Result = new Vector(999, 999);//异常接收退出
                Log.Error("相机数据获取超时！！！");
            }
            //返回数据
            return Result;
        }
        /// <summary>
        /// 返回相机坐标反推额实际坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector Get_Cam_Actual_Point(decimal x, decimal y)
        {
            return new Vector(x * Para_List.Parameter.Cam_Trans_Affinity.Stretch_X + y * Para_List.Parameter.Cam_Trans_Affinity.Distortion_X + Para_List.Parameter.Cam_Trans_Affinity.Delta_X, y * Para_List.Parameter.Cam_Trans_Affinity.Stretch_Y + x * Para_List.Parameter.Cam_Trans_Affinity.Distortion_Y + Para_List.Parameter.Cam_Trans_Affinity.Delta_Y);
        }

    }
}
