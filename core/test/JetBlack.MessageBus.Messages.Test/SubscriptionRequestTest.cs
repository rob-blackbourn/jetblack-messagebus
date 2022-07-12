using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Messages.Test
{
    [TestClass]
    public class SubscriptionRequestTest
    {
        [TestMethod]
        public void TestAddSubscriptionRequest()
        {
            using (var stream = new MemoryStream())
            {
                var source = new SubscriptionRequest("FOO", "bar", true);
                source.Write(new DataWriter(stream));
                stream.Seek(0, SeekOrigin.Begin);
                var dest = Message.Read(new DataReader(stream));
                Assert.AreEqual(source, dest);
            }
        }

        [TestMethod]
        public void TestRemoveSubscriptionRequest()
        {
            using (var stream = new MemoryStream())
            {
                var source = new SubscriptionRequest("FOO", "bar", false);
                source.Write(new DataWriter(stream));
                stream.Seek(0, SeekOrigin.Begin);
                var dest = Message.Read(new DataReader(stream));
                Assert.AreEqual(source, dest);
            }
        }
    }
}
