using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyTLV.Extention;

namespace EasyTLV
{
    internal class TLVTagValue
    {
        private readonly byte[] tag;
        private readonly byte[] value;

        public byte[] Value => value.Copy();

        public TLVTagValue(byte[] tag, byte[] value)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            if (tag.Length == 0) throw new ArgumentException("Tag with zero length is not allowed.");
            if (value == null) throw new ArgumentNullException(nameof(value));

            this.tag = tag.Copy();
            this.value = value.Copy();
        }

        public bool HasInnerTLV
        {
            get
            {
                const byte constractedFlag = 0b00100000;
                return (tag[0] & constractedFlag) != constractedFlag;
            }
        }

        public TLV GetInnerTLV()
        {
            if (!HasInnerTLV)
                throw new InvalidOperationException("This is not a structured tag.");

            return TLV.Create(value);
        }


        public byte[] ToByteArray()
        {
            return
                tag.Copy()
                .Concat(GetTLVLength())
                .Concat(Value)
                .ToArray();
        }

        private byte[] GetTLVLength()
        {
            var length  = value.Length;
            if (length < 0x80)
            {
                return new [] { (byte)length };
            } 
            else
            {
                var lengthLength = (length.ToString("X2").Length + 1) / 2;
                var firstByte = (byte)(0x80 | lengthLength);
                var result = new byte[lengthLength + 1];
                result[0] = firstByte;
                for(int i = 1; i <= lengthLength; i++)
                {
                    var shift = 8 * (lengthLength - i);
                    result[i] = (byte)(length >> shift);
                }

                return result;
            }
        }





    }
}