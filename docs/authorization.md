# Authorization

## Overview

Authorization is supported through an entitlements system.

When a client makes a subscription request on a feed that requires
authorization, and authorization request is made. The authorization
client responds with a list of UUIDs which represent the data to
which the client is entitled to receive.

The data is published as a list of tuples of the entitlement UUID and
the payload body. The server forwards to the subscriber only the tuples
for which it has been entitled.