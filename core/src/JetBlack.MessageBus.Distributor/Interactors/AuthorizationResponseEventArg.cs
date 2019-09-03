#nullable enable

using System;
using JetBlack.MessageBus.Messages;

namespace JetBlack.MessageBus.Distributor.Interactors
{
    public class AuthorizationResponseEventArg : EventArgs
    {
        public AuthorizationResponseEventArg(Interactor authorizer, Interactor requester, AuthorizationResponse response, bool isInitial)
        {
            Authorizer = authorizer;
            Requester = requester;
            Response = response;
        }

        public Interactor Authorizer { get; }
        public Interactor Requester { get; }
        public AuthorizationResponse Response { get; }
        public bool IsInitial { get; }
    }
}
