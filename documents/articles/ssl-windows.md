# SSL - Windows

This article shows how to use SSL connections with the message bus on Windows.

## Certificates

In order to use an SSL connection you will need some certificates.
See the article [here](ssl-windows-certs.md) for creating SSL
certificates on windows,

## Configuration

To use SSL we need a custom configuration. We can see the standard
configuration [here][configuration.md]. 
Create the file `appsettings-ssl.ssl` with the following contents.


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
        "ssl": {
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


The SSL configuration is under the `ssl` tag.
The
`[storeLocation](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storelocation)`
refers to the location of the certificates in the trust store. This will either
be `LocalMachine` or `CurrentUser` depending on how you've set it up.
The `subjectName` is the `CN` in the certificate.

Now create a `distributor-ssl.bat` file to run the application. The
following assumes the distributor was unpacked to `C:\Distributor`.

```bat
JetBlack.MessageBus.Distributor %~dp0%\appsettings-ssl.json
```

The settings file is provided as the first argument and *must* be an absolute
path.

When the distributor is started we get the following message.

```
2021-08-01 09:05:15.2016415 info: JetBlack.MessageBus.Distributor.Acceptor[0]
      Listening on 0.0.0.0:9001 with SSL enabled with NULL authentication
```

## Clients

To Enable SSL on the clients the `isSslEnabled` flag must be set to
`true` when the client is created.

```cs
var client = Client.Create("localhost", 9001, isSslEnabled: true);
```
