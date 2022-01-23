using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTLV.Extention
{
    internal static class ByteArrayEx
    {
        public static byte[] Copy(this byte[] source)
        {
            var result = new byte[source.Length];
            source.CopyTo(result, 0);
            return result;
        }
    }
}
