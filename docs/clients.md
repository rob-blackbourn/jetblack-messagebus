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

- ClientId: UUID
- Host: string
- User: string
- Feed: string
- Topic: string
- IsImage: boolean
- Data: { Header: UUID, Body: byte[]}[]

### Multicast data

Data sent to all subscribers of a topic.

- Host: string
- User: string
- Feed: string
- Topic: string
- IsImage: boolean
- Data: { Header: UUID, Body: byte[]}[]

### Forwarded Subscription Request

The subscription request made by a client.

- ClientId: UUID
- Host: string
- User: string
- Feed: string
- Topic: string
- IsAdd: boolean

### Authorization Request

A request from a server for authorization for a client to a topic on a feed.

- User: string
- Host: string
- ClientId: UUID
- Feed: string
- Topic: string


## Messages Sent

### Subscription Request

A request to the server for a subscription to be added or removed.

- Feed: string
- Topic: string
- IsAdd: boolean

### Notification Request

A request to the server to be notified when any client makes a subscription to a feed.

- Feed: string
- IsAdd: boolean

### Send

Send data to a specific client

- ClientId: UUID
- Feed: string
- Topic: string
- IsImage: bool
- Data: { Header: UUID, Body: byte[]}[]

### Publish

Publish data to all subscribers of a feed and topic.

- Feed: string
- Topic: string
- IsImage: bool
- Data: { Header: UUID, Body: byte[]}[]

### Authorize

Send authorization details for a clients request to subscribe to a topic on a feed.

- ClientId: UUID
- Feed: string
- Topic: string
- IsAuthorizationRequired: bool
- Entitlements: UUID[]

## Data Types

### ClientId

The client id is a UUID which uniquely identifies each client to the server.
It can be used for sending data directly to a specific client (see [Send](#send))
or authorizing a clients subscription (see [Authorize](#authorize))

### Host

The host name of an authenticated client.

### User

The user name of an authenticated client.

### Feed

The name of the feed.

### Topic

The name of a topic in a feed.

### Data

The data sent or received. This is a sequence of header and body tuples.
The header is a UUID which represents the entitlement. The body contains
the actual data.