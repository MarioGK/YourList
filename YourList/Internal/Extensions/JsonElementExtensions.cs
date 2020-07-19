using System.Text.Json;

namespace YourList.Internal.Extensions
{
    internal static class JsonElementExtensions
    {
        public static JsonElement? GetPropertyOrNull(this JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var result) ? result : (JsonElement?) null;
        }
    }
}