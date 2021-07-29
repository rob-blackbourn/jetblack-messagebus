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

Create a new console project call "SelectfeedPublisher" and add the
adapters nuget package.