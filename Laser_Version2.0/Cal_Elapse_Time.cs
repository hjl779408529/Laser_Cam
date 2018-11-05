using System;
using System.Diagnostics;
using System.Globalization;

namespace Laser_Version2._0
{
    class Cal_Elapse_Time
    {
        //调用方式
        //Elapse_ms(delegate () { function(); })
        public static long Elapse_ms(System.Action function_01)
        {
            Stopwatch swTask = new Stopwatch();
            swTask.Start();
            /*执行函数*/
            function_01.Invoke();
            swTask.Stop();
            return swTask.ElapsedMilliseconds;
        }

        //获取当前时间
        public static string Get_Current_Time(int type)
        {
            string Result = null;
            if (type==0)
            {
                Result = DateTime.Now.ToString("yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
            }
            else if (type == 1)
            {
                Result = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss", DateTimeFormatInfo.InvariantInfo);
            }
            else if (type == 2)
            {
                Result = DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒", DateTimeFormatInfo.InvariantInfo);
            }
            else if (type == 10)//10位时间戳 s
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                Result = Convert.ToInt64(ts.TotalSeconds).ToString();
            }
            else if (type == 11)//13位时间戳 ms
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                Result = Convert.ToInt64(ts.TotalMilliseconds).ToString();
            }
            return Result;
        }

        //10位或13位时间戳 转换为特定格式
        public static string unixTimeToTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime;
            if (timeStamp.Length.Equals(10))//判断是10位
            {
                lTime = long.Parse(timeStamp + "0000000");
            }
            else
            {
                lTime = long.Parse(timeStamp + "0000");//13位
            }
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime daTime = dtStart.Add(toNow);
            string time = daTime.ToString("yyyyMMddHHmmss");//转为了string格式
            return time;

        }

    }
}