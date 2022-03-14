using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace EasyTLV.Extention
{
    internal static class StringEx
    {
        public static byte[] HexToByteArray(this string hex)
        {
            if (string.IsNullOrEmpty(hex)) return new byte[0];
            if (hex.Length % 2 != 0) throw new ArgumentException("String length must be even.", nameof(hex));
            
            return Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray();
        }

        public static bool IsHex(this string source)
        {
            // 空の場合は評価しない。
            if (string.IsNullOrEmpty(source))
                return true;

            var hexRegex = new Regex("^[0-9A-Fa-f]+$", RegexOptions.Compiled);
            return hexRegex.IsMatch(source);
        }
    }
}
