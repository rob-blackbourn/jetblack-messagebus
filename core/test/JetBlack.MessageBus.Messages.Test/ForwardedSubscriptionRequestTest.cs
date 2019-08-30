using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages.Test
{
    [TestClass]
    public class ForwardedSubscriptionRequestTest
    {
        [TestMethod]
        public void TestAddForwardedSubscriptionRequest()
        {
            using (var stream = new MemoryStream())
            {
                var source = new ForwardedSubscriptionRequest("USER", "HOST", Guid.NewGuid(), "FOO", "bar", true);
                source.Write(new DataWriter(stream));
                stream.Seek(0, SeekOrigin.Begin);
                var dest = Message.Read(new DataReader(stream));
                Assert.AreEqual(source, dest);
            }
        }

        [TestMethod]
        public void TestRemoveForwardedSubscriptionRequest()
        {
            using (var stream = new MemoryStream())
            {
                var source = new ForwardedSubscriptionRequest("USER", "HOST", Guid.NewGuid(), "FOO", "bar", false);
                source.Write(new DataWriter(stream));
                stream.Seek(0, SeekOrigin.Begin);
                var dest = Message.Read(new DataReader(stream));
                Assert.AreEqual(source, dest);
            }
        }
    }
}
