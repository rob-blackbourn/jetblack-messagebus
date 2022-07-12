using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages.Test
{
    [TestClass]
    public class AuthorizationResponseTest
    {
        [TestMethod]
        public void RoundTrip()
        {
            using (var stream = new MemoryStream())
            {
                var source = new AuthorizationResponse(Guid.NewGuid(), "FEED", "TOPIC", true, new HashSet<int> { 1 });
                source.Write(new DataWriter(stream));
                stream.Seek(0, SeekOrigin.Begin);
                var dest = Message.Read(new DataReader(stream));
                Assert.AreEqual(source, dest);
            }
        }
    }
}
