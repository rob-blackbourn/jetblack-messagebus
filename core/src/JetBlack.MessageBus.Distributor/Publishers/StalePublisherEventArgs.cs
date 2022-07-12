using System.Collections.Generic;

using JetBlack.MessageBus.Distributor.Interactors;
using JetBlack.MessageBus.Messages;

namespace JetBlack.MessageBus.Distributor.Publishers
{
    public class StalePublisherEventArgs : InteractorEventArgs
    {
        public StalePublisherEventArgs(Interactor interactor, IList<FeedTopic> feedsAndTopics)
            : base(interactor)
        {
            FeedsAndTopics = feedsAndTopics;
        }

        public IList<FeedTopic> FeedsAndTopics { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, FeedsAndTopics=[{string.Join(",", FeedsAndTopics)}]";
        }
    }
}
