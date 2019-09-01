# Select Feed

## Overview

A select feed is provided by a publisher who only sends data
that has been subscribed to.

## Implementation

### Step 1

Request notification on subscriptions for a feed.

### Step 2

When a subscription is received retrieve the data.

If there is no data send a message to the client with no data to
indicate the is no data available.

If there is data send an image of the data to the client.

### Step 3

When new data arrives for a topic that has been requested publish
the data.
