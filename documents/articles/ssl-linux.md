# SSL - Linux

This article shows how to use SSL connections with the message bus on Linux.

## Certificates

In order to use an SSL connection you will need some certificates.
See the repo [here](https://github.com/rob-blackbourn/ssl-certs) for creating SSL
certificates on Linux,

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


The SSL configuration is under the `ssl` tag.
The `certFile` property is the path to the PEM certificate file,
and the `keyFile` the path to the key file.

Now create a `distributor-ssl.sh` file with execute permissions to run the distributor.

```bash
#!/bin/bash

# The path to the config file must be absolute.
CONFIG_FILE=`pwd`/appsettings-ssl.json

./JetBlack.MessageBus.Distributor $CONFIG_FILE
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

