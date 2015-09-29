using System;
using System.Collections.Generic;
using NUnit.Framework;
using DY.NET.LSIS.XGT;
using System.Linq;

namespace DY.NET.TEST
{
    [TestFixture]
    public class XGTOptimizationTest
    {
        [Test]
        public void ConvertBitAddrToWordAddrTest()
        {
            List<string> bitList = new List<string>() 
            {
                "M00200",
                "M00201",
                "M00210",
                "D01234.0",
            };
            List<string> expectedList = new List<string>() 
            {
                "%MW0020",
                "%MW0020",
                "%MW0021",
                "%DW01234",
            };
            List<string> wordList = new List<string>();
            var opt = new XGTOptimization();
            foreach (var item in bitList)
                wordList.Add(opt.ConvertBitAddrToWordAddr(item));
            Assert.True(bitList.SequenceEqual(wordList));
        }
    }
}
