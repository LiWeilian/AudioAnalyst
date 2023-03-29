using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFTWithNAudio
{
    public class Utils
    {
        public static string ConvertIntToTimeFormat(int value)
        {
            int num = value / 3600;
            int num2 = value % 3600 / 60;
            int num3 = value % 60;
            return string.Format("{0}:{1}:{2}", num.ToString(), num2.ToString().PadLeft(2, '0'), num3.ToString().PadLeft(2, '0'));
        }
    }
}
