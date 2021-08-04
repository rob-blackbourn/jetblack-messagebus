# Authentication

The distributor server has a pluggable authentication module. As you can see from the
default `appsettings.json` the default authenticator is the `NullAuthenticator`.

```json
{
  "distributor": {
    ...
    "authentication": {
      "assemblyPath": null,
      "assemblyName": "JetBlack.MessageBus.Common",
      "typeName": "JetBlack.MessageBus.Common.Security.Authentication.NullAuthenticator",
      "args": []
    },
    ...
  }
  ...
}
```

The client must connect with the appropriate client side authenticator. For the
`NullAuthenticator` this is the `NullClientAuthenticator` from the
`JetBlack.MessageBus.Adapters` assembly. If no authenticator is specified
when the client is created the null authenticator is used by default.

```cs
var client = Client.Create("localhost", 9001);
```

This is equivalent to the code above.

```cs
var authenticator = new NullClientAuthenticator();
var client = Client.Create("localhost", 9001, authenticator: authenticator);
```

## Password File Authenticator

### Distributor

A simple password file authenticator is provided as an extension. The extension
can be downloaded from the 
[Releases](https://github.com/rob-blackbourn/jetblack-messagebus/releases) 
page as `JetBlack.MessageBus.Extension.PasswordFileAuthentication`. Unpack the
files into the folder "extensions" under the distributor. Also download the
`MakePassword` utility and unpack it into a `utilities` folder under the
distributor. The folder structure should look like as follows.

```
distributor
|   appsettings.json
|   distributor.bat
|   JetBlack.MessageBus.Distributor.exe
|   JetBlack.MessageBus.Distributor.pdb
|
+---extensions
|   +---JetBlack.MessageBus.Extension.PasswordFileAuthentication
|           JetBlack.MessageBus.Extension.PasswordFileAuthentication.deps.json
|           JetBlack.MessageBus.Extension.PasswordFileAuthentication.dll
|           JetBlack.MessageBus.Extension.PasswordFileAuthentication.pdb
|           Newtonsoft.Json.dll
|
+---utilities
    +---MakePassword
            MakePassword.exe
            MakePassword.pdb
            passwords.json
```

Now create copy the `appsettings.json` to `appsettings-pwd.json` and create a
script to run the distributor called `distributor-pwd.bat` with the following
contents.

```bat
REM Start the distributor

set MESSAGEBUS_HOME=%~dp0
set CONFIG_FILE=%MESSAGEBUS_HOME%\appsettings-pwd.json

JetBlack.MessageBus.Distributor %CONFIG_FILE%
```

No change the `authentication` section of `appsettings-pwd.json` to be the
following.

```json
{
  "distributor": {
    ...
    "authentication": {
      "assemblyPath": "%MESSAGEBUS_HOME%/extensions/JetBlack.MessageBus.Extension.PasswordFileAuthentication/JetBlack.MessageBus.Extension.PasswordFileAuthentication.dll",
      "assemblyName": "JetBlack.MessageBus.Extension.PasswordFileAuthentication",
      "typeName": "JetBlack.MessageBus.Extension.PasswordFileAuthentication.PasswordFileAuthenticator",
      "args": [
        "%MESSAGEBUS_HOME%/utilities/MakePassword/passwords.json"
      ]
    },    ...
  }
  ...
}
```

For simplicity we we not set up the SSL configuration here, but as usernames
and passwords will be exchange this should also be configured.

The password file authenticator takes a single argument in the `args` property
which is the path to the passwords file. The `MESSAGEBUS_HOME` environment
variable was set up in the launch script. The password file is created using
the `MakePassword` utility and for simplicity we will keep the file in the
same folder. Make a password in the following manner.

```
MakePassword passwords.json rob
Enter Password: ********
```

Not start the distributor with the `distributor-pwd` script. You should see the
following:

```
info: JetBlack.MessageBus.Distributor.Server[0]
      Starting server version 5.1.0.0
info: JetBlack.MessageBus.Distributor.Server[0]
      Server started
info: JetBlack.MessageBus.Distributor.Program[0]
      Waiting for SIGTERM/SIGINT on PID 21864
info: JetBlack.MessageBus.Distributor.Acceptor[0]
      Listening on 0.0.0.0:9001 with SSL disabled with BASIC authentication
```

The `BASIC authentication` indicates the password file authenticator is being
used.

### Client

The client should use the `BasicClientAuthenticator` as follows.

```cs
var authenticator = new BasicClientAuthenticator(username, password);
var client = Client.Create(server, 9002, authenticator: authenticator);
```

Using the code found in [Getting Started - Windows](getting-started-windows.md)
change the client creation for the subscriber and publisher to use the code above
with the username and password that were created with `MakePassword`.

## LDAP Authenticator

There is an extension which can perform LDAP authentication.

### Distributor

Download the `JetBlack.MessageBus.Extension.LdapAuthentication` package and
unpack it in the same manner as before. Copy the `appsettings.json` to
`appsettings-ldap.json` and change the `authentication` section to the following.

```json
{
  "distributor": {
    ...
    "authentication": {
      "assemblyPath": "%MESSAGEBUS_HOME%/extensions/JetBlack.MessageBus.Extension.LdapAuthentication/JetBlack.MessageBus.Extension.LdapAuthentication.dll",
      "assemblyName": "JetBlack.MessageBus.Extension.LdapAuthentication",
      "typeName": "JetBlack.MessageBus.Extension.LdapAuthentication.LdapAuthenticator",
      "args": [
          "ldap.jetblack.net",
          "636"
      ]
    },
    ...
  }
  ...
}
```

This extension takes two arguments in the `args` array property. The first is
the host of the LDAP service, and the second is the port.

### Client

The `BasicClientAuthenticator` class is used for the client, as the same
properties are required (username and password). This time pass in valid LDAP
credentials.

## JWT Authentication

The JWT authenticator uses JSON web tokens to perform authentication.

### Distributor

Download the `JetBlack.MessageBus.Extension.JwtAuthentication` package and
unpack it in the same manner as before. Copy the `appsettings.json` to
`appsettings-jwt.json` and change the `authentication` section to the following.

```json
{
  "distributor": {
    ...
    "authentication": {
      "assemblyPath": "%MESSAGEBUS_HOME%/extensions/JetBlack.MessageBus.Extension.JwtAuthentication/JetBlack.MessageBus.Extension.JwtAuthentication.dll",
      "assemblyName": "JetBlack.MessageBus.Extension.JwtAuthentication",
      "typeName": "JetBlack.MessageBus.Extension.JwtAuthenticator",
      "args": [
        "A secret of more than 15 characters"
      ]
    },
    ...
  }
  ...
}
```

This extension takes a single argument in the `args` array property, the secret
used to sign the token.

### Client

The client uses the `TokenClientAuthenticator` for authentication in the
following manner.

```cs
var authenticator = new TokenClientAuthenticator(jwtToken);
var client = Client.Create(server, 9002, authenticator: authenticator);
```
