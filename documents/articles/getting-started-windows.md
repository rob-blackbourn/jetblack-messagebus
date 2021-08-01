# Getting Started - Windows

Here we write a simple notifier, publisher, and subscriber
in the Windows environment.

## Installation

First download and install the message bus service.

To get the distributor go to the
[Releases](https://github.com/rob-blackbourn/jetblack-messagebus/releases) 
page of the repo and download the distributor for your platform: typically.
distributor-5.0.0-win10-x64.zip.

Unzip the archive into a folder of your choice and double-click on
`JetBlack.MessageBus.Distributor.exe` to start it. The first time
the service is run Windows will pop up a security alert indicating
the service wants to access a network port. Click on "Allow access"
to continue.

## Write some code

### Notifier

We'll start by writing a *notifier*. The notifier listens to other client's
subscriptions.

Open Visual Studio and create a new C# console project call "SimpleNotifier"
using the target framework ".NET Core 3.1".

Once the project has been created right-click on the "Dependencies" and
select "Manage Nuget Packages...". In the "Browse" tab search for the package
`JetBlack.MessageBus.Adapters`, and install it.

Change the file `Program.cs` to have the following content:

```cs
using System;

using JetBlack.MessageBus.Adapters;

namespace SimpleNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the client.
            var client = Client.Create("localhost", 9001);
            client.OnForwardedSubscription +=
              (sender, args) => Console.WriteLine(args);

            // Request notifications on the "TEST" feed.
            Console.WriteLine("Requesting notifications on feed \"TEST\".");
            client.AddNotification("TEST");
            Console.WriteLine("Press ENTER to quit");
            Console.ReadLine();
            
            client.Dispose();
        }
    }
}
```

The program is pretty simple. It creates a client which connects to `localhost`
on port `9001`, which matches the distributor. It adds an event handler for
the event `OnForwardedSubscription`. These are subscriptions made
by another client that have been "forwarded" to this client. The event handler
writes the event to the console. Finally it calls `AddNotification` for the
*feed* `TEST` then waits for the `ENTER` key to be pressed after which it will
dispose of the client and exit.

When we run the program it responds:

```
Requesting notifications on feed "TEST".
Press ENTER to quit
```

Looking at the console for the distributor we see the following new lines:

```
2021-07-29 12:49:45.2520206 info: JetBlack.MessageBus.Distributor.Interactors.Interactor[0]
      Authenticated with NULL as nobody
2021-07-29 12:49:45.2591651 info: JetBlack.MessageBus.Distributor.Interactors.InteractorManager[0]
      Adding interactor: 46e44dfc-961b-4fae-b6e3-78cabfea9df5: nobody() DESKTOP-011OALS()
2021-07-29 12:49:46.2827504 dbug: JetBlack.MessageBus.Distributor.Server[0]
      OnMessage(sender=46e44dfc-961b-4fae-b6e3-78cabfea9df5: nobody() DESKTOP-011OALS(), message=MessageType=NotificationRequest,Feed="TEST",IsAdd=True
2021-07-29 12:49:46.2836294 info: JetBlack.MessageBus.Distributor.Notifiers.NotificationManager[0]
      Handling notification request for 46e44dfc-961b-4fae-b6e3-78cabfea9df5: nobody() DESKTOP-011OALS() on MessageType=NotificationRequest,Feed="TEST",IsAdd=True
```

We can see the client (or *interactor*) connecting. Then the receipt and handling
of the notification request.

### Subscriber

Leave the notifier running and create a new C# console project called "SimpleSubscriber"
in the same way we did for "SimpleNotifier", adding the nuget package for adapters.

Change the code in "Program.cs" to be:

```cs
using System;
using System.Text;

using JetBlack.MessageBus.Adapters;

namespace SimpleSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = Client.Create("localhost", 9001);

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
            if (e.DataPackets == null)
            {
                Console.WriteLine("No data");
                return;
            }

            foreach (var packet in e.DataPackets)
            {
                if (packet.Data != null)
                {
                    var message = Encoding.UTF8.GetString(packet.Data);
                    Console.WriteLine($"Received: \"{message}\"");
                }
            }
        }
    }
}
```

The subscriber creates the client in the same way as the notifier. This time
it adds an event handler for `OnDataReceived`, which just writes the data it
receives to the console. The message bus is agnostic to the format of the
data. In this case we will be receiving `utf-8` strings. Note that the data
is presented as *packets*. This is the mechanism by which authorisation is
implemented. There is no authentication or authorisation in this example, but
if there was the client would only receive packets for which it was authorised.

After adding the event handler the client calls `AddSubscription` to the feed `TEST` and the topic `FOO`.

Running the program provides the following output:

```
Subscribing to feed "TEST" topic "FOO".
Press ENTER to quit
```

However in the notifier we see the following:

```
User="nobody",Host="DESKTOP-011OALS",ClientId=0098299b-e985-4c3e-a617-b45c53bf078b,Feed="TEST",Topic="FOO",IsAdd=True
```

The notifier has received the forwarded subscription. It receives the clients credntials, it's Id, the feed, topic, and whether the subscription has been added or removed.

In the distributor console we see a bunch of information:

```
2021-07-29 13:39:26.1057383 info: JetBlack.MessageBus.Distributor.Interactors.Interactor[0]
      Authenticated with NULL as nobody
2021-07-29 13:39:26.1093019 info: JetBlack.MessageBus.Distributor.Interactors.InteractorManager[0]
      Adding interactor: 0098299b-e985-4c3e-a617-b45c53bf078b: nobody() DESKTOP-011OALS()
2021-07-29 13:39:26.7359492 dbug: JetBlack.MessageBus.Distributor.Server[0]
      OnMessage(sender=0098299b-e985-4c3e-a617-b45c53bf078b: nobody() DESKTOP-011OALS(), message=MessageType=SubscriptionRequest,Feed="TEST",Topic="FOO",IsAdd=True
2021-07-29 13:39:26.7511101 info: JetBlack.MessageBus.Distributor.Subscribers.SubscriptionManager[0]
      Received subscription from 0098299b-e985-4c3e-a617-b45c53bf078b: nobody() DESKTOP-011OALS() on "MessageType=SubscriptionRequest,Feed="TEST",Topic="FOO",IsAdd=True"
2021-07-29 13:39:26.7526426 dbug: JetBlack.MessageBus.Distributor.Interactors.InteractorManager[0]
      Requesting authorization Interactor=0098299b-e985-4c3e-a617-b45c53bf078b: nobody() DESKTOP-011OALS(), Feed=TEST, Topic=FOO
2021-07-29 13:39:26.7533329 dbug: JetBlack.MessageBus.Distributor.Interactors.InteractorManager[0]
      No authorization required
2021-07-29 13:39:26.7558562 dbug: JetBlack.MessageBus.Distributor.Interactors.InteractorManager[0]
      Accepting an authorization response from 0098299b-e985-4c3e-a617-b45c53bf078b: nobody() DESKTOP-011OALS() with MessageType=AuthorizationResponse,ClientId=0098299b-e985-4c3e-a617-b45c53bf078b,Feed="TEST",Topic="FOO",IsAuthorizationRequired=False,Entitlements.Count=0.
2021-07-29 13:39:26.7655983 dbug: JetBlack.MessageBus.Distributor.Notifiers.NotificationManager[0]
      Notifying interactors[46e44dfc-961b-4fae-b6e3-78cabfea9df5: nobody() DESKTOP-011OALS()] of subscription MessageType=ForwardedSubscriptionRequest,User="nobody",Host="DESKTOP-011OALS",ClientId=0098299b-e985-4c3e-a617-b45c53bf078b,Feed="TEST",Topic="FOO",IsAdd=True
```

First we see the subscriber connect. Next the distributor receives the subscription
request. It checks for authorization, then finally informs clients requesting
notifications of the subscription.

Lastly let's run another subscriber. Right click over "Program.cs" and click
"Open Containg Folder". Navigate to the `bin\Debug\netcoreapp3.1` and double-click
on `SimpleSubscriber.exe`. This will start a second subscriber.

The notifier should now receive a second forwarded subscription from the second subscriber.

### Publisher

Leave the subscriber and publisher running and create a new c# console app
"SimplePublisher.cs", adding the adapters nuget package. Replace the "Program.cs"
with the following:

```cs
using System;
using System.Text;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Common.IO;

namespace SimplePublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = Client.Create("localhost", 9001);

            Console.WriteLine("Publishing message to feed \"TEST\" and topic \"FOO\".");
            var dataPackets = new[]
            {
                new DataPacket(null, Encoding.UTF8.GetBytes("Hello, World!"))
            };
            client.Publish("TEST", "FOO", true, dataPackets);

            Console.WriteLine("Press ENTER to quit");
            Console.ReadLine();

            client.Dispose();
        }
    }
}
```

We create a client as before. Then we create an array of a single `DataPacket`.
The first argument to the DataPacket is the entitlements, for which we pass
null as this feed is not using authorisation. The contents must be a `byte[]`,
to we encode the string. Finally we called `Publish` with the packets and the feed
`TEST` and the topic `FOO`.

Running the publisher gives the following output.

```
Publishing message to feed "TEST" and topic "FOO".
Press ENTER to quit
```

There's nothing new in the notifier, but both subscribers have the following
output.

```
Subscribing to feed "TEST" topic "FOO".
Press ENTER to quit
Received: "Hello, World!"
```

Go to the publisher's console and hit `ENTER` to quit. The publisher should stop.
In the window for the subscribers we can see the following output:

```
Subscribing to feed "TEST" topic "FOO".
Press ENTER to quit
Received: "Hello, World!"
No data
```

When all publishers on a topic have disconnected the distributor will send an
empty message to all the subscribers. This can be used as a staleness" indicator.

Now press `ENTER` in  the window of one of the subscribers.

Looking in the notifier window we see the following:

```
Requesting notifications on feed "TEST".
Press ENTER to quit
User="nobody",Host="DESKTOP-011OALS",ClientId=df250556-98bb-4916-b860-47221be39ea4,Feed="TEST",Topic="FOO",IsAdd=True
User="nobody",Host="DESKTOP-011OALS",ClientId=83f8c7c5-e143-4ae2-860f-2dfd00d0a3ed,Feed="TEST",Topic="FOO",IsAdd=True
User="nobody",Host="DESKTOP-011OALS",ClientId=df250556-98bb-4916-b860-47221be39ea4,Feed="TEST",Topic="FOO",IsAdd=False
```

The notifier has been informed that the client that disconnected is not subscribing
to the feed/topic any more. Pressing `ENTER` on the other subscriber shows the following in the notifier.

```
Requesting notifications on feed "TEST".
Press ENTER to quit
User="nobody",Host="DESKTOP-011OALS",ClientId=df250556-98bb-4916-b860-47221be39ea4,Feed="TEST",Topic="FOO",IsAdd=True
User="nobody",Host="DESKTOP-011OALS",ClientId=83f8c7c5-e143-4ae2-860f-2dfd00d0a3ed,Feed="TEST",Topic="FOO",IsAdd=True
User="nobody",Host="DESKTOP-011OALS",ClientId=df250556-98bb-4916-b860-47221be39ea4,Feed="TEST",Topic="FOO",IsAdd=False
User="nobody",Host="DESKTOP-011OALS",ClientId=83f8c7c5-e143-4ae2-860f-2dfd00d0a3ed,Feed="TEST",Topic="FOO",IsAdd=False
```

Now noone is listening!

Now we have a basic understanding of how this work we can build a [selectfeed](selectfeed.md).
