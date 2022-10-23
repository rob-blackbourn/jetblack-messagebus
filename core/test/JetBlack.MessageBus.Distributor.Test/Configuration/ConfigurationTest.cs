using System.IO;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JetBlack.MessageBus.Distributor.Configuration;

namespace JetBlack.MessageBus.Distributor.Test.Configuration
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void ShouldLoadConfiguration()
        {
            const string json = @"
{
    ""distributor"": {
        ""address"": ""0.0.0.0"",
        ""port"": 9002,
        ""authentication"": {
                ""assemblyPath"": ""%MESSAGEBUS_HOME%/extensions/authentication/JetBlack.MessageBus.Extension.PasswordFileAuthentication/JetBlack.MessageBus.Extension.PasswordFileAuthentication.dll"",
            ""assemblyName"": ""JetBlack.MessageBus.Extension.PasswordFileAuthentication"",
            ""typeName"": ""JetBlack.MessageBus.Extension.PasswordFileAuthentication.PasswordFileAuthenticator"",
            ""args"": [
                ""%JETBLACK_MESSAGEBUS_ROOT%/examples/server-config/password-file/passwords.json""
            ]
        },
        ""heartbeatInterval"": ""00:00:00"",
        ""ssl"": {
                ""isEnabled"": true,
            ""certFile"": ""%HOME%/.keys/server.crt"",
            ""keyFile"": ""%HOME%/.keys/server.key""
        },
        ""allow"": [
            ""None""
        ],
        ""deny"": [
            ""All""
        ],
        ""isAuthorizationRequired"": true,
        ""feedRoles"": {
                ""UNAUTH"": {
                    ""isAuthorized"": false,
                ""allow"": [
                    ""All""
                    ],
                ""deny"": [
                    ""None""
                    ]
            },
            ""AUTH"": {
                    ""isAuthorized"": true,
                ""allow"": [
                    ""None""
                ],
                ""deny"": [
                    ""All""
                ],
                ""interactorRoles"": {
                        ""%HOSTNAME%"": {
                            ""tom"": {
                                ""allow"": [
                                    ""Subscribe""
                                ],
                            ""deny"": [
                                ""Publish"",
                                ""Notify"",
                                ""Authorize""
                                ]
                        },
                        ""dick"": {
                                ""allow"": [
                                    ""Subscribe""
                            ],
                            ""deny"": [
                                ""Publish"",
                                ""Notify"",
                                ""Authorize""
                            ]
                        },
                        ""harry"": {
                                ""allow"": [
                                    ""Publish"",
                                ""Notify""
                            ],
                            ""deny"": [
                                ""Subscribe"",
                                ""Authorize""
                            ]
                        },
                        ""mary"": {
                                ""allow"": [
                                    ""Authorize""
                            ],
                            ""deny"": [
                                ""Publish"",
                                ""Notify"",
                                ""Subscribe""
                            ]
                        }
                        }
                    }
                }
            }
        },
    ""Logging"": {
        ""LogLevel"": {
            ""Default"": ""Debug"",
            ""System"": ""Information"",
            ""Microsoft"": ""Information""
        },
        ""Console"": {
            ""IncludeScopes"": true
        }
    }
}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            var distributorConfig = configuration.GetSection("distributor").Get<DistributorConfig>();
            Assert.IsNotNull(distributorConfig);
        }
    }
}
