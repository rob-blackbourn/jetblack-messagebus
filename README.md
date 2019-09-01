# message-bus

A real time message bus written in .Net Core 3.0 running on Linux.

This is work in progress.

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

## Usage

See the examples folder for more detailed examples.

### Prerequisites

You must have dotnet core 3.0 installed.

### Distributor

Download the distributor tarball from the GitHub [releases](https://github.com/rob-blackbourn/message-bus/releases) page. It will look something like `message-bus-0.0.1.tar.gz`. Untar it into a folder and run it using the dotnet runtime.

```bash
~ $ tar xzf ~/Downloads/message-bus-0.1.0.tar.gz
~ $ cd message-bus-0.1.0
~/message-bus-0.1.0 $ dotnet JetBlack.MessageBus.Distributor.dll
2019-08-31 09:01:10,482 [1] INFO  JetBlack.MessageBus.Distributor.Server [?] - Starting server version 1.0.0.0
2019-08-31 09:01:10,495 [1] INFO  JetBlack.MessageBus.Distributor.Server [?] - Server started
Press any key to stop...
2019-08-31 09:01:10,497 [7] INFO  JetBlack.MessageBus.Distributor.Acceptor [?] - Listening on 0.0.0.0:9091
```

### Clients

#### Notifier

Create a notifier in a new terminal.

```bash
~ $ mkdir notifier
~/ $ cd notifier
~/notifier $ dotnet new console
~/notifier $ dotnet add package JetBlack.MessageBus.Adapters
~/notifier $ cat > Program.cs
using System;

using JetBlack.MessageBus.Adapters;

namespace notifier
{
  class Program
  {
    static void Main(string[] args)
    {
      var authenticator = new NullClientAuthenticator();
      var client = Client.Create("localhost", 9091);
      client.OnForwardedSubscription +=
        (sender, args) => Console.WriteLine(args);
      Console.WriteLine("Requesting notifications on feed \"TEST\".");
      client.AddNotification("TEST");
      Console.WriteLine("Press ENTER to quit");
      Console.ReadLine();
      client.Dispose();
    }
  }
}
^D
~/notifier $ dotnet run
```

#### Subscriber

Create a subscriber in a new terminal.

```bash
~ $ mkdir subscriber
~ $ cd subscriber
~/subscriber $ dotnet new console
~/subscriber $ dotnet add package JetBlack.MessageBus.Adapters
~/subscriber $ cat > Program.cs
using System;
using System.Collections.Generic;
using System.Linq;

using JetBlack.MessageBus.Adapters;

namespace subscriber
{
  class Program
  {
    static void Main(string[] args)
    {
        var authenticator = new NullClientAuthenticator();
        var client = Client.Create("localhost", 9091);

        client.OnDataReceived += OnDataReceived;

        Console.WriteLine("Subscribing to feed \"TEST\" topic \"FOO\".");
        client.AddSubscription("TEST", "FOO");

        Console.WriteLine("Press ENTER to quit");
        Console.ReadLine();

        client.Dispose();
      }

    private static void OnDataReceived(
      object sender,
      DataReceivedEventArgs e)
    {
      if (e.Data == null)
        return;

      foreach (var packet in e.Data)
      {
        if (packet.Body is Dictionary<string, object>)
        {
          var data = (Dictionary<string, object>)packet.Body;
          Console.WriteLine(
            "Data: " + 
            string.Join(",", data.Select(x => $"{x.Key}={x.Value}")));
        }
        else
            Console.WriteLine(packet.Body);
      }
    }
  }
}
^D
~/subscriber $ dotnet run
```

When the subscriber is run the notifier should report the subscription.

#### Publisher

Create a publisher in a new terminal

```bash
~ $ mkdir publisher
~ $ cd publisher
~/publisher $ dotnet new console
~/publisher $ dotnet add package JetBlack.MessageBus.Adapters
~/publisher $ cat > Program.cs
using System;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Common.IO;

namespace publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var authenticator = new NullClientAuthenticator();
            var client = Client.Create("localhost", 9091);

	    Console.WriteLine("Publishing message to feed \"TEST\" and topic \"FOO\".");
            var data = new[] { new DataPacket(Guid.NewGuid(), "Hello, World!") };
            client.Publish("TEST", "FOO", true, data);

            Console.WriteLine("Press ENTER to quit");
	    Console.ReadLine();

            client.Dispose();
        }
    }
}
^D
~/publisher $ dotnet run
```

When the program is run the subscriber should print out the message.

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

