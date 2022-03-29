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

        public void Add(string tag, TLV innerTLV)
        {
            var innerTLVValue = new TLVTagValue(tag.HexToByteArray(), innerTLV);
            if (!innerTLVValue.HasInnerTLV)
                throw new ArgumentException("tag is not a constracted tag.");

            tagValues.Add(tag, innerTLVValue);
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

        public bool HasInnerTLV(string tag)
        {
            return tagValues[tag].HasInnerTLV;
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
            var parser = new TLVParser(tlvData);
            return parser.Parse();
        }

    }
}