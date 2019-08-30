#nullable enable

using System;

namespace JetBlack.MessageBus.Distributor.Roles
{
    public class InteractorRole
    {
        public InteractorRole(string host, string user, Role allow, Role deny)
        {
            Host = host;
            User = user;
            Allow = allow;
            Deny = deny;
        }

        public string Host { get; }
        public string User { get; }
        public Role Allow { get; }
        public Role Deny { get; }

        public bool HasRole(Role role, bool decision)
        {
            if (Allow.HasFlag(role))
                decision = true;

            if (Deny.HasFlag(role))
                decision = false;

            return decision;
        }

        public override string ToString() => $"{nameof(Host)}=\"{Host}\",{nameof(User)}=\"{User}\",{nameof(Allow)}={Allow},{nameof(Deny)}={Deny}";

        public struct Key : IEquatable<Key>, IComparable<Key>
        {
            public Key(string host, string user)
            {
                Host = host;
                User = user;
            }

            public string Host { get; }
            public string User { get; }

            public int CompareTo(Key other)
            {
                var diff = string.Compare((Host ?? string.Empty).ToString(), (other.Host ?? string.Empty).ToString(), StringComparison.Ordinal);
                if (diff == 0)
                    diff = string.Compare((User ?? string.Empty), other.User ?? string.Empty, StringComparison.Ordinal);
                return diff;
            }

            public bool Equals(Key other)
            {
                return Equals(Host, other.Host) && User == other.User;
            }

            public override bool Equals(object? obj)
            {
                return obj is Key && Equals((Key)obj);
            }

            public override int GetHashCode()
            {
                return (Host ?? string.Empty).GetHashCode() ^ (User ?? string.Empty).GetHashCode();
            }

            public override string ToString() => $"{nameof(Host)}=\"{Host}\",{nameof(User)}=\"{User}\"";
        }
    }
}
