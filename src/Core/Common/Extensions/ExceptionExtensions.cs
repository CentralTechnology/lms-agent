namespace LMS.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ExceptionExtensions
    {
        private static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (TSource current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        private static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }

        public static string GetFullMessage(this Exception exception)
        {
            IEnumerable<string> messages = exception.FromHierarchy(ex => ex.InnerException)
                .Select(ex => ex.Message);
            return string.Join(Environment.NewLine, messages);
        }

        public static IEnumerable<Exception> GetAllExceptions(this Exception exception)
        {
            return exception.FromHierarchy(ex => ex.InnerException).Select(ex => ex.InnerException);
        }
    }
}