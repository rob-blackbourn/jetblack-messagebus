using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages.Test
{
    [TestClass]
    public class MulticastDataTest
    {
        [TestMethod]
        public void TestHeartbeat()
        {
            using (var stream = new MemoryStream())
            {
                var source = new MulticastData(
                    "__admin__",
                    "heartbeat",
                    true,
                    null);
                source.Write(new DataWriter(stream));
                stream.Seek(0, SeekOrigin.Begin);
                var dest = Message.Read(new DataReader(stream));
                Assert.AreEqual(source, dest);
            }
        }

        [TestMethod]
        public void TestPayload()
        {
            using (var stream = new MemoryStream())
            {
                var source = new MulticastData(
                    "__admin__",
                    "heartbeat",
                    true,
                    new DataPacket[]
                    {
                         new DataPacket(
                             new HashSet<int> {1},
                             new byte[] { 1, 2, 3, 4, 5, 6 })
                    });

                source.Write(new DataWriter(stream));
                stream.Seek(0, SeekOrigin.Begin);
                var dest = Message.Read(new DataReader(stream));
                Assert.AreEqual(source, dest);
            }
        }
    }
}
