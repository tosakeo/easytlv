using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTLVTest.Utility
{
    internal static class ByteArrayUtility
    {

            
        public static string ToHexString(this byte[] source)
        {
            return BitConverter.ToString(source).Replace("-", "");
        }
    }
}
