#nullable enable

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JetBlack.MessageBus.Common.IO;
using System.Collections.Generic;

namespace JetBlack.MessageBus.Messages.Test
{
    [TestClass]
    public class UnicastDataTest
    {
        [TestMethod]
        public void TestNoPayload()
        {
            using (var stream = new MemoryStream())
            {
                var source = new UnicastData(
                  Guid.NewGuid(),
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
                var source = new UnicastData(
                  Guid.NewGuid(),
                  "__admin__",
                  "heartbeat",
                  true,
                  new DataPacket[]
                  {
                      new DataPacket(
                          new HashSet<int> { 1 },
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
