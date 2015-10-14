using NUnit.Framework;
using System.Reflection;

namespace DY.NET.Filter
{
    public class CompressorFilter
    {
        public void EncodeTest(IProtocol readProtocol, IProtocolCompressor compressor, byte[] expectation)
        {
            byte[] result = compressor.Encode(readProtocol);
            Assert.AreEqual(result, expectation);
        }

        public void DecodeTest(byte[] recvData, IProtocolCompressor compressor, IProtocol expectation)
        {
            IProtocol result = compressor.Decode(recvData, expectation.Type);

            Assert.AreEqual(result.Data.Count, expectation.Data.Count);
            for(int i = 0; i < result.Data.Count; i++)
            {
                Assert.AreEqual(result.Data[i].Address, expectation.Data[i].Address);
                Assert.AreEqual(result.Data[i].Value, expectation.Data[i].Value);
            }

            PropertyInfo[] expect_properties_info = expectation.GetType().GetProperties();
            foreach (PropertyInfo info in expect_properties_info)
            {
                if(info.Name != "Data")
                {
                    object result_property_value = result.GetType().GetProperty(info.Name).GetValue(result, null);
                    object expect_property_value = expectation.GetType().GetProperty(info.Name).GetValue(expectation, null);
                    Assert.AreEqual(result_property_value, expect_property_value);
                }
            }
        }
    }
}