# JetBlack.MessageBus.Extension.LdapAuthentication

An LDAP authentication plugin.

## Usage

The plugin requires the following configuration.

```json
{
    "distributor": {

        ...

        "authentication": {
            "assemblyPath": "%JETBLACK_MESSAGEBUS_ROOT%/extensions/authentication/src/JetBlack.MessageBus.Extension.LdapAuthentication/bin/Debug/netstandard2.1/JetBlack.MessageBus.Extension.LdapAuthentication.dll",
            "assemblyName": "JetBlack.MessageBus.Extension.LdapAuthentication",
            "typeName": "JetBlack.MessageBus.Extension.LdapAuthentication.LdapAuthenticator",
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