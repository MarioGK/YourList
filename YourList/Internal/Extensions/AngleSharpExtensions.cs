using System;
using AngleSharp.Dom;

namespace YourList.Internal.Extensions
{
    internal static class AngleSharpExtensions
    {
        public static IElement QuerySelectorOrThrow(this IParentNode parent, string selector)
        {
            return parent.QuerySelector(selector) ??
                   throw new InvalidOperationException($"Can't find any element matching selector '{selector}'.");
        }

        public static string GetAttributeOrThrow(this IElement element, string attribute)
        {
            return element.GetAttribute(attribute) ??
                   throw new InvalidOperationException($"Can't find attribute '{attribute}'.");
        }
    }
}