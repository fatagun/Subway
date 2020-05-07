using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Cnd.Core.Common
{

    [DebuggerStepThrough]
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Ensure.That("action", action).Is.NotNull();

            Ensure.That("source enumerable", source).Is.NotNull();

            foreach (var t in source)
            {
                action(t);
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            if (!enumerable.Any())
            {
                return true;
            }

            return false;
        }

        public static bool IsNullOrEmpty<T>(this T enumerable)
            where T : class, new()
        {
            return enumerable == null;
        }


        public static bool ContainsAny<T>(this IEnumerable<T> t, params T[] items)
        {
            return items.Any(t.Contains);
        }

        public static bool ContainsAny<T>(this IEnumerable<T> t, IEnumerable<T> items)
        {
            return items.Any(t.Contains);
        }

        public static string Join<T>(this IEnumerable<T> values)
        {
            return values.Join(",");
        }

        public static string Join<T>(this IEnumerable<T> values, string separator)
        {
            string splitter = string.Empty;
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            splitter = separator;

            using (var enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return string.Empty;
                }

                var builder = new StringBuilder();
                if (!Equals(enumerator.Current, default(T)))
                {
                    builder.Append(enumerator.Current);
                }

                while (enumerator.MoveNext())
                {
                    builder.Append(splitter);
                    if (!Equals(enumerator.Current, default(T)))
                    {
                        builder.Append(enumerator.Current);
                    }
                }

                return builder.ToString();
            }
        }

        public static IList<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ReadOnlyCollection<T>(enumerable.ToList());
        }

        public static Stack<TSource> ToStack<TSource>(this IEnumerable<TSource> source)
        {
            var stack = new Stack<TSource>();
            foreach (var item in source.Reverse())
            {
                stack.Push(item);
            }

            return stack;
        }

        public static Queue<TSource> ToQueue<TSource>(this IEnumerable<TSource> source)
        {
            var queue = new Queue<TSource>();
            foreach (var item in source)
            {
                queue.Enqueue(item);
            }

            return queue;
        }

        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int chunksize)
        {
            Ensure.That("chunksize", chunksize).Is.GreaterThan(1);
            Ensure.That("Source", source).Is.NotNull();

            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static string ToString<T>(this IEnumerable<T> values, string separator)
        {
            var buffer = new StringBuilder();
            var needSeparator = false;

            foreach (var value in values)
            {
                if (needSeparator)
                {
                    buffer.Append(separator);
                }

                buffer.Append(value);
                needSeparator = true;
            }

            return buffer.ToString();
        }
    }
}
