using Microsoft.VisualStudio.TestTools.UnitTesting;

using JetBlack.MessageBus.Common.Security.Authentication;
using JetBlack.MessageBus.Distributor.Roles;

namespace JetBlack.MessageBus.Distributor.Test.Roles
{
    [TestClass]
    public class InteractorRoleTest
    {
        [TestMethod]
        public void ShouldUnderstandRoles()
        {
            var role = new InteractorRole("host", "user", Role.Subscribe, Role.Publish | Role.Notify | Role.Authorize);
            Assert.IsTrue(role.HasRole(Role.Subscribe, true));
            Assert.IsFalse(role.HasRole(Role.Publish, true));
            Assert.IsFalse(role.HasRole(Role.Notify, true));
            Assert.IsFalse(role.HasRole(Role.Authorize, true));
        }
    }
}
