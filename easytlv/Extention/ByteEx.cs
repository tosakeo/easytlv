using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyTLV.Extention
{
    internal static class ByteEx
    {
        internal static bool HasFlag(this byte source, byte flag)
        {
            return (source & flag) == flag;
        }
    }
}
