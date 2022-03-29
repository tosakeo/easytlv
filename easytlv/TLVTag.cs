using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasyTLV.Extention;

namespace EasyTLV
{
    internal class TLVTag : IEquatable<TLVTag>
    {
        private const byte CONSTRUCTED_FLAG = 0b00100000;

        private readonly byte[] _tag;

        public bool IsConstructedTag => _tag[0].HasFlag(CONSTRUCTED_FLAG);

        internal TLVTag(byte[] tagBytes)
        {
            if (tagBytes == null || tagBytes.Length == 0)
                throw new ArgumentException("tagHex is not allowed empty.");

            _tag = tagBytes;
        }

        public override bool Equals(object? obj)
        {
            return this.Equals(obj as TLVTag);
        }

        public bool Equals(TLVTag? other)
        {
            return
                other != null
                && this._tag.SequenceEqual(other._tag);
        }

        public override int GetHashCode()
        {
            int result = 0;
            foreach (var byteValue in _tag.Select((b, i) => b << (i % 4)))
            {
                result ^= byteValue;
            }
            return result;
        }

        public byte[] ToByteArray()
        {
            return _tag.Copy();
        }

        public static byte[] GetTagFromBytes(byte[] source, int startIndex)
        {
            const byte seeSubsequentFlag = (byte)0b00011111;
            const byte anotherFollowsFlag = (byte)0b10000000;

            try
            {
                var position = startIndex;
                var firstTagByte = source[position++];

                if (!firstTagByte.HasFlag(seeSubsequentFlag))
                {
                    return new byte[] { firstTagByte };
                }

                var tagBytes = new List<byte>();
                tagBytes.Add(firstTagByte);

                while(true)
                {
                    byte nextTagByte = source[position++];
                    tagBytes.Add(nextTagByte);

                    if (!nextTagByte.HasFlag(anotherFollowsFlag))
                        break;
                }

                return tagBytes.ToArray();
            }
            catch (IndexOutOfRangeException iex)
            {
                throw new ArgumentException("tlv tag is invalid.", iex);
            }
        }

        public static bool IsValidTag(byte[] source)
        {
            try
            {
                var resolovedTag = GetTagFromBytes(source, 0);
                return resolovedTag.Length == source.Length;
            }
            catch(ArgumentException)
            {
                return false;
            }
        }

    }
}
