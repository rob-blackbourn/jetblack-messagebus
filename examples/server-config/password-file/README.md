# Password File Authenticated Distributor

## Overview

This folder contains an `appsettings.json` file configured for the
password file authenticator using SSL.

It expects the environment variable `JETBLACK_MESSAGEBUS_ROOT` to be set
to the project root of `jetblack-messagebus`, the server certificate
to be found in `$HOME/.keys/server.crt`, the server key to be found
in `$HOME/.keys/server.key`, and the `HOSTNAME` variable to be exported
(not: bash sets `HOSTNAME`, but does not export it).
