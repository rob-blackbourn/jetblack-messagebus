#nullable enable

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JetBlack.MessageBus.Common.IO;
using JetBlack.MessageBus.Common.Security.Authentication;
using System.Security;

namespace JetBlack.MessageBus.Common.PasswordFileAuthentication.Test
{
    [TestClass]
    public class PasswordFileAuthenticatorTest
    {
        [TestMethod]
        public void ValidTest()
        {
            var authenticator = new PasswordFileAuthenticator(new[] { "somefile.json" });
            authenticator.Manager.Set("john", "trustno1");
            authenticator.Manager.Set("mary", "password");

            using (var stream = new MemoryStream())
            {
                var writer = new DataWriter(stream);
                writer.Write("john");
                writer.Write("trustno1");

                stream.Seek(0, SeekOrigin.Begin);
                var identity = authenticator.Authenticate(stream);
                Assert.AreEqual(identity.Name, "john");
                Assert.AreEqual(identity.AuthenticationType, "BASIC");
            }
        }

        [TestMethod]
        public void InvalidTest()
        {
            var authenticator = new PasswordFileAuthenticator(new[] { "somefile.json" });
            authenticator.Manager.Set("john", "trustno1");
            authenticator.Manager.Set("mary", "password");

            using (var stream = new MemoryStream())
            {
                var writer = new DataWriter(stream);
                writer.Write("john");
                writer.Write("bad password");

                stream.Seek(0, SeekOrigin.Begin);
                Assert.ThrowsException<SecurityException>(() =>
                {
                    authenticator.Authenticate(stream);
                });
            }
        }
    }
}
