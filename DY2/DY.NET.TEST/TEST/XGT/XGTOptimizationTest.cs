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
        [TestCase("M00200", "M0020")]
        [TestCase("M00201", "M0020")]
        [TestCase("M0022A", "M0022")]
        [TestCase("D01234.0", "D01234")]
        public void ConvertBitAddrToWordAddrTest(string bitAddr, string expect)
        {
            XGTOptimizationTool opt = new XGTOptimizationTool();
            Assert.AreEqual(opt.ConvertBitAddrToWordAddr(bitAddr), expect);
        }

        [Test]
        [TestCase("M100A", typeof(bool), "%MX100A")]
        [TestCase("D1001.1", typeof(bool), "%DX10011")]
        [TestCase("M1018", typeof(byte), "%MB203")]
        [TestCase("M00208", typeof(byte), "%MB41")]
        [TestCase("D0020.0", typeof(byte), "%DB40")]
        [TestCase("D1022", typeof(ushort), "%DW1022")]
        [TestCase("M1020", typeof(int), "%MD510")]
        [TestCase("M1020", typeof(uint), "%MD510")]
        [TestCase("M400", typeof(ulong), "%ML100")]
        public void ConvertToGlopaVariableNameTest(string norAddr, Type type, string expect)
        {
            XGTOptimizationTool opt = new XGTOptimizationTool();
            Assert.AreEqual(opt.ToGlopaVariableName(norAddr, type), expect);
        }

        [Test]
        [TestCase("%MX100A", (ushort)0x1234, false)]
        [TestCase("%MX100B", (ushort)0x1234, false)]
        [TestCase("%MX100C", (ushort)0x1234, true)]
        [TestCase("%MX100D", (ushort)0x1234, false)]
        public void SearchBooleanFromUInt16Test(string addr, ushort value, bool expect)
        {
            XGTOptimizationTool opt = new XGTOptimizationTool();
            Assert.AreEqual(opt.SearchBooleanFromUInt16(addr, value), expect);
        }

        [Test]
        public void PackageTest()
        {
            IList<IProtocolItemWithType> list = new List<IProtocolItemWithType>()
            {
                new DetailProtocolData("M00000", typeof(bool)),
                new DetailProtocolData("M00001", typeof(bool)),
                new DetailProtocolData("M00002", typeof(bool)),
                new DetailProtocolData("M00003", typeof(bool)),
                new DetailProtocolData("M00004", typeof(bool))  
            };
            string expect_addr = "%MW0000";
            IList<IProtocolItemWithType>[] ret = XGTHelper.Classify(list);

            Assert.AreEqual(ret.Count(), 1);
            Assert.AreEqual(ret[0].Count(), 1);
            Assert.AreEqual(ret[0][0].Address, expect_addr);
        }

        [Test]
        public void MatchTest()
        {
            IList<IProtocolItemWithType> list = new List<IProtocolItemWithType>()
            {
                new DetailProtocolData("M00000", typeof(bool)),
                new DetailProtocolData("M00208", typeof(sbyte)),
                new DetailProtocolData("M0145", typeof(ushort)),
                new DetailProtocolData("D0112", typeof(short)),
                new DetailProtocolData("D0024", typeof(long)),
                new DetailProtocolData("M0012", typeof(int)),
            };
            IList<IProtocolItem> recv = new List<IProtocolItem>()
            {
                new ProtocolData("%MW0000", (ushort)0xFFFF),
                new ProtocolData("%MB41", (sbyte)-1),
                new ProtocolData("%MW0145", (ushort)0xFFFF),
                new ProtocolData("%DW0112", (short)-1),
                new ProtocolData("%DL6",    (long)-1),
                new ProtocolData("%MD6",    (int)-1),
            };
            object[] expect = new object[] { true, (sbyte)-1, (ushort)0xFFFF, (short)-1, (long)-1, (int)-1 };
            XGTHelper.Match(list, recv);

            for (int i = 0; i < 5; i++)
                Assert.AreEqual(list[i].Value, expect[i]);
        }

        [Test]
        public void ClassifyTest()
        {
            IList<IProtocolItemWithType> list = new List<IProtocolItemWithType>()
            {
                new DetailProtocolData("M00000", typeof(bool)),
                new DetailProtocolData("M00001", typeof(bool)),
                new DetailProtocolData("M00002", typeof(bool)),
                new DetailProtocolData("M00003", typeof(bool)),
                new DetailProtocolData("M00004", typeof(bool)),

                new DetailProtocolData("D8012", typeof(short)),
                new DetailProtocolData("M0032", typeof(short)),
                new DetailProtocolData("M0145", typeof(short)),
                new DetailProtocolData("D0112", typeof(short)),

                new DetailProtocolData("M0132", typeof(ushort)),
                new DetailProtocolData("M0111", typeof(ushort)),
                new DetailProtocolData("D6012", typeof(ushort)),
                new DetailProtocolData("M1032", typeof(ushort)),

                new DetailProtocolData("M1011", typeof(ushort)),
                new DetailProtocolData("M1145", typeof(ushort)),
                new DetailProtocolData("M1532", typeof(ushort)),
                new DetailProtocolData("M1611", typeof(ushort)),

                new DetailProtocolData("M1345", typeof(short)),
                new DetailProtocolData("D1412", typeof(short)),
                new DetailProtocolData("M1502", typeof(short)),
                new DetailProtocolData("M1614", typeof(short)),

                new DetailProtocolData("M1745", typeof(short)),
                new DetailProtocolData("M0245", typeof(short)),
                new DetailProtocolData("D0012", typeof(short)),

                new DetailProtocolData("M06540", typeof(byte)),
                new DetailProtocolData("D0024", typeof(long)),
                new DetailProtocolData("M0012", typeof(uint)),

                new DetailProtocolData("M0000", typeof(short)),
            };
            List<string> expect_addr = new List<string>() {
                "%MW0000", 
                
                "%DW8012", "%MW0032", "%MW0145", "%DW0112", 
                "%MW0132", "%MW0111", "%DW6012", "%MW1032", 
                "%MW1011", "%MW1145","%MW1532", "%MW1611", 
                "%MW1345", "%DW1412", "%MW1502", "%MW1614", 

                "%MW1745", "%MW0245", "%DW0012", 

                "%MB1308",
                "%DL6",
                "%MD6" };

            IList<IProtocolItemWithType>[] ret = XGTHelper.Classify(list);

            Assert.AreEqual(ret.Count(), 5);
            Assert.AreEqual(ret[0].Count(), 9);
            Assert.AreEqual(ret[1].Count(), 11);
            Assert.AreEqual(ret[2].Count(), 1);
            Assert.AreEqual(ret[3].Count(), 1);
            Assert.AreEqual(ret[4].Count(), 1);

            foreach(var item in ret)
            {
                foreach (var i in item)
                {
                    string addr = i.Address;
                    Assert.AreNotEqual(expect_addr.Find(x => x == addr), null);
                }
            }
        }
    }
}