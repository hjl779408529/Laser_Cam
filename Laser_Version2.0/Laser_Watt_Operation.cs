using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laser_Version2._0
{    
    class Laser_Watt_Operation
    {
        public decimal Current_Watt;
        public int Rec_Number = 0;
        private List<int> Rec_Data = new List<int>();
        public void Resolve_Com_Data()
        {
            int wan, qian, bai, shi, ge;
            byte[] tmp = new byte[Initialization.Initial.Laser_Watt_Com.Receive_Byte.Length];
            tmp = (byte[])Initialization.Initial.Laser_Watt_Com.Receive_Byte.Clone();
            if (tmp.Length==1) Rec_Data.Add(Convert.ToChar(tmp[0]));
            if (Rec_Data.Count >= 64)
            {
                Rec_Number = 0;
                for (int i = 0;i< 57;i++)
                {
                    if (Rec_Data[i + 0] == 170 && Rec_Data[i + 1] == 170 && Rec_Data[i + 2] == 170)
                    {
                        if (Rec_Data[i + 3] <= 9)
                        {
                            wan = Rec_Data[i + 3];
                        }
                        else
                        {
                            continue;
                        }
                        if (Rec_Data[i + 4] <= 9)
                        {
                            qian = Rec_Data[i + 4];
                        }
                        else
                        {
                            continue;
                        }
                        if (Rec_Data[i + 5] <= 9)
                        {
                            bai = Rec_Data[i + 5];
                        }
                        else
                        {
                            continue;
                        }
                        if (Rec_Data[i + 6] <= 9)
                        {
                            shi = Rec_Data[i + 6];
                        }
                        else
                        {
                            continue;
                        }
                        if (Rec_Data[i + 7] <= 9)
                        {
                            ge = Rec_Data[i + 7];
                        }
                        else
                        {
                            continue;
                        }
                        Current_Watt = (decimal)(wan * 10000 + qian * 1000 + bai * 100 + shi * 10 + ge);
                        break;
                    }
                }
                //数组清空
                Rec_Data.Clear();
            }
        }
    }
}
