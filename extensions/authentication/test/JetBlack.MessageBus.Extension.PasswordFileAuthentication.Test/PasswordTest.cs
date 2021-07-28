#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using JetBlack.MessageBus.Common.Security.Authentication;

namespace JetBlack.MessageBus.Extension.PasswordFileAuthentication.Test
{
    [TestClass]
    public class PasswordTest
    {
        [TestMethod]
        public void SmokeTest()
        {
            var password = Password.Create("trustno1");
            Assert.IsTrue(password.IsValid("trustno1"));
        }
    }
}
