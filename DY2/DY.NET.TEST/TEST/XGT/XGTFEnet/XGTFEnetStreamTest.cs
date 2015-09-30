using System;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using DY.NET.LSIS.XGT;
using System.IO;
using System.Linq;

namespace DY.NET.TEST.TEST.XGTFEnet
{
    [TestFixture]
    public class XGTFEnetStreamTest
    {
        private class FakeStraem : AFakeStream
        {
            public FakeStraem()
            {
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                await Task.Delay(ReadDalayTime);
                System.Buffer.BlockCopy(this.Buffer, 0, buffer, offset, Buffer.Length);
                return this.Buffer.Length;
            }

            public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                await Task.Delay(WriteDalayTime);
                //응답 XGTCnetProtocol을 byte[]로 변환하는 과정
                List<byte> buf = new List<byte>();
                byte[] header_buf = new byte[] { 0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,
                0x00, 0x00, //Reserved
                (byte)(XGTFEnetCpuType.XGK_CPUH.ToByte() | XGTFEnetClass.MASTER.ToByte() | XGTFEnetCpuState.CPU_NOR.ToByte()), 
                XGTFEnetPLCSystemState.RUN.ToByte(), //PLC Info
                XGTFEnetCpuInfo.XGK.ToByte(),        //Cpu Info
                XGTFEnetStreamDirection.PLC2PC.ToByte(),//Stream
                0x00,0x00,  //InvokeID
                0x00,0x00,  //Length
                0x30,       //Position 0011 0000
                0x00,       //Reserved 
                };

                XGTFEnetCommand cmd = (XGTFEnetCommand)XGTFEnetTranslator.ToValue(new byte[] { buffer[20], buffer[21] }, typeof(ushort));
                XGTFEnetDataType dataType = (XGTFEnetDataType)XGTFEnetTranslator.ToValue(new byte[] { buffer[22], buffer[23] }, typeof(ushort));
                ushort block_cnt = (ushort)XGTFEnetTranslator.ToValue(new byte[] { buffer[26], buffer[27] }, typeof(ushort));
                buf.AddRange(cmd == XGTFEnetCommand.READ_REQT ? XGTFEnetCommand.READ_RESP.ToBytes().Reverse() : XGTFEnetCommand.WRITE_RESP.ToBytes().Reverse());
                buf.AddRange(dataType.ToBytes().Reverse());
                buf.AddRange(new byte[] { 0x00, 0x00 }); //reserved
                buf.AddRange(new byte[] { 0x00, 0x00 }); //err
                buf.AddRange(XGTFEnetTranslator.ToASCII(block_cnt, typeof(ushort))); //블록 수

                if (cmd == XGTFEnetCommand.READ_REQT)
                {
                    var size = block_cnt;
                    for (int i = 0; i < size; i++)
                    {
                        buf.AddRange(dataType.ToBytes().Reverse());
                        if (dataType == XGTFEnetDataType.BIT)
                            buf.AddRange(new byte[] { 0x30 });
                        else if (dataType == XGTFEnetDataType.BYTE)
                            buf.AddRange(new byte[] { 0x30 });
                        else if (dataType == XGTFEnetDataType.WORD)
                            buf.AddRange(new byte[] { 0x30, 0x30 });
                        else if (dataType == XGTFEnetDataType.DWORD)
                            buf.AddRange(new byte[] { 0x30, 0x30, 0x30, 0x30 });
                        else if (dataType == XGTFEnetDataType.LWORD)
                            buf.AddRange(new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 });
                    }
                }
                byte[] length = XGTFEnetTranslator.ToASCII(buf.Count, typeof(ushort));
                System.Buffer.BlockCopy(length, 0, header_buf, 16, length.Length);
                buf.InsertRange(0, header_buf);
                this.Buffer = buf.ToArray();
            }
        }

        private class FakeXGTFEnetStream : XGTFEnetStream
        {
            private Stream m_Stream;

            public FakeXGTFEnetStream(System.IO.Stream stream)
            {
                this.m_Stream = stream;
            }

            public override Stream GetStream()
            {
                return this.m_Stream;
            }

            public override async Task OpenAsync()
            {
                await Task.Run(() => { });
            }

            public override void Close()
            {
            }

            public override bool IsOpend()
            {
                return true;
            }
        }

        [Ignore]
        [Test]
        [ExpectedException(typeof(WriteTimeoutException))]
        public async void WhenFakeXGTFENetStreamFloated_ExpectedWriteTimeoutException()
        {
            int writeDelayTime = 50;
            int writeTimeout = 25;
            string addr = "%MW100";
            ushort value = 0;
            var cmd = XGTFEnetCommand.WRITE_REQT;
            var type = typeof(ushort);
            XGTFEnetProtocol resquest = new XGTFEnetProtocol(type, cmd);
            resquest.Items = new List<IProtocolItem>() { new ProtocolData(addr, value) };

            var fakeCnetStream = new FakeXGTFEnetStream(new FakeStraem() { WriteDalayTime = writeDelayTime }) { SendTimeout = writeTimeout };
            await fakeCnetStream.OpenAsync();
            XGTCnetProtocol response = await fakeCnetStream.SendAsync(resquest) as XGTCnetProtocol;
        }

        [Ignore]
        [Test]
        [ExpectedException(typeof(ReadTimeoutException))]
        public async void WhenFakeXGTFENetStreamFloated_ExpectedReadTimeoutException()
        {
            int readDelayTime = 50;
            int readTimeout = 25;
            string addr = "%MW100";
            ushort value = 0;
            var cmd = XGTFEnetCommand.WRITE_REQT;
            var type = typeof(ushort);
            XGTFEnetProtocol resquest = new XGTFEnetProtocol(type, cmd);
            resquest.Items = new List<IProtocolItem>() { new ProtocolData(addr, value) };

            var fakeCnetStream = new FakeXGTFEnetStream(new FakeStraem() { ReadDalayTime = readDelayTime }) { ReceiveTimeout = readTimeout };
            await fakeCnetStream.OpenAsync();
            XGTCnetProtocol response = await fakeCnetStream.SendAsync(resquest) as XGTCnetProtocol;
        }

        [Test]
        [TestCase(XGTFEnetCommand.READ_REQT, XGTFEnetCommand.READ_RESP)]
        [TestCase(XGTFEnetCommand.WRITE_REQT, XGTFEnetCommand.WRITE_RESP)]
        public async void WhenFakeXGTFEnetStreamFloated_ReceivedSuccessfully(XGTFEnetCommand reqt, XGTFEnetCommand resp)
        {
            ushort value = 0;
            XGTFEnetProtocol expectedResult = new XGTFEnetProtocol(typeof(ushort), resp)
            {
                CompanyID = XGTFEnetCompanyID.LSIS_XGT,
                CpuType = XGTFEnetCpuType.XGK_CPUH,
                Class = XGTFEnetClass.MASTER,
                CpuState = XGTFEnetCpuState.CPU_NOR,
                PLCState = XGTFEnetPLCSystemState.RUN,
                StreamDirection = XGTFEnetStreamDirection.PLC2PC,
                CpuInfo = XGTFEnetCpuInfo.XGK,
                InvokeID = 0,
                SlotPosition = 0,
                BasePosition = 3,
                Items = new List<IProtocolItem>() { new ProtocolData("%MW100", value) }
            };
            string addr = "%MW100";
            XGTFEnetProtocol request = new XGTFEnetProtocol(typeof(ushort), reqt) { InvokeID = expectedResult.InvokeID };
            request.ItemType = value.GetType();
            request.Items = new List<IProtocolItem>() { new ProtocolData(addr, value) };
            FakeXGTFEnetStream stream = new FakeXGTFEnetStream(new FakeStraem());
            await stream.OpenAsync();
            XGTFEnetProtocol response = await stream.SendAsync(request) as XGTFEnetProtocol;

            Assert.AreEqual(response.CompanyID, expectedResult.CompanyID);
            Assert.AreEqual(response.StreamDirection, expectedResult.StreamDirection);
            Assert.AreEqual(response.InvokeID, expectedResult.InvokeID);
            Assert.AreEqual(response.DataType, expectedResult.DataType);
            Assert.AreEqual(response.Error, expectedResult.Error);
            if (resp != XGTFEnetCommand.WRITE_RESP)
                Assert.AreEqual(response.Items.Count, expectedResult.Items.Count);
        }
    }
}