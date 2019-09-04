# MakePassword

## Overview

This is a utility program to set a password in a password file for
use with `JetBlack.MessageBus.Common.Security.PasswordFileAuthenticator`.

## Usage

Provide the location of the password file and the username.

```bash
MakePassword $ dotnet run -- passwords.json tom
Enter password: *******
```