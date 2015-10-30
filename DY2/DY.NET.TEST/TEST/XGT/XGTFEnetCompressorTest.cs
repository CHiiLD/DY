using System;
using System.Collections.Generic;
using NUnit.Framework;
using DY.NET.LSIS.XGT;

namespace DY.NET.Test
{
    [TestFixture]
    public class XGTFEnetCompressorTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenEncodeRespProtocol_ExpectArgumentException()
        {
            string addr = "%MW100";
            ushort tag = 0x00;
            XGTFEnetProtocol fenet = new XGTFEnetProtocol(typeof(ushort), XGTFEnetCommand.READ_RESP);
            fenet.Data = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr) };
            fenet.InvokeID = tag;
            XGTFEnetCompressor compressor = new XGTFEnetCompressor();

            byte[] ascii_code = compressor.Encode(fenet);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenInvalidASCIIDecode_ExpectArgumentException()
        {
            byte[] recv_ascii = new byte[] { 0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,
            0x00, 0x00, //Reserved
            (byte)(XGTFEnetCpuType.XGK_CPUH.ToByte() | XGTFEnetClass.MASTER.ToByte() | XGTFEnetCpuState.CPU_NOR.ToByte()), 
            XGTFEnetPLCSystemState.RUN.ToByte(), //PLC Info
            XGTFEnetCpuInfo.XGK.ToByte(),        //Cpu Info
            XGTFEnetStreamDirection.PLC2PC.ToByte(),//Stream
            0x00,0x00,  //InvokeID
            0xA0,0x00,  //Length
            0x30,       //Position 0011 0000
            0x00,       //Reserved
            0x54, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00 };
            XGTFEnetCompressor compressor = new XGTFEnetCompressor();
            XGTFEnetProtocol request = new XGTFEnetProtocol(typeof(ushort), XGTFEnetCommand.READ_REQT) { Data = new List<IProtocolData>() { new ProtocolData("%WM100") } };
            XGTFEnetProtocol result = compressor.Decode(recv_ascii, request) as XGTFEnetProtocol;
        }

        [Test]
        public void WhenReceivedErrorProtocol_CatchError()
        {
            byte[] recv_ascii = new byte[] { 0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,
            0x00, 0x00, //Reserved
            (byte)(XGTFEnetCpuType.XGK_CPUH.ToByte() | XGTFEnetClass.MASTER.ToByte() | XGTFEnetCpuState.CPU_NOR.ToByte()), 
            XGTFEnetPLCSystemState.RUN.ToByte(), //PLC Info
            XGTFEnetCpuInfo.XGK.ToByte(),        //Cpu Info
            XGTFEnetStreamDirection.PLC2PC.ToByte(),//Stream
            0x00,0x00,  //InvokeID
            0xA0,0x00,  //Length
            0x30,       //Position 0011 0000
            0x00,       //Reserved
            0x55, 0x00, 0x02, 0x00, 0x00, 0x00, 0xFF, 0xFF, 
            0x02, 0x00 };
            XGTFEnetCompressor compressor = new XGTFEnetCompressor();
            XGTFEnetProtocol request = new XGTFEnetProtocol(typeof(ushort), XGTFEnetCommand.READ_REQT) { Data = new List<IProtocolData>() { new ProtocolData("%WM100") } };

            XGTFEnetProtocol result = compressor.Decode(recv_ascii, request) as XGTFEnetProtocol;

            Assert.AreEqual(result.Error, XGTFEnetError.DATA_TYPE);
        }

        [Test]
        public void RSS2ASCII()
        {
            string addr = "%MW100";
            ushort tag = 0x00;
            byte[] expectedResult = new byte[] { 0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,
            0x00, 0x00, //Reserved
            0x00, 0x00, //PLC Info
            0x00,       //Cpu Info
            0x33,       //Stream
            0x00,0x00,  //InvokeID
            0x10,0x00,  //Length
            0x00,       //Position
            0x9E,       //Reserved
            0x54, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x06, 0x00, 
            0x25, 0x4D, 0x57, 0x31, 0x30, 0x30 };
            XGTFEnetProtocol fenet = new XGTFEnetProtocol(typeof(ushort), XGTFEnetCommand.READ_REQT);
            fenet.Data = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr) };
            fenet.InvokeID = tag;
            XGTFEnetCompressor compressor = new XGTFEnetCompressor();

            byte[] ascii_code = compressor.Encode(fenet);

            Assert.AreEqual(ascii_code, expectedResult);
        }

        [Test]
        public void WSS2ASCII()
        {
            string addr = "%MW100";
            ushort value = 0x1234;
            ushort tag = 0x00;
            byte[] expectedResult = new byte[] { 0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,
            0x00, 0x00, //Reserved
            0x00, 0x00, //PLC Info
            0x00,       //Cpu Info
            0x33,       //Stream
            0x00,0x00,  //InvokeID
            0x14,0x00,  //Length
            0x00,       //Position
            0xA2,       //Reserved
            0x58, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x06, 0x00, 
            0x25, 0x4D, 0x57, 0x31, 0x30, 0x30,
            0x02, 0x00,
            0x34, 0x12 };
            XGTFEnetProtocol fenet = new XGTFEnetProtocol(typeof(ushort), XGTFEnetCommand.WRITE_REQT);
            fenet.Data = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr, value) };
            fenet.InvokeID = tag;
            XGTFEnetCompressor compressor = new XGTFEnetCompressor();

            byte[] ascii_code = compressor.Encode(fenet);

            Assert.AreEqual(ascii_code, expectedResult);
        }

        [Test]
        public void ASCII2RSS()
        {
            XGTFEnetProtocol expectedResult = new XGTFEnetProtocol(typeof(ushort), XGTFEnetCommand.READ_RESP)
            {
                CompanyID = XGTFEnetCompanyID.LSIS_XGT,
                CpuType = XGTFEnetCpuType.XGK_CPUH,
                Class = XGTFEnetClass.MASTER,
                CpuState = XGTFEnetCpuState.CPU_NOR,
                PLCState = XGTFEnetPLCSystemState.RUN,
                StreamDirection = XGTFEnetStreamDirection.PLC2PC,
                CpuInfo = XGTFEnetCpuInfo.XGK,
                InvokeID = 0,
                BodyLength = 0x00A0,
                SlotPosition = 0,
                BasePosition = 3,
                Data = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(0x1234) }
            };
            byte[] recv_ascii = new byte[] { 0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,
            0x00, 0x00, //Reserved
            (byte)(expectedResult.CpuType.ToByte() | expectedResult.Class.ToByte() | expectedResult.CpuState.ToByte()), expectedResult.PLCState.ToByte(), //PLC Info
            expectedResult.CpuInfo.ToByte(),        //Cpu Info
            expectedResult.StreamDirection.ToByte(),//Stream
            0x00,0x00,  //InvokeID
            0xA0,0x00,  //Length
            0x30,       //Position 0011 0000
            0x00,       //Reserved
            0x55, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 
            0x02, 0x00, 
            0x34, 0x12 };
            XGTFEnetCompressor compressor = new XGTFEnetCompressor();
            XGTFEnetProtocol request = new XGTFEnetProtocol(typeof(ushort), XGTFEnetCommand.READ_REQT) { Data = new List<IProtocolData>() { new ProtocolData("%WM100") } };
            XGTFEnetProtocol result = compressor.Decode(recv_ascii, request) as XGTFEnetProtocol;

            Assert.AreEqual(result.CompanyID, expectedResult.CompanyID);
            Assert.AreEqual(result.CpuType, expectedResult.CpuType);
            Assert.AreEqual(result.Class, expectedResult.Class);
            Assert.AreEqual(result.CpuState, expectedResult.CpuState);
            Assert.AreEqual(result.PLCState, expectedResult.PLCState);
            Assert.AreEqual(result.StreamDirection, expectedResult.StreamDirection);
            Assert.AreEqual(result.InvokeID, expectedResult.InvokeID);
            Assert.AreEqual(result.BodyLength, expectedResult.BodyLength);
            Assert.AreEqual(result.BasePosition, expectedResult.BasePosition);
            Assert.AreEqual(result.SlotPosition, expectedResult.SlotPosition);
            Assert.AreEqual(result.Command, expectedResult.Command);
            Assert.AreEqual(result.DataType, expectedResult.DataType);
            Assert.AreEqual(result.Error, expectedResult.Error);
            Assert.AreEqual(result.Data.Count, expectedResult.Data.Count);
        }

        [Test]
        public void ASCII2WSS()
        {
            XGTFEnetProtocol expectedResult = new XGTFEnetProtocol(typeof(ushort), XGTFEnetCommand.WRITE_RESP)
            {
                CompanyID = XGTFEnetCompanyID.LSIS_XGT,
                CpuType = XGTFEnetCpuType.XGK_CPUH,
                Class = XGTFEnetClass.MASTER,
                CpuState = XGTFEnetCpuState.CPU_NOR,
                PLCState = XGTFEnetPLCSystemState.RUN,
                StreamDirection = XGTFEnetStreamDirection.PLC2PC,
                CpuInfo = XGTFEnetCpuInfo.XGK,
                InvokeID = 0,
                BodyLength = 0x00A0,
                SlotPosition = 0,
                BasePosition = 3,
                Data = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData("%MW100", 0x1234) }
            };
            byte[] recv_ascii = new byte[] { 0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,
            0x00, 0x00, //Reserved
            (byte)(expectedResult.CpuType.ToByte() | expectedResult.Class.ToByte() | expectedResult.CpuState.ToByte()), expectedResult.PLCState.ToByte(), //PLC Info
            expectedResult.CpuInfo.ToByte(),        //Cpu Info
            expectedResult.StreamDirection.ToByte(),//Stream
            0x00,0x00,  //InvokeID
            0xA0,0x00,  //Length
            0x30,       //Position 0011 0000
            0x00,       //Reserved
            0x59, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00 };
            XGTFEnetCompressor compressor = new XGTFEnetCompressor();
            XGTCnetProtocol request = new XGTCnetProtocol(typeof(ushort), XGTCnetCommand.READ) { Data = new List<IProtocolData>() { new ProtocolData("%WM100") } };
            XGTFEnetProtocol result = compressor.Decode(recv_ascii, request) as XGTFEnetProtocol;

            Assert.AreEqual(result.CompanyID, expectedResult.CompanyID);
            Assert.AreEqual(result.CpuType, expectedResult.CpuType);
            Assert.AreEqual(result.Class, expectedResult.Class);
            Assert.AreEqual(result.CpuState, expectedResult.CpuState);
            Assert.AreEqual(result.PLCState, expectedResult.PLCState);
            Assert.AreEqual(result.StreamDirection, expectedResult.StreamDirection);
            Assert.AreEqual(result.InvokeID, expectedResult.InvokeID);
            Assert.AreEqual(result.BodyLength, expectedResult.BodyLength);
            Assert.AreEqual(result.BasePosition, expectedResult.BasePosition);
            Assert.AreEqual(result.SlotPosition, expectedResult.SlotPosition);
            Assert.AreEqual(result.Command, expectedResult.Command);
            Assert.AreEqual(result.DataType, expectedResult.DataType);
            Assert.AreEqual(result.Error, expectedResult.Error);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void WhenProtocolDataAddItemToo_ExpectException()
        {
            var cmd = XGTFEnetCommand.READ_REQT;
            string addr = "%MW100";
            ushort value = 0x00E2;
            XGTFEnetProtocol fenet = new XGTFEnetProtocol(value.GetType(), cmd);
            fenet.Data = new System.Collections.Generic.List<IProtocolData>();
            for (int i = 0; i < 17; i++)
                fenet.Data.Add(new ProtocolData(addr, value));

            XGTFEnetCompressor cnet_comp = new XGTFEnetCompressor();
            byte[] ascii_code = cnet_comp.Encode(fenet);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void WhenAddressLengthLongToo_ExpectException()
        {
            var cmd = XGTFEnetCommand.READ_REQT;
            string addr = "%MW45678901212213";
            ushort value = 0x00E2;
            XGTFEnetProtocol fenet = new XGTFEnetProtocol(value.GetType(), cmd);
            fenet.Data = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr, value) };
            XGTFEnetCompressor cnet_comp = new XGTFEnetCompressor();
            byte[] ascii_code = cnet_comp.Encode(fenet);
        }
    }
}
