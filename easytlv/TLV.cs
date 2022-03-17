using System;
using System.Text.RegularExpressions;
using EasyTLV.Extention;

namespace EasyTLV
{
    public class TLV
    {
        private readonly Dictionary<string, TLVTagValue> tagValues = new();

        public void Add(string tag, string hexValue)
        {
            Add(tag, hexValue.HexToByteArray());
        }

        public void Add(string tag, byte[] value)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            if (tag.Length == 0) throw new ArgumentException("tag cannot be empty");
            if (tag.Length % 2 != 0 || !tag.IsHex()) 
                throw new ArgumentException("tag is invalid. tag needs hex.");

            tagValues.Add(tag, new TLVTagValue(tag.HexToByteArray(), value));
        }

        public bool Contains(string tag)
        {
            return tagValues.ContainsKey(tag);
        }

        public byte[] Get(string tag)
        {
            if (!Contains(tag))
                throw new KeyNotFoundException();

            return tagValues[tag].Value;
        }

        public TLV GetInnerTLV(string tag)
        {
            return tagValues[tag].GetInnerTLV();
        }

        public byte[] ToByteArray()
        {
            IEnumerable<byte> result = new byte[0];
            foreach(var tagValue in tagValues.Values)
            {
                result = result.Concat(tagValue.ToByteArray());
            }
            return result.ToArray();
        }

        public override string ToString()
        {
            return ToByteArray().ToHexString();
        }

        public static TLV Create(string tlvHex)
        {
            if (string.IsNullOrEmpty(tlvHex)) 
                return new TLV();

            return Create(tlvHex.HexToByteArray());
        }


        public static TLV Create(byte[] tlvData)
        {
            if (tlvData == null) throw new ArgumentNullException(nameof(tlvData));
            var position = 0;
            
            var tlv = new TLV();
            while (position < tlvData.Length)
            {
                var tagBytes = GetNextTag(tlvData, ref position);
                var lengthBytes = GetNextLength(tlvData, ref position);
                var value = GetValue(tlvData, lengthBytes, ref position);
                tlv.Add(tagBytes.ToHexString(), value);
            }
            return tlv;
        }

        private static byte[] GetNextTag(byte[] tlvData, ref int position)
        {
            try
            {
                var firstTagByte = tlvData[position++];

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
                    nextTagByte = tlvData[position++];
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

        private static int GetNextLength(byte[] tlvData, ref int position)
        {
            try
            {
                var firstByte = tlvData[position++];
                if (firstByte < 0x80)
                {
                    return firstByte;
                }

                int result = 0;
                var lengthLength = firstByte - 0x80;
                for (var i = 0; i < lengthLength; i++)
                {
                    result = (result << 8) | tlvData[position++];
                }
                return result;
            }
            catch (IndexOutOfRangeException iex)
            {
                throw new ArgumentException("tlvData is invalid.", iex);
            }
        }

        public static byte[] GetValue(byte[] tlvData, int length, ref int position)
        {
            if (length > tlvData.Length - position)
                throw new ArgumentException("tlvData.Length is too short than length");
            var skipPosition = position;
            position = position + length;
            return tlvData.Skip(skipPosition).Take(length).ToArray();
        }
    }
}