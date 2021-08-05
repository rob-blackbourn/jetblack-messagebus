# Authorization

The distributor can be configured to require authorization.

## Configuration

When a feed is authorized the following roles are available.

* `Subscribe`
* `Publish`
* `Notify`
* `Authorize`

The are also roles `None` and `All`.

The authorization is configured under the `feedRoles` property, where the
key for each section is the name of the feed. The following sets the feed role
for `FOO` to *not* require authorization.

```json
{
  "distributor": {
    ...
    "feedRoles": {
      "FOO": {
        "isAuthorized": false,
      }
    }
  }
}
```

The roles can be allowed or denied. For example the following allows *all*
roles and denies *none* for the feed `FOO`. Furthermore, THis achieves the same
as the above.

```json
{
  "distributor": {
    ...
    "feedRoles": {
      "FOO": {
        "isAuthorized": true,
        "allow": [
          "All"
        ],
        "deny": [
          "None"
        ]
      }
      }
  }
}
```

For finer grained control we can add an `interactorRoles` section. The interactor
roles are keyed by a *host* which contains an object which is keyed by a *username*.
The final nested object has an `allow` list and a `deny` list.

The following example shows that on `windowsvm.jetblack.net` the user `tom` can
subscribe, publish and notify, but not authorize, while the user `dick` can
only authorize.

```json
{
  "distributor": {
    ...
    "feedRoles": {
      "AUTH": {
        "isAuthorized": true,
        "allow": [
          "None"
        ],
        "deny": [
          "All"
        ],
        "interactorRoles": {
          "windowsvm.jetblack.net": {
            "tom": {
              "allow": [
                "Subscribe",
                "Publish",
                "Notify"
              ],
              "deny": [
                "Authorize"
              ]
            },
            "dick": {
              "allow": [
                "Authorize"
              ],
              "deny": [
                "Subscribe",
                "Publish",
                "Notify",
              ]
            }
          }
        }
      }
    }
    ...
  }
  ...
}
```

## Client Authorizer

It is not enough to configure authorization, there must also be a client to
provide entitlements for a given feed and topic. For example it is common in a
financial feed to have one cost to see the basics of a stock (bid, ask, etc),
but an additional cost to see the market depth (all the bids and asks from each
of the brokers)

To achieve this we make a client which adds a handler to the `OnAuthorizationRequest`
event. When it receives an authorization request it can then use the `Authorize`
method to instruct the distributor on the entitlements. This is shown in the
following code.

```cs
#nullable enable

using System;
using System.Collections.Generic;

using JetBlack.MessageBus.Adapters;
using JetBlack.MessageBus.Common;

using Common;

namespace AuthEntitler
{
  class Program
  {
    public static int Level1 = 42;
    public static int Level2 = 43;

    static void Main(string[] args)
    {
      var authenticator = new BasicClientAuthenticator("dick", "dicksPassword");
      var client = Client.Create(
        "localhost", 9002,
        authenticator: authenticator,
        isSslEnabled: true);

      client.OnAuthorizationRequest += OnAuthorizationRequest;

      Console.WriteLine("Press ENTER to quit");
      Console.ReadLine();
      client.Dispose();
    }

    private static void OnAuthorizationRequest(object? sender, AuthorizationRequestEventArgs e)
    {
      if (!(sender is Client)) return;
      var client = (Client)sender;

      if (e.User == "tom" && e.Feed == "NASDAQ" && e.Topic == "AAPL")
      {
          Console.WriteLine("tom can see both level1 and level2");
          client.Authorize(
            e.ClientId,
            e.Feed, e.Topic,
            true,
            new HashSet<int> { Constants.Level1, Constants.Level2 });
      }
      else if (e.User == "harry" && e.Feed == "NASDAQ" && e.Topic == "AAPL")
      {
          Console.WriteLine("harry can only see level1");
          client.Authorize(
            e.ClientId,
            e.Feed, e.Topic,
            true,
            new HashSet<int> { Constants.Level1 });
      }
      else
      {
          Console.WriteLine("others have no entitlements");
          client.Authorize(e.ClientId, e.Feed, e.Topic, true, null);
      }

      Console.WriteLine($"OnAuthorizationRequest: {e}");
    }
  }
}
```

The signature of the `OnAuthorizationRequest` is as follows.

```cs
void OnAuthorizationRequest(object? sender, AuthorizationRequestEventArgs e)
```

The send will be the message bus client, which can later be used for sending
authorizations. The `AuthorizationRequestEventArgs` has the following properties.

```cs
using System;

namespace JetBlack.MessageBus.Adapters
{
  public class AuthorizationRequestEventArgs : EventArgs
  {
    ...

    public Guid ClientId { get; private set; }
    public string Host { get; private set; }
    public string User { get; private set; }
    public string Feed { get; private set; }
    public string Topic { get; private set; }

    ...
  }
}

```

The `Authorize` instance method has the following prototype.

```cs
public void Authorize(
  Guid clientId,
  string feed, string topic,
  bool isAuthorizationRequired,
  ISet<int>? entitlements)
```