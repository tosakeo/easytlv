using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTLV.Extention
{
    internal static class StringEx
    {
        public static byte[] HexToByteArray(this string hex)
        {
            if (string.IsNullOrEmpty(hex)) return new byte[0];

            return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
        }
    }
}
