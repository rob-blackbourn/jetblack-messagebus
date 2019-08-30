#nullable enable

namespace JetBlack.MessageBus.Common.Configuration
{
    public class PluginConfig
    {
        public string? FullTypeName { get; set; }
        public string[]? Args { get; set; }

        public override string ToString()
        {
            return $"nameof(FullTypeName)={FullTypeName}, Args={string.Join(",", Args)}";
        }

        public T? Construct<T>() where T : class
        {
            var type = FullTypeName?.LoadType();
            return type == null ? null : type.Construct<T>(Args ?? new string[0]);
        }
    }
}