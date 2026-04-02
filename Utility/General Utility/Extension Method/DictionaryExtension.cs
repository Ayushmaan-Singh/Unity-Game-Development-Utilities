using System.Collections.Generic;

namespace Astek
{
    public static class DictionaryExtension
    {
        public static void AddRange<K, V>(this Dictionary<K, V> dict, params KeyValuePair<K, V>[] pairs) =>
            pairs.ForEach(kvPair => dict.TryAdd(kvPair.Key, kvPair.Value));
        public static void AddRange<K, V>(this Dictionary<K, V> dict, params (K Key, V Value)[] pairs) =>
            pairs.ForEach(kvPair => dict.TryAdd(kvPair.Key, kvPair.Value));
        public static void AddRange<K, V>(this Dictionary<K, V> dict, Dictionary<K,V> dictVal) =>
            dictVal.ForEach(kvPair => dict.TryAdd(kvPair.Key, kvPair.Value));
    }
}