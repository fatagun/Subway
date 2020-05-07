using System.Collections.Generic;
using System.Diagnostics;

namespace Cnd.Core.Common
{
    [DebuggerStepThrough]
    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var obj) ? obj : default(TValue);
        }

        public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            if (dictionary.TryGetValue(key, out var valueObj) && valueObj is T variable)
            {
                value = variable;
                return true;
            }

            value = default(T);
            return false;
        }
    }
}
