# message-bus

A real time message bus written in .Net Core 3.0 running on Linux.

## Features

- Broker based
- Broadcast feed
- Select feed
- SSL
- Authentication
- Authorization

### Broker Based

A broker based message bus acts like a network switch or router. Clients subscribe to data that is published by other clients. All traffic passes through a hub which holds state about the subscriptions. This contrasts with programs like RabbitMQ which has a store and forward paradigm, and Kafka with a horizontally scalable architecture.

### Broadcast Feed

A broadcast feed publishes all of its data as soon as it is available. An example of this is the feed from a financial exchange. Typically every data event is published, and it is up to the client to filter out the unwanted information,

### Select Feed

In contrast to a broadcast feed, a select feed waits for a client to request the data before it starts to publish it. This presents less network traffic, but provides more complexity in the message bus and publisher.

### SSL

This message bus supports SSL connections between clients and the server.

### Authentication

Authentication is provided as a pluggable component. Out of the box three methods are supported:

- No authentication
- Passwrod File
- LDAP

SSL connections are recommended when using authentication to prevent password snooping.

### Authorization

Many financial feeds require the distribution of data to be restricted to those who have paid for it. This message bus supports such authorization, such that it will not transmit data to clients that are not entitled to receive it.

