# Client Messages

## Overview

A client sends and receives messages.

The following messages can be received:

- [Unicast data](#unicast-data)
- [Multicast data](#multicast-data)
- [Forwarded subscription request](#forwarded-subscription-request)
- [Authorization request](#authorization-request)

The following messages can be sent:

- [Subscription request](#subscription-request)
- [Notification request](#notification-request)
- [Send](#send)
- [Publish](#publish)
- [Authorize](#authorize)

## Messages Received

### Unicast Data

Data sent to this specific client.

- [ClientId](#clientid)
- [Host](#host)
- [User](#user)
- [Feed](#feed)
- [Topic](#topic)
- [IsImage](#isimage)
- [DataPackets](#datapackets)

### Multicast data

Data sent to all subscribers of a topic.

- [Host](#host)
- [User](#user)
- [Feed](#feed)
- [Topic](#topic)
- [IsImage](#isimage)
- [DataPackets](#datapackets)

### Forwarded Subscription Request

The subscription request made by a client.

- [ClientId](#clientid)
- [Host](#host)
- [User](#user)
- [Feed](#feed)
- [Topic](#topic)
- [IsAdd](#isadd)

### Authorization Request

A request from a server for authorization for a client to a topic on a feed.

- [User](#user)
- [Host](#host)
- [ClientId](#clientid)
- [Feed](#feed)
- [Topic](#topic)


## Messages Sent

### Subscription Request

A request to the server for a subscription to be added or removed.

- [Feed](#feed)
- [Topic](#topic)
- [IsAdd](#isadd)

### Notification Request

A request to the server to be notified when any client makes a subscription to a feed.

- [Feed](#feed)
- [IsAdd](#isadd)

### Send

Send data to a specific client

- [ClientId](#clientid)
- [Feed](#feed)
- [Topic](#topic)
- [IsImage](#isimage)
- [DataPackets](#datapackets)

### Publish

Publish data to all subscribers of a feed and topic.

- [Feed](#feed)
- [Topic](#topic)
- [IsImage](#isimage)
- [DataPackets](#datapackets)

### Authorize

Send authorization details for a clients request to subscribe to a topic on a feed.

- [ClientId](#clientid)
- [Feed](#feed)
- [Topic](#topic)
- [IsAuthorizationRequired](#isauthorizationrequired)
- [Entitlements](#entitlements)

## Data Types

### ClientId

The client id is a UUID which uniquely identifies each client to the server.
It can be used for sending data directly to a specific client (see [Send](#send))
or authorizing a clients subscription (see [Authorize](#authorize))

### Host

The host name of an authenticated client as a string.

### User

The user name of an authenticated client as a string.

### Feed

The name of the feed as a string.

### Topic

The name of a topic in a feed as a string.

### DataPackets

The data sent or received. This is a sequence of entitlement and data tuples.
The entitlements is a set of ints which represents the entitlement. The data
contains the actual data as an array of bytes.

### IsImage

A boolean indicating if the data represents a full set or just the changes. 

### IsAdd

A boolean indicating whether a subscription was or has been added or removed.

### IsAuthorizationRequired

A boolean indicating whether a subscription requires authorization.

### Entitlements

A set of ints. These are sent in an authorization response, and then
when sending or publishing data. The server will only forward data to a client
for the intss it has been authorized with.
