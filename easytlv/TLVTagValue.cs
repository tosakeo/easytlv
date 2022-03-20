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
        private readonly byte[] _tag;
        private readonly byte[] _value;
        private readonly TLV _innerTLV;

        public byte[] Value => _value.Copy();

        public TLVTagValue(byte[] tag, byte[] value)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            if (tag.Length == 0) throw new ArgumentException("Tag with zero length is not allowed.");
            
            if (value == null) throw new ArgumentNullException(nameof(value));

            _tag = tag.Copy();
            _value = value.Copy();

            _innerTLV = IsConstractedTag(tag) ? TLV.Create(value) : new TLV();
        }

        public TLVTagValue(byte[] tag, TLV innerTLV) : this(tag)
        {
            if (innerTLV == null)
                throw new ArgumentNullException(nameof(innerTLV));

            _innerTLV = innerTLV;
            _value = innerTLV.ToByteArray();
        }

        private TLVTagValue(byte[] tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            if (tag.Length == 0) throw new ArgumentException("Tag with zero length is not allowed.");
            _tag = tag.Copy();
        }

        public bool HasInnerTLV
        {
            get
            {
                return IsConstractedTag(_tag);
            }
        }

        public TLV GetInnerTLV()
        {
            if (!HasInnerTLV)
                throw new InvalidOperationException("This is not a structured tag.");

            return _innerTLV;
        }


        public byte[] ToByteArray()
        {
            return
                _tag.Copy()
                .Concat(GetTLVLength())
                .Concat(Value)
                .ToArray();
        }

        private byte[] GetTLVLength()
        {
            var length = _value.Length;
            if (length < 0x80)
            {
                return new[] { (byte)length };
            }

            var lengthLength = (length.ToString("X2").Length + 1) / 2;
            var firstByte = (byte)(0x80 | lengthLength);
            var result = new byte[lengthLength + 1];
            result[0] = firstByte;
            for (int i = 1; i <= lengthLength; i++)
            {
                var shift = 8 * (lengthLength - i);
                result[i] = (byte)(length >> shift);
            }

            return result;
        }

        private static bool IsConstractedTag(byte[] tag)
        {
            const byte constractedFlag = 0b00100000;
            return tag[0].HasFlag(constractedFlag);
        }

        




    }
}