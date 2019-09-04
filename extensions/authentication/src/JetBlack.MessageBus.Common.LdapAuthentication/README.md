# JetBlack.MessageBus.Common.LdapAuthentication

An LDAP authentication plugin.

## Usage

The plugin requires the following configuration.

```json
{
    "distributor": {

        ...

        "authentication": {
            "assemblyPath": "%JETBLACK_MESSAGEBUS_ROOT%/extensions/authentication/src/JetBlack.MessageBus.Common.LdapAuthentication/bin/Debug/netstandard2.1/JetBlack.MessageBus.Common.LdapAuthentication.dll",
            "assemblyName": "JetBlack.MessageBus.Common.LdapAuthentication",
            "typeName": "JetBlack.MessageBus.Common.Security.Authentication.LdapAuthenticator",
            "args": [
                "ldap-primary.example.com",
                "636"
            ]
        },

        ...

    },

    ...

}
```