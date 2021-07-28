# JetBlack.MessageBus.Extension.PasswordFileAuthentication

## Overview

A password file authenticator. It stores the passwords as an asymmetric hash with a salt, so it is reasonably secure.

You can use the `MakePassword` utility to set passwords.

## Usage

Specify the following configuration in the `appsettings.json` file. Not the `%SOMETHING%` parts are environment variables.

```json
{
    "distributor": {

        ...

        "authentication": {
            "assemblyPath": "%AUTH_PLUGIN_FOLDER%/JetBlack.MessageBus.Extension.PasswordFileAuthentication.dll",
            "assemblyName": "JetBlack.MessageBus.Extension.PasswordFileAuthentication",
            "typeName": "JetBlack.MessageBus.Common.Security.Authentication.PasswordFileAuthenticator",
            "args": [
                "%PASSWORD_FILE_FOLDER%/passwords.json"
            ]
        },

        ...
    },

    ...
}
```

The plugin uses a file watcher. Add the following lines to the `/etc/sysctl.conf` file.

```bash
fs.inotify.max_user_watches=524288
fs.inotify.max_user_instances=524288
```

Then reload the configuration.

```bash
$ sudo sysctl -p
```
