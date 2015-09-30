using System;
using NUnit.Framework;
using DY.NET.LSIS.XGT;

namespace DY.NET.Test
{
    [TestFixture]
    public class XGTFEnetProtocolTest
    {
        [Test]
        public void WhenXGTFEnetProtocolCreated_ConstructorCallInitialize()
        {
            XGTFEnetProtocol fenet = new XGTFEnetProtocol(typeof(byte), XGTFEnetCommand.READ_REQT);
            fenet.Initialize();
            Assert.AreEqual(fenet.CompanyID, XGTFEnetCompanyID.NONE);
            Assert.AreEqual(fenet.CpuType, XGTFEnetCpuType.NONE);
            Assert.AreEqual(fenet.Class, XGTFEnetClass.NONE);
            Assert.AreEqual(fenet.CpuState, XGTFEnetCpuState.NONE);
            Assert.AreEqual(fenet.PLCState, XGTFEnetPLCSystemState.NONE);
            Assert.AreEqual(fenet.StreamDirection, XGTFEnetStreamDirection.NONE);
            Assert.AreEqual(fenet.SlotPosition, 0);
            Assert.AreEqual(fenet.BasePosition, 0);
            Assert.AreNotEqual(fenet.InvokeID, ushort.MaxValue);
            Assert.AreEqual(fenet.BodyLength, 0);
            Assert.AreEqual(fenet.Error, XGTFEnetError.OK);
            Assert.AreEqual(fenet.Command, XGTFEnetCommand.NONE);
            Assert.AreEqual(fenet.DataType, XGTFEnetDataType.NONE);
        }
    }
}