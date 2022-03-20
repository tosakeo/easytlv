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
            tlv.Add("DA", "AA");
            tlv.Add("CB", "BBBB");
            Assert.AreEqual("DA01AACB02BBBB", tlv.ToString());
            
        }

        [TestMethod]
        public void get_value()
        {
            var tlv = new TLV();
            tlv.Add("DA", "AA");
            tlv.Add("CB", "BBBB");
            var hexValue = tlv.Get("CB").ToHexString();
            Assert.AreEqual("BBBB", hexValue);
        }


        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void get_not_exist_value()
        {
            var tlv = new TLV();
            tlv.Add("DA", "AA");
            tlv.Add("CB", "BBBB");
            var hexValue = tlv.Get("CC").ToHexString();
        }

        [TestMethod]
        public void add_byte_array()
        {
            var tlv = new TLV();
            tlv.Add("DA", new byte[] { 0xAA });
            tlv.Add("CB", new byte[] { 0x0B, 0x1B });
            Assert.AreEqual("DA01AACB020B1B", tlv.ToString());
        }

        [TestMethod]
        public void tlv_data_127_byte_value()
        {
            var byteCount = 127;
            var tlv = new TLV();
            var value = Enumerable.Repeat((byte)0xAA, byteCount).ToArray();
            tlv.Add("DA", value);
            Assert.AreEqual("DA7F" + new string('A' , byteCount * 2), tlv.ToString());
        }

        [TestMethod]
        public void tlv_data_128_byte_value()
        {
            var byteCount = 128;
            var tlv = new TLV();
            var value = Enumerable.Repeat((byte)0xAA, byteCount).ToArray();
            tlv.Add("DA", value);
            Assert.AreEqual("DA8180" + new string('A', byteCount * 2), tlv.ToString());
        }

        [TestMethod]
        public void create_from_byte_array()
        {
            var source = new byte[] { 0xDA, 0x02, 0x01, 0x02, 0xCB, 0x01, 0xAB };
            var tlv = TLV.Create(source);
            Assert.AreEqual(
                source.ToHexString(),
                tlv.ToString()
                );
        }

        [TestMethod]
        public void create_from_hex_string()
        {
            var source = "DA020102CB01AB";
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
            var source = "DF2A01AA1FD12202BBBB";
            var tlv = TLV.Create(source);
            Assert.AreEqual(source, tlv.ToString());
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void create_invalid_length_at_value()
        {
            var source = "DA0201";
            var tlv = TLV.Create(source);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void create_invalid_length_at_length()
        {
            var source = "CA";
            var tlv = TLV.Create(source);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void add_tag_hex_length_invalid()
        {
            var tlv = new TLV();
            tlv.Add("D", "AA");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void add_value_hex_length_invalid()
        {
            var tlv = new TLV();
            tlv.Add("DA", "A");
        }


        [TestMethod]
        public void get_inner_tlv()
        {
            var tlv = new TLV();
            tlv.Add("AA", "50010A5102DDDD");
            var innerTLV = tlv.GetInnerTLV("AA");
            var tag51Value = innerTLV.Get("51").ToHexString();
            Assert.AreEqual("DDDD", tag51Value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void get_inner_tlv_but_not_constracted_tag()
        {
            var tlv = new TLV();
            tlv.Add("DA", "50010A5102DDDD");
            var innerTLV = tlv.GetInnerTLV("DA");
        }


        [TestMethod]
        public void create_inner_tlv()
        {
            var tlv = TLV.Create("AA0750010A5102DDDD");
            var innerTLV = tlv.GetInnerTLV("AA");
            var tag51Value = innerTLV.Get("51").ToHexString();
            Assert.AreEqual("DDDD", tag51Value);
        }

        [TestMethod]
        public void add_inner_tlv()
        {
            var innerTLVInput = new TLV();
            innerTLVInput.Add("50", "0A");
            innerTLVInput.Add("51", "DDDD");

            var tlv = new TLV();
            tlv.Add("AA", innerTLVInput);
            var innerTLV = tlv.GetInnerTLV("AA");
            var tag51Value = innerTLV.Get("51").ToHexString();
            Assert.AreEqual("DDDD", tag51Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void add_inner_tlv_but_not_constracted_tag()
        {
            var innerTLVInput = new TLV();
            innerTLVInput.Add("50", "0A");
            innerTLVInput.Add("51", "DDDD");

            var tlv = new TLV();
            tlv.Add("DA", innerTLVInput);
        }
    }
}