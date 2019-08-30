using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JetBlack.MessageBus.Common.Json.Test
{
    [TestClass]
    public class JsonByteEncoderTest
    {
        [TestMethod]
        public void ShouldEncodeDictionary()
        {
            var source = new Dictionary<string, object>
            {
                { "NAME", "John Smith" },
                { "AGE", 21 }
            };

            var encoder = new JsonByteEncoder();
            var buf = encoder.Encode(source);
            var dest = (Dictionary<string, object>)encoder.Decode(buf);
            Assert.IsTrue(source.AsEnumerable().SequenceEqual(dest.AsEnumerable()));
        }
    }
}
