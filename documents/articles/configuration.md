# Configuration

## Default

The default configuration looks like this.

```json
{
  "distributor": {
    "address": "0.0.0.0",
    "port": 9001,
    "authentication": {
      "assemblyPath": null,
      "assemblyName": "JetBlack.MessageBus.Common",
      "typeName": "JetBlack.MessageBus.Common.Security.Authentication.NullAuthenticator",
      "args": []
    },
    "heartbeatInterval": "00:00:00",
    "prometheus": {
      "port": 9002
    },
    "allow": [
      "All"
    ],
    "deny": [
      "None"
    ],
    "isAuthorizationRequired": false,
    "useJsonLogger": false
  },
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Information",
            "Microsoft": "Information"
        },
        "Console": {
            "IncludeScopes": true,
            "TimestampFormat": "yyyy-MM-dd HH:mm:ss.fffffff "
        },
        "JsonConsole": {
            "Timestamp": "utc"
        }
    }
}
```

This provides a distributor running on port 9001 without SSL,
authentication or authorisation.

## SSL - Windows

SSL configuration is done in the `sslConfig` section
of the `distributor` configuration.

```json
{
    "distributor": {
        "address": "0.0.0.0",
        "port": 9001,
        "authentication": {
            "assemblyPath": null,
            "assemblyName": "JetBlack.MessageBus.Common",
            "typeName": "JetBlack.MessageBus.Common.Security.Authentication.NullAuthenticator",
            "args": []
        },
        "heartbeatInterval": "00:00:00",
        "sslConfig": {
            "isEnabled": true,
            "storeLocation": "LocalMachine",
            "subjectName":  "windowsvm.jetblack.net"
        },
        "allow": [
            "All"
        ],
        "deny": [
            "None"
        ],
        "isAuthorizationRequired": false
    },
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Information",
            "Microsoft": "Information"
        },
        "Console": {
            "IncludeScopes": true
        }
    }
}
```

For Windows the
`[storeLocation](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storelocation)`
refers to the location of the certificates in the trust store. This will either
be `LocalMachine` or `CurrentUser` depending on how you've set it up.
The `subjectName` is the `CN` in the certificate.

## SSL - Linux

The Linux SSL configuration uses PEM certificates and keys from files.

```json
{
    "distributor": {
        "address": "0.0.0.0",
        "port": 9001,
        "authentication": {
            "assemblyPath": null,
            "assemblyName": "JetBlack.MessageBus.Common",
            "typeName": "JetBlack.MessageBus.Common.Security.Authentication.NullAuthenticator",
            "args": []
        },
        "heartbeatInterval": "00:00:00",
        "sslConfig": {
            "isEnabled": true,
            "certFile": "%HOME%/.keys/server.crt",
            "keyFile": "%HOME%/.keys/server.key"
        },
        "allow": [
            "All"
        ],
        "deny": [
            "None"
        ],
        "isAuthorizationRequired": false
    },
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Information",
            "Microsoft": "Information"
        },
        "Console": {
            "IncludeScopes": true
        }
    }
}
```

The `certFile` property specifies the path to the certificate,
while the `keyFile` specifies the path to the key.

## Authentication - JWT

The following configuration provides authentication through JSON Web Tokens.

```json
{
    "distributor": {
        "address": "0.0.0.0",
        "port": 9001,
        "authentication": {
            "assemblyPath": "%MESSAGEBUS_EXTENSIONS%/JetBlack.MessageBus.Extension.JwtAuthentication.dll",
            "assemblyName": "JetBlack.MessageBus.Extension.JwtAuthentication",
            "typeName": "JetBlack.MessageBus.Extension.JwtAuthenticator",
            "args": [
                "A secret of more than 15 characters"
            ]
        },
        "heartbeatInterval": "00:00:00",
        "sslConfig": {
            "isEnabled": true,
            "certFile": "%HOME%/.keys/server.crt",
            "keyFile": "%HOME%/.keys/server.key"
        },
        "prometheus": {
            "port": 9002,
            "isEnabled": true
        },
        "allow": [
            "All"
        ],
        "deny": [
            "None"
        ],
        "isAuthorizationRequired": false
    },
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "System": "Information",
            "Microsoft": "Information"
        },
        "Console": {
            "IncludeScopes": true
        }
    }
}
```

## Authentication - LDAP

The following configuration file provides authentication for LDAP.

```json
{
    "distributor": {
        "address": "0.0.0.0",
        "port": 9001,
        "authentication": {
            "assemblyPath": "%MESSAGEBUS_EXTENSIONS%/JetBlack.MessageBus.Extension.LdapAuthentication.dll",
            "assemblyName": "JetBlack.MessageBus.Extension.LdapAuthentication",
            "typeName": "JetBlack.MessageBus.Extension.LdapAuthentication.LdapAuthenticator",
            "args": [
                "%LDAP_HOST%",
                "636"
            ]
        },
        "heartbeatInterval": "00:00:00",
        "sslConfig": {
            "isEnabled": true,
            "certFile": "%HOME%/.keys/server.crt",
            "keyFile": "%HOME%/.keys/server.key"
        },
        "allow": [
            "All"
        ],
        "deny": [
            "None"
        ],
        "isAuthorizationRequired": false
    },
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Information",
            "Microsoft": "Information"
        },
        "Console": {
            "IncludeScopes": true
        }
    }
}
```

## Authentication - Password File

The following provides authentication using a password file.

```json
{
    "distributor": {
        "address": "0.0.0.0",
        "port": 9001,
        "authentication": {
            "assemblyPath": "%MESSAGEBUS_EXTENSIONS%/JetBlack.MessageBus.Extension.PasswordFileAuthentication.dll",
            "assemblyName": "JetBlack.MessageBus.Extension.PasswordFileAuthentication",
            "typeName": "JetBlack.MessageBus.Extension.PasswordFileAuthentication.PasswordFileAuthenticator",
            "args": [
                "%PASSWORD_FOLDER%/passwords.json"
            ]
        },
        "heartbeatInterval": "00:00:00",
        "sslConfig": {
            "isEnabled": true,
            "certFile": "%HOME%/.keys/server.crt",
            "keyFile": "%HOME%/.keys/server.key"
        },
        "allow": [
            "All"
        ],
        "deny": [
            "None"
        ],
        "isAuthorizationRequired": false
    },
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Information",
            "Microsoft": "Information"
        },
        "Console": {
            "IncludeScopes": true
        }
    }
}
```

## Authorization

The following is an example of authorisation.

```json
{
    "distributor": {
        "address": "0.0.0.0",
        "port": 9001,
        "authentication": {
            "assemblyPath": "%MESSAGEBUS_EXTENSIONS%/JetBlack.MessageBus.Extension.PasswordFileAuthentication.dll",
            "assemblyName": "JetBlack.MessageBus.Extension.PasswordFileAuthentication",
            "typeName": "JetBlack.MessageBus.Extension.PasswordFileAuthentication.PasswordFileAuthenticator",
            "args": [
                "%JETBLACK_MESSAGEBUS_ROOT%/examples/server-config/password-file/passwords.json"
            ]
        },
        "heartbeatInterval": "00:00:00",
        "sslConfig": {
            "isEnabled": true,
            "certFile": "%HOME%/.keys/server.crt",
            "keyFile": "%HOME%/.keys/server.key"
        },
        "allow": [
            "None"
        ],
        "deny": [
            "All"
        ],
        "isAuthorizationRequired": true,
        "feedRoles": {
            "UNAUTH": {
                "isAuthorized": false,
                "allow": [
                    "All"
                ],
                "deny": [
                    "None"
                ]
            },
            "AUTH": {
                "isAuthorized": true,
                "allow": [
                    "None"
                ],
                "deny": [
                    "All"
                ],
                "interactorRoles": {
                    "%HOSTNAME%": {
                        "tom": {
                            "allow": [
                                "Subscribe"
                            ],
                            "deny": [
                                "Publish",
                                "Notify",
                                "Authorize"
                            ]
                        },
                        "dick": {
                            "allow": [
                                "Subscribe"
                            ],
                            "deny": [
                                "Publish",
                                "Notify",
                                "Authorize"
                            ]
                        },
                        "harry": {
                            "allow": [
                                "Publish",
                                "Notify"
                            ],
                            "deny": [
                                "Subscribe",
                                "Authorize"
                            ]
                        },
                        "mary": {
                            "allow": [
                                "Authorize"
                            ],
                            "deny": [
                                "Publish",
                                "Notify",
                                "Subscribe"
                            ]
                        }
                    }
                }
            }
        }
    },
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Information",
            "Microsoft": "Information"
        },
        "Console": {
            "IncludeScopes": true
        }
    }
}
```