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
        private int _position;

        public TLVParser(byte[] tlvData)
        {
            if (tlvData == null) 
                throw new ArgumentNullException(nameof(tlvData));

            this._tlvData = tlvData;
        }

        public TLV Parse()
        {
            _position = 0;

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
            var tagBytes = TLVTag.GetTagFromBytes(_tlvData, _position);
            _position += tagBytes.Length;

            return tagBytes;
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
