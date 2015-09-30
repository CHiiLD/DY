using System;
using NUnit.Framework;
using DY.NET.LSIS.XGT;

namespace DY.NET.TEST.TEST.XGTFEnet
{
    [TestFixture]
    public class XGTFEnetCompressorTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenResponseProtocolEncode_ExpectArgumentException()
        {
            string addr = "%MW100";
            ushort tag = 0x00;
            XGTFEnetProtocol fenet = new XGTFEnetProtocol(typeof(ushort), XGTFEnetCommand.READ_RESP);
            fenet.Items = new System.Collections.Generic.List<IProtocolItem>() { new ProtocolData(addr) };
            fenet.InvokeID = tag;
            XGTFEnetCompressor compressor = new XGTFEnetCompressor();

            byte[] ascii_code = compressor.Encode(fenet);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenWrongASCIIDecode_ExpectedArgumentException()
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

            XGTFEnetProtocol result = compressor.Decode(recv_ascii, null) as XGTFEnetProtocol;
        }

        [Test]
        public void WhenReceivedNakProtocol_CatchErrorSuccessfully()
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

            XGTFEnetProtocol result = compressor.Decode(recv_ascii, null) as XGTFEnetProtocol;

            Assert.AreEqual(result.Error, XGTFEnetError.DATA_TYPE);
        }

        [Test]
        public void WhenRSSProtocolCompressorEncode_CompressSuccessfully()
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
            fenet.Items = new System.Collections.Generic.List<IProtocolItem>() { new ProtocolData(addr) };
            fenet.InvokeID = tag;
            XGTFEnetCompressor compressor = new XGTFEnetCompressor();

            byte[] ascii_code = compressor.Encode(fenet);

            Assert.AreEqual(ascii_code, expectedResult);
        }

        [Test]
        public void WhenWSSProtocolCompressorEncode_CompressSuccessfully()
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
            fenet.Items = new System.Collections.Generic.List<IProtocolItem>() { new ProtocolData(addr, value) };
            fenet.InvokeID = tag;
            XGTFEnetCompressor compressor = new XGTFEnetCompressor();

            byte[] ascii_code = compressor.Encode(fenet);

            Assert.AreEqual(ascii_code, expectedResult);
        }

        [Test]
        public void WhenRSSProtocolCompressorDecode_DecompressSuccessfully()
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
                Items = new System.Collections.Generic.List<IProtocolItem>() { new ProtocolData(0x1234) }
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

            XGTFEnetProtocol result = compressor.Decode(recv_ascii, typeof(ushort)) as XGTFEnetProtocol;

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
            Assert.AreEqual(result.Items.Count, expectedResult.Items.Count);
        }

        [Test]
        public void WhenWSSProtocolCompressorDecode_DecompressSuccessfully()
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
                Items = new System.Collections.Generic.List<IProtocolItem>() { new ProtocolData("%MW100", 0x1234) }
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

            XGTFEnetProtocol result = compressor.Decode(recv_ascii, null) as XGTFEnetProtocol;

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
        public void WhenProtocolEncoded_BlockCountOverError()
        {
            var cmd = XGTFEnetCommand.READ_REQT;
            string addr = "%MW100";
            ushort value = 0x00E2;
            XGTFEnetProtocol fenet = new XGTFEnetProtocol(value.GetType(), cmd);
            fenet.Items = new System.Collections.Generic.List<IProtocolItem>();
            for (int i = 0; i < 17; i++)
                fenet.Items.Add(new ProtocolData(addr, value));

            XGTFEnetCompressor cnet_comp = new XGTFEnetCompressor();
            byte[] ascii_code = cnet_comp.Encode(fenet);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void WhenProtocolEncoded_AddressLengthOverError()
        {
            var cmd = XGTFEnetCommand.READ_REQT;
            string addr = "%MW45678901212213";
            ushort value = 0x00E2;
            XGTFEnetProtocol fenet = new XGTFEnetProtocol(value.GetType(), cmd);
            fenet.Items = new System.Collections.Generic.List<IProtocolItem>() { new ProtocolData(addr, value) };
            XGTFEnetCompressor cnet_comp = new XGTFEnetCompressor();
            byte[] ascii_code = cnet_comp.Encode(fenet);
        }
    }
}
