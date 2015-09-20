#define XGTCnetStreamTestIgnore

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using NUnit.Framework;
using DY.NET.LSIS.XGT;
using System.IO.Ports;
using System.IO;

namespace DY.NET.TEST
{
    [TestFixture]
    public class XGTCnetStreamTest
    {
        private class FakeStraem : AFakeStream
        {
            public FakeStraem()
                : base()
            {
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                await Task.Delay(ReadDalay);
                System.Buffer.BlockCopy(this.Buffer, 0, buffer, offset, Buffer.Length);
                return this.Buffer.Length;
            }

            public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                await Task.Delay(WriteDalay);
                //응답 XGTCnetProtocol을 byte[]로 변환하는 과정
                List<byte> buf = new List<byte>();
                buf.Add(XGTCnetHeader.ACK.ToByte());
                buf.AddRange(new byte[] { buffer[1], buffer[2]});
                buf.Add(buffer[3]);
                buf.AddRange(XGTCnetCommandType.SS.ToBytes());
                buf.AddRange(new byte[] { buffer[6], buffer[7] });

                if (buffer[3] == XGTCnetCommand.R.ToByte())
                {
                    var size = XGTCnetTranslator.InfoDataToUInt16(new byte[] { buffer[6], buffer[7] });
                    for (int i = 0; i < size; i++)
                    {
                        if (DataType == typeof(bool))
                        {
                            buf.AddRange(XGTCnetTranslator.UInt16ToInfoData(1));
                            buf.AddRange(new byte[] { 0x30, 0x30 });
                        }
                        else if (DataType == typeof(byte) || DataType == typeof(sbyte))
                        {
                            buf.AddRange(XGTCnetTranslator.UInt16ToInfoData(1));
                            buf.AddRange(new byte[] { 0x30, 0x30 });
                        }
                        else if (DataType == typeof(ushort) || DataType == typeof(short))
                        {
                            buf.AddRange(XGTCnetTranslator.UInt16ToInfoData(2));
                            buf.AddRange(new byte[] { 0x30, 0x30, 0x30, 0x30 });
                        }
                        else if (DataType == typeof(int) || DataType == typeof(uint))
                        {
                            buf.AddRange(XGTCnetTranslator.UInt16ToInfoData(4));
                            buf.AddRange(new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 });
                        }
                        else if (DataType == typeof(long) || DataType == typeof(ulong))
                        {
                            buf.AddRange(XGTCnetTranslator.UInt16ToInfoData(8));
                            buf.AddRange(new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 });
                        }
                    }
                }
                buf.Add(XGTCnetHeader.ETX.ToByte());
                this.Buffer = buf.ToArray();
            }
        }

        private class FakeXGTCnetStream : IProtocolStream
        {
            protected XGTCnetCompressor m_Compressor = new XGTCnetCompressor();
            protected byte[] Buffer;

            public int ReceiveTimeout { get; set; }
            public int SendTimeout { get; set; }

            private System.IO.Stream m_Stream;

            public FakeXGTCnetStream(System.IO.Stream stream)
            {
                SendTimeout = -1;
                ReceiveTimeout = -1;
                this.m_Stream = stream;
            }

            public virtual Stream GetStream()
            {
                return this.m_Stream;
            }

            public virtual async Task<bool> OpenAsync()
            {
                Buffer = new byte[1024 * 4];
                return true;
            }

            public virtual async Task CloseAsync()
            {

            }

            public virtual bool IsOpend()
            {
                return true;
            }

            public void DiscardInBuffer()
            {
                
            }

            public virtual async Task<IProtocol> SendAsync(IProtocol protocol)
            {
                byte[] code = m_Compressor.Encode(protocol);
                Stream stream = GetStream();
                
                CancellationTokenSource cts = new CancellationTokenSource();
                Task write_task = stream.WriteAsync(code, 0, code.Length, cts.Token);
                if (await Task.WhenAny(write_task, Task.Delay(SendTimeout, cts.Token)) != write_task)
                {
                    cts.Cancel();
                    throw new WriteTimeoutException();
                }
                await write_task;

                cts = new CancellationTokenSource();
                int idx = 0;
                Array.Clear(this.Buffer, 0, this.Buffer.Length);
                DiscardInBuffer();
                do
                {
                    Task read_task = stream.ReadAsync(this.Buffer, idx, this.Buffer.Length - idx, cts.Token);
                    if (await Task.WhenAny(read_task, Task.Delay(ReceiveTimeout, cts.Token)) != read_task)
                    {
                        cts.Cancel();
                        throw new ReadTimeoutException();
                    }
                    idx += await (Task<int>)read_task;
                } while (Buffer[idx -1] != XGTCnetHeader.ETX.ToByte());

                var buffer = new byte[idx];
                System.Buffer.BlockCopy(this.Buffer, 0, buffer, 0, buffer.Length);
                return m_Compressor.Decode(buffer);
            }

            public void Dispose()
            {

            }
        }

        [Test]
        public async void WhenFakeXGTCnetStreamOpend_SendProtocol()
        {
            ushort localport = 20;
            var cmd = XGTCnetCommand.R;
            string addr = "%MW100";
            XGTCnetProtocol resquest = new XGTCnetProtocol(localport, cmd);
            resquest.Items = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(addr) };
            XGTCnetProtocol expectedResult = new XGTCnetProtocol(localport, cmd)
            {
                Header = XGTCnetHeader.ACK,
                Tail = XGTCnetHeader.ETX,
                Items = new System.Collections.Generic.List<IProtocolData>() { new ProtocolData(0) }
            };

            var fakeCnetStream = new FakeXGTCnetStream(new FakeStraem());
            await fakeCnetStream.OpenAsync();
            XGTCnetProtocol response = await fakeCnetStream.SendAsync(resquest) as XGTCnetProtocol;

            Assert.AreEqual(response.GetErrorCode(), expectedResult.GetErrorCode());
            Assert.AreEqual(response.Header, expectedResult.Header);
            Assert.AreEqual(response.Tail, expectedResult.Tail);
            Assert.AreEqual(response.CommandType, expectedResult.CommandType);
            Assert.AreEqual(response.Command, expectedResult.Command);
            Assert.AreEqual(response.LocalPort, expectedResult.LocalPort);
            Assert.AreEqual(response.Items.Count, expectedResult.Items.Count);
        }
    }
}