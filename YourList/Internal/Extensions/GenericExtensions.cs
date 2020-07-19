using System;

namespace YourList.Internal.Extensions
{
    internal static class GenericExtensions
    {
        public static TOut Pipe<TIn, TOut>(this TIn input, Func<TIn, TOut> transform)
        {
            return transform(input);
        }
    }
}