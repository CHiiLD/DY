using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using DY.NET.LSIS.XGT;

namespace DY.NET.Test
{
    [TestFixture]
    public class MC1ECompressorTest
    {
        public void EncodeHeaderPartByASCII()
        {
            byte[] expectation = new byte[] { 0x35, 0x30, 0x30, 0x30, //sub
            0x00, 0x00, //net
            0x46, 0x46, //plc
            0x30, 0x33, 0x46, 0x46, //io
            0x30, 0x30,  //local
            0x30, 0x30, 0x31, 0x38, //len
            0x30, 0x30, 0x31, 0x30, //timer
            };
        }

        public void EncodeByASCII()
        {
            //비트단위 일괄읽기
            byte[] expectation = { 0x30, 0x34, 0x30, 0x31, 
                                 0x30, 0x30, 0x30, 0x31,
                                 0x40, 0x2A, 
                                 0x30, 0x30, 0x30, 0x31, 0x30, 0x30, 0x03,
                                 0x30, 0x30, 0x30, 0x38,
                                 };

        }

        public void EncodeByBinary()
        {
            //비트단위 일괄읽기
            byte[] expectation = { 0x01, 0x04, 
                                     0x01, 0x00,
                                     0x64, 0x00, 0x00,
                                     0x90,
                                     0x08, 0x00
                                 };

        }
    }
}
