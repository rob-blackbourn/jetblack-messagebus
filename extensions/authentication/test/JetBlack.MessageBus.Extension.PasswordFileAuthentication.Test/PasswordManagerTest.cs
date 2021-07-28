#nullable enable

using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using JetBlack.MessageBus.Common.Security.Authentication;

using JetBlack.MessageBus.Extension.PasswordFileAuthentication;

namespace JetBlack.MessageBus.Extension.PasswordFileAuthentication.Test
{
    [TestClass]
    public class PasswordManagerTest
    {
        [TestMethod]
        public void ValidityTest()
        {
            var manager = new PasswordManager();
            manager.Set("john", "trustno1");
            Assert.IsTrue(manager.IsValid("john", "trustno1"));
            Assert.IsFalse(manager.IsValid("john", "trustsome1"));
            Assert.IsFalse(manager.IsValid("jim", "trustno1"));
        }

        [TestMethod]
        public void FileTest()
        {
            var fileName = Path.GetTempFileName();

            try
            {
                var source = new PasswordManager();
                source.Set("john", "trustno1");
                source.Set("jane", "trustsome1");
                source.Save(fileName);

                var dest = PasswordManager.Load(fileName);
                Assert.IsTrue(dest.IsValid("john", "trustno1"));
                Assert.IsTrue(dest.IsValid("jane", "trustsome1"));
                Assert.IsFalse(dest.IsValid("john", "bad password"));
                Assert.IsFalse(dest.IsValid("jane", "bad password"));
                Assert.IsFalse(dest.IsValid("bad user", "bad password"));
            }
            finally
            {
                File.Delete(fileName);
            }
        }

    }
}
