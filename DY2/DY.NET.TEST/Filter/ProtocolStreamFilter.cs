using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DY.NET.Filter
{
    public class ProtocolStreamFilter
    {
        public async void Test(IProtocolStream stream, IProtocol requestProtocol)
        {
            await OpenCloseOpenCloseTest(stream);
            await OpenTimeout(stream);
            await OutputTimeoutTest(stream, requestProtocol);
            await InputTimeoutTest(stream, requestProtocol);
        }

        private async Task OpenCloseOpenCloseTest(IProtocolStream stream)
        {
            stream.OpenTimeout = 1000;
            await stream.OpenAsync();
            stream.Close();
            await stream.OpenAsync();
            stream.Close();
        }

        private async Task OpenTimeout(IProtocolStream stream)
        {
            Assert.Throws(typeof(TimeoutException), async () =>
            {
                stream.OpenTimeout = 1;
                await stream.OpenAsync();
            });
        }

        private async Task InputTimeoutTest(IProtocolStream stream, IProtocol requestProtocol)
        {
            stream.OpenTimeout = 1000;
            await stream.OpenAsync();

            Assert.Throws(typeof(ReadTimeoutException), async () =>
            {
                stream.InputTimeout = 1;
                await stream.SendAsync(requestProtocol);
            });
            stream.InputTimeout = 1000;
        }

        private async Task OutputTimeoutTest(IProtocolStream stream, IProtocol requestProtocol)
        {
            stream.OpenTimeout = 1000;
            await stream.OpenAsync();

            Assert.Throws(typeof(WriteTimeoutException), async () =>
            {
                stream.OutputTimeout = 1;
                await stream.SendAsync(requestProtocol);
            });
            stream.OutputTimeout = 1000;
        }
    }
}