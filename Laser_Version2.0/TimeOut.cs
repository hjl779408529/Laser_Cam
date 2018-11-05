using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Laser_Version2._0
{
    //delegate.BeginInvoke可以实现代码代码的异步执行
    //等待一个Timespan
    //ManualResetEvent.WaitOne(timespan, false),其返回值代码其是否在特定时间内收到信号
    //而我们恰好可以利用这个布尔值 外加一个标记变量 来判断一个方法是否执行超时

    /// <summary>
    /// 超时事件委托
    /// </summary>
    public delegate void DoHandler();
    public class Timeout_Check 
    {
        private ManualResetEvent mTimeoutObject;
        //标记变量
        private bool mBoTimeout;
        //超时事件
        public event DoHandler Time_Out_Handle; 
        public Timeout_Check() 
        {
            //初始状态为 停止
            this.mTimeoutObject = new ManualResetEvent(true);
        }
        
        ///<summary>
        /// 指定超时时间 异步执行某个方法
        ///</summary>
        ///<returns>执行 是否超时</returns> 
        public bool DoWithTimeout(TimeSpan timeSpan)
        {
            //判断是否注册事件
            if (this.Time_Out_Handle == null)
            {
                return false;
            }
            //重置
            this.mTimeoutObject.Reset();
            this.mBoTimeout = true; //标记
            this.Time_Out_Handle.BeginInvoke(DoAsyncCallBack, null);
            // 等待 信号Set
            if (!this.mTimeoutObject.WaitOne(timeSpan, false))
            {
                this.mBoTimeout = true;
            }
            return this.mBoTimeout;
        }
        ///<summary>
        /// 异步委托 回调函数
        ///</summary>
        ///<param name="result"></param>
        private void DoAsyncCallBack(IAsyncResult result)
        {
            try
            {
                this.Time_Out_Handle.EndInvoke(result);
                // 指示方法的执行未超时
                this.mBoTimeout = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.mBoTimeout = true;
            }
            finally
            {
                this.mTimeoutObject.Set();
            }
        }
    }
}
