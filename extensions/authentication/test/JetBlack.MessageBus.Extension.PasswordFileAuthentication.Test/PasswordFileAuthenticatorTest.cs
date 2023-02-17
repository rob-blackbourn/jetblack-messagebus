using System.IO;
using System.Security;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Extension.PasswordFileAuthentication.Test
{
    [TestClass]
    public class PasswordFileAuthenticatorTest
    {
        [TestMethod]
        public void ValidTest()
        {
            var authenticator = new PasswordFileAuthenticator(new[] { "somefile.json" });
            authenticator.PasswordManager.Set("john", "trustno1");
            authenticator.PasswordManager.Set("mary", "password");

            using (var stream = new MemoryStream())
            {
                var writer = new DataWriter(stream);
                writer.Write("Username=john;Password=trustno1");

                stream.Seek(0, SeekOrigin.Begin);
                var identity = authenticator.Authenticate(stream);
                Assert.AreEqual(identity.User, "john");
                Assert.AreEqual(identity.Method, "BASIC");
            }
        }

        [TestMethod]
        public void InvalidTest()
        {
            var authenticator = new PasswordFileAuthenticator(new[] { "somefile.json" });
            authenticator.PasswordManager.Set("john", "trustno1");
            authenticator.PasswordManager.Set("mary", "password");

            using (var stream = new MemoryStream())
            {
                var writer = new DataWriter(stream);
                writer.Write("Username=john;Password=bad password");

                stream.Seek(0, SeekOrigin.Begin);
                Assert.ThrowsException<SecurityException>(() =>
                {
                    authenticator.Authenticate(stream);
                });
            }
        }
    }
}
