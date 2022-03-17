using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasyTLV.Extention;

namespace EasyTLV
{
    internal class TLVParser
    {
        private readonly byte[] _tlvData;
        private int _position = 0;

        public TLVParser(byte[] tlvData)
        {
            if (tlvData == null) 
                throw new ArgumentNullException(nameof(tlvData));

            this._tlvData = tlvData;
        }

        public TLV Parse()
        {
            var tlv = new TLV();
            while (_position < _tlvData.Length)
            {
                var tagBytes = GetNextTag();
                var lengthBytes = GetNextLength();
                var value = GetValue(lengthBytes);
                tlv.Add(tagBytes.ToHexString(), value);
            }
            return tlv;
        }

        private byte[] GetNextTag()
        {
            try
            {
                var firstTagByte = _tlvData[_position++];

                const byte seeSubsequentFlag = (byte)0b00011111;
                if (!firstTagByte.HasFlag(seeSubsequentFlag))
                {
                    return new byte[] { firstTagByte };
                }

                var tagBytes = new List<byte>();
                tagBytes.Add(firstTagByte);

                byte nextTagByte;
                do
                {
                    nextTagByte = _tlvData[_position++];
                    tagBytes.Add(nextTagByte);
                }
                while (nextTagByte.HasFlag(0b10000000));

                return tagBytes.ToArray();
            }
            catch (IndexOutOfRangeException iex)
            {
                throw new ArgumentException("tlvData is invalid.", iex);
            }

        }

        private int GetNextLength()
        {
            try
            {
                var firstByte = _tlvData[_position++];
                if (firstByte < 0x80)
                {
                    return firstByte;
                }

                int result = 0;
                var lengthLength = firstByte - 0x80;
                for (var i = 0; i < lengthLength; i++)
                {
                    result = (result << 8) | _tlvData[_position++];
                }
                return result;
            }
            catch (IndexOutOfRangeException iex)
            {
                throw new ArgumentException("tlvData is invalid.", iex);
            }
        }

        private byte[] GetValue(int length)
        {
            if (length > _tlvData.Length - _position)
                throw new ArgumentException("tlvData.Length is too short than length");
            var skipPosition = _position;
            _position = _position + length;
            return _tlvData.Skip(skipPosition).Take(length).ToArray();
        }
    }
}
