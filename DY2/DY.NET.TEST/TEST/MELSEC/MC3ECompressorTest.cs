using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using DY.NET.Mitsubishi.MELSEC;

namespace DY.NET.TEST
{
    [TestFixture]
    public class MC3ECompressorTest
    {
        [Test]
        public void EncodeQHeaderByASCII()
        {
            byte[] expectation = new byte[] { 
                0x30, 0x30, //net
                0x46, 0x46, //plc
                0x30, 0x33, 0x46, 0x46, //io
                0x30, 0x30,  //local
                0x30, 0x30, 0x31, 0x38, //len
                0x30, 0x30, 0x31, 0x30, //timer
            };
            MC3EProtocol target = new MC3EProtocol();
            target.Initialize();
            target.NetworkNumber = 0x00;
            target.PLCNumber = 0xFF;
            target.ModuleIONumber = 0x03FF;
            target.ModuleLocalNumber = 0x00;
            target.DataLength = 0x18;
            target.CPUMonitorTimer = 0x10;
            MC3ECompressor comp = new MC3ECompressor();
            comp.Format = MCProtocolFormat.ASCII;

            byte[] result = comp.EncodeQHeader(target);
            Assert.AreEqual(result, expectation);
        }

        [Test]
        public void DecodeQHeaderByASCII()
        {
            byte[] target = new byte[] { 
                0x35, 0x30, 0x30, 0x30,
                0x30, 0x30, //net
                0x46, 0x46, //plc
                0x30, 0x33, 0x46, 0x46, //io
                0x30, 0x30,  //local
                0x30, 0x30, 0x30, 0x43, //len
                0x30, 0x30, 0x30, 0x30, //timer
            };
            MC3EProtocol expectation = new MC3EProtocol();
            expectation.Initialize();
            MC3ECompressor comp = new MC3ECompressor();
            comp.Format = MCProtocolFormat.ASCII;

            comp.DecodeQHeader(target, expectation);
            
            Assert.AreEqual(expectation.NetworkNumber, 0x00);
            Assert.AreEqual(expectation.PLCNumber, 0xFF);
            Assert.AreEqual(expectation.ModuleIONumber, 0x03FF);
            Assert.AreEqual(expectation.ModuleLocalNumber, 0x00);
            Assert.AreEqual(expectation.DataLength, 0x0C);
            Assert.AreEqual(expectation.Error, MCEFrameError.OK);
        }

        [Test]
        public void EncodeQHeaderByBinary()
        {
            MC3EProtocol target = new MC3EProtocol();
            target.Initialize();
            target.NetworkNumber = 0x00;
            target.PLCNumber = 0xFF;
            target.ModuleIONumber = 0x03FF;
            target.ModuleLocalNumber = 0x00;
            target.DataLength = 0x00;
            target.CPUMonitorTimer = 0x10;
            byte[] expectation = new byte[] { 
                0x00, //net
                0xFF, //plc
                0xFF, 0x03, //io
                0x00,  //local
                0x00, 0x00, //len
                0x10, 0x00, //timer
            };
            MC3ECompressor comp = new MC3ECompressor();
            comp.Format = MCProtocolFormat.BINARY;

            byte[] result = comp.EncodeQHeader(target);
            Assert.AreEqual(result, expectation);
        }

        [Test]
        public void DecodeQHeaderByBinary()
        {
            byte[] target = new byte[] { 
                0xD0, 0x00,
                0x00, //net
                0xFF, //plc
                0xFF, 0x03, //io
                0x00,  //local
                0x06, 0x00, //len
                0x00, 0x00, //timer
            };
            MC3EProtocol expectation = new MC3EProtocol();
            expectation.Initialize();
            MC3ECompressor comp = new MC3ECompressor();
            comp.Format = MCProtocolFormat.BINARY;

            comp.DecodeQHeader(target, expectation);

            Assert.AreEqual(expectation.NetworkNumber, 0x00);
            Assert.AreEqual(expectation.PLCNumber, 0xFF);
            Assert.AreEqual(expectation.ModuleIONumber, 0x03FF);
            Assert.AreEqual(expectation.ModuleLocalNumber, 0x00);
            Assert.AreEqual(expectation.DataLength, 0x06);
            Assert.AreEqual(expectation.Error, MCEFrameError.OK);
        }

        [Test]
        public void Encode1EProtocolByASCII()
        {
            byte[] extention = new byte[] {
                0x35, 0x30, 0x30, 0x30, //sub header
                0x30, 0x30, //net
                0x46, 0x46, //plc
                0x30, 0x33, 0x46, 0x46, //io
                0x30, 0x30,  //local
                0x30, 0x30, 0x31, 0x38, //len
                0x30, 0x30, 0x31, 0x30, //timer
                0x30, 0x34, 0x30, 0x31, //command
                0x30, 0x30, 0x30, 0x31, //sub cmd
                0x40, 0x2A, //device code
                0x30, 0x30, 0x30, 0x31, 0x30, 0x30, //선두디바이스
                0x30, 0x30, 0x30, 0x38, //디바이스 점수 

            };
        }
    }
}
