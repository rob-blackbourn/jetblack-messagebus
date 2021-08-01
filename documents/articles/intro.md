# Introduction

## Why?

There are a number of excellent message bus implementations available, all
with different features. What makes this different?

The distinguishing feature of this message bus is it's support for *select* 
feed publishers. Typically a message bus implements a *broadcast* feed 
(e.g. RabbitMQ, Redis, etc). The source publishes messages on topics while
clients subscribe to topics of interest to them.

In a select feed the publisher gets *notified* when a client subscribes or
unsubscribes from a topic. This allows the publisher to only publish data
that is being listened to, and also to send an initial *image* to the new
subscriber, after which it publishes *deltas* to all subscribers.

## Pros

The notification capability provides a number of useful features:

* It reduces network bandwidth by only sending messages that clients have requested.
* It allows for dynamic messaging. For example a client may request AAPL.2020-01-01/2020-02-01
    to get prices for the given range.

## Cons

As all messages pass through a single broker, this beccomes a processiong bottleneck.

## Other Feastures

There are a number of other features provided:

* SSL transport
* Authentication: LDAP, JWT, and password file.
* Authorization. The server can be configured to only send data for which the client is authorized.
