namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class Permission
    {
        public Permission(Role allow, Role deny)
        {
            Allow = allow;
            Deny = deny;
        }

        public Role Allow { get; }
        public Role Deny { get; }
    }
}