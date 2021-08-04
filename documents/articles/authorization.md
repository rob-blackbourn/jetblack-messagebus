# Authorization

The distributor can be configured to require authorization. When a feed is
authorized the following roles are available.

* `Subscribe`
* `Publish`
* `Notify`
* `Authorize`

The are also roles `None` and `All`.

The authorization is configured under the `feeedRoles` property, where the
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

Typically finer grained control is required.

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
          "host1.jetblack.net": {
            "tom": {
              "allow": [
                "Subscribe"
              ],
              "deny": [
                "Publish",
                "Notify",
                "Authorize"
              ]
            },
            "dick": {
              "allow": [
                "Subscribe"
              ],
              "deny": [
                "Publish",
                "Notify",
                "Authorize"
              ]
            }
          },
          "host2.jetblack.net": {
            "harry": {
              "allow": [
                "Publish",
                "Notify",
              ],
              "deny": [
                "Subscribe"
                "Authorize"
              ]
            },
            "mary": {
              "allow": [
                "Authorize"
              ],
              "deny": [
                "Publish",
                "Notify",
                "Subscribe"
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