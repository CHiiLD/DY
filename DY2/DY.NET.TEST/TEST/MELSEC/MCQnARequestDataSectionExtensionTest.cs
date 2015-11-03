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
    public class MCQnARequestDataSectionExtensionTest
    {
        [Test]
        public void EncodeCharacterSectionA()
        {
            byte[] expectation = new byte[] { 
                0x40, 0x2A, //코드
                0x30, 0x30, 0x30, 0x31, 0x30, 0x30, //선두디바이스
                0x30, 0x30, 0x30, 0x38 }; //디바이스 점수
            MC3EProtocol protocol = new MC3EProtocol(MCQnADataType.BIT, MCQnACommand.READ);

            byte[] result = protocol.SetData4Read("M100", 8).EncodeCharacterSectionA(MCProtocolFormat.ASCII);

            Assert.AreEqual(expectation, result);
        }
    }
}