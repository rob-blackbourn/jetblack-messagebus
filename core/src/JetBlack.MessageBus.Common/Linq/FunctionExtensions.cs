#nullable enable

using System;

namespace JetBlack.MessageBus.Common.Linq
{
    /// <summary>
    /// Extensions to values which involve functions.
    /// </summary>
    public static class FunctionExtensions
    {
        /// <summary>
        /// Apply the given function to a value.
        /// </summary>
        /// <param name="value">The value with which the function will be invoked.</param>
        /// <param name="func">The function to invoke with the given value.</param>
        /// <typeparam name="TIn">The type of the value.</typeparam>
        /// <typeparam name="TOut">The type of the result.</typeparam>
        /// <returns>The result of applying the function to the value.</returns>
        public static TOut Apply<TIn, TOut>(this TIn value, Func<TIn, TOut> func)
        {
            return func(value);
        }
    }
}
