using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasyTLV.Extention;

namespace EasyTLV
{
    internal class TLVTag
    {
        private readonly string _tagHex;

        internal TLVTag(string tagHex)
        {
            if (string.IsNullOrEmpty(tagHex))
                throw new ArgumentException("tagHex is not allowed empty.");

            _tagHex = tagHex;
        }

        public byte[] ToByteArray()
        {
            return _tagHex.HexToByteArray();
        }

        


    }
}
