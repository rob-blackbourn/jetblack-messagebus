using System.Collections.Generic;
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
            var role = new RoleManager(
                    new DistributorRole(
                        Role.Subscribe,
                        Role.Authorize | Role.Notify | Role.Publish,
                        false,
                        false,
                        false),
                    "beast.jetblack.net",
                    "admin",
                    null,
                    null,
                    new Dictionary<string, Dictionary<string, Permission>>
                    {
                        {
                            "UNAUTH",
                            new Dictionary<string, Permission>
                            {
                                {
                                    "*",
                                    new Permission(Role.All, Role.None)
                                }
                            }
                        },
                        {
                            "AUTH",
                            new Dictionary<string, Permission>
                            {
                                {
                                    "beast.jetblack.net",
                                    new Permission(Role.Publish, Role.None)
                                }
                            }
                        }
                    });
            Assert.IsTrue(role.HasRole("UNAUTH", Role.Subscribe));
            Assert.IsTrue(role.HasRole("UNAUTH", Role.Publish));
            Assert.IsTrue(role.HasRole("UNAUTH", Role.Notify));
            Assert.IsTrue(role.HasRole("UNAUTH", Role.Authorize));
            Assert.IsTrue(role.HasRole("AUTH", Role.Subscribe));
            Assert.IsTrue(role.HasRole("AUTH", Role.Publish));
            Assert.IsFalse(role.HasRole("AUTH", Role.Notify));
            Assert.IsFalse(role.HasRole("AUTH", Role.Authorize));
        }
    }
}
