using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyTLV;
using EasyTLVTest.Utility;

namespace EasyTLVTest
{
    [TestClass]
    public class TLVTest
    {
        [TestMethod]
        public void add_string_value()
        {
            var tlv = new TLV();
            tlv.Add("AA", "AA");
            tlv.Add("BB", "BBBB");
            Assert.AreEqual("AA01AABB02BBBB", tlv.ToString());
            
        }

        [TestMethod]
        public void get_value()
        {
            var tlv = new TLV();
            tlv.Add("AA", "AA");
            tlv.Add("BB", "BBBB");
            var hexValue = tlv.Get("BB").ToHexString();
            Assert.AreEqual("BBBB", hexValue);
        }


        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void get_not_exist_value()
        {
            var tlv = new TLV();
            tlv.Add("AA", "AA");
            tlv.Add("BB", "BBBB");
            var hexValue = tlv.Get("CC").ToHexString();
        }

        [TestMethod]
        public void add_byte_array()
        {
            var tlv = new TLV();
            tlv.Add("AA", new byte[] { 0xAA });
            tlv.Add("BB", new byte[] { 0x0B, 0x1B });
            Assert.AreEqual("AA01AABB020B1B", tlv.ToString());
        }

        [TestMethod]
        public void tlv_data_127_byte_value()
        {
            var byteCount = 127;
            var tlv = new TLV();
            var value = Enumerable.Repeat((byte)0xAA, byteCount).ToArray();
            tlv.Add("AA", value);
            Assert.AreEqual("AA7F" + new string('A' , byteCount * 2), tlv.ToString());
        }

        [TestMethod]
        public void tlv_data_128_byte_value()
        {
            var byteCount = 128;
            var tlv = new TLV();
            var value = Enumerable.Repeat((byte)0xAA, byteCount).ToArray();
            tlv.Add("AA", value);
            Assert.AreEqual("AA8180" + new string('A', byteCount * 2), tlv.ToString());
        }

        [TestMethod]
        public void create_from_byte_array()
        {
            var source = new byte[] { 0xAA, 0x02, 0x01, 0x02, 0xBB, 0x01, 0xAB };
            var tlv = TLV.Create(source);
            Assert.AreEqual(
                source.ToHexString(),
                tlv.ToString()
                );
        }

        [TestMethod]
        public void create_from_hex_string()
        {
            var source = "AA020102BB01AB";
            var tlv = TLV.Create(source);
            Assert.AreEqual(source, tlv.ToString());

        }

        [TestMethod]
        public void create_empty_tlv_from_empty_hex()
        {
            var source = "";
            var tlv = TLV.Create(source);
            Assert.AreEqual(source, tlv.ToString());
        }

        [TestMethod]
        public void create_some_byte_tag_tlv()
        {
            var source = "DF2A01AA1FA12202BBBB";
            var tlv = TLV.Create(source);
            Assert.AreEqual(source, tlv.ToString());
        }


        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void create_invalid_length_at_value()
        {
            var source = "AA0201";
            var tlv = TLV.Create(source);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void create_invalid_length_at_length()
        {
            var source = "AA";
            var tlv = TLV.Create(source);
        }

    }
}