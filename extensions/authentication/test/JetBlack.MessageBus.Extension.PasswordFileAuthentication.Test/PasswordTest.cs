using Microsoft.VisualStudio.TestTools.UnitTesting;

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
