# Selectfeed

## Overview

Most publish/subscribe systems follow the broadcast feed model. Let's say
we have a feed of data from a financial exchange. In RabbitMQ we would
publish the information to the exchange with a routing key, and clients
would create non0-durable queues to receive the data in which they were
interested. This is called a *broadcast* feed. All of the data is sent
regardless of whether any clients are subscribing.

In a *selectfeed* the publisher does nothing until it is notified of a
subscription. At this point it sends all the data it has (an *image*) to
that the client, and then subsequently sends any new or changed data
(the *delta*) to all subscribing clients.

## Selectfeed Publisher

There is an example of a selectfeed publisher
[here](https://github.com/rob-blackbourn/jetblack-messagebus/tree/master/examples/selectfeed/publisher).

Much of the code is for creating a mock exchange feed. The mock exchange
feed has a public property `Data` which provides a dictionary indexed by
ticker of a dictionary of field names and values. It also has an event
`OnData` which is raised when the data for a ticker changes. To summarise
we have a database of tickers with field-value properties, and an event
that gets raised when the values for a ticker changes.

The code for the publisher is in
[Publisher.cs](https://github.com/rob-blackbourn/jetblack-messagebus/blob/master/examples/selectfeed/publisher/Publisher.cs). The first place to look
is in the constructor.

```cs
public Publisher(ExchangeFeed exchangeFeed)
{
    _exchangeFeed = exchangeFeed;
    _exchangeFeed.OnData += HandleExchangeData;

    _client = Client.Create("localhost", 9001);
    _client.OnForwardedSubscription += HandleForwardedSubscription;
    _client.AddNotification(Feed);
}
```

The constructor takes the mock exchange feed as an argument and it adds
an event handler of the `OnData` event. We'll see how that's implemented
a little later. It then creates a client, adds an event handler for the
forwarded subscription, the requests notifications for the event. We'll
come to the event hasndler shortly. The key piece of state data we now
need is to hold what tickers have been subscribed to, and by whome. We
make an instance field to hold this.

```cs
private readonly Dictionary<string, Dictionary<Guid, int>> _subscriptions = new Dictionary<string, Dictionary<Guid, int>>();
```

The outer dictioary will be keyed by the ticker (or "topic" - these are
synonymous), and the inner dictionary is keyed by the client identifier
with the value holding the number of subscriptions to this ticker a client
has made. Now we can implement the `HandleExchangeData` event handler.

```cs
private void HandleExchangeData(object sender, ExchangeEventArgs args)
{
    lock (this)
    {
        if (!_subscriptions.ContainsKey(args.Ticker))
            return;

        Console.WriteLine($"Publishing {args.Ticker}");
        _client.Publish(Feed, args.Ticker, false, ToDataPackets(args.Delta));
    }
}
```

When a ticker is updated we want to publish it if there are subscriptions,
so all we need to do is check if the `_subscriptions` state has any client
entries. If it does we call `Publish` on the client giving the `Feed`,
`args.Ticker` (for the topic), and the `args.Delta` (the dictionary of
changes). The `ToDataPackets` method is encoding the dictionary into JSON
then into bytes.

Note how all interaction is done inside a lock, as the events will be
called from different threads.

The `HandleForwardedSubscriptions` event handler must deal with adding and
removing subscriptions.

```cs
private void HandleForwardedSubscription(object sender, ForwardedSubscriptionEventArgs args)
{
    lock (this)
    {
        if (args.IsAdd)
        {
            AddSubscription(args.ClientId, args.Feed, args.Topic);
        }
        else
        {
            RemoveSubscription(args.ClientId, args.Feed, args.Topic);
        }
    }
}
```

First let's look at adding a subscription.

```cs
public void AddSubscription(Guid clientId, string feed, string topic)
{
    lock (this)
    {
        if (!_subscriptions.TryGetValue(topic, out var topicSubscriptions))
        {
            topicSubscriptions = new Dictionary<Guid, int>();
            _subscriptions.Add(topic, topicSubscriptions);
        }

        if (topicSubscriptions.ContainsKey(clientId))
        {
            topicSubscriptions[clientId] += 1;
        }
        else
        {
            Console.WriteLine($"Sending image of {topic} to {clientId}");
            topicSubscriptions[clientId] = 1;
            _client.Send(clientId, feed, topic, true, ToDataPackets(_exchangeFeed.Data[topic]));
        }
    }
}
```

The goal is to increment the count for a given ticker and client. First
we try to get the topic subscriptions for the ticker. If there are none
we create a new empty dictionary for them. If the topic subscriptions
already contains a subscription from the client we increment it. Otherwise
we make a new entry for the client and set the count to 1.

Removing a subscription does the reverse.

```cs
public void RemoveSubscription(Guid clientId, string feed, string topic)
{
    lock (this)
    {
        if (!_subscriptions.TryGetValue(topic, out var topicSubscriptions))
        {
            return;
        }

        if (!topicSubscriptions.ContainsKey(clientId))
        {
            return;
        }

        topicSubscriptions[clientId] -= 1;
        if (topicSubscriptions[clientId] > 0)
        {
            return;
        }

        topicSubscriptions.Remove(clientId);
        if (topicSubscriptions.Count > 0)
        {
            return;
        }

        _subscriptions.Remove(topic);

        Console.WriteLine($"Stopped publishing {topic}");
    }
}
```

First we handle the edge case where we have been sent a subscription
for a ticker that doesn't exist and we ensure the provided client
has actually made a subscription. With these checks complete we can
decrement the subscription count for this ticker and client. If the
client still has subscriptions we return, otherwise we remove the client
form those subscribing to the ticker. After removing the client from
those subscribing to the ticker we check to see if the are any clients
still subscribing to the ticker. If there are not we remove the ticker
from the subscriptions.
